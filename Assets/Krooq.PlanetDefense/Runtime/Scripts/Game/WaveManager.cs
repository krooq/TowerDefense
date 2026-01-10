using UnityEngine;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Krooq.Common;
using Krooq.Core;
using Sirenix.OdinInspector;
using System;
using Random = UnityEngine.Random;

namespace Krooq.PlanetDefense
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField, ReadOnly] private bool _isWaveActive = false;
        [SerializeField, ReadOnly] private Camera _cam;

        protected GameManager GameManager => this.GetSingleton<GameManager>();

        public bool IsWaveActive => _isWaveActive;

        protected void Start()
        {
            _cam = Camera.main;
        }

        public async void StartWave(int waveNumber)
        {
            var waveIndex = waveNumber - 1;
            _isWaveActive = true;
            var data = GameManager.Data;

            // Calculate budget
            var powerBudget = data.WaveDifficultyCurve.Evaluate(waveNumber);
            var currentPower = 0f;

            var groupSpawnRate = Mathf.Max(data.MinSpawnRate, data.BaseSpawnRate - (waveIndex * data.SpawnRateDecreasePerWave));

            while (currentPower < powerBudget)
            {
                if (GameManager.State != GameState.Playing) break;

                // Pick a threat
                if (data.Threats == null || data.Threats.Count == 0) break;

                var availableThreats = data.Threats.FindAll(t => t.MinWave <= waveNumber);
                if (availableThreats.Count == 0) break;

                var threatData = availableThreats[Random.Range(0, availableThreats.Count)];

                // Calculate group size
                var groupSize = Random.Range(threatData.MinGroupSize, threatData.MaxGroupSize + 1);

                // Spawn group
                for (int i = 0; i < groupSize; i++)
                {
                    if (GameManager.State != GameState.Playing) break;

                    SpawnThreat(threatData);
                    currentPower += threatData.PowerLevel;

                    if (i < groupSize - 1)
                    {
                        await UniTask.Delay(TimeSpan.FromSeconds(threatData.GroupSpawnInterval));
                    }
                }

                if (currentPower >= powerBudget) break;

                await UniTask.Delay(TimeSpan.FromSeconds(groupSpawnRate));
            }

            // Wait for all threats to be gone
            while (GameManager.HasThreats)
            {
                if (GameManager.State != GameState.Playing) break;
                await UniTask.Delay(TimeSpan.FromSeconds(0.5f));
            }

            _isWaveActive = false;
            GameManager.EndWave();
        }

        protected void SpawnThreat(ThreatData threatData)
        {
            if (_cam == null) _cam = Camera.main;

            var height = 2f * _cam.orthographicSize;
            var width = height * _cam.aspect;
            var topEdge = _cam.transform.position.y + _cam.orthographicSize;
            var leftEdge = _cam.transform.position.x - width / 2f;
            var rightEdge = _cam.transform.position.x + width / 2f;

            Vector3 spawnPos;

            var spawnMargin = 0.2f;
            if (threatData.MovementType == ThreatMovementType.Ground)
            {
                // Spawn on left or right edge, within the configured Y range
                var diff = GameManager.Data.HorizonLevel - GameManager.Data.GroundLevel;
                var spawnY = Random.Range(GameManager.Data.GroundLevel + diff / 10f, GameManager.Data.HorizonLevel - diff / 10);
                var leftSide = Random.value < 0.5f;
                var spawnX = leftSide ? leftEdge - spawnMargin : rightEdge + spawnMargin;
                spawnPos = new Vector3(spawnX, spawnY, 0);
            }
            else
            {
                // Air or Constant - Spawn above top edge
                var spawnY = topEdge + spawnMargin;
                var spawnX = Random.Range(leftEdge, rightEdge);
                spawnPos = new Vector3(spawnX, spawnY, 0);
            }

            var threat = GameManager.Spawn(GameManager.Data.ThreatPrefab);
            threat.transform.SetPositionAndRotation(spawnPos, Quaternion.identity);
            threat.Init(threatData);
        }
    }
}

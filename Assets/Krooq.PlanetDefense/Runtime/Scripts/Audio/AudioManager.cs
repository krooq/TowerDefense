using UnityEngine;
using Krooq.Core;
using Krooq.Common;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine.Audio;

namespace Krooq.PlanetDefense
{
    public class AudioManager : MonoBehaviour
    {
        protected GameManager GameManager => this.GetSingleton<GameManager>();
        protected MultiGameObjectPool Pool => this.GetSingleton<MultiGameObjectPool>();

        public async void PlaySound(AudioResource resource, float volume = 1f, float pitch = 1f)
        {
            if (resource == null) return;

            var source = Pool.Get(GameManager.Data.AudioSourcePrefab);
            source.transform.SetParent(transform);
            source.resource = resource;
            source.volume = volume;
            source.pitch = pitch;
            source.spatialBlend = 0f; // 2D Sound
            source.Play();

            while (source != null && source.gameObject != null && source.isPlaying)
            {
                await UniTask.Delay(1000);
            }

            if (source != null && source.gameObject != null)
            {
                source.Stop();
                source.resource = null;
                Pool.Release(source.gameObject);
            }
        }
    }
}

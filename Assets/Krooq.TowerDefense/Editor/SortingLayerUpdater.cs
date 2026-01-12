using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace Krooq.TowerDefense.Editor
{
    public class SortingLayerUpdater : EditorWindow
    {
        private string _targetSortingLayer = "Projectiles";
        private DefaultAsset _targetFolder;
        private int _targetSortingOrder = 0;
        private bool _updateSortingOrder = false;

        [MenuItem("Tools/Krooq/Sorting Layer Updater")]
        public static void ShowWindow()
        {
            GetWindow<SortingLayerUpdater>("Sorting Layer Updater");
        }

        private void OnEnable()
        {
            // Auto-select folder if one is currently selected in Project view
            if (Selection.activeObject != null && Selection.activeObject is DefaultAsset)
            {
                string path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (AssetDatabase.IsValidFolder(path))
                {
                    _targetFolder = Selection.activeObject as DefaultAsset;
                }
            }
        }

        private void OnGUI()
        {
            GUILayout.Label("Update Sorting Layers", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Folder Selection
            _targetFolder = (DefaultAsset)EditorGUILayout.ObjectField("Target Folder", _targetFolder, typeof(DefaultAsset), false);

            EditorGUILayout.Space();

            // Sorting Layer Selection
            var sortingLayers = new List<string>();
            foreach (var layer in SortingLayer.layers)
            {
                sortingLayers.Add(layer.name);
            }

            int selectedIndex = sortingLayers.IndexOf(_targetSortingLayer);
            if (selectedIndex == -1) selectedIndex = 0;

            selectedIndex = EditorGUILayout.Popup("Target Sorting Layer", selectedIndex, sortingLayers.ToArray());
            _targetSortingLayer = sortingLayers[selectedIndex];

            // Optional Sorting Order
            _updateSortingOrder = EditorGUILayout.Toggle("Update Sorting Order", _updateSortingOrder);
            if (_updateSortingOrder)
            {
                _targetSortingOrder = EditorGUILayout.IntField("Sorting Order", _targetSortingOrder);
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Update Prefabs"))
            {
                UpdatePrefabs();
            }
        }

        private void UpdatePrefabs()
        {
            if (_targetFolder == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a target folder.", "OK");
                return;
            }

            string path = AssetDatabase.GetAssetPath(_targetFolder);
            if (!AssetDatabase.IsValidFolder(path))
            {
                EditorUtility.DisplayDialog("Error", "Selected object is not a valid folder.", "OK");
                return;
            }

            string[] guids = AssetDatabase.FindAssets("t:Prefab", new[] { path });
            int processedCount = 0;
            int changedCount = 0;

            try
            {
                AssetDatabase.StartAssetEditing();

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                    if (prefab != null)
                    {
                        bool isDirty = false;

                        // Update Sprite Renderers
                        var spriteRenderers = prefab.GetComponentsInChildren<SpriteRenderer>(true);
                        foreach (var sr in spriteRenderers)
                        {
                            if (sr.sortingLayerName != _targetSortingLayer || (_updateSortingOrder && sr.sortingOrder != _targetSortingOrder))
                            {
                                Undo.RecordObject(sr, "Update Sorting Layer");
                                sr.sortingLayerName = _targetSortingLayer;
                                if (_updateSortingOrder)
                                {
                                    sr.sortingOrder = _targetSortingOrder;
                                }
                                isDirty = true;
                            }
                        }

                        // Update Particle Systems
                        var particleSystems = prefab.GetComponentsInChildren<ParticleSystem>(true);
                        foreach (var ps in particleSystems)
                        {
                            var renderer = ps.GetComponent<ParticleSystemRenderer>();
                            if (renderer != null)
                            {
                                if (renderer.sortingLayerName != _targetSortingLayer || (_updateSortingOrder && renderer.sortingOrder != _targetSortingOrder))
                                {
                                    Undo.RecordObject(renderer, "Update Sorting Layer");
                                    renderer.sortingLayerName = _targetSortingLayer;
                                    if (_updateSortingOrder)
                                    {
                                        renderer.sortingOrder = _targetSortingOrder;
                                    }
                                    isDirty = true;
                                }
                            }
                        }

                        if (isDirty)
                        {
                            EditorUtility.SetDirty(prefab);
                            changedCount++;
                        }
                        processedCount++;
                    }
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            if (changedCount > 0)
            {
                AssetDatabase.SaveAssets();
                Debug.Log($"<b>[SortingLayerUpdater]</b> Processed {processedCount} prefabs. Updated {changedCount} prefabs in {path}");
            }
            else
            {
                Debug.Log($"<b>[SortingLayerUpdater]</b> Processed {processedCount} prefabs. No changes needed.");
            }
        }
    }
}

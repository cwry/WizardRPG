using System.Collections.Generic;
using Tiled2Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomTiledImporter]
public class SceneImporter : ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties) {
        //nothing
    }

    public void CustomizePrefab(GameObject prefab, string path) {
        UnityEngine.Object actualPrefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
        if (actualPrefab == null) {
            ImportUtils.ReadyToWrite(path);
            GameObject empty = new GameObject("empty");
            actualPrefab = PrefabUtility.CreatePrefab(path, empty);
            MonoBehaviour.DestroyImmediate(empty);
        }

        string scenePath = "Assets/Scenes/Imported/" + prefab.name + ".unity";

        UnityEngine.Object sceneObject = AssetDatabase.LoadAssetAtPath(scenePath, typeof(SceneAsset));
        if (sceneObject == null) {
            Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            PrefabUtility.InstantiatePrefab(actualPrefab, scene);
            EditorSceneManager.SaveScene(scene, scenePath, false);
            EditorSceneManager.CloseScene(scene, true);
        }
    }
}

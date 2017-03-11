using System.Collections.Generic;
using Tiled2Unity;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[CustomTiledImporter]
public class PrefabImporter : ICustomTiledImporter {

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties) {
        //nothing
    }

    public void CustomizePrefab(GameObject prefab, string path) {
        var map = prefab.GetComponent<TiledMap>();
        float scale = map.NumTilesWide / (map.ExportScale * map.MapWidthInPixels);
        prefab.transform.localScale = new Vector3(scale, scale, scale);
        prefab.transform.position = new Vector3(-0.5f, scale * map.ExportScale * map.MapHeightInPixels - 0.5f, 0);
        prefab.AddComponent<TiledMapHider>();
    }
}
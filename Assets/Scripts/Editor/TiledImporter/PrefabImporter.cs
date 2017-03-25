using System;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEngine;

[CustomTiledImporter]
public class PrefabImporter : ICustomTiledImporter {

    Dictionary<int, Dictionary<int, TileType>> tileTypeData = new Dictionary<int, Dictionary<int, TileType>>();

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties) {
        if (customProperties.ContainsKey("tile_type")) {
            int x = (int)Math.Round(gameObject.transform.position.x); 
            int y = (int)Math.Round(gameObject.transform.position.y);
            try {
                TileType t = (TileType)Enum.Parse(typeof(TileType), customProperties["tile_type"].ToUpper());
                if (t != TileType.NONE_EXPLICIT) AddNewTileType(x, y, t);
            } catch(Exception e) {
                Debug.LogError("Couldn't parse TileType Enum " + customProperties["tile_type"].ToUpper() + "\n" + e.ToString());
            }
        }
    }

    void AddNewTileType(int x, int y, TileType type) {
        if (!tileTypeData.ContainsKey(x)) {
            tileTypeData.Add(x, new Dictionary<int, TileType>());
        }
        tileTypeData[x].Add(y, type);
    }

    TileType[] GetTileTypeDictAsArray(int w, int h) {
        var arr = new TileType[w * h];
        foreach(var kv1 in tileTypeData) {
            int x = kv1.Key;
            foreach(var kv2 in kv1.Value) {
                int y = h + kv2.Key;
                arr[x + y * w] = kv2.Value;
            }
        }
        return arr;
    }

    public void CustomizePrefab(GameObject prefab, string path) {
        Transform tileTypeObjectLayer = prefab.transform.Find("tiletype_object");
        Transform tileTypeLayer = prefab.transform.Find("tiletype");
        if (tileTypeObjectLayer != null) MonoBehaviour.DestroyImmediate(tileTypeObjectLayer.gameObject);
        if (tileTypeLayer != null) MonoBehaviour.DestroyImmediate(tileTypeLayer.gameObject);

        var map = prefab.GetComponent<TiledMap>();
        prefab.AddComponent<MapInfo>().MapTileTypes = new Flattened2DArray<TileType>(GetTileTypeDictAsArray(map.NumTilesWide, map.NumTilesHigh), map.NumTilesWide);
        prefab.AddComponent<TiledMapHider>();

        float scale = map.NumTilesWide / (map.ExportScale * map.MapWidthInPixels);
        prefab.transform.localScale = new Vector3(scale, scale, scale);
        prefab.transform.position = new Vector3(-0.5f, scale * map.ExportScale * map.MapHeightInPixels - 0.5f, 0);
    }
}

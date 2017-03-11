using System;
using System.Collections.Generic;
using Tiled2Unity;
using UnityEngine;

[CustomTiledImporter]
public class TileTypeImporter : ICustomTiledImporter {

    Dictionary<int, Dictionary<int, TileType>> tileTypeData = new Dictionary<int, Dictionary<int, TileType>>();

    public void HandleCustomProperties(GameObject gameObject, IDictionary<string, string> customProperties) {
        if (customProperties.ContainsKey("tile_type")) {
            int x = (int)Math.Round(gameObject.transform.position.x); 
            int y = (int)Math.Round(gameObject.transform.position.y);
            TileType t = (TileType)Enum.Parse(typeof(TileType), customProperties["tile_type"].ToUpper());
            AddNewTileType(x, y, t);
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
                int y = h - 1 + kv2.Key;
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
        var tileTypeInfo = prefab.AddComponent<TileTypeInfo>();
        tileTypeInfo.SetTileInfo(GetTileTypeDictAsArray(map.NumTilesWide, map.NumTilesHigh), map.NumTilesWide);
    }
}

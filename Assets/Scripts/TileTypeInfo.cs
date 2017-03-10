using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeInfo : MonoBehaviour {
    [HideInInspector]
    public TileType[] tileInfoFlattened;
    [HideInInspector]
    public int tileInfoPersistentDataStride;

    public TileType[,] tileInfo;

    void Awake() {
        unflattenTileInfo();
    }

    void unflattenTileInfo() {
        int w = tileInfoPersistentDataStride;
        int h = tileInfoFlattened.Length / tileInfoPersistentDataStride;
        tileInfo = new TileType[w, h];

        for(int x = 0; x < w; x++) {
            for(int y = 0; y < h; y++) {
                tileInfo[x, y] = tileInfoFlattened[x + y * tileInfoPersistentDataStride];
            }
        }
    }
}

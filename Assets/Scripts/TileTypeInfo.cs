using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTypeInfo : MonoBehaviour {
    [SerializeField][HideInInspector]
    private TileType[] tileInfo;
    [SerializeField][HideInInspector]
    private int tileInfoStride;

    private Flattened2DArray<TileType> _tileTypes;
    public Flattened2DArray<TileType> TileTypes {
        get {
            if (_tileTypes == null) {
                if (tileInfo == null || tileInfoStride <= 0) return null;
                _tileTypes = new Flattened2DArray<TileType>(tileInfo, tileInfoStride);
            }
            return _tileTypes;
        }
    }

    public void SetTileInfo(TileType[] tileInfo, int stride) {
        this.tileInfo = tileInfo;
        this.tileInfoStride = stride;
        _tileTypes = null;
    }

    void OnDrawGizmos() {
        var arr = TileTypes;
        for(int x = 0; x < arr.Width; x++) {
            for(int y = 0; y < arr.Height; y++) {
                switch(arr[x, y]) {
                    case TileType.SOLID :
                        Gizmos.color = new Color(1, 0, 0, 0.5f);
                        break;
                    default:
                        continue;
                }
                
                Gizmos.DrawCube(new Vector3(x, y), Vector3.one);
            }
        }
    }
}

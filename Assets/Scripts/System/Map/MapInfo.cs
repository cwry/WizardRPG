using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapInfo : MonoBehaviour {

    static MapInfo _current;
    public static MapInfo Current {
        get {
            if (_current != null) return _current;
            var mapInfo = FindObjectOfType<MapInfo>();
            if (mapInfo == null) {
                Debug.LogError("Couldn't find Map");
                return null;
            }
            _current = mapInfo;
            return mapInfo;
        }
    }

    Flattened2DArray<TileType> _tileTypes;
    public Flattened2DArray<TileType> TileTypes {
        get {
            if (_tileTypes == null) _tileTypes = RenderTileTypes();
            return _tileTypes;
        }
    }

    [SerializeField][HideInInspector]
    TileType[] serializedMapTileTypes;
    [SerializeField][HideInInspector]
    int serializedMapTileTypesStride;
    Flattened2DArray<TileType> _mapTileTypes;
    public Flattened2DArray<TileType> MapTileTypes {
        get {
            if (_mapTileTypes == null) {
                if (serializedMapTileTypes == null || serializedMapTileTypesStride <= 0) return null;
                _mapTileTypes = new Flattened2DArray<TileType>(serializedMapTileTypes, serializedMapTileTypesStride);
            }
            return _mapTileTypes;
        }

        set {
            serializedMapTileTypes = value.ToArray();
            serializedMapTileTypesStride = value.Width;
            _mapTileTypes = null;
            Width = value.Width;
            Height = value.Height;
            OnDirty();
        }
    }

    Flattened2DArray<TileType> _staticObjectTileTypes;
    public Flattened2DArray<TileType> StaticObjectTileTypes {
        get {
            if (_staticObjectTileTypes == null) {
                StaticObject.Dirty -= OnDirty;
                StaticObject.Dirty += OnDirty;
                _staticObjectTileTypes = RenderStaticObjectTileTypes();
            }
            return _staticObjectTileTypes;
        }
    }

    [SerializeField]
    int _width;
    public int Width {
        get { return _width; }
        private set { _width = value; }
    }

    [SerializeField]
    int _height;
    public int Height {
        get { return _height; }
        private set { _height = value; }
    }

    Flattened2DArray<TileType> RenderStaticObjectTileTypes() {
        if (MapTileTypes == null) return null;

        var priorityBuffer = new Flattened2DArray<int>(MapTileTypes.Width, Height);
        for (int i = 0; i < priorityBuffer.Length; i++) {
            priorityBuffer[i] = int.MinValue;
        }

        var staticObjects = FindObjectsOfType<StaticObject>();
        var staticObjectTileTypes = new Flattened2DArray<TileType>(Width, Height);
        foreach(var staticObject in staticObjects) {
            if (staticObject.enabled == false) continue;
            var tilePosition = staticObject.GetComponent<TilePosition>();
            var position = tilePosition.Position;
            for(int x = 0; x < staticObject.Footprint.Width; x++) {
                for(int y = 0; y < staticObject.Footprint.Height; y++) {
                    int xIndex = position.x + x - staticObject.Footprint.Width / 2;
                    int yIndex = position.y + y - staticObject.Footprint.Height / 2;
                    int priority = (int)tilePosition.Layer * 100 + tilePosition.FightDrawPriority;
                    if (
                        staticObject.Footprint[x, y] == TileType.NONE ||
                        xIndex < 0 ||
                        xIndex >= Width ||
                        yIndex < 0 ||
                        yIndex >= Height ||
                        priorityBuffer[xIndex, yIndex] > priority
                    ) {
                        continue;
                    }
                    priorityBuffer[xIndex, yIndex] = priority;
                    staticObjectTileTypes[xIndex, yIndex] = staticObject.Footprint[x, y];
                }
            }
        }
        return staticObjectTileTypes;
    }

    Flattened2DArray<TileType> RenderTileTypes() {
        if (MapTileTypes == null || StaticObjectTileTypes == null) return null;
        var tileTypes = new Flattened2DArray<TileType>(Width, Height);
        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Height; y++) {
                var mapTileType = MapTileTypes[x, y];
                var staticObjectTileType = StaticObjectTileTypes[x, y];
                tileTypes[x, y] = staticObjectTileType == TileType.NONE ? mapTileType : staticObjectTileType;
            }
        }
        return tileTypes;
    }

    void OnDirty() {
        _tileTypes = null;
        _staticObjectTileTypes = null;
    }

    void OnDestroy() {
        StaticObject.Dirty -= OnDirty;
    }
}

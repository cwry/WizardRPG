using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(TilePosition))]
public class StaticObject : MonoBehaviour {
    public static event Action Dirty;

    [SerializeField][HideInInspector]
    TileType[] serializedFootprint;
    [SerializeField][HideInInspector]
    int serializedFootprintStride;
    Flattened2DArray<TileType> _footprint;
    public Flattened2DArray<TileType> Footprint {
        get {
            if (_footprint == null) {
                if (serializedFootprint == null || serializedFootprintStride <= 0) {
                    Footprint = new Flattened2DArray<TileType>(3, 3);
                    return Footprint;
                }
                _footprint = new Flattened2DArray<TileType>(serializedFootprint, serializedFootprintStride);
            }
            return _footprint;
        }

        set {
            _footprint = value;
            serializedFootprint = _footprint.ToArray();
            serializedFootprintStride = _footprint.Width;
            if (Dirty != null) Dirty();
        }
    }

    void fireDirty() {
        if (Dirty != null) Dirty();
    }

    void Awake() {
        GetComponent<TilePosition>().Changed += fireDirty;
        fireDirty();
    }

    void OnEnable() {
        GetComponent<TilePosition>().Changed -= fireDirty;
        GetComponent<TilePosition>().Changed += fireDirty;
        fireDirty();
    }

    void OnDisable() {
        GetComponent<TilePosition>().Changed -= fireDirty;
        fireDirty();
    }

    void OnDestroy() {
        fireDirty();
        GetComponent<TilePosition>().Changed -= fireDirty;
    }
}

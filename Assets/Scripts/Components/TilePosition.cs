using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[SelectionBase]
[DisallowMultipleComponent]
[ExecuteInEditMode]

public class TilePosition : MonoBehaviour {

    public enum DrawLayer {
        DEFAULT = 0,
        BACKGROUND_LOW = -2,
        BACKGROUND_HIGH = -1,
        FOREGROUND_LOW = 1,
        FOREGROUND_HIGH = 2
    }

    public static string DrawLayerToSortingLayerName(DrawLayer layer) {
        switch (layer) {
            case DrawLayer.DEFAULT:
                return "Main";
            case DrawLayer.BACKGROUND_LOW:
                return "Background-1";
            case DrawLayer.BACKGROUND_HIGH:
                return "Background+1";
            case DrawLayer.FOREGROUND_LOW:
                return "Foreground-1";
            case DrawLayer.FOREGROUND_HIGH:
                return "Foreground+1";
        }
        return "Default";
    }

    [SerializeField][HideInInspector]
    DrawLayer _layer;
    public DrawLayer Layer {
        get {
            return _layer;
        }

        set {
            _layer = value;
            FireOrderChanged();
        }
    }

    [SerializeField][HideInInspector]
    int _fightDrawPriority = 0;
    public int FightDrawPriority{
        get {
            return _fightDrawPriority;
        }

        set {
            _fightDrawPriority = value;
            FireOrderChanged();
        }
    }


    Vec2i? _lastPosition = null;
    Vec2i lastPosition {
        get {
            if (_lastPosition == null) _lastPosition = Position;
            return (Vec2i) _lastPosition;
        }
        set {
            _lastPosition = value;
        }
    }

    public Vec2i Position {
        get { return new Vec2i(Mathf.RoundToInt(transform.position.x - Offset.x), Mathf.RoundToInt(transform.position.y - Offset.y)); }
        set { WorldPosition = new Vector2(value.x, value.y); }
    }

    public Vector2 WorldPosition {
        get { return new Vector2(transform.position.x - Offset.x, transform.position.y - Offset.y); }
        set {
            transform.position = new Vector3(value.x + Offset.x, value.y + Offset.y);
            if (Position != lastPosition) {
                lastPosition = Position;
                FirePositionChanged();
            }
        }
    }

    public Vector2 Offset { get; set; }

    public event Action PositionChanged;
    public event Action OrderChanged;
    public event Action Changed;

    void OnEnable() {
        transform.hideFlags = HideFlags.HideInInspector;
    }

    void OnDisable() {
        enabled = true;
    }

    bool IsTopLevelTilePositionInHierarchy(GameObject go) {
        var tilePosition = go.GetComponent<TilePosition>();
        if (tilePosition == this) return true;
        if (tilePosition != null) return false;

        foreach (Transform t in go.transform) {
            if (IsTopLevelTilePositionInHierarchy(t.gameObject)) return true;
        }

        return false;
    }

    void Update() {
#if UNITY_EDITOR
        UpdateSpriteRendererOrder();
        while (UnityEditorInternal.ComponentUtility.MoveComponentUp(this)) { }
        if (!EditorApplication.isPlayingOrWillChangePlaymode) {
            if(Selection.activeGameObject != null && IsTopLevelTilePositionInHierarchy(Selection.activeGameObject)) {
                var pos = Position;
                if (MapInfo.Current != null) {
                    pos.x = Mathf.Max(Mathf.Min(pos.x, MapInfo.Current.Width - 1), 0);
                    pos.y = Mathf.Max(Mathf.Min(pos.y, MapInfo.Current.Height - 1), 0);
                }
                Position = pos;
                transform.localScale = Vector3.one;
                transform.rotation = Quaternion.identity;
            }
        }
#endif

        if (transform.hasChanged) {
            transform.hasChanged = false;
            if(Position != lastPosition) {
                lastPosition = Position;
                FirePositionChanged();
            }
        }
    }

    void UpdateSpriteRendererOrder(GameObject go = null) {
        if (go == null) go = gameObject;
        var spriteRenderer = go.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null) {
            spriteRenderer.sortingLayerName = DrawLayerToSortingLayerName(Layer);
            if(Layer == DrawLayer.DEFAULT) {
                spriteRenderer.sortingOrder = int.MaxValue - Position.y * 10 + FightDrawPriority;
            }else {
                spriteRenderer.sortingOrder = FightDrawPriority;
            }
        }

        foreach (Transform t in go.transform) {
            if(t.gameObject.GetComponent<TilePosition>() == null) {
                UpdateSpriteRendererOrder(t.gameObject);
            }
        }
    }

    void OnDestroy() {
        transform.hideFlags = HideFlags.None;
    }

    void FirePositionChanged() {
        Debug.Log(Position.x + " | " + Position.y);
        UpdateSpriteRendererOrder();
        if (PositionChanged != null) PositionChanged();
        if (Changed != null) Changed();
    }

    void FireOrderChanged() {
        UpdateSpriteRendererOrder();
        if (OrderChanged != null) OrderChanged();
        if (Changed != null) Changed();
    }
}

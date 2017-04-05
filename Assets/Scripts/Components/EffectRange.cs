using UnityEngine;

[RequireComponent(typeof(TilePosition))]
public class EffectRange : MonoBehaviour {

    public enum EffectGridStates {
        FALSE,
        TRUE
    }

    public string Name;

    [SerializeField][HideInInspector]
    EffectGridStates[] serializedGridState;
    [SerializeField][HideInInspector]
    int serializedGridStateStride;

    Flattened2DArray<EffectGridStates> _gridState;
    public Flattened2DArray<EffectGridStates> GridState {
        get {
            if(_gridState == null) {
                if (serializedGridState == null) {
                    GridState = new Flattened2DArray<EffectGridStates>(3, 3);
                    return GridState;
                };
                _gridState = new Flattened2DArray<EffectGridStates>(serializedGridState, serializedGridStateStride);
            }
            return _gridState;
        }

        set {
            _gridState = value;
            serializedGridState = value.ToArray();
            serializedGridStateStride = value.Width;
        }
    }

    TilePosition _tilePosition = null;
    public TilePosition TilePosition {
        get {
            if (_tilePosition == null) {
                _tilePosition = GetComponent<TilePosition>();
            }
            return _tilePosition;
        }
    }

    public bool this[int x, int y] {
        get {
            x -= TilePosition.Position.x;
            y -= TilePosition.Position.y;
            if (x < 0 || x >= GridState.Width || y < 0 || y >= GridState.Height) return false;
            return GridState[x, y] == EffectGridStates.TRUE;
        }
    }

}

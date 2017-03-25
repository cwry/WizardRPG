using UnityEngine;

public enum TileType {
    NONE,
    NONE_EXPLICIT,
    SOLID
}

public static class TileTypeUtil {
    public static Color ToColor(TileType t) {
        switch (t) {
            case TileType.SOLID:
                return Color.red;
            case TileType.NONE_EXPLICIT:
                return Color.green;
            default:
                return Color.white;
        }
    }
}
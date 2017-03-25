#pragma warning disable 0660, 0661
using UnityEngine;

public struct Vec2i{

    public Vec2i(int x, int y){
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;

    public static bool operator ==(Vec2i v1, Vec2i v2) {
        return v1.x == v2.x && v1.y == v2.x;
    }

    public static bool operator !=(Vec2i v1, Vec2i v2) {
        return v1.x != v2.x || v1.y != v2.y;
    }

    public Vector2 ToVector2() {
        return new Vector2(x, y);
    }
}
#pragma warning restore 0660, 0661

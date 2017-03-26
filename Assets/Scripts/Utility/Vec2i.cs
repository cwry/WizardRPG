#pragma warning disable 0660, 0661
using System;
using UnityEngine;

public struct Vec2i{

    public Vec2i(int x, int y){
        this.x = x;
        this.y = y;
    }

    public int x;
    public int y;

    public static bool operator ==(Vec2i v1, Vec2i v2) {
        return v1.x == v2.x && v1.y == v2.y;
    }

    public static bool operator !=(Vec2i v1, Vec2i v2) {
        return v1.x != v2.x || v1.y != v2.y;
    }

    public Vector2 ToVector2() {
        return new Vector2(x, y);
    }

    public override bool Equals(object obj) {
        return this == (Vec2i)obj;
    }

    public override int GetHashCode() {
        unchecked {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }
}
#pragma warning restore 0660, 0661

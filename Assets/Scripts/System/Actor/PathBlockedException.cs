using System;

public class PathBlockedException : Exception {

    public Vec2i Position { get; set; }

    public PathBlockedException(Vec2i pos) {
        Position = pos;
    }
}

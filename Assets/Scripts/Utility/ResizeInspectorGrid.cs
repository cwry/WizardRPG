using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class Util{
    public static Flattened2DArray<T> ResizeInspectorGrid<T>(Flattened2DArray<T> src, int w, int h) {
        var resized = new Flattened2DArray<T>(w, h);
        int dx = w / 2 - src.Width / 2;
        int dy = h / 2 - src.Height / 2;

        for (int x = 0; x < src.Width; x++) {
            for (int y = 0; y < src.Height; y++) {
                int xIndex = x + dx;
                int yIndex = y + dy;
                if (xIndex < 0 || xIndex >= w || yIndex < 0 || yIndex >= h) continue;
                resized[xIndex, yIndex] = src[x, y];
            }
        }
        return resized;
    }
}

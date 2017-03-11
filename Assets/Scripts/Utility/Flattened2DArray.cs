using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Flattened2DArray<T> : IEnumerable<T> {
    [SerializeField]
    private T[] source { get; set; }
    [SerializeField]
    private int stride { get; set; }

    public int Length {
        get { return source.Length; }
    }

    public int Width {
        get { return stride; }
    }

    public int Height {
        get { return source.Length / stride; }
    }

    public T this[int x, int y] {
        get { return source[x + y * stride]; }
        set { source[x + y * stride] = value; }
    }

    public T this[int i] {
        get { return source[i]; }
        set { source[i] = value; }
    }

    public Flattened2DArray(T[,] source) {
        int w = source.GetLength(0);
        int h = source.GetLength(1);
        var flattenedSource = new T[w * h];
        for(int x = 0; x < w; x++) {
            for(int y = 0; y < h; y++) {
                flattenedSource[x + y * w] = source[x, y];
            }
        }
        this.source = flattenedSource;
        stride = w;
    }

    public Flattened2DArray(T[] source, int stride){
        this.source = source;
        this.stride = stride;
    }

    public Flattened2DArray(int width, int height) {
        source = new T[width * height];
        stride = width;
    }

    public int GetIndexFromPosition(int x, int y) {
        return x + y * stride;
    }

    public int GetXFromIndex(int i) {
        return i % stride;
    }

    public int GetYFromIndex(int i) {
        return i / stride;
    }

    public T[] ToArray() {
        return source;
    }

    public IEnumerator<T> GetEnumerator() {
        return (IEnumerator<T>)source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return source.GetEnumerator();
    }
}

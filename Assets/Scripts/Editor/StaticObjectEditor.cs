using UnityEngine;
using UnityEditor;
using System.Linq;
using Eppy;
using System;

[CustomEditor(typeof(StaticObject))]
public class StaticObjectEditor : Editor {
    Flattened2DArray<TileType> lastFootprint;
    bool footprintFoldout = true;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var tar = (StaticObject)target;

        //footprint foldout
        footprintFoldout = EditorGUILayout.Foldout(footprintFoldout, "Footprint");
        if (footprintFoldout) {
            if (lastFootprint == null) lastFootprint = tar.Footprint;

            //draw footprint size editor
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size");
            int newW = EditorGUILayout.IntField(tar.Footprint.Width);
            if (newW <= 0) newW = 1;
            if (newW % 2 == 0) newW++;
            int newH = EditorGUILayout.IntField(tar.Footprint.Height);
            if (newH <= 0) newH = 1;
            if (newH % 2 == 0) newH++;
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) {
                tar.Footprint = resizeFootprint(newW, newH);
                EditorUtility.SetDirty(target);
            }

            EditorGUI.BeginChangeCheck();
            var newFootprint = InspectorDrawHelpers.DrawEnumGrid(tar.Footprint, (val, x, y) => {
                var color = TileTypeUtil.ToColor(val);
                if (x == tar.Footprint.Width / 2 && y == tar.Footprint.Height / 2) {
                    color.r /= 2;
                    color.g /= 2;
                    color.b /= 2;
                }
                return color;
            });
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Edit Footprint");
                tar.Footprint = newFootprint;
                lastFootprint = newFootprint;
            }
        }
    }

    Flattened2DArray<TileType> resizeFootprint(int w, int h) {
        var resizedFootprint = new Flattened2DArray<TileType>(w, h);
        int dx = w / 2 - lastFootprint.Width / 2;
        int dy = h / 2 - lastFootprint.Height / 2; 

        for (int x = 0; x < lastFootprint.Width; x++) {
            for(int y = 0; y < lastFootprint.Height; y++) {
                int xIndex = x + dx;
                int yIndex = y + dy;
                if (xIndex < 0 || xIndex >= w || yIndex < 0 || yIndex >= h) continue;
                resizedFootprint[xIndex, yIndex] = lastFootprint[x, y];
            }
        }
        return resizedFootprint; 
    }
}

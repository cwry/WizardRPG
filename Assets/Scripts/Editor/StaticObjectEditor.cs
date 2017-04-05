using UnityEngine;
using UnityEditor;
using System.Linq;
using Eppy;
using System;

[CustomEditor(typeof(StaticObject))]
public class StaticObjectEditor : Editor {
    Flattened2DArray<TileType> lastFootprint;
    bool gridStateFoldout = true;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var tar = (StaticObject)target;

        //footprint foldout
        gridStateFoldout = EditorGUILayout.Foldout(gridStateFoldout, "Footprint");
        if (gridStateFoldout) {
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
                tar.Footprint = Util.ResizeInspectorGrid(lastFootprint, newW, newH);
                EditorUtility.SetDirty(target);
            }

            EditorGUI.BeginChangeCheck();
            var newFootprint = InspectorDrawUtil.DrawEnumGrid(tar.Footprint, (val, x, y) => {
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
}

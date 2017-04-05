using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EffectRange))]
public class EffectRangeEditor : Editor {
    Flattened2DArray<EffectRange.EffectGridStates> lastGrid;
    bool footprintFoldout = true;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        var tar = (EffectRange)target;

        //footprint foldout
        footprintFoldout = EditorGUILayout.Foldout(footprintFoldout, "Grid");
        if (footprintFoldout) {
            if (lastGrid == null) lastGrid = tar.GridState;

            //draw footprint size editor
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Size");
            int newW = EditorGUILayout.IntField(tar.GridState.Width);
            if (newW <= 0) newW = 1;
            if (newW % 2 == 0) newW++;
            int newH = EditorGUILayout.IntField(tar.GridState.Height);
            if (newH <= 0) newH = 1;
            if (newH % 2 == 0) newH++;
            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck()) {
                tar.GridState = Util.ResizeInspectorGrid(lastGrid, newW, newH);
                EditorUtility.SetDirty(target);
            }

            EditorGUI.BeginChangeCheck();
            var newGridState = InspectorDrawUtil.DrawEnumGrid(tar.GridState, (val, x, y) => {
                Color? clr = null;
                if(val == EffectRange.EffectGridStates.TRUE) {
                    clr = Color.green;
                }else {
                    clr = Color.white;
                }

                Color color = (Color)clr;
                if (x == tar.GridState.Width / 2 && y == tar.GridState.Height / 2) {
                    color.r /= 2;
                    color.g /= 2;
                    color.b /= 2;
                }
                return color;
            });
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(target, "Edit Footprint");
                tar.GridState = newGridState;
                lastGrid = newGridState;
            }
        }
    }
}

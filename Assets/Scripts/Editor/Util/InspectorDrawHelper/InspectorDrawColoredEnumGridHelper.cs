using System;
using UnityEditor;
using UnityEngine;
using Eppy;

public static partial class InspectorDrawUtil{

    public static Flattened2DArray<T> DrawEnumGrid<T>(Flattened2DArray<T> enums, Func<T, int, int, Color> mapColor){
        var result = new Flattened2DArray<T>(enums.Width, enums.Height);
        var defaultGUIColor = GUI.color;
        var defaultGUIContentColor = GUI.contentColor;
        GUI.contentColor = new Color(0, 0, 0, 0);
        for (int y = enums.Height - 1; y >= 0; y--) {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            for (int x = 0; x < enums.Width; x++) {
                GUI.color = mapColor(enums[x, y], x, y);
                result[x, y] = (T)(object)EditorGUILayout.EnumPopup((Enum)(object)enums[x, y], "Box", GUILayout.Width(25), GUILayout.Height(25));
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
        GUI.color = defaultGUIColor;
        GUI.contentColor = defaultGUIContentColor;
        return result;
    }
}

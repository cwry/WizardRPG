using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TilePosition))]
public class TilePositionEditor : Editor {
    Tool lastTool = Tool.None;

    public override void OnInspectorGUI(){
        var tilePosition = (TilePosition)target;
        EditorGUI.BeginChangeCheck();
        GUILayout.BeginHorizontal();
        var pos = tilePosition.Position;
        EditorGUILayout.LabelField("Position");
        pos.x = EditorGUILayout.IntField(pos.x);
        pos.y = EditorGUILayout.IntField(pos.y);
        GUILayout.EndHorizontal();
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(tilePosition.gameObject.transform, "Change Tile Position");
            tilePosition.Position = pos;
        }

        EditorGUILayout.Space();

        EditorGUI.BeginChangeCheck();
        TilePosition.DrawLayer drawLayer = (TilePosition.DrawLayer)EditorGUILayout.EnumPopup("Draw Layer", tilePosition.Layer);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Change Draw Layer");
            tilePosition.Layer = drawLayer;
        }

        EditorGUI.BeginChangeCheck();
        int fightDrawPriority = EditorGUILayout.IntSlider("Fight Draw Priority", tilePosition.FightDrawPriority, 0, 9);
        if (EditorGUI.EndChangeCheck()) {
            Undo.RecordObject(target, "Change Draw Priority");
            tilePosition.FightDrawPriority = fightDrawPriority;
        }

        EditorUtility.SetDirty(target);

        Tools.current = Tools.current == Tool.Move ? Tool.Move : Tool.None;
    }

    void OnEnable() {
        lastTool = Tools.current;
        Tools.current = Tools.current == Tool.Move ? Tool.Move : Tool.None;
    }

    void OnDisable() {
        Tools.current = lastTool;
    }
}

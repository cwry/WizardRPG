using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TiledMapHider))]
public class TiledMapHiderEditor : Editor {
    Tool lastTool = Tool.None;

    public override void OnInspectorGUI() {
        Tools.current = Tool.None;
        DrawDefaultInspector();
    }

    void OnEnable() {
        lastTool = Tools.current;
        Tools.current = Tool.None;
    }

    void OnDisable() {
        Tools.current = lastTool;
    }
}
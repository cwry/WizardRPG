using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class EditorUtilEditorWindow : EditorWindow{
    public void Awake() {
        SceneView.onSceneGUIDelegate += EditorUtil.DrawTileTypes;
    }
    [MenuItem("Window/EditorUtil")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(EditorUtilEditorWindow));
    }
    private void OnGUI() {
        EditorUtil.RenderTileTypes();
        EditorUtil.DrawSaveButton();
        SceneView.RepaintAll();
    }
}

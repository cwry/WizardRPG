using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

public class EditorUtilEditorWindow : EditorWindow{
    bool RenderTileTypesFoldout;

    public void OnFocus() {
        SceneView.onSceneGUIDelegate -= OnHandles;
        SceneView.onSceneGUIDelegate += OnHandles;
    }
    [MenuItem("Window/EditorUtil")]
    public static void ShowWindow() {
        EditorWindow.GetWindow(typeof(EditorUtilEditorWindow));
    }

    void OnGUI() {
        if(RenderTileTypesFoldout = EditorGUILayout.Foldout(RenderTileTypesFoldout, "Render Map Info")) EditorUtilWindowUtil.RenderMapInfoEditor();
        EditorUtilWindowUtil.DrawSaveButton();
    }

    void OnHandles(SceneView sceneView) {
        EditorUtilWindowUtil.RenderMapInfoHandles();
        sceneView.Repaint();
    }
}

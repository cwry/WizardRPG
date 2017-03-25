using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class EditorUtilWindowUtil {

    public static void DrawSaveButton() {
        if(GUILayout.Button("Save Game")) {
            SaveTest test = new SaveTest();
            test.testSave();
        }
    }
}

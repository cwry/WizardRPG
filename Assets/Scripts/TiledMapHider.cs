using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
[SelectionBase]
public class TiledMapHider : MonoBehaviour {
    #if UNITY_EDITOR
    void Update() {
        if (!EditorApplication.isPlayingOrWillChangePlaymode) {
            gameObject.hideFlags = HideFlags.NotEditable;
            foreach(Transform t in GetComponentsInChildren<Transform>()) {
                if (t == transform) continue;
                t.gameObject.hideFlags = (HideFlags.NotEditable | HideFlags.HideInHierarchy);
            }
        }
    }
    #endif
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
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

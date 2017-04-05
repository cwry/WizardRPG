using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransition : MonoBehaviour {

    public EffectRange EffectRange;
    public string TargetLevelName;
    public Vec2i TargetPosition;

    void Awake() {
        PlayerController.Current.PreMove += OnPrePlayerMove;
    }

    void OnPrePlayerMove(Vec2i pos) {
        if(isActiveAndEnabled && gameObject.activeInHierarchy && EffectRange[pos.x, pos.y]) {
            SceneManager.LoadScene(TargetLevelName, LoadSceneMode.Single);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (PlayerController.Current != null) {
            PlayerController.Current.Actor.TilePosition.Position = TargetPosition;
        }
    }

    void OnDestroy() {
        PlayerController.Current.PreMove -= OnPrePlayerMove;
    }
}

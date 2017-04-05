using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor), typeof(CombatStats))]
[DisallowMultipleComponent]
public class Enemy : MonoBehaviour {
    public EffectRange AggroRange;

    void Awake() {
        PlayerController.Current.PostMove += OnPostPlayerMove;
    }

    void OnPostPlayerMove(Vec2i pos) {
        //start battle
    }

    void OnDestroy() {
        PlayerController.Current.PostMove -= OnPostPlayerMove;
    }
}

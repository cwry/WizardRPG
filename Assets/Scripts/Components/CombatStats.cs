using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
[DisallowMultipleComponent]
public class CombatStats : MonoBehaviour {
    public float MaxHP = 100;
    [HideInInspector]
    public float HP;

    void Awake() {
        HP = MaxHP;
    }
}

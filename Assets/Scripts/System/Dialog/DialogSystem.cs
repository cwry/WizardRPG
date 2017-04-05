using RSG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour {
    static DialogSystem _current;
    static DialogSystem current {
        get {
            if(_current == null) {
                _current = Instantiate(Resources.Load("DialogUI") as GameObject).GetComponent<DialogSystem>();
            }
            return _current;
        }
    }

    public static IPromise ShowText(string txt) {
        var promise = new Promise();
        current.gameObject.SetActive(true);
        current.dialogTextField.text = txt;
        current.StartCoroutine(current.WaitForInputCoroutine(() => {
            current.gameObject.SetActive(false);
            promise.Resolve();
        }));
        return promise;
    }

    public IEnumerator WaitForInputCoroutine(Action done) {
        while (!Input.GetKeyDown(KeyCode.Space)) {
            yield return null;
        }
        if(done != null) done();
    }

    [SerializeField]
    Text dialogTextField;
}

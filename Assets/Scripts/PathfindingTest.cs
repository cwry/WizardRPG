using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingTest : MonoBehaviour {

	void Awake() {
        DoStuff();
    }

    void DoStuff() {
        var actor = GetComponent<Actor>();
        var tilePosition = GetComponent<TilePosition>();
        var goal = new Vec2i(0, 31);


        if (tilePosition.Position == new Vec2i(0, 31)) {
            goal = new Vec2i(31, 31);
        }

        actor.FindAndMoveOnPath(goal, 2f)
            .Catch(e => {
                DoStuff();
            })
            .Done(() => {
                DoStuff();
            });
    }
}

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
        var goal = new Vec2i(Random.Range(0, 32), Random.Range(0, 32));

        while(MapInfo.Current.TileTypes[goal.x, goal.y] == TileType.SOLID) {
            goal = new Vec2i(Random.Range(0, 32), Random.Range(0, 32));
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

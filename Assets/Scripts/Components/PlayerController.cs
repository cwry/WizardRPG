using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Actor))]
public class PlayerController : MonoBehaviour {
    static PlayerController _current;
    public static PlayerController Current {
        get {
            if (_current != null) return _current;
            var playerController = FindObjectOfType<PlayerController>();
            if (playerController == null) {
                Debug.LogError("Couldn't find PlayerController");
                return null;
            }
            _current = playerController;
            return playerController;
        }
    }

    Actor _actor = null;
    public Actor Actor {
        get {
            if (_actor == null) {
                _actor = GetComponent<Actor>();
            }
            return _actor;
        }
    }

    public event Action<Vec2i> PreMove;
    public event Action<Vec2i> PostMove;

    bool IsPositionValid(Vec2i pos) {
        if (pos.x < 0 || pos.x >= MapInfo.Current.Width || pos.y < 0 || pos.y >= MapInfo.Current.Height) return false;
        var tileType = MapInfo.Current.TileTypes[pos.x, pos.y];
        if (tileType == TileType.SOLID) return false; //is blocked terrain?

        var distanceToTile = new Vector2(pos.x - Actor.TilePosition.WorldPosition.x, pos.y - Actor.TilePosition.WorldPosition.y).magnitude;
        var actors = MapInfo.Current.Actors[pos.x, pos.y];
        foreach (var actor in actors) {
            if (actor != this && new Vector2(pos.x - actor.TilePosition.WorldPosition.x, pos.y - actor.TilePosition.WorldPosition.y).magnitude < distanceToTile) return false; //isn't nearest to tile?
        }
        return true;
    }

    void Update() {
        var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if(Mathf.Abs(input.x) > 0.5 || Mathf.Abs(input.y) > 0.5) {
            if(Mathf.Abs(input.x) > Mathf.Abs(input.y)) {
                input.y = 0;
            }else {
                input.x = 0;
            }

            input.Normalize();
            var targetTile = new Vec2i(Actor.TilePosition.Position.x + (int)input.x, Actor.TilePosition.Position.y + (int)input.y);
            if (!Actor.IsMoving) {
                if (PreMove != null) PreMove(targetTile);
                if (IsPositionValid(targetTile)) {
                    MapInfo.Current.PlayerTargetPosition = targetTile;
                    Actor.MoveToTile(new Vec2i(Actor.TilePosition.Position.x + (int)input.x, Actor.TilePosition.Position.y + (int)input.y), 4, IsPositionValid)
                        .Catch(e => MapInfo.Current.PlayerTargetPosition = null)
                        .Done(() => {
                            MapInfo.Current.PlayerTargetPosition = null;
                            if (PostMove != null) PostMove(targetTile);
                        });
                }
            }
        }
    }
}

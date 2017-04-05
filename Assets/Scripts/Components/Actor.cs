using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using System;
using System.Linq;

[RequireComponent(typeof(TilePosition))]
[DisallowMultipleComponent]
public class Actor : MonoBehaviour {

    [Serializable]
    public struct AppearanceDefinition {
        public SpriteRenderer renderer;
        public Sprite front;
        public Sprite back;
        public Sprite leftSide;
        public Sprite rightSide;
    }

    public enum Orientation {
        FRONT,
        BACK,
        LEFT,
        RIGHT
    }

    public static Orientation DeltaToOrientation(Vec2i delta) {
        if(Mathf.Abs(delta.y) >= Mathf.Abs(delta.x)) {
            if (delta.y <= 0) return Orientation.FRONT;
            return Orientation.BACK;
        }
        if (delta.x <= 0) return Orientation.LEFT;
        return Orientation.RIGHT;
    }

    public AppearanceDefinition[] appearances;

    public bool IsMoving { get; private set; }

    TilePosition _tilePosition = null;
    public TilePosition TilePosition {
        get {
            if (_tilePosition == null) {
                _tilePosition = GetComponent<TilePosition>();
            }
            return _tilePosition;
        }
    }

    void Awake() {
        MapInfo.Current.Actors[TilePosition.Position.x, TilePosition.Position.y].Add(this);
        TilePosition.PositionChanged += UpdateInActorList;
    }

    void UpdateInActorList() {
        MapInfo.Current.Actors[TilePosition.LastPosition.x, TilePosition.LastPosition.y].Remove(this);
        MapInfo.Current.Actors[TilePosition.Position.x, TilePosition.Position.y].Add(this);
    }

    void OnDestroy() {
        TilePosition.PositionChanged -= UpdateInActorList;
        if(MapInfo.Current != null) {
            MapInfo.Current.Actors[TilePosition.Position.x, TilePosition.Position.y].Remove(this);
        }
    }

    void UpdateAppearances(Orientation o) {
        switch (o) {
            case Orientation.FRONT:
                foreach(var appearance in appearances) {
                    if(appearance.front != null) {
                        appearance.renderer.sprite = appearance.front;
                        appearance.renderer.flipX = false;
                    }
                }
                break;
            case Orientation.BACK:
                foreach (var appearance in appearances) {
                    if (appearance.back != null) {
                        appearance.renderer.sprite = appearance.back;
                        appearance.renderer.flipX = false;
                    }
                }
                break;
            case Orientation.LEFT:
                foreach (var appearance in appearances) {
                    if (appearance.leftSide != null) {
                        appearance.renderer.sprite = appearance.leftSide;
                        appearance.renderer.flipX = false;
                    }else if (appearance.rightSide != null) {
                        appearance.renderer.sprite = appearance.rightSide;
                        appearance.renderer.flipX = true;
                    }
                }
                break;
            case Orientation.RIGHT:
                foreach (var appearance in appearances) {
                    if (appearance.rightSide != null) {
                        appearance.renderer.sprite = appearance.rightSide;
                        appearance.renderer.flipX = false;
                    } else if (appearance.leftSide != null) {
                        appearance.renderer.sprite = appearance.leftSide;
                        appearance.renderer.flipX = true;
                    }
                }
                break;
        }
    }

    bool IsPositionValid(Vec2i pos) {
        if (pos == MapInfo.Current.PlayerTargetPosition) return false; //is player target?

        var tileType = MapInfo.Current.TileTypes[pos.x, pos.y];
        if (tileType == TileType.SOLID) return false; //is blocked terrain?

        var distanceToTile = new Vector2(pos.x - TilePosition.WorldPosition.x, pos.y - TilePosition.WorldPosition.y).magnitude;
        var actors = MapInfo.Current.Actors[pos.x, pos.y];
        foreach (var actor in actors) {
            var playerController = actor.GetComponent<PlayerController>();
            if (playerController != null && playerController.isActiveAndEnabled) return false; //is player on tile?

            if (actor != this && new Vector2(pos.x - actor.TilePosition.WorldPosition.x, pos.y - actor.TilePosition.WorldPosition.y).magnitude < distanceToTile) return false; //isn't nearest to tile?
        }
        return true;
    }

    float GetTileCost(Vec2i pos) {
        if (pos.x < 0 || pos.x >= MapInfo.Current.Width || pos.y < 0 || pos.y >= MapInfo.Current.Height) return -1; //is out of bounds?
        var tileType = MapInfo.Current.TileTypes[pos.x, pos.y];
        if (tileType == TileType.SOLID) return -1; //is blocked terrain?
        return 1;
    }

    public IPromise FindAndMoveOnPath(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null, Func<Vec2i, float> getTileCost = null, float recalculationPenalty = 10) {
        var promise = new Promise();

        if (isPositionValid == null) isPositionValid = IsPositionValid;
        if (getTileCost == null) getTileCost = GetTileCost;

        Action move = null;

        Action<Exception> handlePathError = e => {
            if (e as PathBlockedException != null) {
                var pathBlockedException = (PathBlockedException)e;

                var oldIsPositionValid = isPositionValid;
                isPositionValid = pos => {
                    if (pos != MapInfo.Current.PlayerTargetPosition && pos == pathBlockedException.Position && MapInfo.Current.TileTypes[pos.x, pos.y] != TileType.SOLID) { //is whitelisted but not player target or blocked terrain?
                        var actors = MapInfo.Current.Actors[pos.x, pos.y];
                        foreach (var actor in actors) {
                            var playerController = actor.GetComponent<PlayerController>();
                            if (playerController != null && playerController.isActiveAndEnabled) {
                                return oldIsPositionValid(pos); //is player on tile?
                            }
                        }
                        return true;
                    }
                    return oldIsPositionValid(pos);
                };

                var oldGetTileCost = getTileCost;
                getTileCost = pos => {
                    if (pos == pathBlockedException.Position && MapInfo.Current.Actors[pos.x, pos.y].Count != 0) { //is whitelisted and there are still actors on tile?
                        return oldGetTileCost(pos) * recalculationPenalty; //apply avoidance penalty
                    }
                    return oldGetTileCost(pos);
                };

                move();
            } else {
                promise.Reject(e);
            }
        };

        move = () => {
            var path = Pathfinding.SearchPath(TilePosition.Position, goal, getTileCost);

            if (path == null) {
                promise.Reject(new ApplicationException("Couldn't find path"));
            } else {
                MoveOnPath(path, speed, isPositionValid)
                    .Catch(handlePathError)
                    .Done(() => promise.Resolve());
            }
        };

        move();

        return promise;
    }


    IPromise MoveOnPath(IEnumerable<Vec2i> path, float speed, Func<Vec2i, bool> isPositionValid = null) {
        var promise = new Promise();

        int index = 1;
        Action nextTile = null;
        nextTile = () => {
            MoveToTile(path.ElementAt(index), speed, isPositionValid)
            .Catch(e => promise.Reject(e))
            .Done(() => {
                if(++index < path.Count()) {
                    nextTile();
                    return;
                }
                promise.Resolve();
            });
        };

        nextTile();

        return promise;
    }

    public IPromise MoveToTile(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null) {
        var promise = new Promise();

        if (IsMoving) {
            promise.Reject(new ApplicationException("Actor " + gameObject.name + " is already moving"));
            return promise;
        }

        var delta = new Vec2i(goal.x - TilePosition.Position.x, goal.y - TilePosition.Position.y);
        UpdateAppearances(DeltaToOrientation(delta));

        IsMoving = true;
        StartCoroutine(TweenToTile(goal, speed, isPositionValid, success => {
            IsMoving = false;
            if (!success) {
                promise.Reject(new PathBlockedException(goal));
            }else {
                promise.Resolve();
            }
        }));
        return promise;
    }

    IEnumerator TweenToTile(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null, Action<bool> onEnd = null, float totalWait = 10) {
        if (isPositionValid == null) isPositionValid = IsPositionValid;

        var start = TilePosition.Position;
        bool reversing = false;
        float progress = 0;
        while (true) {
            if (progress >= 1) progress = 1;
            if (!reversing && !isPositionValid(goal)) {
                reversing = true;
                progress = 1 - progress;
                var oldGoal = goal;
                goal = start;
                start = oldGoal;
            }

            TilePosition.WorldPosition = Vector2.Lerp(start.ToVector2(), goal.ToVector2(), progress);

            if (progress >= 1) {
                if (reversing) {
                    //determine if actor should wait after being blocked off
                    var thisPlayerController = GetComponent<PlayerController>();
                    if (thisPlayerController == null || !thisPlayerController.isActiveAndEnabled) { //if the actor isn't the player
                        if (start == MapInfo.Current.PlayerTargetPosition) yield return new WaitForSeconds(1); //if the target position is player target -> wait
                        var actors = MapInfo.Current.Actors[start.x, start.y];
                        foreach (var actor in actors) {
                            var playerController = actor.gameObject.GetComponent<PlayerController>();
                            if ((playerController != null && playerController.isActiveAndEnabled) || (actor.IsMoving && GetInstanceID() < actor.GetInstanceID())) {
                                yield return new WaitForSeconds(1); //if player or moving higher priority actor is on tile -> wait
                            }
                            break;
                        }
                    }
                }
            
                if (onEnd != null) onEnd(!reversing);
                yield break;
            }

            yield return null;

            progress += speed * Time.deltaTime;
        }
    }
}

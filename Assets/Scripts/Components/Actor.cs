using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using System;
using System.Linq;

[RequireComponent(typeof(TilePosition))]
[DisallowMultipleComponent]
public class Actor : MonoBehaviour {

    bool isMoving { get; set; }

    TilePosition _cachedTilePosition = null;
    TilePosition CachedTilePosition {
        get {
            if (_cachedTilePosition == null) {
                _cachedTilePosition = GetComponent<TilePosition>();
            }
            return _cachedTilePosition;
        }
    }

    void Awake() {
        MapInfo.Current.Actors[CachedTilePosition.Position.x, CachedTilePosition.Position.y].Add(this);
        CachedTilePosition.PositionChanged += UpdateInActorList;
    }

    void UpdateInActorList() {
        MapInfo.Current.Actors[CachedTilePosition.LastPosition.x, CachedTilePosition.LastPosition.y].Remove(this);
        MapInfo.Current.Actors[CachedTilePosition.Position.x, CachedTilePosition.Position.y].Add(this);
    }

    void OnDestroy() {
        CachedTilePosition.PositionChanged -= UpdateInActorList;
        if(MapInfo.Current != null) {
            MapInfo.Current.Actors[CachedTilePosition.Position.x, CachedTilePosition.Position.y].Remove(this);
        }
    }

    public IPromise FindAndMoveOnPath(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null, Func<Vec2i, bool> isPositionValidCommitted = null, Func<Vec2i, float> getTileCost = null, int maxRecalcs = int.MaxValue, float recalculationPenalty = 10) {
        var promise = new Promise();
        int recalcs = 0;

        if (isPositionValidCommitted == null) isPositionValidCommitted = pos => {
            var tileType = MapInfo.Current.TileTypes[pos.x, pos.y];
            if (tileType == TileType.SOLID) return false;
            return true;
        };

        if (isPositionValid == null) isPositionValid = pos => {
            if (!isPositionValidCommitted(pos)) return false;
            var actors = MapInfo.Current.Actors[pos.x, pos.y];
            if (actors.Contains(this)) {
                if (actors.Count > 1) return false;
            } else {
                if (actors.Count > 0) return false;
            }
            return true;
        };

        if (getTileCost == null) getTileCost = pos => {
            if (pos.x < 0 || pos.x >= MapInfo.Current.Width || pos.y < 0 || pos.y >= MapInfo.Current.Height) return -1;
            var tileType = MapInfo.Current.TileTypes[pos.x, pos.y];
            if (tileType == TileType.SOLID) return -1;
            return 1;
        };

        Action move = null;

        Action<Exception> handlePathError = e => {
            if (e as PathBlockedException != null) {
                var pathBlockedException = (PathBlockedException)e;

                var oldIsPositionValid = isPositionValid;
                isPositionValid = pos => {
                    if (pos == pathBlockedException.Position && MapInfo.Current.TileTypes[pos.x, pos.y] != TileType.SOLID) {
                        return true;
                    }
                    return oldIsPositionValid(pos);
                };

                var oldGetTileCost = getTileCost;
                getTileCost = pos => {
                    if (pos == pathBlockedException.Position && isPositionValid(pos) && MapInfo.Current.Actors[pos.x, pos.y].Count != 0) {
                        return oldGetTileCost(pos) * recalculationPenalty;
                    }
                    return oldGetTileCost(pos);
                };

                if (recalcs >= maxRecalcs) {
                    promise.Reject(new ApplicationException("Exceeded max recalculations"));
                }

                recalcs++;

                move();
            } else {
                promise.Reject(e);
            }
        };

        move = () => {
            var path = Pathfinding.SearchPath(CachedTilePosition.Position, goal, getTileCost);

            if (path == null) {
                promise.Reject(new ApplicationException("Couldn't find path"));
            } else {
                MoveOnPath(path, speed, recalcs < maxRecalcs ? isPositionValid : isPositionValidCommitted)
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

    IPromise MoveToTile(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null) {
        var promise = new Promise();
        if (isMoving) {
            promise.Reject(new ApplicationException("Actor " + gameObject.name + " is already moving"));
            return promise;
        }
        isMoving = true;
        StartCoroutine(TweenToTile(goal, speed, isPositionValid, success => {
            isMoving = false;
            if (!success) {
                promise.Reject(new PathBlockedException(goal));
            }else {
                promise.Resolve();
            }
        }));
        return promise;
    }

    IEnumerator TweenToTile(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null, Action<bool> onEnd = null) {
        if (isPositionValid == null) isPositionValid = _ => true;

        var start = CachedTilePosition.Position;
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

            CachedTilePosition.WorldPosition = Vector2.Lerp(start.ToVector2(), goal.ToVector2(), progress);

            if (progress >= 1) {
                if (reversing) yield return new WaitForSeconds(UnityEngine.Random.value > 0.5 ? UnityEngine.Random.Range(0.25f, 1f) : 0);
                if (onEnd != null) onEnd(!reversing);
                yield break;
            }

            yield return null;

            progress += speed * Time.deltaTime;
        }
    }
}

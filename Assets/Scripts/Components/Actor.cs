using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RSG;
using System;

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
        MoveToTile(new Vec2i(0, 0), 0.1f)
            .Catch(e => Debug.LogError(e))
            .Then(() => Debug.Log("yay!"));
    }

    IPromise MoveToTile(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null) {
        var promise = new Promise();
        if (isMoving) {
            promise.Reject(new ApplicationException("Actor " + gameObject.name + " is already moving"));
            return promise;
        }
        isMoving = true;
        StartCoroutine(TweenToTile(goal, speed, isPositionValid, () => {
            isMoving = false;
            promise.Resolve();
        }));
        return promise;
    }

    IEnumerator TweenToTile(Vec2i goal, float speed, Func<Vec2i, bool> isPositionValid = null, Action onEnd = null) {
        if (isPositionValid == null) isPositionValid = _ => true;

        var start = CachedTilePosition.Position;
        var delta = new Vector2(goal.x - start.x, goal.y - start.y);
        bool reversing = false;
        if (delta != Vector2. zero) {
            float time = 0f;
            while (true) {
                time += Time.deltaTime;

                float progress = time * speed;
                if (progress >= 1) progress = 1;
                if (!reversing && !isPositionValid(goal)) {
                    reversing = true;
                    time *= (1 - progress);
                    goal = start;
                    start = goal;
                }

                var accumulatedDelta = delta;
                accumulatedDelta.Scale(new Vector2(progress, progress));
                var newPos = start.ToVector2() + accumulatedDelta;
                CachedTilePosition.WorldPosition = newPos;

                if (progress >= 1) {
                    break;
                }

                yield return null;
            }
        }
        if (onEnd != null) onEnd();
    }}

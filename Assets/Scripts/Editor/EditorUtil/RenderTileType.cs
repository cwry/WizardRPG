using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public partial class EditorUtil {
    public static bool RenderTileOutlines;
    public static bool RenderMapTileTypes;
    public static bool RenderStaticObjectTileTypes;
    static bool RenderTileTypeInfo = true;

    public static void RenderTileTypes() {
        RenderTileTypeInfo = EditorGUILayout.Foldout(RenderTileTypeInfo, "Render Tile Types");
        if (RenderTileTypeInfo) {
            RenderTileOutlines = EditorGUILayout.Toggle("RenderTileOutlines", RenderTileOutlines);
            RenderMapTileTypes = EditorGUILayout.Toggle("RenderMapTileTypes", RenderMapTileTypes);
            RenderStaticObjectTileTypes = EditorGUILayout.Toggle("RenderStaticObjectTileTypes", RenderStaticObjectTileTypes);
        }
    }
    public static void DrawTileTypes(SceneView view) { 
        if (MapInfo.Current == null || MapInfo.Current.TileTypes == null) return;

        Flattened2DArray<TileType> arr = null;
        if (RenderMapTileTypes && RenderStaticObjectTileTypes) {
            arr = MapInfo.Current.TileTypes;
        } else if (RenderMapTileTypes) {
            arr = MapInfo.Current.MapTileTypes;
        } else if (RenderStaticObjectTileTypes) {
            arr = MapInfo.Current.StaticObjectTileTypes;
        }

        if (arr != null) {
            for (int x = 0; x < arr.Width; x++) {
                for (int y = 0; y < arr.Height; y++) {
                    if (arr[x, y] == TileType.NONE) continue;
                    var color = TileTypeUtil.ToColor(arr[x, y]);
                    color.a = 0.5f;
                    Handles.color = color;
                    Handles.CubeCap(0, new Vector3(x, y), Quaternion.identity, 1);
                }
            }
        }

        if (RenderTileOutlines) {
            Handles.color = Color.black;
            for (int i = 0; i <= MapInfo.Current.Height; i++) {
                Handles.DrawLine(new Vector3(i - 0.5f, -0.5f), new Vector3(i - 0.5f, MapInfo.Current.Height - 0.5f));
            }

            for (int i = 0; i <= MapInfo.Current.Width; i++) {
                Handles.DrawLine(new Vector3(-0.5f, i - 0.5f), new Vector3(MapInfo.Current.Width - 0.5f, i - 0.5f));
            }
        }
    }
}

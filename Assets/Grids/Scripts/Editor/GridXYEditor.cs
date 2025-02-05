using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridXY))]
public class GridXYEditor : Editor
{
    bool showGrid = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GridXY grid = (GridXY)target;
        GUILayout.BeginHorizontal();
        
        
        if (!showGrid)
        {
            if (GUILayout.Button("Show Grid"))
            {
                showGrid = true;
                grid.ShowGird(true);
                grid.GenerateGird();
                EditorUtility.SetDirty(grid);
            }
        }
        else
        {
            if (GUILayout.Button("Hide Grid"))
            {
                showGrid = false;
                grid.ShowGird(false);
                EditorUtility.SetDirty(grid);
            }
        }

        if (GUILayout.Button("Generate Grid"))
        {
            //grid.GenerateGridGameObjectInterface();
        }
        
        GUILayout.EndHorizontal();
    }
}

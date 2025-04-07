using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainManager))]
public class TerrainManagerEditor : Editor
{
    private TerrainManager terrainManager;

    private static string autoupdateOn = "Automatic updating enabled";
    private static string autoupdateOff = "Automatic updating disabled";

    private static string autoUpdateStr = autoupdateOn;

    private static bool recentlyDeletedChunks = false;
    private static bool autoUpdate = true;
    private bool update = false;
    public override void OnInspectorGUI()
    {
        // Gets reference to target planet
        terrainManager = (TerrainManager) target;


        Editor editor = CreateEditor(terrainManager);

        update |= editor.DrawDefaultInspector();

        Buttons();

    }

    private void Buttons()
    {
        if (GUILayout.Button(autoUpdateStr))
        {
            autoUpdate = !autoUpdate;
            if (autoUpdateStr.Equals(autoupdateOn))
            {
                autoUpdateStr = autoupdateOff;
            }
            else
            {
                autoUpdateStr = autoupdateOn;
            }
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate Mesh") || (update && autoUpdate && !recentlyDeletedChunks))
        {
            terrainManager.Start();
            recentlyDeletedChunks = false;
        }

        if (GUILayout.Button("Delete Mesh"))
        {
            terrainManager.DeleteChunks();
            recentlyDeletedChunks = true;
        }
        GUILayout.EndHorizontal();
    }
}

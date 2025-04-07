using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlanetTerrain))]
public class PlanetTerrainEditor : Editor
{
    private PlanetTerrain planet;

    private static string autoupdateOn = "Automatic updating enabled";
    private static string autoupdateOff = "Automatic updating disabled";

    private static string autoUpdateStr = autoupdateOn;

    private static bool recentlyDeletedPlanet = false;
    private static bool autoUpdate = true;
    private bool update = false;
    public override void OnInspectorGUI()
    {
        // Gets reference to target planet
        planet = (PlanetTerrain) target;


        Editor editor = CreateEditor(planet);

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
        if (GUILayout.Button("Generate Mesh") || (update && autoUpdate && !recentlyDeletedPlanet))
        {
            planet.Start();
            recentlyDeletedPlanet = false;
        }

        if (GUILayout.Button("Delete Mesh"))
        {
            planet.DeleteTerrain();
            recentlyDeletedPlanet = true;
        }
        GUILayout.EndHorizontal();
    }
}

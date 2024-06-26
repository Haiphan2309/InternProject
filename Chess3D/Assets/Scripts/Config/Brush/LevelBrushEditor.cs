using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

[CustomEditor(typeof(LevelBrush))]
public class LevelBrushEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelBrush brush = (LevelBrush)target;

        // Dropdown for selecting the target Tilemap
        EditorGUILayout.LabelField("Target Tilemap", EditorStyles.boldLabel);

        Tilemap[] tilemaps = FindObjectsOfType<Tilemap>();
        List<string> tilemapNames = new List<string>();
        foreach (Tilemap tilemap in tilemaps)
        {
            tilemapNames.Add(tilemap.name);
        }

        int selectedTilemapIndex = Mathf.Max(tilemapNames.IndexOf(brush.targetTilemapName), 0);
        selectedTilemapIndex = EditorGUILayout.Popup("Active Tilemap", selectedTilemapIndex, tilemapNames.ToArray());
        brush.targetTilemapName = tilemapNames[selectedTilemapIndex];

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("Prefab List", EditorStyles.boldLabel);

        // Display the list of prefabs
        for (int i = 0; i < brush.prefabs.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            brush.prefabs[i] = (GameObject)EditorGUILayout.ObjectField(brush.prefabs[i], typeof(GameObject), false);

            if (GUILayout.Button("Remove"))
            {
                brush.prefabs.RemoveAt(i);
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Prefab"))
        {
            brush.prefabs.Add(null);
        }

        // Display a dropdown to select the current prefab
        EditorGUILayout.Space();
        brush.selectedIndex = EditorGUILayout.Popup("Selected Prefab", brush.selectedIndex, brush.prefabs.ConvertAll(p => p != null ? p.name : "None").ToArray());

        // Checkbox for manual offset
        brush.manualOffset = EditorGUILayout.Toggle("Manual Offset", brush.manualOffset);

        if (brush.manualOffset)
        {
            // Field for manual offset value
            brush.manualOffsetValue = EditorGUILayout.IntField("Manual Offset Value", brush.manualOffsetValue);
        }

        // Dropdown for rotation angle
        brush.rotationAngle = (LevelBrush.RotationAngle)EditorGUILayout.EnumPopup("Rotation Angle", brush.rotationAngle);

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(brush);
        }
    }
}
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelBrush))]
public class LevelBrushEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LevelBrush brush = (LevelBrush)target;

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

        // Checkbox for rotation with tile
        brush.rotateWithTile = EditorGUILayout.Toggle("Rotate With Tile", brush.rotateWithTile);

        if (brush.rotateWithTile)
        {
            // Field for rotation angle
            brush.rotationAngle = EditorGUILayout.FloatField("Rotation Angle", brush.rotationAngle);
        }

        // Save changes
        if (GUI.changed)
        {
            EditorUtility.SetDirty(brush);
        }
    }
}
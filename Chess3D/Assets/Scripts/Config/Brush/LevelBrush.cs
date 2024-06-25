using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Level Brush", menuName = "Brushes/Level Brush", order = 1)]
[CustomGridBrush(false, true, false, "LevelBrush")]
public class LevelBrush : GridBrushBase
{
    public List<GameObject> prefabs = new List<GameObject>();
    public int selectedIndex = 0;
    public bool manualOffset = false;
    public int manualOffsetValue = 0;

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (prefabs != null && prefabs.Count > 0 && brushTarget != null)
        {
            GameObject selectedPrefab = prefabs[selectedIndex];

            if (selectedPrefab != null)
            {
                // Calculate the world position of the cell and center the GameObject within the cell
                Vector3 cellWorldPosition = grid.CellToWorld(position);
                int offset = manualOffset ? manualOffsetValue : GetOffsetBasedOnTilemapName(brushTarget);

                // Instantiate the prefab
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);

                // Adjust the position to center the GameObject within the cell
                Vector3 adjustedPosition = new Vector3(cellWorldPosition.x + 0.5f, 0, cellWorldPosition.z + 0.5f);
                adjustedPosition.y += offset;

                // Set the position and parent of the instance
                instance.transform.position = adjustedPosition;
                instance.transform.parent = brushTarget.transform;

                // Register the created object for undo functionality
                Undo.RegisterCreatedObjectUndo(instance, "Painted GameObject");
            }
        }
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (brushTarget != null)
        {
            // Calculate the world position of the cell and center the GameObject within the cell
            Vector3 cellWorldPosition = grid.CellToWorld(position);
            int offset = manualOffset ? manualOffsetValue : GetOffsetBasedOnTilemapName(brushTarget);

            // Adjust the position to match the position used in Paint
            Vector3 adjustedPosition = new Vector3(cellWorldPosition.x + 0.5f, 0, cellWorldPosition.z + 0.5f);
            adjustedPosition.y += offset;

            // Find the object at the adjusted position and remove it
            Transform erased = GetObjectInCell(brushTarget.transform, adjustedPosition);
            if (erased != null)
            {
                Undo.DestroyObjectImmediate(erased.gameObject);
            }
        }
    }

    private Transform GetObjectInCell(Transform parent, Vector3 position)
    {
        foreach (Transform child in parent)
        {
            if (child.position == position)
            {
                return child;
            }
        }
        return null;
    }

    private int GetOffsetBasedOnTilemapName(GameObject brushTarget)
    {
        if (brushTarget != null)
        {
            string name = brushTarget.name;
            if (name.StartsWith("Level_"))
            {
                if (int.TryParse(name.Substring(6), out int level))
                {
                    return level;
                }
            }
        }
        return 0; // Default offset if no valid level is found
    }
}
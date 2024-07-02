using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Level Brush", menuName = "Brushes/Level Brush", order = 1)]
[CustomGridBrush(false, true, false, "LevelBrush")]
public class LevelBrush : GridBrushBase
{
    // Active tilemap
    public string targetTilemapName = "";

    // Add and set the Prefab to paint
    public List<GameObject> prefabs = new List<GameObject>();
    public int selectedIndex = 0;

    // Toggle to manual set offset
    public bool manualOffset = false;

    // Manually set the offset value (Only use when manual offset == true)
    public int manualOffsetValue = 0;

    // Toggle to rotate GameObject with the tile
    //public bool rotateWithTile = true;

    // Enum for rotation angles
    //public RotationAngle rotationAngle = RotationAngle.Degree0; 
    //public enum RotationAngle
    //{
    //    Degree0 = 0,
    //    Degree90 = 90,
    //    Degree180 = 180,
    //    Degree270 = 270
    //}

    public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        if (prefabs != null && prefabs.Count > 0 && brushTarget != null)
        {
            GameObject selectedPrefab = prefabs[selectedIndex];
            Tilemap targetTilemap = GetTilemapByName(targetTilemapName);

            if (selectedPrefab != null && targetTilemap != null)
            {
                // Calculate the world position of the cell and center the GameObject within the cell
                Vector3 cellWorldPosition = grid.CellToWorld(position);
                int offset = manualOffset ? manualOffsetValue : GetOffsetBasedOnTilemapName(targetTilemap);
                //Quaternion rotation = Quaternion.identity;

                //if (rotateWithTile)
                //{
                //    rotation = Quaternion.Euler(0, (float)rotationAngle, 0);
                //}

                // Adjust the position to center the GameObject within the cell
                Vector3 adjustedPosition = new Vector3(cellWorldPosition.x + 0.5f, 0, cellWorldPosition.z + 0.5f);
                adjustedPosition.y += offset;

                // Check the cell have object or not
                Transform duplicateObject = GetObjectInCell(grid, targetTilemap.transform, adjustedPosition);

                if (duplicateObject != null) return;

                // Instantiate the prefab
                GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);

                // Set rotation
                //instance.transform.rotation = rotation;

                // Set the position and parent of the instance
                instance.transform.position = adjustedPosition;

                if (targetTilemap != null)
                {
                    instance.transform.parent = targetTilemap.transform;
                }

                // Register the created object for undo functionality
                Undo.RegisterCreatedObjectUndo(instance, "Painted GameObject");
            }
        }
    }

    public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
    {
        Tilemap targetTilemap = GetTilemapByName(targetTilemapName);
        if (brushTarget != null)
        {
            // Calculate the world position of the cell and center the GameObject within the cell
            Vector3 cellWorldPosition = grid.CellToWorld(position);
            int offset = manualOffset ? manualOffsetValue : GetOffsetBasedOnTilemapName(targetTilemap);

            // Adjust the position to match the position used in Paint
            Vector3 adjustedPosition = new Vector3(cellWorldPosition.x + 0.5f, 0, cellWorldPosition.z + 0.5f);
            adjustedPosition.y += offset;

            Debug.Log(adjustedPosition);

            // Find the object at the adjusted position and remove it
            Transform erased = GetObjectInCell(grid, targetTilemap.transform, adjustedPosition);
            Debug.Log((erased == null));
            if (erased != null)
            {
                Undo.DestroyObjectImmediate(erased.gameObject);
            }
        }
    }

    private Transform GetObjectInCell(GridLayout grid, Transform parent, Vector3 position)
    {
        //Vector3 worldPosition = grid.CellToWorld(position);
        foreach (Transform child in parent)
        {
            if (child.position == position)
            {
                return child;
            }
        }
        return null;
    }

    private int GetOffsetBasedOnTilemapName(Tilemap tilemap)
    {
        if (tilemap != null)
        {
            string name = tilemap.name;
            if (name.StartsWith("Floor_"))
            {
                if (int.TryParse(name.Substring(6), out int level))
                {
                    return level;
                }
            }
        }
        return 0; // Default offset if no valid level is found
    }

    private Tilemap GetTilemapByName(string name)
    {
        Tilemap[] tilemaps = Object.FindObjectsOfType<Tilemap>();
        foreach (Tilemap tilemap in tilemaps)
        {
            if (tilemap.name == name)
            {
                return tilemap;
            }
        }
        return null;
    }
}
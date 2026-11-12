using UnityEngine;
using UnityEngine.Tilemaps;

public class IsoGridReader : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private TileBase goalTile;
    [SerializeField] private TileBase hazardTile;

    // Convert grid cell (e.g., (2, 3, 0)) to exact world position for PrimeTween movement
    public Vector3 GridToWorld(Vector3Int cell)
    {
        return groundTilemap.GetCellCenterWorld(cell);
    }

    // Check what type of tile is at a given cell before or after moving
    public bool IsWalkable(Vector3Int cell)
    {
        return groundTilemap.HasTile(cell); // True if any floor tile is painted here
    }

    public bool IsGoal(Vector3Int cell) => groundTilemap.GetTile(cell) == goalTile;
    public bool IsHazard(Vector3Int cell) => groundTilemap.GetTile(cell) == hazardTile;
}
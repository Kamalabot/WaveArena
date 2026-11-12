using UnityEngine;
using UnityEngine.Tilemaps;
using PrimeTween;

public class IsoGridHopper : MonoBehaviour
{
    [Header("Tilemap & Grid")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Vector3Int startCell = Vector3Int.zero;

    [Header("Hop Feel")]
    [SerializeField] private float hopDuration = 0.22f;
    [SerializeField] private float hopArcHeight = 0.35f;
    [SerializeField] private Vector3 visualOffset = new Vector3(0f, 0.25f, 0f);

    private Vector3Int currentCell;
    private bool isMoving;
    private Vector3 baseScale;

    private void Start()
    {
        if (groundTilemap == null)
        {
            Debug.LogError("[IsoGridHopper] Assign Ground Tilemap in the Inspector.", this);
            enabled = false;
            return;
        }

        baseScale = transform.localScale;
        currentCell = startCell;
        transform.position = groundTilemap.GetCellCenterWorld(currentCell) + visualOffset;
    }

    private void Update()
    {
        if (isMoving) return;

        Vector3Int step = Vector3Int.zero;

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            step = Vector3Int.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            step = Vector3Int.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            step = Vector3Int.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            step = Vector3Int.right;

        if (step != Vector3Int.zero)
        {
            TryHopTo(currentCell + step);
        }
    }

    /// <summary>
    /// Moves the character one isometric cell if a valid tile exists at targetCell.
    /// Can also be called directly by a command queue (MoveForward, etc.).
    /// </summary>
    public bool TryHopTo(Vector3Int targetCell)
    {
        if (isMoving) return false;

        // Prevent walking off painted tiles
        if (!groundTilemap.HasTile(targetCell))
        {
            // Subtle bump feedback when blocked
            Tween.PunchScale(transform, new Vector3(0.15f, -0.15f, 0f), duration: 0.18f);
            return false;
        }

        isMoving = true;
        Vector3 startPos = transform.position;
        Vector3 endPos = groundTilemap.GetCellCenterWorld(targetCell) + visualOffset;

        // Flip sprite horizontally based on screen-space X direction
        if (!Mathf.Approximately(endPos.x, startPos.x))
        {
            float dirX = Mathf.Sign(endPos.x - startPos.x);
            transform.localScale = new Vector3(Mathf.Abs(baseScale.x) * dirX, baseScale.y, baseScale.z);
        }

        // 1. Parabolic hop interpolation from startPos to endPos
        Tween.Custom(0f, 1f, new TweenSettings(hopDuration, Ease.OutQuad), t =>
        {
            Vector3 flatPos = Vector3.Lerp(startPos, endPos, t);
            float arcY = 4f * hopArcHeight * t * (1f - t); // Smooth parabola peaking at t = 0.5
            transform.position = flatPos + new Vector3(0f, arcY, 0f);
        })
        .OnComplete(() =>
        {
            currentCell = targetCell;
            transform.position = endPos;
            isMoving = false;
        });

        // 2. Simultaneous stretch-on-takeoff, squash-on-landing
        Sequence.Create()
            .Chain(Tween.ScaleY(transform, baseScale.y * 1.18f, new TweenSettings(hopDuration * 0.45f, Ease.OutQuad)))
            .Chain(Tween.ScaleY(transform, baseScale.y * 0.85f, new TweenSettings(hopDuration * 0.35f, Ease.InQuad)))
            .Chain(Tween.ScaleY(transform, baseScale.y, new TweenSettings(hopDuration * 0.20f, Ease.OutBack)));

        return true;
    }

    public Vector3Int CurrentCell => currentCell;
    public bool IsMoving => isMoving;
}
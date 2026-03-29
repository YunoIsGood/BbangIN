using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerDig : MonoBehaviour
{
    public Tilemap groundTilemap;
    public Tilemap safeZoneTilemap; 
    public LayerMask treasureLayer;
    public float digRadius = 1.5f;
    public float collectRadius = 1.2f;

    void Update()
    {
        if (GameManager.instance != null && GameManager.instance.isSafeZone) return;

        if (Input.GetKeyDown(KeyCode.C)) DigGround();
        if (Input.GetKeyDown(KeyCode.E)) TryCollectTreasure();
    }

    void DigGround()
    {
        if (groundTilemap == null) return;
        Vector3Int playerCellPos = groundTilemap.WorldToCell(transform.position);
        int range = Mathf.CeilToInt(digRadius);

        for (int x = -range; x <= range; x++) {
            for (int y = -range; y <= range; y++) {
                Vector3Int tilePos = new Vector3Int(playerCellPos.x + x, playerCellPos.y + y, 0);
                if (Vector2.Distance(transform.position, groundTilemap.GetCellCenterWorld(tilePos)) <= digRadius)
                {
                    if (safeZoneTilemap != null && safeZoneTilemap.HasTile(tilePos)) continue;
                    groundTilemap.SetTile(tilePos, null);
                }
            }
        }
    }

    void TryCollectTreasure()
    {
        Collider2D[] treasures = Physics2D.OverlapCircleAll(transform.position, collectRadius, treasureLayer);
        foreach (Collider2D t in treasures) {
            Vector3Int cellPos = groundTilemap.WorldToCell(t.transform.position);
            if (!groundTilemap.HasTile(cellPos)) { 
                Debug.Log("보물을 수집했습니다.");
                Destroy(t.gameObject);
            }
        }
    }
}
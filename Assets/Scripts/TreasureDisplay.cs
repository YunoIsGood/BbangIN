using UnityEngine;
using UnityEngine.Tilemaps;

public class TreasureDisplay : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private RectTransform canvasRect;
    private Transform playerTransform;

    [Header("데이터")]
    public string treasureName;
    public int price;

    
    public Tilemap groundTilemap;    // 일반 땅 타일맵 드래그
    public Tilemap safeZoneTilemap;  // 세이프존 타일맵 드래그

    [Header("UI 설정")]
    public GameObject uiPrefab;
    public float showDistance = 3.0f; 
    
    private GameObject myUI;
    private RectTransform uiRect;

    void Awake()
    {
        groundTilemap = GameObject.Find("Ground").GetComponent<Tilemap>();
        safeZoneTilemap = GameObject.Find("SafeZone").GetComponent<Tilemap>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // 캔버스와 플레이어는 씬에서 자동으로 찾음
        Canvas canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas != null) canvasRect = canvas.GetComponent<RectTransform>();
        
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null) playerTransform = player.transform;
    }

    public void SetTreasure(TreasureData data)
    {
        if (data == null) return;
        treasureName = data.name;
        price = data.price;
        if (spriteRenderer != null) spriteRenderer.sprite = data.sprite;
        gameObject.name = data.name;
    }

    void Update()
    {
        // 직접 연결된 타일맵이 없으면 아무것도 안 함 (Mismatch 방지)
        if (groundTilemap == null || safeZoneTilemap == null || Camera.main == null || uiPrefab == null || canvasRect == null || playerTransform == null) return;

        // 보물 위치를 타일 좌표로 변환
        Vector3Int cellPos = groundTilemap.WorldToCell(transform.position);

        // 1. 타일 존재 여부 체크
        bool hasGround = groundTilemap.HasTile(cellPos);
        bool hasSafeZone = safeZoneTilemap.HasTile(cellPos);

        // 땅도 없고 세이프존도 없을 때 노출된 것으로 판정
        bool isExposed = !hasGround && !hasSafeZone;

        // 2. 거리 계산
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        // 3. 조건 충족 시 UI 표시
        if (isExposed && distance <= showDistance)
        {
            if (myUI == null)
            {
                myUI = Instantiate(uiPrefab, canvasRect);
                uiRect = myUI.GetComponent<RectTransform>();
            }
            myUI.SetActive(true);

            Vector3 worldPos = transform.position + Vector3.up * 0.8f;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(worldPos);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, screenPoint, Camera.main, out Vector2 localPoint);

            uiRect.anchoredPosition = localPoint;
        }
        else if (myUI != null)
        {
            myUI.SetActive(false);
        }
    }

    void OnDestroy()
    {
        if (myUI != null) Destroy(myUI);
    }
}
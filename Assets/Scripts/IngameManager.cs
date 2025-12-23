using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] string seed;
    MapNode[,] currentMap;
    [SerializeField] MapManager mapManager;

    [Header("Map Gizmo Settings")]
    [SerializeField] bool drawMapGizmos = true;
    [SerializeField] Vector2 mapOrigin = Vector2.zero; // (0,0) 방의 월드 좌표 기준점
    [SerializeField] float cellSize = 1f;              // 방 간 간격 / 크기

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MapGen(this.seed);
            this.mapManager.CreateMapEntity(this.currentMap);
            this.mapManager.MoveToStart();
        }
    }

    void MapGen(string _seed)
    {
        this.currentMap = MapGenerator.Generate(_seed);
    }

    public void ChangeMap(int _dir)
    {
        this.mapManager.ChangeMap(_dir);
    }

    void OnDrawGizmos()
    {
        if (!this.drawMapGizmos)
            return;

        if (this.currentMap == null)
            return;

        int t_width = this.currentMap.GetLength(0);
        int t_height = this.currentMap.GetLength(1);

        for (int t_x = 0; t_x < t_width; t_x++)
        {
            for (int t_y = 0; t_y < t_height; t_y++)
            {
                MapNode t_node = this.currentMap[t_x, t_y];

                // 방이 없는 칸도 테두리만 보여주고 싶으면 여기서 처리
                if (t_node == null)
                {
                    Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.3f);
                }
                else
                {
                    switch (t_node.roomType)
                    {
                        case RoomType.NomalMonster:
                            Gizmos.color = new Color(0.3f, 0.8f, 0.3f, 0.8f); // 초록
                            break;
                        case RoomType.EliteMonster:
                            Gizmos.color = new Color(0.9f, 0.4f, 0.2f, 0.9f); // 주황/빨강
                            break;
                        case RoomType.Treasure:
                            Gizmos.color = new Color(1.0f, 0.9f, 0.2f, 0.9f); // 노랑
                            break;
                        case RoomType.Boss:
                            Gizmos.color = new Color(0.6f, 0.2f, 0.9f, 0.9f); // 보라
                            break;
                    }
                }

                // 그리드 좌표 → 월드 좌표 변환
                Vector3 t_pos = new Vector3(
                    this.mapOrigin.x + t_x * this.cellSize,
                    this.mapOrigin.y + t_y * this.cellSize,
                    0f
                );

                Vector3 t_size = Vector3.one * (this.cellSize * 0.9f);

                if (t_node == null)
                {
                    // 방이 없으면 테두리만
                    Gizmos.DrawWireCube(t_pos, t_size);
                }
                else
                {
                    // 방이 있으면 채워진 사각형 + 테두리
                    Gizmos.DrawCube(t_pos, t_size);
                    Gizmos.color = Color.black;
                    Gizmos.DrawWireCube(t_pos, t_size);
                }
            }
        }
    }
}

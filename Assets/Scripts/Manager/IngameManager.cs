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
    [SerializeField] public GameState gameState;

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
        }
    }

    void MapGen(string _seed)
    {
        this.currentMap = MapGenerator.GenerateStage(_seed);
    }

    public void MoveMap(int _dir)
    {
        this.mapManager.MoveMap(_dir);
        
    }


}
public enum GameState
{
    Running,
    Paused
}
using UnityEngine;

public class IngameManager : MonoBehaviour
{
    public static IngameManager instance;
    private void Awake()
    {
        instance = this;
    }
    [Header("현재 게임 정보")]
    [SerializeField] string seed;
    MapNode[,] currentMap;
    [SerializeField] MapManager mapManager;
    [SerializeField] public GameState gameState;
    [SerializeField] public int currentWeapon;
    [SerializeField] PlayerCharacter player;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MapGen(this.seed);
            this.mapManager.CreateMapEntity(this.currentMap);
            this.player.Initialization();
            this.gameState = GameState.Running;
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

    public PlayerCharacter GetPlayer() => this.player;
}
public enum GameState
{
    Running,
    Paused
}
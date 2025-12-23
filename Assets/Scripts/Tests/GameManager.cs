using UnityEngine;

public class GameManagerr : MonoBehaviour
{
    #region SingleTon
    public static GameManagerr instance;
    public GameObject pauseUI;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameState gameState;
    public PlayerScript playerScript;
    [SerializeField] EnemySpawner spanwer;
    [SerializeField] LevelUpUI levelUpUI;
    public int killCount = 0;
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(this.gameState == GameState.Paused)
            {
                this.pauseUI.SetActive(false);
                this.gameState = GameState.Running;
            }
            else
            {
                this.pauseUI.SetActive(true);
                this.gameState = GameState.Paused;
            }

        }
    }

    public void AddKillCount()
    {
        this.killCount++;

        if(this.killCount >= 5)
        {
            this.killCount = 0;
            this.levelUpUI.gameObject.SetActive(true);
            gameState = GameState.Paused;
        }
    }
}

public enum GameState
{
    Running,
    Paused,
}
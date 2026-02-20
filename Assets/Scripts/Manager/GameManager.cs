using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);



        DontDestroyOnLoad(this.gameObject);
    }

    public async void ChangeToGameScene()
    {
        await SceneLoadManager.ChangeToGameScene();
        IngameManager.instance.Initialization();

    }

    public string currentSeed;
}

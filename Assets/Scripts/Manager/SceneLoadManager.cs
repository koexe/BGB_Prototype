using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneLoadManager
{
    public static void ChangeScene(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);


    }
    public async static UniTask ChangeToBattleScene()
    {
        UIManager.instance.AddOrUpdateUI<FadeUI>(new FadeUIData
        {
            identifier = FadeUI.identifier,
            fadeIn = false,
            fadeOut = true,
        });
    }

    public async static void StartGame()
    {
        // 씬 로드 (비동기)
        var t_op = SceneManager.LoadSceneAsync("GameScene");
        await t_op.ToUniTask();   // 로드 완료까지 대기
        UIManager.instance.AddOrUpdateUI<FadeUI>(new FadeUIData 
        { 
            identifier = FadeUI.identifier,
            fadeIn = false,
            fadeOut = true,
        });
    }
    public static async UniTask ChangeToGameScene()
    {
        // 씬 로드 (비동기)
        var t_op = SceneManager.LoadSceneAsync("GameScene");
        await t_op.ToUniTask();   // 로드 완료까지 대기

    }
}

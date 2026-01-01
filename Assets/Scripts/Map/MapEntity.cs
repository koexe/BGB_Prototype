using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MapEntity : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] int stage;
    [SerializeField] int roomCode;
    [SerializeField] RoomType roomType;
    [SerializeField] Transform[] doorTr;
    [SerializeField] GameObject doorPrefab;
    [Space(20)]
    [Header("Wave Settings")]
    [SerializeField] List<EnemyWaveSetting> waveSetting;
    [SerializeField] int currentKillCount;
    [SerializeField] int targetKillCount;

    [SerializeField] bool isWaveEnded;

    public (int stage, RoomType roomType) GetRoomInfo() => (this.stage, this.roomType);
    public int GetRoomCode() => this.roomCode;

    public void CreateDoors(bool _isSplited)
    {
        if (_isSplited)
        {
            Instantiate(this.doorPrefab, this.doorTr[0]).GetComponent<MapDoor>().Initialization(() => IngameManager.instance.MoveMap(0));
            Instantiate(this.doorPrefab, this.doorTr[2]).GetComponent<MapDoor>().Initialization(() => IngameManager.instance.MoveMap(1));
        }
        else
        {
            Instantiate(this.doorPrefab, this.doorTr[1]).GetComponent<MapDoor>().Initialization(() => IngameManager.instance.MoveMap(0));
        }
    }

    public void OnKill()
    {
        this.currentKillCount++;
    }

    public async void WaveStart()
    {
        for (int i = 0; i < this.waveSetting.Count; i++)
        {
            this.targetKillCount = this.waveSetting[i].GetKillCount();
            await this.waveSetting[i].StartWave(OnKill);

            while (this.targetKillCount != this.currentKillCount)
            {
                await UniTask.WaitForSeconds(1000);
            }
            this.currentKillCount = 0;
        }
    }
}


[System.Serializable]
public class EnemySetting
{
    public EnemyBase enemy;
    public Transform position;
    public float interval;

    public async UniTask Spawn(Action _enemyKillAction)
    {
        await UniTask.WaitForSeconds(this.interval * 1000);
        GameObject.Instantiate(this.enemy, this.position.position, Quaternion.identity).GetComponent<EnemyBase>().Initialization(_enemyKillAction);
    }
}


[System.Serializable]
public class EnemyWaveSetting
{
    public List<EnemySetting> enemySetting;
    public float interval;

    public async UniTask StartWave(Action _enemyKillAction)
    {
        await UniTask.WaitForSeconds(this.interval * 1000);
        for (int i = 0; i < this.enemySetting.Count; i++)
        {
            await enemySetting[i].Spawn(_enemyKillAction);
        }
    }

    public int GetKillCount() => this.enemySetting.Count;
}

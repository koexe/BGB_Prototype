using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class DataLibrary : MonoBehaviour
{
    public static DataLibrary instance;

    public void Awake()
    {
        instance = this;
        LoadAllAssets();
    }

    Dictionary<int, Dictionary<RoomType, List<MapEntity>>> mapDics;

    public async void LoadAllAssets()
    {
        await LoadMaps();
        Debug.Log("All good");
    }

    AsyncOperationHandle<IList<GameObject>> mapHandle;

    async UniTask LoadMaps()
    {
        mapDics = new Dictionary<int, Dictionary<RoomType, List<MapEntity>>>();

        this.mapHandle = Addressables.LoadAssetsAsync<GameObject>("MapEntity", (map) =>
        {
            var t_mapEntity = map.transform.GetComponent<MapEntity>();
            var t_info = t_mapEntity.GetRoomInfo();
            if (!this.mapDics.ContainsKey(t_info.stage))
            {
                this.mapDics.Add(t_info.stage, new Dictionary<RoomType, List<MapEntity>>());
                for (int i = 0; i < 5; i++)
                {
                    this.mapDics[t_info.stage].Add((RoomType)i, new List<MapEntity>());
                }
            }

            this.mapDics[t_info.stage][t_info.roomType].Add(t_mapEntity);

        });

        foreach (var t_list in this.mapDics.Values)
        {
            foreach (var t_maps in t_list.Values)
            {
                t_maps.Sort((a, b) => b.GetRoomCode().CompareTo(a.GetRoomCode()));
            }
        }

        await mapHandle.ToUniTask();
    }


    public List<MapEntity> GetMaps(int _stage, RoomType _type)
    {
        if(this.mapDics.ContainsKey(_stage))
        {
            if (this.mapDics[_stage].ContainsKey(_type))
            {
                return this.mapDics[_stage][_type];
            }
        }
        return null;
    }
}

public enum RoomType
{
    Start = 0,
    NomalMonster = 1,
    EliteMonster = 2,
    Shop = 3,
    Boss = 4
}


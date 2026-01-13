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
    Dictionary<int, WeaponInfo> weaponDics;
    Dictionary<string, GameObject> uiPrefabs;
    public async void LoadAllAssets()
    {
        await LoadMaps();
        await LoadWeapons();
        Debug.Log("All good");
    }

    AsyncOperationHandle<IList<GameObject>> mapHandle;
    AsyncOperationHandle<IList<TextAsset>> weaponInfoHandle;
    AsyncOperationHandle<IList<GameObject>> uiHandle;
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
    async UniTask LoadWeapons()
    {
        this.weaponInfoHandle = Addressables.LoadAssetsAsync<TextAsset>("WeaponInfo", (t_info) =>
        {
            this.weaponDics = CSVReader.ReadWeaponCSV(t_info);
        });
        await weaponInfoHandle.ToUniTask();
    }


    public List<MapEntity> GetMaps(int _stage, RoomType _type)
    {
        if (this.mapDics.ContainsKey(_stage))
        {
            if (this.mapDics[_stage].ContainsKey(_type))
            {
                return this.mapDics[_stage][_type];
            }
        }
        return null;
    }

    async UniTask LoadUIPrefab()
    {

        this.uiPrefabs = new Dictionary<string, GameObject>();
        this.uiHandle = Addressables.LoadAssetsAsync<GameObject>("UIPrefab", (t_prefab) =>
        {
            this.uiPrefabs.Add(t_prefab.name, t_prefab);
        });
        await uiHandle.ToUniTask();

        return;
    }

    #region Get
    public WeaponInfo GetWeaponInfo(int _code)
    {
        if (this.weaponDics.TryGetValue(_code, out var t_weaponinfo))
        {
            return t_weaponinfo;
        }
        else
        {
            LogUtil.LogError($"No such Weapon Code: {_code}");
            return null;
        }
    }
    public GameObject GetUI(string _identifier)
    {
        if (this.uiPrefabs.TryGetValue(_identifier, out var _value))
        {
            return _value;
        }
        else
        {
            LogUtil.Log("UI Name Error");
            return null;
        }
    }
    #endregion
}

public class WeaponInfo
{
    public int weaponCode;
    public string weaponName;
    public float atk;
    public float ats;
    public float reload;
    public float mag;
    public float crit;
    public float critMag;
}


public enum RoomType
{
    Start = 0,
    NomalMonster = 1,
    EliteMonster = 2,
    Shop = 3,
    Boss = 4
}

public enum StatType
{
    //Base
    Atk = 1,
    Ats = 2,
    ReloadTime = 3,
    Mag = 4,
    Crit = 5,
    CritMag = 6,
    BulletAmount = 7,
    Ricochet = 8,

    //Additional
    Wound = 101,
    Adrenaline = 102,
    ComboAttack = 103,
    Contempt = 104,
    ExplodeCorps = 105,
    TaticalSlide = 106,

}

public enum StatSign
{
    Static = 0,
    Plus = 1,
    Minus = 2,
    Percentage = 3,
}
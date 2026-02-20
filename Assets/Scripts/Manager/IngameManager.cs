using UnityEngine;
using System.Collections.Generic;

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
    [SerializeField] public EnemyBase enemyBasePrefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Initialization();
        }
    }
    public void Initialization()
    {
        MapGen(this.seed);
        this.mapManager.CreateMapEntity(this.currentMap);
        this.player.Initialization();
        this.gameState = GameState.Running;
    }

    void MapGen(string _seed)
    {
        this.currentMap = MapGenerator.GenerateStage(_seed);
    }

    public void MoveMap(int _dir)
    {
        this.mapManager.MoveMap(_dir);

    }

    public void GetReward()
    {
        var t_pos = this.mapManager.GetCurrentNodePos();
        var t_node = this.currentMap[t_pos.depth, t_pos.width];

        System.Random t_random = new System.Random(t_node.id);

        int t_depth = t_pos.depth;

        double t_optionRate;
        double t_weaponPerkRate;

        // ---------- 구간별 확률 설정 ----------
        if (t_depth <= 2) // 1~3방
        {
            t_optionRate = 0.7;
            t_weaponPerkRate = 0.3;
        }
        else if (t_depth <= 5) // 4~6방
        {
            t_optionRate = 0.6;
            t_weaponPerkRate = 0.4;
        }
        else // 7~9방 (보스 포함)
        {
            t_optionRate = 0.5;
            t_weaponPerkRate = 0.5;
        }

        // ---------- 랜덤 결정 ----------
        double t_roll = t_random.NextDouble();

        PerkType t_selectedType;

        if (t_roll < t_optionRate)
            t_selectedType = PerkType.Option;
        else
            t_selectedType = PerkType.Perk;

        // ---------- 실제 Perk 가져오기 ----------
        List<PerkInfo> t_reward =
            DataLibrary.instance.GetRandomPerk(t_selectedType, t_random, 3);
        List<RewardUIData.RewardInfo> t_rewardInfos = new List<RewardUIData.RewardInfo>();
        foreach (var t_rewardElement in t_reward)
        {
            var t_rewardInfo = new RewardUIData.RewardInfo();
            t_rewardInfo.perkInfo = t_rewardElement;
            if (t_rewardElement.perkType == PerkType.Option)
                t_rewardInfo.optionGrade = t_random.Next(0, 3);
            t_rewardInfos.Add(t_rewardInfo);
        }
        UIManager.instance.AddOrUpdateUI<RewardUI>(new RewardUIData() { identifier = RewardUI.identifier, rewardInfos = t_rewardInfos });
    }

    public PlayerCharacter GetPlayer() => this.player;
}
public enum GameState
{
    Running,
    Paused
}
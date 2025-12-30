using UnityEngine;
using System;

public static class MapGenerator
{
    /// <summary>
    /// 시드 문자열 기반 직렬+분기 경로 맵 생성
    /// 결과: [depth, lane] 형태의 2D 배열 (lane은 0~1)
    /// </summary>
    public static MapNode[,] GenerateStage(string _seedString, int _roomCount = 8)
    {
        int t_seed = Utils.FNVHash(_seedString);
        System.Random t_random = new System.Random(t_seed);

        // [depth, lane] (depth: 0~_roomCount-1, lane: 0~1)
        MapNode[,] t_rooms = new MapNode[_roomCount, 2];

        // ID 카운터 (시드 + 생성 순서 기반으로 항상 동일)
        int t_idCounter = 0;

        // ---------- depth 0: 시작 방 ----------
        {
            MapNode t_start = new MapNode();
            t_start.id = t_idCounter++;
            t_start.depth = 0;
            t_start.lane = 0;
            t_start.isBoss = false;
            t_start.hasReward = false; // 시작 방이라 보상 없음
            t_start.isElite = false;

            t_rooms[0, 0] = t_start;
            // [0,1] 은 null
        }

        // ---------- 나머지 depth ----------
        for (int t_depth = 1; t_depth < _roomCount; t_depth++)
        {
            bool t_isBoss = (t_depth == _roomCount - 1);

            if (t_isBoss)
            {
                // 보스 방은 하나만 (lane 0)
                MapNode t_boss = new MapNode();
                t_boss.id = t_idCounter++;
                t_boss.depth = t_depth;
                t_boss.lane = 0;
                t_boss.isBoss = true;
                t_boss.hasReward = false;
                t_boss.isElite = false;

                t_rooms[t_depth, 0] = t_boss;
                // [depth,1] 은 null
                continue;
            }

            // 보스가 아닌 일반 구간
            // 50% 확률로 2갈래
            bool t_hasBranch = t_random.NextDouble() < 0.5;

            if (t_hasBranch)
            {
                // 두 개의 서로 다른 보상 타입 생성
                RewardType t_first = GetRandomReward(t_random);
                RewardType t_second = GetRandomRewardExcept(t_random, t_first);

                // lane 0
                MapNode t_node0 = new MapNode();
                t_node0.id = t_idCounter++;
                t_node0.depth = t_depth;
                t_node0.lane = 0;
                t_node0.isBoss = false;
                t_node0.hasReward = true;
                t_node0.isElite = false; // 엘리트 로직 나중에 붙이면 여기서 true 처리
                t_node0.reward = t_first;

                // lane 1
                MapNode t_node1 = new MapNode();
                t_node1.id = t_idCounter++;
                t_node1.depth = t_depth;
                t_node1.lane = 1;
                t_node1.isBoss = false;
                t_node1.hasReward = true;
                t_node1.isElite = false;
                t_node1.reward = t_second;

                t_rooms[t_depth, 0] = t_node0;
                t_rooms[t_depth, 1] = t_node1;
            }
            else
            {
                // 1갈래만 존재 (lane 0)
                RewardType t_reward = GetRandomReward(t_random);

                MapNode t_node = new MapNode();
                t_node.id = t_idCounter++;
                t_node.depth = t_depth;
                t_node.lane = 0;
                t_node.isBoss = false;
                t_node.hasReward = true;
                t_node.isElite = false;
                t_node.reward = t_reward;

                t_rooms[t_depth, 0] = t_node;
                // [depth,1] 은 null
            }
        }

        return t_rooms;
    }

    // ----------------- 보상 확률 로직 -----------------

    // 무기개조 (15%), 탄약 특성 (40%), 재화(25%), 상점 (20%)
    static RewardType GetRandomReward(System.Random _r)
    {
        double t_r = _r.NextDouble();

        if (t_r < 0.15)
            return RewardType.WeaponUpgrade;

        if (t_r < 0.15 + 0.40)
            return RewardType.AmmoTrait;

        if (t_r < 0.15 + 0.40 + 0.25)
            return RewardType.Currency;

        return RewardType.Shop;
    }

    static double GetWeight(RewardType _type)
    {
        switch (_type)
        {
            case RewardType.WeaponUpgrade: return 0.15;
            case RewardType.AmmoTrait: return 0.40;
            case RewardType.Currency: return 0.25;
            case RewardType.Shop: return 0.20;
        }
        return 0.0;
    }

    // 첫 번째 타입을 제외하고 가중치 재분배
    static RewardType GetRandomRewardExcept(System.Random _r, RewardType _except)
    {
        double t_total = 1.0 - GetWeight(_except);
        double t_r = _r.NextDouble() * t_total;
        double t_acc = 0.0;

        RewardType[] t_all =
        {
            RewardType.WeaponUpgrade,
            RewardType.AmmoTrait,
            RewardType.Currency,
            RewardType.Shop
        };

        foreach (var t_type in t_all)
        {
            if (t_type == _except)
                continue;

            double t_w = GetWeight(t_type);
            t_acc += t_w;

            if (t_r <= t_acc)
                return t_type;
        }

        // 방어 코드
        foreach (var t_type in t_all)
        {
            if (t_type != _except)
                return t_type;
        }

        return RewardType.Currency;
    }
}

[System.Serializable]
public class MapNode
{
    public int id;            // 시드 + 생성 순서 기반 고유 ID
    public int depth;         // 0 ~ (_roomCount - 1)
    public int lane;          // 0 또는 1
    public bool isBoss;       // 마지막 방
    public bool hasReward;    // 보스/시작 방은 false
    public bool isElite;      // 나중에 엘리트 방 로직에 사용
    public RewardType reward; // hasReward == true 일 때 유효
}

public enum RewardType
{
    WeaponUpgrade,   // 무기 개조
    AmmoTrait,       // 탄약 특성
    Currency,        // 재화
    Shop             // 상점
}

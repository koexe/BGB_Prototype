using UnityEngine;
using System.Collections.Generic;
using System;

public class MapManager : MonoBehaviour
{
    MapEntity[,] mapEntities;
    MapNode[,] mapNodes;

    [SerializeField] MapEntity currentMap;

    [Header("Map Gizmo Settings")]
    [SerializeField] bool drawMapGizmos = true;
    [SerializeField] Vector2 gizmoOrigin = Vector2.zero; // depth 0, lane 0 위치
    [SerializeField] float gizmoCellSize = 3f;           // depth/lane 간 간격
    [SerializeField] float gizmoRoomSize = 2.5f;         // 방 표시 크기
    [SerializeField] bool drawConnections = true;        // 방 사이 선 표시

    [SerializeField] int currentDepth = 0;
    public void CreateMapEntity(MapNode[,] _map)
    {
        int t_depthCount = _map.GetLength(0);
        int t_laneCount = _map.GetLength(1); // 보통 2

        var t_mapEntitys = new MapEntity[t_depthCount, t_laneCount];

        var t_nomalBattleMap = DataLibrary.instance.GetMaps(0, RoomType.NomalMonster);
        var t_eliteBattleMap = DataLibrary.instance.GetMaps(0, RoomType.EliteMonster);
        var t_shopMap = DataLibrary.instance.GetMaps(0, RoomType.Shop);
        var t_bossMap = DataLibrary.instance.GetMaps(0, RoomType.Boss);

        for (int i = 0; i < t_depthCount; i++)
        {
            for (int j = 0; j < t_laneCount; j++)
            {
                MapNode t_node = _map[i, j];
                if (t_node == null)
                    continue;

                MapEntity t_pickedRoom = null;

                // 보스 우선 처리
                if (t_node.isBoss)
                {
                    if (t_bossMap != null && t_bossMap.Count > 0)
                    {
                        int t_idx = new System.Random(t_node.id).Next(0, t_bossMap.Count);
                        t_pickedRoom = t_bossMap[t_idx];
                    }
                }
                else
                {
                    switch (t_node.reward)
                    {
                        case RewardType.AmmoTrait:
                        case RewardType.WeaponUpgrade:
                        case RewardType.Currency:
                            if (t_node.isElite)
                            {
                                if (t_eliteBattleMap != null && t_eliteBattleMap.Count > 0)
                                {
                                    int t_idx = new System.Random(t_node.id).Next(0, t_eliteBattleMap.Count);
                                    t_pickedRoom = t_eliteBattleMap[t_idx];
                                }
                            }
                            else
                            {
                                if (t_nomalBattleMap != null && t_nomalBattleMap.Count > 0)
                                {
                                    int t_idx = new System.Random(t_node.id).Next(0, t_nomalBattleMap.Count);
                                    t_pickedRoom = t_nomalBattleMap[t_idx];
                                }
                            }
                            break;

                        case RewardType.Shop:
                            if (t_shopMap != null && t_shopMap.Count > 0)
                            {
                                int t_idx = new System.Random(t_node.id).Next(0, t_shopMap.Count);
                                t_pickedRoom = t_shopMap[t_idx];
                            }
                            break;

                        default:
                            t_pickedRoom = null;
                            break;
                    }
                }

                if (t_pickedRoom == null)
                    continue;

                var t_room = Instantiate(t_pickedRoom);

                // 다음 depth의 브랜치 여부에 따라 문 2개/1개 생성
                bool t_nextHasBranch = false;
                if (i + 1 < t_depthCount)
                {
                    t_nextHasBranch = (_map[i + 1, 0] != null && _map[i + 1, 1] != null);
                }

                t_room.CreateDoors(t_nextHasBranch);
                t_mapEntitys[i, j] = t_room;
                t_room.gameObject.SetActive(false);
            }
        }

        this.mapEntities = t_mapEntitys;
        this.mapNodes = _map;
        this.currentMap = this.mapEntities[0, 0];
        this.mapEntities[0,0].gameObject.SetActive(true);
        this.currentMap.MoveMap();
    }

    public void MoveMap(int _dir)
    {
        this.currentDepth++;

        if(this.mapEntities[this.currentDepth, _dir] == null)
        {
            LogUtil.LogWarning("not exist path");
            return;
        }
        this.currentMap.gameObject.SetActive(false);
        this.currentMap = this.mapEntities[this.currentDepth, _dir];
        this.currentMap.gameObject.SetActive(true);
        this.currentMap.MoveMap();
    }
    // ----------------- Gizmo -----------------

    void OnDrawGizmos()
    {
        if (!this.drawMapGizmos)
            return;

        if (this.mapNodes == null)
            return;

        int t_depthCount = this.mapNodes.GetLength(0);
        int t_laneCount = this.mapNodes.GetLength(1);

        // 방 그리기
        for (int i = 0; i < t_depthCount; i++)
        {
            for (int j = 0; j < t_laneCount; j++)
            {
                MapNode t_node = this.mapNodes[i, j];
                if (t_node == null)
                    continue;

                // 색 지정
                Color t_color;

                if (t_node.isBoss)
                {
                    t_color = new Color(0.8f, 0.2f, 0.2f, 0.9f); // 보스 - 빨강
                }
                else if (!t_node.hasReward)
                {
                    t_color = new Color(0.8f, 0.8f, 0.8f, 0.9f); // 시작 방 등
                }
                else if (t_node.isElite)
                {
                    t_color = new Color(0.8f, 0.3f, 1.0f, 0.9f); // 엘리트
                }
                else
                {
                    switch (t_node.reward)
                    {
                        case RewardType.WeaponUpgrade:
                            t_color = new Color(0.3f, 0.8f, 1.0f, 0.9f); // 파랑 계열
                            break;
                        case RewardType.AmmoTrait:
                            t_color = new Color(0.3f, 1.0f, 0.5f, 0.9f); // 초록 계열
                            break;
                        case RewardType.Currency:
                            t_color = new Color(1.0f, 0.9f, 0.3f, 0.9f); // 노랑 계열
                            break;
                        case RewardType.Shop:
                            t_color = new Color(1.0f, 0.5f, 0.2f, 0.9f); // 주황 계열
                            break;
                        default:
                            t_color = new Color(0.5f, 0.5f, 0.5f, 0.9f);
                            break;
                    }
                }

                Vector3 t_pos = GetNodePosition(i, j);
                Vector3 t_size = Vector3.one * this.gizmoRoomSize;

                Gizmos.color = t_color;
                Gizmos.DrawCube(t_pos, t_size);

                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(t_pos, t_size);
            }
        }

        // 연결선 그리기
        if (!this.drawConnections)
            return;

        Gizmos.color = Color.white;

        for (int i = 0; i < t_depthCount - 1; i++)
        {
            for (int j = 0; j < t_laneCount; j++)
            {
                MapNode t_from = this.mapNodes[i, j];
                if (t_from == null)
                    continue;

                Vector3 t_fromPos = GetNodePosition(i, j);

                for (int nj = 0; nj < t_laneCount; nj++)
                {
                    MapNode t_to = this.mapNodes[i + 1, nj];
                    if (t_to == null)
                        continue;

                    Vector3 t_toPos = GetNodePosition(i + 1, nj);
                    Gizmos.DrawLine(t_fromPos, t_toPos);
                }
            }
        }
    }

    Vector3 GetNodePosition(int _depth, int _lane)
    {
        // depth는 X축, lane은 Y축으로 배치
        float t_x = this.gizmoOrigin.x + _depth * this.gizmoCellSize;
        float t_y = this.gizmoOrigin.y + _lane * this.gizmoCellSize;
        return new Vector3(t_x, t_y, 0f);
    }
}

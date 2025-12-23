using UnityEngine;
using System.Collections.Generic;

public static class MapGenerator
{
    public static MapNode[,] Generate(
        string _seed,
        int _width = 5,
        int _height = 5,
        int _minRooms = 10,
        int _maxRooms = 12)
    {
        int t_seed = StringToSeed(_seed);

        // 🔹 System.Random 기반 RNG 생성
        System.Random t_random = new System.Random(t_seed);

        MapNode[,] t_grid = new MapNode[_width, _height];

        int t_targetRoomCount = NextRange(t_random, _minRooms, _maxRooms + 1);

        // 중앙 시작
        Vector2Int t_center = new Vector2Int(_width / 2, _height / 2);
        Vector2Int t_currentPos = t_center;

        List<Vector2Int> t_roomPositions = new List<Vector2Int>();

        // 시작 방 생성
        CreateRoom(t_grid, t_currentPos, t_roomPositions);

        // 4방향
        Vector2Int[] t_dirs =
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
        };

        // 1. 방 배치
        while (t_roomPositions.Count < t_targetRoomCount)
        {
            List<Vector2Int> t_candidates = new List<Vector2Int>();
            List<float> t_weights = new List<float>();

            for (int t_i = 0; t_i < t_dirs.Length; t_i++)
            {
                Vector2Int t_nextPos = t_currentPos + t_dirs[t_i];

                if (!IsInside(t_nextPos, _width, _height))
                    continue;

                if (t_grid[t_nextPos.x, t_nextPos.y] != null)
                    continue;

                int t_adjCount = CountAdjacentRooms(t_grid, t_nextPos, _width, _height);
                float t_weight = 1f / (1 + t_adjCount);

                t_candidates.Add(t_nextPos);
                t_weights.Add(t_weight);
            }

            if (t_candidates.Count == 0)
            {
                t_currentPos = t_roomPositions[NextRange(t_random, 0, t_roomPositions.Count)];
                continue;
            }

            Vector2Int t_chosenPos = WeightedRandomChoice(t_random, t_candidates, t_weights);

            CreateRoom(t_grid, t_chosenPos, t_roomPositions);
            t_currentPos = t_chosenPos;
        }

        // 2. BFS 거리 계산
        int[,] t_dist = new int[_width, _height];
        for (int x = 0; x < _width; x++)
            for (int y = 0; y < _height; y++)
                t_dist[x, y] = -1;

        Queue<Vector2Int> t_queue = new Queue<Vector2Int>();
        t_queue.Enqueue(t_center);
        t_dist[t_center.x, t_center.y] = 0;

        while (t_queue.Count > 0)
        {
            Vector2Int t_pos = t_queue.Dequeue();

            for (int i = 0; i < t_dirs.Length; i++)
            {
                Vector2Int t_next = t_pos + t_dirs[i];

                if (!IsInside(t_next, _width, _height)) continue;
                if (t_grid[t_next.x, t_next.y] == null) continue;
                if (t_dist[t_next.x, t_next.y] != -1) continue;

                t_dist[t_next.x, t_next.y] = t_dist[t_pos.x, t_pos.y] + 1;
                t_queue.Enqueue(t_next);
            }
        }

        // 기본은 전부 일반 몬스터
        foreach (Vector2Int p in t_roomPositions)
            t_grid[p.x, p.y].roomType = RoomType.NomalMonster;

        // 3-1. 보스 방 선정
        int t_maxDist = -1;
        List<Vector2Int> t_farthestRooms = new List<Vector2Int>();

        foreach (Vector2Int p in t_roomPositions)
        {
            int d = t_dist[p.x, p.y];
            if (d > t_maxDist)
            {
                t_maxDist = d;
                t_farthestRooms.Clear();
                t_farthestRooms.Add(p);
            }
            else if (d == t_maxDist)
            {
                t_farthestRooms.Add(p);
            }
        }

        Vector2Int t_bossPos = t_farthestRooms[NextRange(t_random, 0, t_farthestRooms.Count)];
        t_grid[t_bossPos.x, t_bossPos.y].roomType = RoomType.Boss;

        // 3-2. Elite 선정
        List<Vector2Int> t_eliteCandidates = new List<Vector2Int>();

        foreach (Vector2Int p in t_roomPositions)
        {
            if (p == t_center) continue;
            if (p == t_bossPos) continue;
            if (t_dist[p.x, p.y] >= 2)
                t_eliteCandidates.Add(p);
        }

        int t_eliteCount = Mathf.Min(
            NextRange(t_random, 1, 3), // 1~2개
            t_eliteCandidates.Count
        );

        Shuffle(t_random, t_eliteCandidates);

        for (int i = 0; i < t_eliteCount; i++)
        {
            Vector2Int p = t_eliteCandidates[i];
            t_grid[p.x, p.y].roomType = RoomType.EliteMonster;
        }

        // 3-3. Treasure 선정
        List<Vector2Int> t_treasureCandidates = new List<Vector2Int>();

        foreach (Vector2Int p in t_roomPositions)
        {
            if (p == t_center) continue;
            if (p == t_bossPos) continue;
            if (t_grid[p.x, p.y].roomType == RoomType.NomalMonster)
                t_treasureCandidates.Add(p);
        }

        int t_treasureCount = Mathf.Min(
            NextRange(t_random, 1, 4), // 1~3
            t_treasureCandidates.Count
        );

        Shuffle(t_random, t_treasureCandidates);

        for (int i = 0; i < t_treasureCount; i++)
        {
            Vector2Int p = t_treasureCandidates[i];
            t_grid[p.x, p.y].roomType = RoomType.Treasure;
        }

        return t_grid;
    }

    // ----------------- RNG Wrappers -----------------

    static int NextRange(System.Random _r, int _min, int _max)
    {
        return _r.Next(_min, _max); // max는 포함 안됨
    }

    static float NextFloat(System.Random _r)
    {
        return (float)_r.NextDouble();
    }

    // -----------------------------------------------

    static bool IsInside(Vector2Int _pos, int _width, int _height)
    {
        return _pos.x >= 0 && _pos.x < _width &&
               _pos.y >= 0 && _pos.y < _height;
    }

    static void CreateRoom(MapNode[,] _grid, Vector2Int _pos, List<Vector2Int> _roomPositions)
    {
        if (_grid[_pos.x, _pos.y] != null)
            return;

        MapNode t_node = new MapNode();
        t_node.id = $"{_pos.x}_{_pos.y}";
        t_node.gridPos = _pos;

        _grid[_pos.x, _pos.y] = t_node;
        _roomPositions.Add(_pos);
    }

    static int CountAdjacentRooms(MapNode[,] _grid, Vector2Int _pos, int _width, int _height)
    {
        int t_count = 0;
        Vector2Int[] dirs =
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
        };

        foreach (var d in dirs)
        {
            Vector2Int nxt = _pos + d;
            if (IsInside(nxt, _width, _height) &&
                _grid[nxt.x, nxt.y] != null)
                t_count++;
        }

        return t_count;
    }

    static Vector2Int WeightedRandomChoice(System.Random _r, List<Vector2Int> _c, List<float> _w)
    {
        float total = 0f;
        foreach (float w in _w) total += w;

        float r = NextFloat(_r) * total;

        float acc = 0f;
        for (int i = 0; i < _c.Count; i++)
        {
            acc += _w[i];
            if (r <= acc)
                return _c[i];
        }

        return _c[_c.Count - 1];
    }

    static void Shuffle<T>(System.Random _r, List<T> _list)
    {
        for (int i = 0; i < _list.Count; i++)
        {
            int j = _r.Next(i, _list.Count);
            (_list[i], _list[j]) = (_list[j], _list[i]);
        }
    }

    // 🔹 문자열 → int 시드 변환 (FNV-1a)
    static int StringToSeed(string _str)
    {
        if (string.IsNullOrEmpty(_str))
            _str = "DEFAULT_SEED";

        const uint prime = 16777619u;
        uint hash = 2166136261u;

        for (int i = 0; i < _str.Length; i++)
        {
            hash ^= _str[i];
            hash *= prime;
        }

        return (int)(hash & 0x7FFFFFFF);
    }
}

public class MapNode
{
    public string id;
    public RoomType roomType;
    public Vector2Int gridPos;
}


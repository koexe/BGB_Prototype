using UnityEngine;

public class MapManager : MonoBehaviour
{
    MapEntity[,] mapEntities;
    static int[] dx = { 0, 0, 1, -1 };
    static int[] dy = { 1, -1, 0, 0 };
    [SerializeField](int x, int y) currentPos;
    [SerializeField] MapEntity currentMap;
    public void CreateMapEntity(MapNode[,] _map)
    {
        System.Random t_rng = new System.Random(Utils.FNVHash(GameManager.instance.currentSeed));
        this.mapEntities = new MapEntity[_map.GetLength(0), _map.GetLength(1)];
        for (int i = 0; i < _map.GetLength(0); i++)
        {
            for (int j = 0; j < _map.GetLength(1); j++)
            {
                var t_node = _map[i, j];

                if (t_node == null) continue;

                var t_maps = DataLibrary.instance.GetMaps(0, _map[i, j].roomType);
                var t_map = t_maps[t_rng.Next(0, t_maps.Count)];
                this.mapEntities[i, j] = Instantiate(t_map);
                var t_mapEntity = this.mapEntities[i, j];
                bool[] availbleDoor = new bool[4];
                for (int d = 0; d < 4; d++)
                {
                    int nx = i + dx[d];
                    int ny = j + dy[d];

                    availbleDoor[d] = !(nx < 0 || ny < 0 || nx >= _map.GetLength(0) || ny >= _map.GetLength(1) || _map[nx, ny] == null);
                }
                t_mapEntity.CreateDoors(availbleDoor);
                t_mapEntity.gameObject.SetActive(false);
            }
        }
    }
    public void MoveToStart()
    {
        this.currentPos.x = 2;
        this.currentPos.y = 2;

        this.mapEntities[this.currentPos.x, this.currentPos.y].gameObject.SetActive(true);
        this.currentMap = this.mapEntities[this.currentPos.x, this.currentPos.y];
    }


    public void ChangeMap(int _dir)
    {
        this.currentMap.gameObject.SetActive(false);

        this.currentPos.x += dx[_dir];
        this.currentPos.y += dy[_dir];

        this.mapEntities[currentPos.x,currentPos.y].gameObject.SetActive(true);
        this.currentMap = this.mapEntities[currentPos.x, currentPos.y];
    }
}

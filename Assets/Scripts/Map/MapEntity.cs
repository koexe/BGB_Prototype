using UnityEngine;

public class MapEntity : MonoBehaviour
{
    [SerializeField] int stage;
    [SerializeField] int roomCode;
    [SerializeField] RoomType roomType;
    [SerializeField] Transform[] doorTr;
    [SerializeField] GameObject doorPrefab;



    public (int stage, RoomType roomType) GetRoomInfo() => (this.stage, this.roomType);
    public int GetRoomCode() => this.roomCode;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_availableWay">0 -> up, 1 -> down, 2-> right, 3-> left</param>
    public void CreateDoors(bool[] _availableWay)
    {
        for (int i = 0; i < 4; i++)
        {
            if (_availableWay[i])
            {
                var t_door = Instantiate(this.doorPrefab, this.doorTr[i]);
                int t_dir = i;
                t_door.GetComponent<MapDoor>().Initialization(() => { IngameManager.instance.ChangeMap(t_dir); });
            }
        }
    }
}

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
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (fileName = "Room Layout", menuName = "MazeGenerator/RoomLayout")]
public class RoomScriptable : ScriptableObject
{

    [SerializeField] private GameObject floor;
    [SerializeField] private GameObject wall;
    [SerializeField] private GameObject door;

    public GameObject GetFloor()
    {
        return floor;
    }
    public GameObject GetWall()
    {
        return wall;
    }
    public GameObject GetDoor()
    {
        return door;
    }
}

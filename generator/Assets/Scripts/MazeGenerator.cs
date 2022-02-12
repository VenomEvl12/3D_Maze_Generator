using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MazeGenerator : MonoBehaviour
{
    [SerializeField] private int gridSize;

    [SerializeField] private int totalRoom;

    [SerializeField] private int totalPathToFinishRoom;

    [SerializeField] private RoomScriptable room;

    [SerializeField] private int totalProp;

    [SerializeField] private GameObject[] props;

    [SerializeField] private bool GeneratePointForStart;

    private Room[,] map;

    private Vector3 sizeOfFloor;
    private Vector3 wallSize;
    private Vector3 doorSize;
    private void Awake()
    {
        MeshRenderer mesh = room.GetFloor().GetComponent<MeshRenderer>();
        MeshRenderer wallMesh = room.GetWall().GetComponent<MeshRenderer>();
        MeshRenderer doorMesh = room.GetDoor().GetComponent<MeshRenderer>();
        sizeOfFloor = mesh.bounds.size;
        wallSize = wallMesh.bounds.size;
        doorSize = doorMesh.bounds.size;
        Initiate();
    }

    private void Start()
    {
        //for(int i = 0; i < gridSize; i++)
        //{
        //    for(int j = 0; j < gridSize; j++)
        //    {
        //        Debug.Log("PosX : " + map[i, j].posX + "| PosY : " + map[i,j].posY + " | room number : " + map[i,j].number);
        //    }
        //}
    }

    private void Initiate()
    {
        // BASE CASE FOR FAILED GENERATED PATH
        if (finishGenerate) return;
        map = new Room[gridSize, gridSize];

        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                map[i, j] = new Room();
            }
        }
        GeneratePath();
    }

    private int roomCounter = 0;
    private int totalRoomGeneratedNow = 0;
    private bool finishGenerate = false;

    private int finalRoomNumber = 0;
    private int startRoomNumber = 0;
    private void GeneratePath()
    {
        roomCounter = 0;
        // GENERATE THE STARTER ROOM
        int startX = Random.Range(0, gridSize);
        int startY = Random.Range(0, gridSize);

        map[startY, startX].number = roomCounter;
        map[startY, startX].parent = new int[2] { startY, startX };
        map[startY, startX].posX = startX;
        map[startY, startX].posY = startY;
        totalRoomGeneratedNow++;


        // FOR ACCESSING THE ROOM BEFORE, FOR SET PARENT
        int beforeX = startX;
        int beforeY = startY;

        // FOR POSSIBLE DEAD END WHEN GENERATE ROOM
        bool toStart = false;
        int startAfterMoveX = startX;
        int startAfterMoveY = startY;

        // FOR FINAL ROOM POSITION
        int finalPosX = 0;
        int finalPosY = 0;

        while(roomCounter <= totalPathToFinishRoom)
        {
            // CHECK IF POSSIBLE TO CREATE A PATH
            bool left = true, right = true, up = true, down = true;
            if(beforeX - 1 > 0) { if (map[beforeY, beforeX - 1].number != int.MinValue) left = false; }
            else left = false;

            if(beforeX + 1 < gridSize) { if (map[beforeY, beforeX + 1].number != int.MinValue) right = false; }
            else right = false;

            if (beforeY - 1 > 0) { if (map[beforeY - 1, beforeX].number != int.MinValue) up = false; }    
            else up = false;

            if (beforeY + 1 < gridSize) { if (map[beforeY + 1, beforeX].number != int.MinValue) down = false; }
            else down = false;


            //IF THERE'S NO POSSIBLE PATH THEN GO TO START POSITION TO GENERATE ANOTHER PATH
            if(!left && !right && !up && !down)
            {
                if (!toStart)
                {
                    startAfterMoveX = beforeX;
                    startAfterMoveY = beforeY;
                    startRoomNumber = roomCounter;
                    beforeX = startX;
                    beforeY = startY;
                    toStart = true;
                    continue;
                }
                // IF NOT POSSIBLE THEN ROOM BREAK THE LOOP THEN SHOULD RESTART
                else break;
            }

            // FOR SET WHERE SHOULD NEXT ROOM POSITION BE
            int nextX = 0, nextY = 0;
            bool nextPathSet = false;
            while (!nextPathSet)
            {
                nextX = 0;
                nextY = 0;
                //RANDOM NEXT PATH
                if((Random.Range(-1, 1) == 0)) nextX = Random.Range(-1, 1) == 0 ? 1 : -1;
                else nextY = Random.Range(-1, 1) == 0 ? 1 : -1;

                // CHECK IF NEXT PATH IS POSSIBLE
                if(nextX == 1 || nextX == -1)
                {
                    if (nextX == 1 && right) nextPathSet = true;
                    else if (nextX == -1 && left) nextPathSet = true;
                }
                if(nextY == 1 || nextY == -1)
                {
                    if (nextY == 1 && down) nextPathSet = true;
                    else if (nextY == -1 && up) nextPathSet = true;
                }
            }

            // SET THE ROOM IN THE NEXT POSITION
            roomCounter++;
            int nextXPosition = beforeX + nextX;
            int nextYPosition = beforeY + nextY;

            // SET THE ROOM DATA
            map[beforeY, beforeX].SetNextConnectionRoom(nextXPosition, nextYPosition);

            map[nextYPosition, nextXPosition].number = roomCounter;
            map[nextYPosition, nextXPosition].parent = new int[2] { beforeY, beforeX };
            map[nextYPosition, nextXPosition].posX = nextXPosition;
            map[nextYPosition, nextXPosition].posY = nextYPosition; 
            map[nextYPosition, nextXPosition].SetDefaultConnectionRoom();
            totalRoomGeneratedNow++;

            // MOVE TO THE NEXT ROOM
            beforeX = nextXPosition;
            beforeY = nextYPosition;

            finalPosX = nextXPosition;
            finalPosY = nextYPosition;
        }

        if(roomCounter == totalPathToFinishRoom + 1)
        {
            finishGenerate = true;
            finalRoomNumber = roomCounter;
            GenerateRandomRoom();
        }
        else
        {
            Initiate();
        }

    }

    private void GenerateRandomRoom()
    {
        while(totalRoomGeneratedNow != totalRoom) { 
            int randomChoose = Random.Range(0, finalRoomNumber);
            if(randomChoose != startRoomNumber)
            {
                for(int i = 0; i < gridSize; i++)
                {
                    for(int j = 0; j < gridSize; j++)
                    {
                        if(map[i, j].number == randomChoose && randomChoose != startRoomNumber)
                        {
                            map[i, j].SetRandomNextRoom(ref map, gridSize,ref totalRoomGeneratedNow, startRoomNumber, finalRoomNumber);
                            if (totalRoomGeneratedNow > (gridSize * gridSize) - gridSize)
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }
        GenerateGameObject();
    }

    private void GenerateGameObject()
    {
        for(int i = 0; i < gridSize; i++)
        {
            for(int j = 0; j < gridSize; j++)
            {
                if(map[i, j].number != int.MinValue)
                {
                    map[i, j].InstantiateTheRoom(sizeOfFloor, room.GetFloor(), room.GetDoor(), room.GetWall());
                }
            }
        }
        GenerateRandomProp();
    }

    private void GenerateRandomProp()
    {
        int propCounter = 0;
        while(propCounter < totalProp)
        {
            int propChoice = Random.Range(0, props.Length);

            int posX = Random.Range(0, gridSize);
            int posY = Random.Range(0, gridSize);

            if (map[posY, posX].number == int.MinValue) continue;

            map[posY, posX].InstantiateProps(props[propChoice], sizeOfFloor);
            propCounter++;
        }
        GenerateStartAndEndPoint();
    }

    public void GenerateStartAndEndPoint()
    {
        if (GeneratePointForStart)
        {
            for(int i = 0; i < gridSize; i++)
            {
                for(int j = 0; j < gridSize; j++)
                {
                    if(map[i, j].number == startRoomNumber)
                    {
                        map[i, j].InstantiatePoint(sizeOfFloor);
                    }else if(map[i, j].number == finalRoomNumber)
                    {
                        map[i, j].InstantiatePoint(sizeOfFloor);
                    }
                }
            }
        }
    }

}

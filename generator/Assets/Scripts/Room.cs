using UnityEngine;
using System.Collections;
public class Room
{
    public int number { get; set; }
    public int[] parent { get; set; }
    public int[] connectionRooms { get; set; }
    public int posX { get; set; }
    public int posY { get; set; }

    public Room()
    {
        connectionRooms = new int[4] { 0, 0, 0, 0 };
        parent = new int[2] { 0, 0 };
        number = int.MinValue;
    }

    public void SetDefaultConnectionRoom()
    {
        // THE ARRAY IS CLOCK WISE FROM UP
        if (parent[0] < posY) connectionRooms[0] = 1;
        if (parent[0] > posY) connectionRooms[2] = 1;
        if (parent[1] < posX) connectionRooms[3] = 1;
        if (parent[1] > posX) connectionRooms[1] = 1;
    }

    public void SetNextConnectionRoom(int posX, int posY)
    {
        // THE ARRAY IS CLOCK WISE FROM UP
        if (posY < this.posY) connectionRooms[0] = 1;
        if (posY > this.posY) connectionRooms[2] = 1;
        if (posX < this.posX) connectionRooms[3] = 1;
        if (posX > this.posX) connectionRooms[1] = 1;
    }

    public void InstantiateTheRoom(Vector3 floorSize, GameObject floor, GameObject door, GameObject wall)
    {
        GameObject floorI = GameObject.Instantiate(floor) as GameObject;
        Vector3 position = new Vector3(posX * floorSize.x, 0, posY * floorSize.z);
        floorI.transform.position = position;
        for(int i = 0; i < 4; i++)
        {
            if(connectionRooms[i] == 0)
            {
                float z = 0;
                float x = 0;
                if(i == 0)
                {
                    z = (posY * floorSize.z) - (floorSize.z / 2);
                    GameObject wallI = GameObject.Instantiate(wall) as GameObject;
                    Vector3 positionWall = new Vector3(posX * floorSize.x, 0, z);
                    wallI.transform.position = positionWall;
                    wallI.transform.rotation = Quaternion.Euler(-90, 90, 0);
                }else if(i == 2)
                {
                    z = (posY * floorSize.z) + (floorSize.z / 2);
                    GameObject wallI = GameObject.Instantiate(wall) as GameObject;
                    Vector3 positionWall = new Vector3(posX * floorSize.x, 0, z);
                    wallI.transform.position = positionWall;
                    wallI.transform.rotation = Quaternion.Euler(-90, 90, 0);
                }else if(i == 1)
                {
                    x = (posX * floorSize.x) + (floorSize.x / 2);
                    GameObject wallI = GameObject.Instantiate(wall) as GameObject;
                    Vector3 positionWall = new Vector3(x, 0, posY * floorSize.z);
                    wallI.transform.position = positionWall;
                }else if(i == 3)
                {
                    x = (posX * floorSize.x) - (floorSize.x / 2);
                    GameObject wallI = GameObject.Instantiate(wall) as GameObject;
                    Vector3 positionWall = new Vector3(x, 0, posY * floorSize.z);
                    wallI.transform.position = positionWall;
                }
            }
            else if(connectionRooms[i] == 1)
            {
                float z = 0;
                float x = 0;
                float y = 0;
                //test
                y = 2.262278f;

                if (i == 0)
                {
                    z = (posY * floorSize.z) - (floorSize.z / 2);
                    GameObject doorI = GameObject.Instantiate(door) as GameObject;
                    Vector3 positionDoor = new Vector3(posX * floorSize.x, y, z);
                    doorI.transform.position = positionDoor;
                }else if(i == 2)
                {
                    z = (posY * floorSize.z) + (floorSize.z / 2);
                    GameObject doorI = GameObject.Instantiate(door) as GameObject;
                    Vector3 positionDoor = new Vector3(posX * floorSize.x, y, z);
                    doorI.transform.position = positionDoor;
                }else if(i == 1)
                {
                    x = (posX * floorSize.x) + (floorSize.x / 2);
                    GameObject doorI = GameObject.Instantiate(door) as GameObject;
                    Vector3 positionDoor = new Vector3(x, y, floorSize.z * posY);
                    doorI.transform.position = positionDoor;
                    doorI.transform.rotation = Quaternion.Euler(0, 90, 0);
                }else if(i == 3)
                {
                    x = (posX * floorSize.x) - (floorSize.x / 2);
                    GameObject doorI = GameObject.Instantiate(door) as GameObject;
                    Vector3 positionDoor = new Vector3(x, y, floorSize.z * posY);
                    doorI.transform.position = positionDoor;
                    doorI.transform.rotation = Quaternion.Euler(0, 90, 0);
                }
            }
        }
    }

    public void InstantiateProps(GameObject prop, Vector3 floorSize)
    {
        GameObject propI = GameObject.Instantiate(prop);
        float randX = Random.Range(((floorSize.x / 2) * -1f) + 1f, (floorSize.x / 2) - 1f);
        float randZ = Random.Range(((floorSize.z / 2) * -1f) + 1f, (floorSize.z / 2) - 1f);

        Vector3 position = new Vector3((floorSize.x * posX) + randX, 0, (floorSize.z * posY) + randZ);
        propI.transform.position = position;
    }

    public void InstantiatePoint(Vector3 floorSize)
    {
        GameObject pointI = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        Vector3 position = new Vector3((floorSize.x * posX), 0, (floorSize.z * posY));
        pointI.transform.position = position;
        pointI.transform.localScale = new Vector3(4, 4, 4);
    }

    public bool SetRandomNextRoom(ref Room[,] map, int gridSize, ref int totalRoomNow, int startRoomNumber, int finalRoomNumber)
    {
        if(number == startRoomNumber || number == finalRoomNumber)
        {
            return false;
        }
        // CHECK IF THERE'S A POSSIBLE PATH
        bool left = true, right = true, up = true, down = true;
        if (posX - 1 > 0) { if (map[posY, posX - 1].number != int.MinValue) left = false; }
        else left = false;

        if (posX + 1 < gridSize) { if (map[posY, posX + 1].number != int.MinValue) right = false; }
        else right = false;

        if (posY - 1 > 0) { if (map[posY - 1, posX].number != int.MinValue) up = false; }
        else up = false;

        if (posY + 1 < gridSize) { if (map[posY + 1, posX].number != int.MinValue) down = false; }
        else down = false;


        // IF THERE'S NO PATH AVAILABLE
        if(left == false && right == false && up == false && down == false)
        {
            int randomConnectionToChoose = Random.Range(0, 4);
            while(connectionRooms[randomConnectionToChoose] != 1)
            {
                randomConnectionToChoose = Random.Range(0, 4);
            }
            switch (randomConnectionToChoose)
            {
                case 0:
                    return map[posY - 1, posX].SetRandomNextRoom(ref map, gridSize,ref totalRoomNow, startRoomNumber, finalRoomNumber);
                case 1:
                    return map[posY, posX + 1].SetRandomNextRoom(ref map, gridSize, ref totalRoomNow, startRoomNumber, finalRoomNumber);
                case 2:
                    return map[posY + 1, posX].SetRandomNextRoom(ref map, gridSize, ref totalRoomNow, startRoomNumber, finalRoomNumber);
                case 3:
                    return map[posY, posX - 1].SetRandomNextRoom(ref map, gridSize, ref totalRoomNow, startRoomNumber, finalRoomNumber);
            }
            return false;
        }

        // GENERATE NEXT ROOM TO NEXT PATH
        bool nextPathSet = false;
        int nextX = 0, nextY = 0;

        while (!nextPathSet)
        {
            nextX = 0;
            nextY = 0;
            if ((Random.Range(-1, 1) == 0)) nextX = Random.Range(-1, 1) == 0 ? 1 : -1;
            else nextY = Random.Range(-1, 1) == 0 ? 1 : -1;

            if(nextX == 1 || nextX == -1)
            {
                if (nextX == 1 && right) nextPathSet = true;
                else if (nextX == -1 && left) nextPathSet = true;
            }else if(nextY == 1 || nextY == -1)
            {
                if (nextY == 1 && down) nextPathSet = true;
                else if(nextY == - 1 && up) nextPathSet = true;
            }
        }

        int nextPosX = posX + nextX;
        int nextPosY = posY + nextY;

        SetNextConnectionRoom(nextPosX, nextPosY);

        // GENERATE ROOM

        map[nextPosY, nextPosX].number = totalRoomNow;
        map[nextPosY, nextPosX].parent = new int[2] { posY, posX };
        map[nextPosY, nextPosX].posX = nextPosX;
        map[nextPosY, nextPosX].posY = nextPosY;
        map[nextPosY, nextPosX].SetDefaultConnectionRoom();
        totalRoomNow++;

        return true;
    }
}

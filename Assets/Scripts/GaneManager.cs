using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class GaneManager : MonoBehaviour
{
    private class Tile
    {
        bool revealed;
        bool isMine;
        int surroudingMines;

        public Tile(bool mine)
        {
            revealed = false;
            isMine = mine;
            surroudingMines = 0;
        }
    }

    Tile[,] gameMap;
    Sprite[] TileContents = Sprite



    // Start is called before the first frame update

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    //Shuffle based on Fisher-Yates method
    bool[] shuffle(bool[] toShuffle)
    {
        Random rng = new Random();

        for (int i = 0; i < toShuffle.Length - 1; i++)
        {
            int j = rng.Next(i ,toShuffle.Length);

            bool temp = toShuffle[i];
            toShuffle[i] = toShuffle[j];
            toShuffle[j] = temp;
        }
        return toShuffle;
    }

    bool[] randomizeMines(int numMines, int mapArea)
    {
        bool[] map = new bool[mapArea];

        if (numMines > mapArea)
        {
            throw new Exception("ERROR: Mines exceed available tiles!");
        }

        for (int i = 0; i < numMines; i++)
        {
            map[i] = true;
        }

        map = shuffle(map);
        return map;
    }

    void createGame(int x = 10, int y = 10, int numMines = 20)
    {
        gameMap = new Tile[x, y];
        bool[] mineLocations = randomizeMines(numMines, x * y);


        for (int j = 0; j < y; j++)
        {
            for (int i = 0; i < x; i++)
            {
                gameMap[i, j] = new Tile(mineLocations[i * j + i]);

            }
        }
    }
}

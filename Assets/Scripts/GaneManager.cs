using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class GaneManager : MonoBehaviour
{
    public GameObject GOTile;
    static Sprite[] TileContents;

    private class Tile
    {
        bool revealed;
        public bool isMine;
        int surroudingMines;
        GameObject obj;

        public Tile(bool mine)
        {
            revealed = false;
            isMine = mine;
            surroudingMines = 0;
        }

        public bool AssignObj(GameObject obj, int x = 0, int y = 0)
        {
            if (obj == null)
            {
                return false;
            }
            this.obj = obj;
            obj.transform.localPosition = new Vector2(x, y);
            return true;
        }

        public int incMines()
        {
            surroudingMines++;
            changeSprite();
            return surroudingMines;
        }

        public void changeSprite()
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();

            if (isMine)
            {
                sr.sprite = TileContents[0];
            }
            else if (surroudingMines == 0)
            {
                sr.sprite = null;
            }
            else
            {
                sr.sprite = TileContents[surroudingMines + 1];
            }
        }
    }

    Tile[,] gameMap;
    



    // Start is called before the first frame update

    
    void Start()
    {
        TileContents = Resources.LoadAll<Sprite>("TileOverlay") as Sprite[];
        createGame(11, 11, 35);
        
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
                gameMap[i, j].AssignObj(GameObject.Instantiate(GOTile), i - x/2, -1* (j - y/2));

                if (mineLocations[i * j + i])
                {
                    if (i > 0)
                        gameMap[i - 1, j].incMines();
                    if (j > 0)
                        gameMap[i, j - 1].incMines();
                    if (i > 0 && j > 0)
                        gameMap[i - 1, j - 1].incMines();
                    if (i < x - 1 && j > 0)
                        gameMap[i + 1, j - 1].incMines();
                    gameMap[i, j].changeSprite();
                }
                else
                {
                    if (i > 0 && gameMap[i - 1, j].isMine)
                        gameMap[i, j].incMines();
                    if (j > 0 && gameMap[i, j - 1].isMine)
                        gameMap[i, j].incMines();
                    if (i > 0 && j > 0 && gameMap[i - 1, j - 1].isMine)
                        gameMap[i, j].incMines();
                    if (i < x - 1 && j > 0 && gameMap[i + 1, j - 1].isMine)
                        gameMap[i, j].incMines();
                }

            }
        }
    }
}

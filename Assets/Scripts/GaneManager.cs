using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class GaneManager : MonoBehaviour
{
    public GameObject GOTile;
    static Sprite[] TileContents;
    static Sprite[] TileOver;

    private class Tile
    {
        public bool revealed;
        public bool isMine;
        public bool isFlagged;
        public bool isHovered;
        public int surroudingMines;
        GameObject obj;
        SpriteRenderer sr;

        public Tile(bool mine)
        {
            revealed = false;
            isMine = mine;
            isFlagged = false;
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
            sr = obj.GetComponent<SpriteRenderer>();
            return true;
        }

        public int incMines()
        {
            surroudingMines++;
            //changeSprite();
            return surroudingMines;
        }

        public void changeSprite()
        {
            revealed = true;

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

        public void toggleFlag()
        {
            if (!revealed)
            {
                isFlagged = !isFlagged;
                sr.sprite = TileOver[(isFlagged) ? 2 : 0];
            }
        }

        public void toggleHover()
        {
            if (!revealed && !isFlagged)
            {
                isHovered = !isHovered;
                sr.sprite = TileOver[(isHovered) ? 1 : 0];
            }
        }
    }

    Tile[,] gameMap;
    



    // Start is called before the first frame update

    
    void Start()
    {
        TileContents = Resources.LoadAll<Sprite>("TileOverlay") as Sprite[];
        TileOver = Resources.LoadAll<Sprite>("Tiles") as Sprite[];
        createGame(11, 11, 35);

        
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 TilePosFloat = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int TilePos = new Vector2Int((int)Math.Round(TilePosFloat.x) + gameMap.GetLength(0)/2,(int) (-1 * Math.Round(TilePosFloat.y) + gameMap.GetLength(1)/2));
            
            revealTile(TilePos.x, TilePos.y);
        }
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

    bool revealTile(int x, int y)
    {
        if (x < 0 || x > gameMap.GetLength(0) || y < 0 ||  y > gameMap.GetLength(1))
        {
            return false;
        }

        Tile selected = gameMap[x, y];
        if (selected.revealed)
        {
            return false;
        }

        selected.changeSprite();
        if (!selected.isMine && selected.surroudingMines <= 0)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (i != 0 && j != 0)
                    {
                        revealTile(i, j);
                    }
                }
            }
        }

        return selected.isMine;

    }
}

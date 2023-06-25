using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class GaneManager : MonoBehaviour
{
    
    //Difficuty Levels by Mine Percentage
    readonly float NORMAL_MINES = 0.15f;
    readonly float HARD_MINES = 0.20f;

    //Difficulty Levels by Size
    readonly Vector2Int NORMAL_MAP = new Vector2Int(11, 11);
    readonly Vector2Int HARD_MAP = new Vector2Int(17, 17);

    //Camera
    readonly float NORMAL_CAMERA_SCALE = 7.7f;
    readonly float NORMAL_CAMERA_Y = -1.1f;
    
    readonly float HARD_CAMERA_SCALE = 11.9f;
    readonly float HARD_CAMERA_Y = -1.7f;

    //Variables
    public GameObject GOTile;
    static Sprite[] TileContents;
    static Sprite[] TileOver;
    static Score scoreboard;
    ResetGame rst;
    Clock clock;
    bool gameCreated;
    bool gameActive;

    int numMines;
    int revealedTiles;



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
                if (!isFlagged)
                    scoreboard.incrementScore();
                else
                    scoreboard.decrementScore();
                sr.sprite = TileOver[(isFlagged) ? 3 : 0];
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

        public void setEndNotMine()
        {
            if (!isMine && isFlagged)
            {
                sr.sprite = TileOver[4];
            }
        }

        public void destroyObject()
        {
            Destroy(obj);
            obj = null;
        }
    }

    Tile[,] gameMap;
    



    // Start is called before the first frame update

    
    void Start()
    {
        TileContents = Resources.LoadAll<Sprite>("TileOverlay") as Sprite[];
        TileOver = Resources.LoadAll<Sprite>("Tiles") as Sprite[];
        scoreboard = GameObject.Find("Score").GetComponent<Score>();
        clock = GameObject.Find("Clock").GetComponent<Clock>();
        rst = GameObject.Find("UIButton").GetComponent<ResetGame>();

        resetGame(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown && gameActive)
        {
            Vector2 TilePosFloat = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2Int TilePos = new Vector2Int((int)Math.Round(TilePosFloat.x) + gameMap.GetLength(0) / 2, (int)(-1 * Math.Round(TilePosFloat.y) + gameMap.GetLength(1) / 2));

            if (TilePos.x >= 0 && TilePos.x < gameMap.GetLength(0) && TilePos.y >= 0 && TilePos.y < gameMap.GetLength(1))
            {
                if (Input.GetButtonDown("Fire1") && !gameMap[TilePos.x, TilePos.y].isFlagged)
                {
                    if (!gameCreated)
                    {
                        createGameWithStart(gameMap.GetLength(0), gameMap.GetLength(1), numMines, TilePos.x, TilePos.y);
                        gameCreated = true;
                        clock.Enabled = true;
                    }

                    revealTile(TilePos.x, TilePos.y);

                    if (gameMap[TilePos.x, TilePos.y].isMine)
                    {
                        endGame(false);
                        
                    }
                }
                else if (Input.GetButtonDown("Fire2") && gameCreated)
                {
                    gameMap[TilePos.x, TilePos.y].toggleFlag();
                }
            }
        }
    }
    
    //Shuffle based on Fisher-Yates method, with modifiactions such that the 3x3 area clicked will never be a mine
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

    //Old method for creating a game during testing, randomize at start rather then at click

    /*
    void createGame(int x = 10, int y = 10, int numMines = 20)
    {
        gameMap = new Tile[x, y];
        bool[] mineLocations = randomizeMines(numMines, x * y);


        for (int j = 0; j < y; j++)
        {
            for (int i = 0; i < x; i++)
            {
                gameMap[i, j] = new Tile(mineLocations[x * j + i]);
                gameMap[i, j].AssignObj(GameObject.Instantiate(GOTile), i - x/2, -1* (j - y/2));

                if (mineLocations[x * j + i])
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
    */
    
    void createEmptyField(int x = 11, int  y = 11)
    {
        gameMap = new Tile[x, y];
        for (int j = 0; j < y; j++)
        {
            for (int i = 0; i < x; i++)
            {
                gameMap[i, j] = new Tile(false);
                gameMap[i, j].AssignObj(GameObject.Instantiate(GOTile), i - x / 2, -1 * (j - y / 2));
            }
        }
    }

    //Attempt to create a better start for the player, with not mines
    void createGameWithStart(int x = 11, int y = 11, int numMines = 35, int xclick = 0, int yclick = 0)
    {
        int skipped = 0;
        int numAttempts = 0;

        int exclusionSize = 9;
        if (xclick <= 0 || xclick >= gameMap.GetLength(0) - 1)
            exclusionSize -= 3;
        if (yclick <= 0 || yclick >= gameMap.GetLength(1) - 1)
            exclusionSize -= (exclusionSize == 6) ? 2 : 3;



        do
        {
            bool[] mineLocations = randomizeMines(numMines, x * y - exclusionSize);
            if (numAttempts >= 1)
            {
                Debug.Log("SHUFFLER BROKEN");
            }


            for (int j = 0; j < y; j++)
            {
                for (int i = 0; i < x; i++)
                {
                    if (i <= xclick + 1 && i >= xclick - 1 && j <= yclick + 1 && j >= yclick - 1)
                    {
                        if (i > 0 && gameMap[i - 1, j].isMine)
                            gameMap[i, j].incMines();
                        if (j > 0 && gameMap[i, j - 1].isMine)
                            gameMap[i, j].incMines();
                        if (i > 0 && j > 0 && gameMap[i - 1, j - 1].isMine)
                            gameMap[i, j].incMines();
                        if (i < x - 1 && j > 0 && gameMap[i + 1, j - 1].isMine)
                            gameMap[i, j].incMines();
                        skipped++;
                        continue;
                    }

                    gameMap[i, j].isMine = mineLocations[x * j + i - skipped];
                    gameMap[i, j].surroudingMines = 0;
                    if (mineLocations[x * j + i - skipped])
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
            
        } while ((gameMap[xclick, yclick].isMine || gameMap[xclick, yclick].surroudingMines > 0) && numAttempts++ < 10);
    }

    bool revealTile(int x, int y)
    {
        if (x < 0 || x >= gameMap.GetLength(0) || y < 0 ||  y >= gameMap.GetLength(1))
        {
            return false;
        }

        Tile selected = gameMap[x, y];
        if (selected.revealed || selected.isFlagged)
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
                    if (!(i == 0 && j == 0))
                    {
                        revealTile(x + i, y + j);
                    }
                }
            }
        }
        revealedTiles++;
        if (revealedTiles >= gameMap.GetLength(0) * gameMap.GetLength(1) - numMines)
        {
            endGame(true);
        } 

        return selected.isMine;

    }

    void revealAllMines(bool autoFlag = false)
    {
        foreach (Tile t in gameMap)
        {
            if (autoFlag && !t.isFlagged && t.isMine)
            {
                t.toggleFlag();
            }
            if (t.isMine && !t.isFlagged)
            {
                t.changeSprite();
            }
            else if (!t.isMine && t.isFlagged)
            {
                t.setEndNotMine();
            }
        }
        
    }
    
    public void endGame(bool winner)
    {
        gameActive = false;
        revealAllMines(winner);
        clock.Enabled = false;
        rst.updateSprite((winner) ? 0 : 4);
        
    }

    public void resetGame(bool hardMode)
    {
        gameCreated = false;
        gameActive = true;

        if (gameMap != null)
        {
            foreach (Tile t in gameMap)
            {
                t.destroyObject();
            }
        }
        gameMap = null;

        Camera cm = Camera.main;

        if (!hardMode)
        {
            cm.orthographicSize = NORMAL_CAMERA_SCALE;
            cm.transform.localPosition = new Vector3(0, NORMAL_CAMERA_Y, -10);
            numMines = (int)(NORMAL_MAP.x * NORMAL_MAP.y * NORMAL_MINES);
            createEmptyField(NORMAL_MAP.x, NORMAL_MAP.y);
            
            clock.CountDown = false;
            clock.StartingTime = 0;
            clock.resetClock();
        }
        else
        {
            cm.orthographicSize = HARD_CAMERA_SCALE;
            cm.transform.localPosition = new Vector3(0, HARD_CAMERA_Y, -10);
            numMines = (int)(HARD_MAP.x * HARD_MAP.y * HARD_MINES);
            createEmptyField(HARD_MAP.x, HARD_MAP.y);
            
            clock.CountDown = true;
            clock.StartingTime = 300;
            clock.resetClock();
        }
        clock.Enabled = false;
        scoreboard.setScore(numMines);
        rst.updateSprite(2);
        revealedTiles = 0;


    }
}

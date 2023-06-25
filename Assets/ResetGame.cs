using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetGame : MonoBehaviour
{
    readonly float timerStart = 2;
    float timer = 2;
    bool started = false;
    GaneManager manager;
    Sprite[] buttons;
    int set;



    private void Awake()
    {
        manager = GameObject.Find("TileTops").GetComponent<GaneManager>();
        buttons = Resources.LoadAll<Sprite>("UIButton") as Sprite[];
        set = 2;
    }

    private void OnMouseDown()
    {
        started = true;
        GetComponent<SpriteRenderer>().sprite = buttons[set + 1];
        
    }

    private void OnMouseOver()
    {
        if (started)
        {
            timer -= Time.deltaTime;
        }
    }
    private void OnMouseUp()
    {
        if (started)
        {
            started = false;

            if (timer > 0)
            {
                manager.resetGame(false);
            }
            else
            {
                manager.resetGame(true);
            }
        }
        GetComponent<SpriteRenderer>().sprite = buttons[set];
        timer = timerStart;
    }
    public void updateSprite(int set)
    {
        this.set = set;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sprite = buttons[set];
        }
        
    }
}

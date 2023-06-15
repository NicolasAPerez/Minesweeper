using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileContents : MonoBehaviour
{
    public bool isMine = false;
    [Range(0, 8)]
    public int tileNumber = 1;

    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        changeSprite();
    }

    public void changeSprite()
    {
        if (isMine)
        {
            sr.sprite = Resources.Load<Sprite>("TileOverlay_0");
        }
        else
        {
            sr.sprite = Resources.Load<Sprite>("TileOverlay_" + (tileNumber + 1).ToString());
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    int score;
    GameObject[] digits;
    Sprite[] segmentDisplay;


    // Start is called before the first frame update
    void Awake()
    {
        score = 0;
        digits = new GameObject[] { transform.Find("Ones").gameObject, transform.Find("Tens").gameObject, transform.Find("Huns").gameObject, transform.Find("Thous").gameObject };
        segmentDisplay = Resources.LoadAll<Sprite>("SevenSegmentDigits") as Sprite[];
    }

    
    
    void setDigit(int digit, int number)
    {
        if (digit == 3 && number < 0)
        {
            digits[3].GetComponent<SpriteRenderer>().sprite = segmentDisplay[11];
        }
        else
        {
            digits[digit].GetComponent<SpriteRenderer>().sprite = segmentDisplay[number];
        }
        
        
    }
    void setDisplay()
    {
        setDigit(3, (score >= 0)? score / 1000 % 10 : -1);
        setDigit(2, Math.Abs(score) / 100 % 10);
        setDigit(1, Math.Abs(score) / 10 % 10);
        setDigit(0, Math.Abs(score) % 10);
    }

    public void incrementScore()
    {
        score++;
        setDisplay();
    }

    public void decrementScore()
    {
        score--;
        setDisplay();
    }

    public void setScore(int score)
    {
        this.score = score;
        setDisplay();
    }
}

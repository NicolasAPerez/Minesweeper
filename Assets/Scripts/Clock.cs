using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clock : MonoBehaviour
{
    public bool Enabled;
    public bool CountDown;
    public int StartingTime;
    GameObject[] digits;
    Sprite[] segmentDisplay;
    float time;
    


    // Start is called before the first frame update
    void Awake()
    {
        time = StartingTime;
        digits = new GameObject[] {transform.Find("SecOne").gameObject, transform.Find("SecTen").gameObject, transform.Find("MinOne").gameObject, transform.Find("MinTen").gameObject };
        segmentDisplay = Resources.LoadAll<Sprite>("SevenSegmentDigits") as Sprite[];
        Enabled = false;
    }

    private void FixedUpdate()
    {
        if (Enabled)
        {
            if (CountDown)
            {
                time = (time <= 0) ? 0 : time - Time.deltaTime;
                Enabled = !(time <= 0);

            }
            else
            {
                time += Time.deltaTime;
            }
        }
        setDisplay();
        
    }

    void setDigit(int digit, int number)
    {
        digits[digit].GetComponent<SpriteRenderer>().sprite = segmentDisplay[number];
    }

    int getDigit(int digit)
    {
        string spriteName = digits[digit].GetComponent<SpriteRenderer>().sprite.name;

        return int.Parse(spriteName.Substring(spriteName.Length - 1));
    }

    void setDisplay()
    {
        setDigit(3, (int)time / 600 % 10);
        setDigit(2, (int)time / 60 % 10);
        setDigit(1, (int)time % 60 / 10 % 10);
        setDigit(0, (int)time % 10);
    }

}

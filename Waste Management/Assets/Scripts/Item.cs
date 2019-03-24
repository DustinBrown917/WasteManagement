using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour {
    public int disposeValue;
    private const int BASE_TIMER = 1050;
    private const int MAX_POINTS = 1000;
    public int timer;
    public int multiplier = 1;

    public string itemName = "";
    public SpriteRenderer sr;
    public Sprite[] sprites;

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer--;
        }
        else
        {
            GetNewItem();
        }
    }

    public int GetPointsToAdd()
    {
        if(timer > BASE_TIMER)
        {
            return MAX_POINTS * multiplier;
        }
        else
        {
            return timer * multiplier;
        }
    }

    public void GetNewItem()
    {
        timer = BASE_TIMER;

        disposeValue = Random.Range(0, 3);

        // Change the Sprite, Name, and DisposeValue
        // eventually this logic needs to be changed to a random item based on random number and then assign the DisposeValue based on that item
        switch (disposeValue)
        {
            case 0:
                itemName = "Chip Bag";
                sr.sprite = sprites[2];
                break;

            case 1:
                itemName = "Paper Stack";
                sr.sprite = sprites[1];
                break;

            case 2:

                itemName = "Pop Can";
                sr.sprite = sprites[0];
                break;

            case 3:
                itemName = "Apple Core";
                sr.sprite = sprites[3];
                break;

            default:
                itemName = "Pop Can";
                sr.sprite = sprites[0];
                break;
        }
    }
}

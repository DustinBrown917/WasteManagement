using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonBehaviour : MonoBehaviour {
    public GameManager gm;

    [SerializeField]
    private Item item;

    [SerializeField]
    private int disposeValue;


    // Update is called once per frame
    void Update () {
		
	}

    public void OnPress()
    {
        if(item.disposeValue == disposeValue)
        {
            // Add to score
            gm.score += item.GetPointsToAdd();

            // Update Score
            gm.UpdateScore();

            // Play positive sound

            // Spawn positive Particle Effect

            print("Correct");
        }
        else
        {
            // Subtract from score
            gm.score -= 500;

            // Update Score
            gm.UpdateScore();

            // Reset Multiplier to 1
            item.multiplier = 1;

            // Play negative sound

            // Spawn negative Particle Effect
            print("Incorrect");
        }
        item.GetNewItem();
    }


}

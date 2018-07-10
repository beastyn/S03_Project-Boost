using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoard : MonoBehaviour {

    
    
    // Use this for initialization
	void Start () {

        int deathCount = GameManager.instance.score;
        int crazyThrust = GameManager.instance.crazyThrust;
        Text text = GetComponent<Text>();
        text.text = "Deaths: " + deathCount +"\n" + "Crazy Thrusts: " + crazyThrust + "\n";

        if (deathCount == 0)
        {
            text.text = text.text + "\n\nCongratulations! You WIN and made it to Home World in time!";
        }
        else
        {
            text.text = text.text + "\n\nWell, your wife was waiting for you for too long. Maybe next time?";
        }

        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

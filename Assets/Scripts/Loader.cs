using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
    public GameManager gameManager;

	void Start ()
    {
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

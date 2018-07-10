using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {

        
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene("Level1");
        }


    }
}

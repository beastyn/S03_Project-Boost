using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {

    
       
    [SerializeField] float speed = 2f;
    [SerializeField] int direction = 1;
    [SerializeField] Vector3 rotationVector = new Vector3(50f, 50f, 50f);
    [SerializeField] GameObject center;



    // Use this for initialization
    void Start () {

     

    }
 
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(center.transform.position, direction * Vector3.forward, speed * Time.deltaTime);
        transform.Rotate(rotationVector * Time.deltaTime);
    }
}

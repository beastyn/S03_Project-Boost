using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Oscillator : MonoBehaviour {

    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 0);
    [SerializeField] float period = 20f;
    [SerializeField] Vector3 rotationVector = new Vector3(50f, 50f, 50f);

    float movementFactor = 0; //move or not

    Vector3 startingPos;
    
    // Use this for initialization
    void Start ()
    {
        startingPos = transform.position;
        	
	}
	
	// Update is called once per frame
	void Update ()
    {
        //counting positioning data
        if (period <= Mathf.Epsilon){ return;}
        float cycles = Time.time / period; 
        const float tau = Mathf.PI * 2;
        float rawSinWave = Mathf.Sin(cycles * tau);
        movementFactor = rawSinWave / 2f + 0.5f;
        Vector3 offset = movementVector * movementFactor;

        transform.position = startingPos + offset;
        transform.Rotate( rotationVector.x * Time.deltaTime, rotationVector.y * Time.deltaTime, rotationVector.z * Time.deltaTime);
		
	}
}

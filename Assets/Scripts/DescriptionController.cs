using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DescriptionController : MonoBehaviour {

    [SerializeField] float delayTime = 0.1f;

    string description;
    Text text;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        description = text.text;
        text.text = "";
        StartCoroutine(TextTyping());
	}

    IEnumerator TextTyping()
    {
        foreach (char letter in description.ToCharArray())
        {
            text.text += letter;
            yield return new WaitForSeconds(delayTime);
            
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

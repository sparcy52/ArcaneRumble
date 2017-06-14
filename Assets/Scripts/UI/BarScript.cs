using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarScript : MonoBehaviour {

	public GameObject bar;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

	}

	public void SetPercent(float percent) {
		// set bar's witdh to percent %
//		Debug.Log(percent);
		bar.transform.localScale = new Vector3 (Mathf.Clamp((float) percent,0f ,1f),bar.transform.localScale.y,bar.transform.localScale.z);
//		Debug.Log ("x: "+percent);
//		Debug.Log ("y: "+bar.transform.localScale.y);
//		Debug.Log ("z: "+bar.transform.localScale.z);

	}
}

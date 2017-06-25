using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class timer : MonoBehaviour {
	
	public GameController gameController;
	public int RoundNumber;
	public Image bar;
	private int round; 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		round = gameController.RoundNumber;
		bar.fillAmount = (float)round / RoundNumber;
	}
}

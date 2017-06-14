using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiComponents : MonoBehaviour {

	public GameObject HpBar;
	public GameObject EpBar;

	public static GuiComponents _instance;

	// Use this for initialization
	void Start () {
		_instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public static GameObject GetHpBar() {
		return _instance.HpBar;
	}

	public static GameObject GetEpBar() {
		return _instance.EpBar;
	}
}

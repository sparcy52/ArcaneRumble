﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiComponents : MonoBehaviour {
	
	private static GuiComponents _instance;

	// Use this for initialization
	private void OnValidate() {
		_instance = this;
	}
	
	// HEALTH / ENERGY BARS
	
	public GameObject HpBar;
	public static GameObject GetHpBar() {
		return _instance.HpBar;
	}

	public GameObject EpBar;
	public static GameObject GetEpBar() {
		return _instance.EpBar;
	}
	
	// HIT EFFECTS
	
	public Color HitPrimaryColor = Color.yellow;
	public float HitPrimaryTime = 0.1f;
	
	public static Color GetHitPrimaryColor()
	{
		return _instance.HitPrimaryColor;
	}
	
	public static float GetHitPrimaryTime()
	{
		return _instance.HitPrimaryTime;
	}
	
	public Color HitSecondaryColor = Color.red;
	public float HitSecondaryTime = 0.1f;
	
	public static Color GetHitSecondaryColor()
	{
		return _instance.HitSecondaryColor;
	}

	public static float GetHitSecondaryTime()
	{
		return _instance.HitSecondaryTime;
	}

}

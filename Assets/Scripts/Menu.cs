using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	public void startGame () {
   
        SceneManager.LoadScene("game");
	}

    public void quit()
    {
        Application.Quit();
    }
}

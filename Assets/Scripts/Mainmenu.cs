using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Mainmenu : MonoBehaviour {

    // Use this for initialization
    public Text scoreText;
	private void Start () {
        scoreText.text = PlayerPrefs.GetInt("Score").ToString();
	}
	
	// Update is called once per frame
	public void toGame()
    {
        SceneManager.LoadScene("Main");
		
	}
}

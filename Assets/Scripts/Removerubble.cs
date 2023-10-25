using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Removerubble : MonoBehaviour {

	// Use this for initialization
	private void OnCollisionEnter(Collision col) {
        Destroy(col.gameObject);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

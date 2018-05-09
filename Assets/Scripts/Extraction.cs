using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Extraction : MonoBehaviour {

	private GameManager gameManager;

	
	void Start () {
		gameManager = GameManager.Instance;
	}

	void OnTriggerEnter (Collider col)
	{
		if (gameManager.levelCleared && col.name == "Player") {
			gameManager.LoadNextScene();
		}
	}
}

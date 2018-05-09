using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScene : MonoBehaviour {

	private GameManager gameManager;

	
	void Start () {
		gameManager = GameManager.Instance;
	}

	void OnTriggerEnter (Collider col)
	{
		if (gameManager.sceneCleared && col.name == "Player") {
			if (gameManager.levelCleared) {
				if(gameManager.LevelIndex >= gameManager.LevelReached)
					gameManager.LevelReached = gameManager.LevelIndex+1;
				gameManager.LoadMainMenu ();
			} else {
				gameManager.LoadNextScene ();
			}

		}
	}
}

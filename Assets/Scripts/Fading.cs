using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fading : MonoBehaviour {

	public Texture2D fadeOutTexture;
	public float fadeSpeed = 0.8f;

	private int drawDepth = -1000;
	private int fadeDir = -1;
	private float alpha = 1.0f;


	void OnGUI ()
	{
		alpha += fadeDir * fadeSpeed * Time.unscaledDeltaTime;

		alpha = Mathf.Clamp01(alpha);

		GUI.color = new Color(0,0,0,alpha);
		GUI.depth = drawDepth;
		GUI.DrawTexture( new Rect(0,0,Screen.width,Screen.height), fadeOutTexture);
	}

	public float BeginFade (int direction)
	{
		fadeDir = direction;
		return fadeSpeed;
	}


}

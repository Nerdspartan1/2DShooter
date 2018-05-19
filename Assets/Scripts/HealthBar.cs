using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	private Unit unit;
	private Image healthBarImage;


	void Start () {
		unit = GetComponentInParent<Unit>();
		healthBarImage = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
		float h = unit.hp / unit.maxHp;
		healthBarImage.fillAmount = h;
		if (h > 0.5f) {
			healthBarImage.color = Color.green + (1-2*(h-0.5f)) * Color.red;
		} else {
			healthBarImage.color= Color.red + 2*h*Color.green;
		}

	}

	void LateUpdate ()
	{
		transform.eulerAngles = Vector3.right * 90;
	}


}

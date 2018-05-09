using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIPath : MonoBehaviour {

	public GameObject target;
	private NavMeshAgent nav;
	private Animator anim;
	private Enemy enemy;
	public float range = 20f;
	public float marge= 2f;
	private bool inRange = false;

	private Vector3 goTo;



	void Start () {
		target = GameObject.Find("Player");
		nav = GetComponent<NavMeshAgent>();
		anim = GetComponent<Animator>();
		enemy = GetComponent<Enemy>();
		goTo = transform.position;
	}

	void Update ()
	{
		Vector3 u = transform.position - target.transform.position;
		float distance = u.magnitude;
		inRange = distance <= range + marge && distance >= range - marge;
		if (enemy.targetInSight) {
			if(!inRange)
				goTo= target.transform.position + range * u.normalized;
		} else if (enemy.LostSightOftarget () || enemy.Heardtarget()) {
			goTo= target.transform.position;
		}





		transform.LookAt(transform.position + nav.velocity);
		nav.destination = goTo;
		anim.SetFloat("speed",nav.velocity.magnitude);

	}

}

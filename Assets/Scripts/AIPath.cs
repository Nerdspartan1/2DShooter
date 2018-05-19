using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Behaviour {HOSTILE, FLEE}


public class AIPath : MonoBehaviour {

	public GameObject target;
	private NavMeshAgent nav;
	private Animator anim;
	private Enemy enemy;
	public float shootRange = 4f;
	public float fleeRange = 20f;
	public float marge= 2f;
	private bool inRange = false;

	public Behaviour behaviour = Behaviour.HOSTILE;

	public Vector3 goTo;



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
		switch (behaviour) {
		case Behaviour.HOSTILE:
			{
				inRange = distance <= shootRange + marge && distance >= shootRange - marge;
				if (enemy.targetInSight) {
					if (!inRange)
						goTo = target.transform.position + shootRange * u.normalized;
				} else if (enemy.LostSightOftarget () || enemy.Heardtarget ()) {
					goTo = target.transform.position;
				}
				break;
			}
		case Behaviour.FLEE:
			{
				if (enemy.targetInSight) {
					goTo = target.transform.position + fleeRange * u.normalized;
				}
				break;
			}

		}

		transform.LookAt(transform.position + nav.velocity);
		nav.destination = goTo;
		anim.SetFloat("speed",nav.velocity.magnitude);

	}

}

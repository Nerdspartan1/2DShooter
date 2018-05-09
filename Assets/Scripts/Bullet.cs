using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

	public float damage;
	public float knockBack;
	public int pierce = 1;
	public float bulletForce;
	private int nbHit=0;

	public GameObject impactEffect;
	new private Collider collider;
	public LayerMask impactEffectOn;
	private Vector3 startPosition;
	void Start ()
	{
		collider = GetComponent<Collider>();
		collider.isTrigger = true;
		startPosition = transform.position;
	}

	void OnTriggerEnter (Collider col)
	{
		if (pierce >= 0) {
			Hit (col);
			nbHit++;
			if (pierce == 0 || col.gameObject.layer == LayerMask.NameToLayer("Wall") || col.gameObject.layer == LayerMask.NameToLayer("Ground")  ) {
				Destroy (gameObject);
			}
			pierce--;
		}
	}

	void Hit (Collider col)
	{
		Unit u = col.transform.GetComponentInParent<Unit> ();
		if (u != null)
			u.TakeDamage (damage,knockBack*collider.attachedRigidbody.velocity.normalized);

		Rigidbody rb = col.attachedRigidbody;
		if (rb != null)
			rb.AddForce(collider.attachedRigidbody.velocity.normalized*knockBack);
				
		RaycastHit hit;
		Physics.Raycast(0.2f*startPosition+0.8f*transform.position,transform.forward, out hit,gameObject.layer);
		GameObject b = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-collider.attachedRigidbody.velocity));
		Destroy(b,1f);
	}


}

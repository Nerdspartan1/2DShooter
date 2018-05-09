using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Unit : MonoBehaviour {

	protected float maxHp = 100;
	public float hp = 100;
	public bool ragdollUponDeath = false;

	private Rigidbody[] bodies;


	protected void Start()
	{
		maxHp = hp;
		bodies = GetComponentsInChildren<Rigidbody> ();
		foreach (Rigidbody rb in bodies) {
			rb.isKinematic = true;
		}


	}


	public virtual void TakeDamage (float damage, Vector3 knockback)
	{
		if (!IsDead ()) {
			hp -= damage;
			if (hp <= 0) {
				hp = 0;
				Death (knockback);
			}
		}
	}


	public virtual void Death(Vector3 knockback)
	{	
		if (ragdollUponDeath) {
			GetComponent<Animator> ().enabled = false;
			GetComponent<Collider> ().enabled = false;
			foreach (Rigidbody rb in bodies) {
				rb.isKinematic = false;
				rb.AddForce(knockback);
			}
			changeLayer(transform, LayerMask.NameToLayer("Corpse"));

		}
		else Destroy(gameObject);

	}

	public static void changeLayer (Transform root, int layer)
	{
		root.gameObject.layer = layer;
		foreach (Transform child in root) {
			changeLayer(child,layer);
		}
	}

	public bool IsFullLife()
	{
		return (hp >= maxHp);
	}

	public bool IsDead ()
	{	
		return (hp <= 0);
	}


}

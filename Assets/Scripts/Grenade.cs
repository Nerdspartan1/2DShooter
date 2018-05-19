using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
	public float damage;
	public float detonationTime;
	public float explosionRadius;
	public float explosionForce;

	public GameObject explosionEffect;


	void Start ()
	{
		StartCoroutine(DetonationCoroutine());
	}

	public IEnumerator DetonationCoroutine ()
	{
		yield return new WaitForSeconds(detonationTime);
		Explode();
	}

	void OnCollisionEnter (Collision col)
	{
		if (col.collider.GetComponent<Enemy> () != null) {
			Explode();
		}
	}

	void Explode ()
	{
		GameObject effect = Instantiate (explosionEffect, transform.position, Quaternion.identity);
		effect.GetComponent<AudioSource> ().pitch = Mathf.Pow (Time.timeScale, 0.25f);
		Destroy (effect, 1.5f);
		LayerMask layerMask;
		if (gameObject.layer == LayerMask.NameToLayer ("Friendly Projectile")) {
			layerMask = ~(1 << LayerMask.NameToLayer ("Player"));
		} else {
			layerMask = ~(1 << LayerMask.NameToLayer("Enemy"));
		}
		 
		Collider[] colliders = Physics.OverlapSphere (transform.position, explosionRadius,layerMask);

		foreach (Collider col in colliders) {
			Unit u = col.GetComponent<Unit> ();
			if (u != null) {
				float m = 1- Mathf.Pow((col.transform.position - transform.position).magnitude/explosionRadius,2);
				if(m < 0) Debug.Log( "Dégâts négatifs !");
				else u.TakeDamage (m * damage, Vector3.zero);
			}
			Rigidbody rb = col.GetComponent<Rigidbody> ();
			if (rb != null) {
				rb.AddExplosionForce(explosionForce,transform.position,explosionRadius);
			}
		}

		Destroy(gameObject);
	}
}

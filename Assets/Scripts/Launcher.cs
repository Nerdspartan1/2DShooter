using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Weapon {

	public GameObject grenadeObject;
	public float launchForce;
	public float detonationTime;
	public float explosionRadius;
	public float explosionForce;


	override public GameObject ShootProjectile (Vector3 startPos, Vector3 aimPos)
	{
		GameObject g = Instantiate (grenadeObject, startPos, muzzle.transform.rotation);
		g.GetComponent<Rigidbody> ().AddForce (((1f - disp) * (aimPos - startPos).normalized + disp * Vector3.ProjectOnPlane (Random.onUnitSphere, Vector3.up).normalized).normalized * launchForce + 0.2f*Vector3.up * launchForce);
		Grenade grenade = g.GetComponent<Grenade> ();

		grenade.damage = damage;
		grenade.explosionRadius = explosionRadius;
		grenade.explosionForce = explosionForce;
		grenade.detonationTime = detonationTime;

		return g;

	}

}

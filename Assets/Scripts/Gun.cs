using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gun : Weapon {

	
	public float knockBack = 10f;
	public int pierce = 0;

	public GameObject bulletImpact;
	public GameObject bullet;
	public float bulletForce;

	override public GameObject ShootProjectile (Vector3 startPos, Vector3 aimPos)
	{
		GameObject bul = Instantiate (bullet, startPos, muzzle.transform.rotation);
		bul.GetComponent<Rigidbody> ().AddForce (((1f - disp) * (aimPos - startPos).normalized + disp * Vector3.ProjectOnPlane (Random.onUnitSphere, Vector3.up).normalized).normalized * bulletForce);
		Bullet bulStats = bul.GetComponent<Bullet> ();


		bulStats.damage = damage;
		bulStats.knockBack = knockBack;
		bulStats.pierce = pierce;

		return bul;

	}






}

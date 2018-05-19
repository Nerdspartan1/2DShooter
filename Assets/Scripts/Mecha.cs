using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecha : Enemy {

	public GameObject weaponObject2;
	[Range(1,3)]public int gun2Tier=1;
	private Gun weapon2;
	public GameObject gun2Holder;
	private bool canShoot2 = true;
	public GameObject bulletStart2;

	new void Start(){
		base.Start();
		hasAimingAnimation = false;
		if (weaponObject2 == null) {
			weaponObject2 = gameManager.GetRandomWeapon(gun2Tier);
		}
		GameObject g = Instantiate (weaponObject2);
		g.name = weaponObject2.name;
		EquipWeapon2 (g);
		SetAnimLayer(g.GetComponent<Gun>().holdingPose);

	}

	 public override void ShootMethod(){
		base.ShootMethod();
		if (targetInSight && distanceToTarget < shootRange) {
			if (canShoot2) {
				ShootFunction2();
			}
		}
	}

	private void ShootFunction2 ()
	{
		weapon2.Shoot (bulletStart2.transform.position, aiming.trueAimPos,!hasInfiniteAmmo);
		canShoot2 = false;
		player.ReplenishBulletTime (weapon2.bulletTimeReplenishFactor);
		Invoke("CanShootAgain2", (weapon2.automatic ? 1 : 1.5f) / weapon2.fireRate);
	}

	private void CanShootAgain2 ()
	{
		canShoot2 = true;
	}

	public void EquipWeapon2 (GameObject g)
	{
		g.transform.position = gun2Holder.transform.position;
		g.transform.rotation = gun2Holder.transform.rotation;
		g.transform.SetParent(gun2Holder.transform);
		weapon2 = g.GetComponent<Gun>();
		weapon2.SetOwner(nav);
		g.GetComponent<Rigidbody>().isKinematic = true;
		Unit.changeLayer(g.transform,LayerMask.NameToLayer("Default"));
		weaponObject2 = g;

	}

	protected override IEnumerator ReactionCoroutine ()
	{
		canShoot = false;
		canShoot2 = false;
		yield return new WaitForSeconds (reactionTime);
		canShoot = true;
		canShoot2 = true;
	}

	public override void Death (Vector3 knockback)
	{
		base.Death(knockback);
		weaponObject2.layer =LayerMask.NameToLayer("Prop");
		weaponObject2.transform.parent = null;
		weaponObject2.GetComponent<Rigidbody>().isKinematic=false;

	}
}

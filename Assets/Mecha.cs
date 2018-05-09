using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mecha : Enemy {

	public GameObject gun2;
	[Range(1,3)]public int gun2Tier=1;
	private Gun gun2Scr;
	public GameObject gun2Holder;
	private bool canShoot2 = true;
	public GameObject bulletStart2;

	new void Start(){
		base.Start();
		hasAimingAnimation = false;
		if (gun2 == null) {
			gun2 = gameManager.GetRandomWeapon(gun2Tier);
		}
		GameObject g = Instantiate (gun2);
		g.name = gun2.name;
		EquipWeapon2 (g);
		SetAnimLayer(g.GetComponent<Gun>().holdingPose);

	}

	 public override void ShootMethod(){
		base.ShootMethod();
		if (targetInSight && distanceToTarget < shootRange) {
			if (canShoot2) {
				StartCoroutine(ShootCoroutine2());
			}
		}
	}

	private IEnumerator ShootCoroutine2 ()
	{
		gun2Scr.Shoot (bulletStart2.transform.position, aiming.trueAimPos,!hasInfiniteAmmo);
		canShoot2 = false;
		player.ReplenishBulletTime (gun2Scr.bulletTimeReplenishFactor);
		yield return new WaitForSeconds ((gun2Scr.automatic ? 1 : 1.5f) / gun2Scr.fireRate);
		canShoot2 = true;
	}

	public void EquipWeapon2 (GameObject g)
	{
		g.transform.position = gun2Holder.transform.position;
		g.transform.rotation = gun2Holder.transform.rotation;
		g.transform.SetParent(gun2Holder.transform);
		gun2Scr = g.GetComponent<Gun>();
		gun2Scr.SetOwner(nav);
		g.GetComponent<Rigidbody>().isKinematic = true;
		Unit.changeLayer(g.transform,LayerMask.NameToLayer("Default"));
		gun2 = g;

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
		gun2.layer =LayerMask.NameToLayer("Prop");
		gun2.transform.parent = null;
		gun2.GetComponent<Rigidbody>().isKinematic=false;

	}
}

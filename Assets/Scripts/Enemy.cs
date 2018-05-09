using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Unit {

	protected GameObject target;
	protected Player player;

	[HideInInspector]
	public bool targetInSightInPreviousFrame = false, targetInSight = false, targetHeard = false;

	protected float distanceToTarget;
	protected bool lostSightOftarget = false;

	public float reactionTime = 2f;

	protected bool hasAimingAnimation = true;
	protected Animator anim;
	protected AimAt aiming;
	public float sightRange = 15f;
	public float shootRange = 8f;

	public bool hasInfiniteAmmo = false;

	public GameObject gun;
	[Range(1,3)]public int gunTier=1;
	private Gun gunScr;
	public GameObject gunHolder;
	protected bool canShoot = true;
	public GameObject bulletStart;

	private GameObject eyes;

	private LineRenderer laser;

	protected AIPath aiPath;
	protected NavMeshAgent nav;
	protected GameManager gameManager;




	public LayerMask cantSeeThrough;





	new protected void Start ()
	{
		base.Start();
		gameManager = GameManager.Instance;
		tag = "Enemy";
		target = GameObject.Find ("Player").transform.Find("ShootingPivot").gameObject;
		anim = GetComponent<Animator> ();
		aiming = GetComponent<AimAt> ();
		if (aiming != null)
			aiming.enabled = true;

		aiming.planePos = Vector3.Project(bulletStart.transform.position,Vector3.up);

		if (aiming.laserSight != null)
			laser = aiming.laserSight.GetComponent<LineRenderer> ();

		nav = GetComponent<NavMeshAgent> ();
		aiPath = GetComponent<AIPath> ();

		if (gun == null) {
			gun = gameManager.GetRandomWeapon(gunTier);
		}
		GameObject g = Instantiate (gun);
		g.name = gun.name;
		EquipWeapon (g);
		if(hasAimingAnimation)
			SetAnimLayer(g.GetComponent<Gun>().holdingPose);

		if (target != null) {
			aiming.target = target;
			player = target.GetComponentInParent<Player>();
		}

		eyes = transform.Find("Eyes").gameObject;
		if(eyes == null) eyes = gameObject;

	}

	void Update ()
	{	
		
		if (target != null) {

			distanceToTarget = Vector3.Distance (target.transform.position, transform.position);
			RaycastHit hit;
			targetInSight = !Physics.Linecast (eyes.transform.position, target.transform.position, out hit, cantSeeThrough)
			&& distanceToTarget < sightRange;

			if(hasAimingAnimation)
				anim.SetBool ("aiming", targetInSight);
			aiming.looksAtTarget = targetInSight; //Ne regarde pas le joueur si il n'est pas en vue

			if (!targetInSight) {
				StopCoroutine(ReactionCoroutine());
			}
			if (Spottedtarget ()) {
				StartCoroutine(ReactionCoroutine());
			}


			ShootMethod();

			if (player.IsDead ()) {
				target=null;
			}
		} else {
			aiming.enabled = false;
		}

		if (laser != null)
			laser.enabled = targetInSight && !IsDead ();

		lostSightOftarget = targetInSightInPreviousFrame && !targetInSight;

		targetInSightInPreviousFrame = targetInSight;
		
	}

	public virtual void ShootMethod(){
		if (targetInSight && distanceToTarget < shootRange) {
			if (canShoot) {
				StartCoroutine(ShootCoroutine());
			}
			
		}
	}


	private IEnumerator ShootCoroutine ()
	{
		gunScr.Shoot (bulletStart.transform.position, aiming.trueAimPos, !hasInfiniteAmmo);
		canShoot = false;
		player.ReplenishBulletTime (gunScr.bulletTimeReplenishFactor);
		yield return new WaitForSeconds ((gunScr.automatic ? 1 : 1.5f) / gunScr.fireRate);
		canShoot = true;
	}

	protected virtual IEnumerator ReactionCoroutine ()
	{
		canShoot = false;
		yield return new WaitForSeconds (reactionTime);
		canShoot = true;
	}

	public override void Death(Vector3 knockback){
		base.Death(knockback);
		aiming.enabled=false;
		nav.enabled = false;
		aiPath.enabled = false;
		gun.layer =LayerMask.NameToLayer("Prop");
		gun.transform.parent = null;
		gun.GetComponent<Rigidbody>().isKinematic=false;
		tag = "Untagged";
		gameManager.enemiesAlive--;
		enabled = false;
	}

	public void SetAnimLayer (int layer)
	{
		for (int i = 1; i < anim.layerCount; i++) {
			anim.SetLayerWeight(i,0);
		}
		if( layer < anim.layerCount)
			anim.SetLayerWeight(layer,1);

	}



	public bool Spottedtarget ()
	{
		return (targetInSight && !targetInSightInPreviousFrame);
	}

	public bool LostSightOftarget ()
	{
		return lostSightOftarget;
	}


	public bool Heardtarget ()
	{
		bool v = targetHeard;
		targetHeard = false;
		return v;
	}

	public void EquipWeapon (GameObject g)
	{
		g.transform.position = gunHolder.transform.position;
		g.transform.rotation = gunHolder.transform.rotation;
		g.transform.SetParent(gunHolder.transform);
		gunScr = g.GetComponent<Gun>();
		gunScr.SetOwner(nav);
		g.GetComponent<Rigidbody>().isKinematic = true;
		Unit.changeLayer(g.transform,LayerMask.NameToLayer("Default"));
		gun = g;

	}

}

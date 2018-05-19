using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Unit {

	protected GameObject target;
	protected Player player;

	[HideInInspector]
	public bool targetInSightInPreviousFrame = false, targetHeard = false;
	public bool targetInSight = false;
	protected float distanceToTarget;
	protected bool lostSightOftarget = false;

	public float reactionTime = 2f;

	protected bool hasAimingAnimation = true;
	protected Animator anim;
	protected AimAt aiming;
	public float sightRange = 15f;
	public float shootRange = 8f;

	public bool hasInfiniteAmmo = false;

	public GameObject weaponObject;
	[Range(0,3)]public int gunTier=1;
	private Weapon weapon;
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
		base.Start ();
		gameManager = GameManager.Instance;
		target = GameObject.Find ("Player").transform.Find ("ShootingPivot").gameObject;
		anim = GetComponent<Animator> ();
		nav = GetComponent<NavMeshAgent> ();
		aiPath = GetComponent<AIPath> ();
		aiming = GetComponent<AimAt> ();

		if (target != null) {
			player = target.GetComponentInParent<Player>();
		}
		if (aiming != null) {
			if(target != null) aiming.target = target;
			aiming.enabled = true;
			aiming.planePos = Vector3.Project (bulletStart.transform.position, Vector3.up);
			if (aiming.laserSight != null)
				laser = aiming.laserSight.GetComponent<LineRenderer> ();
		}


		if (weaponObject == null) {
			if(gunTier >0)
				weaponObject = gameManager.GetRandomWeapon (gunTier);
			else {
				RunAway ();
				SetAnimLayer (0);
			}
		} 
		if (weaponObject != null) {
			GameObject w = Instantiate (weaponObject);
			w.name = weaponObject.name;
			EquipWeapon (w);
			if (hasAimingAnimation)
				SetAnimLayer (w.GetComponent<Gun> ().holdingPose);
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

			if (weaponObject != null) {
				if (hasAimingAnimation)
					anim.SetBool ("aiming", targetInSight);
					if(aiming !=null)
					aiming.looksAtTarget = targetInSight; //Ne regarde pas le joueur si il n'est pas en vue

				if (!targetInSight) {
					StopCoroutine (ReactionCoroutine ());
				}
				if (Spottedtarget ()) {
					StartCoroutine (ReactionCoroutine ());
				}
				ShootMethod ();
			}

			if (player.IsDead ()) {
				target=null;
			}

		} else {
			if(aiming != null)
			aiming.enabled = false;
		}

		if (laser != null)
			laser.enabled = targetInSight && !IsDead ();

		lostSightOftarget = targetInSightInPreviousFrame && !targetInSight;

		targetInSightInPreviousFrame = targetInSight;
		
	}

	public virtual void ShootMethod ()
	{
		if (targetInSight && distanceToTarget < shootRange && aiPath.behaviour == Behaviour.HOSTILE) {
			if (canShoot) {
				if (!ShootFunction ()) {
					RunAway();
				}
			}
			
		}
	}


	private bool ShootFunction ()
	{
		
		canShoot = false;
		player.ReplenishBulletTime (weapon.bulletTimeReplenishFactor);
		Invoke("CanShootAgain", (weapon.automatic ? 1 : 1.5f) / weapon.fireRate);
		return weapon.Shoot (bulletStart.transform.position, aiming.trueAimPos, !hasInfiniteAmmo);

	}

	private void CanShootAgain ()
	{
		canShoot = true;
	}

	public void RunAway ()
	{
		aiPath.behaviour = Behaviour.FLEE;
		if(aiming != null) aiming.looksAtTarget = false;
	}

	protected virtual IEnumerator ReactionCoroutine ()
	{
		canShoot = false;
		yield return new WaitForSeconds (reactionTime);
		canShoot = true;
	}

	public override void Death (Vector3 knockback)
	{
		base.Death (knockback);
		if(aiming!=null) aiming.enabled = false;
		nav.enabled = false;
		aiPath.enabled = false;
		if (weaponObject != null) {
			weaponObject.layer = LayerMask.NameToLayer ("Prop");
			weaponObject.transform.parent = null;
			weaponObject.GetComponent<Rigidbody> ().isKinematic = false;
		}
		if(tag == "Enemy")
			gameManager.enemiesAlive--;
		tag = "Untagged";

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

	public void EquipWeapon (GameObject w)
	{
		w.transform.position = gunHolder.transform.position;
		w.transform.rotation = gunHolder.transform.rotation;
		w.transform.SetParent(gunHolder.transform);
		weapon = w.GetComponent<Weapon>();
		weapon.SetOwner(nav);
		w.GetComponent<Rigidbody>().isKinematic = true;
		Unit.changeLayer(w.transform,LayerMask.NameToLayer("Default"));
		weaponObject = w;

	}

}

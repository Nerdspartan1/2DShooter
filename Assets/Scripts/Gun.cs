using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Gun : MonoBehaviour {

	private GameObject muzzle;

	private Player player;
	private NavMeshAgent nav;

	public float fireRate = 15f;
	public float damage = 10f;
	public float knockBack = 10f;
	public bool automatic = true;
	public int ammo = 30;
	[Range(0,0.2f)] public float disp=0.1f;
	public int pierce = 0;
	public int bulPerShot = 1;
	public float noise = 8f;

	[Range(0,1f)] public float stress;
	[Range(0,100f)]public float bulletTimeReplenishFactor;

	private AudioSource audioSource;
	private ParticleSystem muzzleFlash;
	public GameObject bulletImpact;
	public AudioClip shotSound;
	public float forceEjec= 50f;

	public GameObject bullet;
	public float bulletForce;

	[Range(0,4)] public int holdingPose;
	private GameObject ejector;
	public GameObject cartridge;





	public void Start(){
		
		muzzle = transform.Find("Muzzle").gameObject;
		muzzleFlash = muzzle.GetComponent<ParticleSystem>();
		audioSource = GetComponent<AudioSource>();
		if(transform.Find("Ejector") != null) ejector = transform.Find("Ejector").gameObject;

	}

	public void SetOwner(Player p){
		nav = null;
		player = p;
	}

	public void SetOwner(NavMeshAgent n){
		nav = n;
		player = null;
	}

	public bool Shoot (Vector3 startPos, Vector3 aimPos, bool consumesAmmo = true)
	{
		if (ammo != 0 || !consumesAmmo) {
			if(consumesAmmo) ammo--;
			muzzleFlash.Play ();
			audioSource.Stop ();
			audioSource.PlayOneShot (shotSound);
			audioSource.pitch = Mathf.Pow (Time.timeScale, 0.25f);

			if (gameObject.layer == LayerMask.NameToLayer ("Player") && noise >0) {
				Collider[] colliders;
				colliders = Physics.OverlapSphere (startPos, noise);
				foreach (Collider col in colliders) {
					if (col.gameObject.layer == LayerMask.NameToLayer ("Enemy")) {
						Enemy enemy;
						enemy = col.gameObject.GetComponent<Enemy> ();
						if (enemy != null)
							enemy.targetHeard = true;
					}

				}

			}


			for (int i = 0; i < bulPerShot; i++) {
				GameObject bul = Instantiate (bullet, startPos, muzzle.transform.rotation);
				bul.GetComponent<Rigidbody> ().AddForce (((1f - disp) * (aimPos - startPos).normalized + disp * Vector3.ProjectOnPlane (Random.onUnitSphere, Vector3.up).normalized).normalized * bulletForce);
				Bullet bulStats = bul.GetComponent<Bullet> ();

				if (gameObject.layer == LayerMask.NameToLayer ("Player"))
					bul.layer = LayerMask.NameToLayer ("Friendly Projectile");
				else
					bul.layer = LayerMask.NameToLayer ("Enemy Projectile");

				bulStats.damage = damage;
				bulStats.knockBack = knockBack;
				bulStats.pierce = pierce;
			
			}

			if (cartridge != null) {
				Vector3 velocity;
				if (player != null) {
					velocity = player.speed;
				} else if (nav != null) {
					velocity = nav.velocity;
				}
			 	else velocity = Vector3.zero;
				GameObject car = GameObject.Instantiate (cartridge, ejector.transform.position,Quaternion.LookRotation(ejector.transform.right,ejector.transform.forward));
				car.GetComponent<Rigidbody> ().AddForce (forceEjec*velocity+(100f + Random.value * 40f) * ejector.transform.right + (-10f + Random.value*20f)* ejector.transform.forward );

			}

			return true;
		}else return false;
	}







}

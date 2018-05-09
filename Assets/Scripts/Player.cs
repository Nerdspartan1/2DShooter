using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : Unit {


	private CharacterController controller;
	private Animator anim;
	private AimAt aiming;
	private WeaponSwitch weaponSwitch;
	private GameManager gameManager;
	private CameraTrack mainCamScript;


	// Physics
	public Vector3 speed;
	public float maxSpeed;
	public float jumpSpeed;
	public float gravity;


	private bool isGrounded;
	private bool wasGroundedLastFrame;

	public Transform groundCheck;
	//private float groundRadius = .2f;
	[HideInInspector] public Image healthBarImage;
	[HideInInspector] public Image bulletTimeImage;
	public float bulletTimeTimeMax;
	private float bulletTimeTime;
	private bool inBulletTime = false;
	private IEnumerator bulletTime;


	private float nextTimeToFire = 0f;
	public GameObject shootingPivot;
	private GameObject bulletStart;

	public GameObject compass;



	new void Start ()
	{
		
		gameManager = GameManager.Instance;

		mainCamScript= transform.Find("Main Camera").GetComponent<CameraTrack>();
		transform.Find("Main Camera").parent = null;

		speed = Vector3.zero;
		controller = GetComponent<CharacterController> ();
		anim = GetComponent<Animator> ();
		aiming = GetComponent<AimAt> ();
		bulletStart=shootingPivot.transform.FindChild("BulletStart").gameObject;
		if(bulletStart == null)	Debug.Log("Error : BulletStart not found !");
		aiming.planePos = Vector3.Project(bulletStart.transform.position,Vector3.up);
		weaponSwitch = GetComponent<WeaponSwitch> ();

		healthBarImage = transform.Find("Canvas/HealthBarBG/HealthBar").GetComponent<Image>();
		bulletTimeImage = transform.Find("Canvas/BulletTimeBar").GetComponent<Image>();

		bulletTimeTime = 0;

		base.Start();

		compass = GameObject.Find("Compass");
		compass.SetActive(false);



	}

	void Update ()
	{
		if (!IsDead ()) {
			Control ();
			Animate ();
			bulletTimeImage.fillAmount = bulletTimeTime / bulletTimeTimeMax;
			if (bulletTimeTime == bulletTimeTimeMax && Input.GetKeyDown (KeyCode.Space))
				DoBulletTime (0.2f);
		}


	}

	public override void Death (Vector3 knockback)
	{
		base.Death(knockback);
		aiming.enabled = false;
		weaponSwitch.enabled = false;
		gameManager.ShowGameOver(true);
		if (weaponSwitch.equippedWeapon != null) {
			weaponSwitch.equippedWeapon.transform.parent = null;
			weaponSwitch.equippedWeaponRb.isKinematic = false;
		}
		InterruptBulletTime();
	}

	void Control ()
	{
	if(!gameManager.gamePaused){
		isGrounded = false;

		//Si il y a du sol dans le groudCheck, ie sous le joueur
		Collider[] colliders = Physics.OverlapSphere (groundCheck.position, 0.2f);
		foreach (Collider col in colliders) {
			if (col.gameObject.layer == 8)
				isGrounded = true;
		}
		//Mouvement
		Vector3 spd = Input.GetAxis ("Horizontal") * Vector3.right + Input.GetAxis ("Vertical") * Vector3.forward;
		if (spd.x != 0 && spd.z != 0)
			spd /= Mathf.Sqrt (2);
		speed.x = spd.x * maxSpeed;
		speed.z = spd.z * maxSpeed;
		/*
		//Saut
		if (isGrounded) {
			if (Input.GetKeyDown ("space")) {
				speed.y = jumpSpeed;
				anim.SetTrigger ("jump");
			}
			if (speed.y < 0)
				speed.y = 0;
		}
		*/
		//Gravité
		if (!isGrounded)
			speed.y -= gravity * Time.deltaTime;
		
		//Si vitesse non nulle, orienter le joueur dans la direction de la vitesse projetée sur le plan horizontal, et dans la direction opposée si il recule en regardant devant lui
		if (Vector3.ProjectOnPlane (speed, Vector3.up).magnitude > 0.1f) {
			int flip;
			if (aiming.looksAtTarget)
				flip = (Vector3.Dot (speed, aiming.AimDir ()) >= 0) ? 1 : -1;
			else
				flip = 1;
			transform.rotation = Quaternion.LookRotation (Vector3.ProjectOnPlane (flip * speed, Vector3.up));
		}
			

		

		//Update Plan de jeu
		//aiming.planePos = bulletStart.transform.position;

		//Position du BulletStart
		shootingPivot.transform.rotation = Quaternion.LookRotation (aiming.TrueAimDir ());
		Debug.DrawRay (shootingPivot.transform.position, aiming.TrueAimDir ());

		//Feu
		if (weaponSwitch.equippedWeapon != null && Time.time >= nextTimeToFire) {
			if ((weaponSwitch.equippedWeaponGunScr.automatic && Input.GetButton ("Fire1")) || Input.GetButtonDown ("Fire1")) {
				if (weaponSwitch.equippedWeaponGunScr.Shoot (bulletStart.transform.position, aiming.aimPos)) {
					anim.SetTrigger ("shoots");
					nextTimeToFire = Time.time + 1 / weaponSwitch.equippedWeaponGunScr.fireRate;
					mainCamScript.StressUp (weaponSwitch.equippedWeaponGunScr.stress);
				}
			}
		}
		//WeaponSwitch
		if (Input.GetButtonDown ("Fire2")) {
			if (weaponSwitch.equippedWeapon == weaponSwitch.defaultWeapon) {
				weaponSwitch.PickUpWeapon ();
			} else {
				GameObject droppedWeapon;
				weaponSwitch.DropWeapon (out droppedWeapon);
				weaponSwitch.PickUpWeapon (droppedWeapon);
			}
		}
	}
	}


	void Animate ()
	{
		//Update Animation
		controller.Move (speed * Time.deltaTime);
		anim.SetFloat ("speed", Vector3.ProjectOnPlane (speed, Vector3.up).magnitude);
		anim.SetBool ("runsBackward", !(Vector3.Dot (speed, aiming.AimDir ()) >= 0));
		//anim.SetBool ("grounded", isGrounded);
		/*
		if (!isGrounded && wasGroundedLastFrame)
			anim.SetTrigger ("falls"); //if start falling

		wasGroundedLastFrame = isGrounded;
		*/
	}

	void LateUpdate ()
	{
		if (gameManager.sceneCleared) {
			compass.SetActive (true);
			compass.transform.rotation = Quaternion.LookRotation(gameManager.nextScene.transform.position - transform.position);
		}
	}

	public void DoBulletTime (float slowFactor)
	{
		bulletTime = DoBulletTimeCoroutine(slowFactor);
		StartCoroutine(bulletTime);

	}

	IEnumerator DoBulletTimeCoroutine (float slowFactor)
	{
		inBulletTime = true;
		Time.timeScale = slowFactor;

		mainCamScript.SetChromaticAberrationIntensity(1);
		while (bulletTimeTime >0) {
			yield return null;
			if(!gameManager.gamePaused) bulletTimeTime -=Time.unscaledDeltaTime;
			bulletTimeImage.fillAmount = Mathf.Pow(bulletTimeTime/bulletTimeTimeMax,2f);

		}
		inBulletTime = false;
		while (Time.timeScale < 1) {
			if(!gameManager.gamePaused) Time.timeScale += 0.5f*Time.unscaledDeltaTime;
			mainCamScript.SetChromaticAberrationIntensity((Time.timeScale-1)/(slowFactor-1));
			yield return null;
		}
	}

	void InterruptBulletTime ()
	{
		if (bulletTime!= null)
			StopCoroutine(bulletTime);
		Time.timeScale = 1;
		mainCamScript.SetChromaticAberrationIntensity(0);

	}

	public void ReplenishBulletTime (float per)
	{
		if(!inBulletTime)
		bulletTimeTime =Mathf.Clamp(bulletTimeTime+ per*bulletTimeTimeMax/100, 0, bulletTimeTimeMax);
		
	}

	public override void TakeDamage (float damage, Vector3 knockback)
	{
		base.TakeDamage(damage,knockback);
		float h = hp / maxHp;
		healthBarImage.fillAmount = h;
		if (h > 0.5f) {
			healthBarImage.color = Color.green + (1-2*(h-0.5f)) * Color.red;
		} else {
			healthBarImage.color= Color.red + 2*h*Color.green;
		}
	}
}

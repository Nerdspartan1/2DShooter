using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour {

	public GameObject defaultWeapon;
	public GameObject equippedWeapon;
	public Rigidbody equippedWeaponRb;
	public Gun equippedWeaponGunScr;
	public GameObject weaponHolder;
	public float pickUpRadius=1f;
	private AimAt aiming;


	private Player player;
	private Animator anim;


	private Text gunNameText;
	private Text ammoText;

	private int layerAmount = 2;

	void Start ()
	{
		Transform canvas = transform.Find("Canvas");
		gunNameText = canvas.Find("GunNameText").GetComponent<Text>();
		ammoText = canvas.Find("AmmoText").GetComponent<Text>();
		anim = GetComponent<Animator>();

		aiming = GetComponent<AimAt> ();
		if (defaultWeapon != null) {
			GameObject w = Instantiate(defaultWeapon);
			w.GetComponent<Gun>().ammo = -1;
			w.tag = "Untagged";
			w.name = defaultWeapon.name;
			defaultWeapon = w;
			EquipWeapon(defaultWeapon);
			defaultWeapon.SetActive(true);
		}
		player = GetComponent<Player>();

		for (int i = 1; i <= layerAmount; i++) {
				anim.SetLayerWeight (i, 0);
			}
		
		if (equippedWeapon != null) {
			aiming.looksAtTarget=true;
			anim.SetLayerWeight (equippedWeaponGunScr.holdingPose, 100);
		}
		else{
			Debug.Log("No Weapon !");
			aiming.looksAtTarget=false;
		}
	}

	void Update ()
	{
		if (equippedWeapon != null) {
			ammoText.text = equippedWeaponGunScr.ammo >= 0 ? equippedWeaponGunScr.ammo.ToString () : "Infinite";
			if (equippedWeaponGunScr.ammo == 0) {
				GameObject g;
				DropWeapon(out g);
			}
		}

	}

	public void DropWeapon (out GameObject droppedWeapon)
	{
		droppedWeapon = null;
		if (equippedWeapon != defaultWeapon) {
			
			equippedWeapon.transform.parent = null;
			equippedWeapon.layer =LayerMask.NameToLayer("Prop");
			equippedWeaponRb.isKinematic = false;
			equippedWeaponRb.AddForce (player.speed * 40f + transform.forward * 60f + transform.right * 40f * Random.Range (-1f, 1f));
			equippedWeaponRb.AddTorque (Random.onUnitSphere * 10f);
			equippedWeaponRb = null;
			equippedWeaponGunScr.SetOwner((Player)null);
			equippedWeaponGunScr = null;
			droppedWeapon = equippedWeapon;
			equippedWeapon = defaultWeapon;
			if (equippedWeapon != null) {
				defaultWeapon.SetActive (true);
				equippedWeaponRb = defaultWeapon.GetComponent<Rigidbody>();
				equippedWeaponGunScr = defaultWeapon.GetComponent<Gun>();
				gunNameText.text = equippedWeaponGunScr.name;

			}
			SetAnimLayer(equippedWeaponGunScr);
		}else
			Debug.Log("Error : Dropping weapon when none is equipped !");
	}
	//Pick up a weapon and returns true if a weapon was found, false if none was found
	public bool PickUpWeapon (GameObject notThis = null)
	{
		GameObject weapon = null;
		Collider[] colliders;
		colliders = Physics.OverlapSphere (transform.position, pickUpRadius);
		foreach (Collider col in colliders) {
			if (col.tag == "Weapon" && col.gameObject !=notThis) {
				if(col.GetComponent<Gun>().ammo !=0){
					weapon = col.gameObject;
					break;
				}
			}
		}
		//Si on trouve une arme
		return EquipWeapon(weapon);
	}

	public bool EquipWeapon (GameObject weapon)
	{
		
		if (weapon != null) {
			defaultWeapon.SetActive(false);
			equippedWeapon = weapon;
			equippedWeapon.transform.position = weaponHolder.transform.position;
			equippedWeapon.transform.rotation = weaponHolder.transform.rotation;
			equippedWeapon.transform.SetParent (weaponHolder.transform);
			equippedWeapon.layer=LayerMask.NameToLayer("Player");
			equippedWeaponRb = equippedWeapon.GetComponent<Rigidbody> ();
			equippedWeaponGunScr = equippedWeapon.GetComponent<Gun> ();
			equippedWeaponRb.isKinematic = true;
			equippedWeaponGunScr.SetOwner(GetComponent<Player>());

			gunNameText.text = equippedWeaponGunScr.name;
			SetAnimLayer(equippedWeaponGunScr);
			return true;
		}else return false;
		

	}

	public void SetAnimLayer (Gun gun)
	{
		for (int i = 1; i <= layerAmount; i++) {
			anim.SetLayerWeight (i, 0);
		}
		if (gun !=null) {
				anim.SetLayerWeight (gun.holdingPose, 100);
				aiming.looksAtTarget = true;
		} else {
				aiming.looksAtTarget = false;
		}
	}

}

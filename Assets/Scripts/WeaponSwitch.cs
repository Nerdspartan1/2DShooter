using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwitch : MonoBehaviour {

	public GameObject defaultWeaponObject;
	public GameObject equippedWeaponObject;
	public Rigidbody equippedWeaponRb;
	public Weapon equippedWeapon;
	public GameObject weaponHolder;
	public float pickUpRadius=1f;
	private AimAt aiming;


	private Player player;
	private Animator anim;


	private Text weaponNameText;
	private Text ammoText;

	private int layerAmount = 2;

	void Start ()
	{
		Transform canvas = transform.Find("Canvas");
		weaponNameText = canvas.Find("GunNameText").GetComponent<Text>();
		ammoText = canvas.Find("AmmoText").GetComponent<Text>();
		anim = GetComponent<Animator>();

		aiming = GetComponent<AimAt> ();
		if (defaultWeaponObject != null) {
			GameObject w = Instantiate(defaultWeaponObject);
			w.GetComponent<Weapon>().ammo = -1;
			w.tag = "Untagged";
			w.name = defaultWeaponObject.name;
			defaultWeaponObject = w;
			EquipWeapon(defaultWeaponObject);
			defaultWeaponObject.SetActive(true);
		}
		player = GetComponent<Player>();

		for (int i = 1; i <= layerAmount; i++) {
				anim.SetLayerWeight (i, 0);
			}
		
		if (equippedWeapon != null) {
			aiming.looksAtTarget=true;
			anim.SetLayerWeight (equippedWeapon.holdingPose, 100);
		}
		else{
			Debug.Log("No Weapon !");
			aiming.looksAtTarget=false;
		}
	}

	void Update ()
	{
		if (equippedWeapon != null) {
			ammoText.text = equippedWeapon.ammo >= 0 ? equippedWeapon.ammo.ToString () : "Infinite";
			if (equippedWeapon.ammo == 0) {
				GameObject g;
				DropWeapon(out g);
			}
		}

	}

	public void DropWeapon (out GameObject droppedWeapon)
	{
		droppedWeapon = null;
		if (equippedWeaponObject != defaultWeaponObject) {
			
			equippedWeaponObject.transform.parent = null;
			equippedWeaponObject.layer =LayerMask.NameToLayer("Prop");
			equippedWeaponRb.isKinematic = false;
			equippedWeaponRb.AddForce (player.speed * 40f + transform.forward * 60f + transform.right * 40f * Random.Range (-1f, 1f));
			equippedWeaponRb.AddTorque (Random.onUnitSphere * 10f);
			equippedWeaponRb = null;
			equippedWeapon.SetOwner((Player)null);
			equippedWeapon = null;
			droppedWeapon = equippedWeaponObject;
			equippedWeaponObject = defaultWeaponObject;
			if (equippedWeaponObject != null) {
				defaultWeaponObject.SetActive (true);
				equippedWeaponRb = defaultWeaponObject.GetComponent<Rigidbody>();
				equippedWeapon = defaultWeaponObject.GetComponent<Weapon>();
				weaponNameText.text = equippedWeapon.name;

			}
			SetAnimLayer(equippedWeapon);
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
				if(col.GetComponent<Weapon>().ammo !=0){
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
			defaultWeaponObject.SetActive(false);
			equippedWeaponObject = weapon;
			equippedWeaponObject.transform.position = weaponHolder.transform.position;
			equippedWeaponObject.transform.rotation = weaponHolder.transform.rotation;
			equippedWeaponObject.transform.SetParent (weaponHolder.transform);
			equippedWeaponObject.layer=LayerMask.NameToLayer("Player");
			equippedWeaponRb = equippedWeaponObject.GetComponent<Rigidbody> ();
			equippedWeapon = equippedWeaponObject.GetComponent<Weapon> ();
			equippedWeaponRb.isKinematic = true;
			equippedWeapon.SetOwner(GetComponent<Player>());

			weaponNameText.text = equippedWeapon.name;
			SetAnimLayer(equippedWeapon);
			return true;
		}else return false;
		

	}

	public void SetAnimLayer (Weapon weapon)
	{
		for (int i = 1; i <= layerAmount; i++) {
			anim.SetLayerWeight (i, 0);
		}
		if (weapon !=null) {
				anim.SetLayerWeight (weapon.holdingPose, 100);
				aiming.looksAtTarget = true;
		} else {
				aiming.looksAtTarget = false;
		}
	}

}

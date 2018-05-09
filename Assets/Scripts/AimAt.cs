using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AimAt : MonoBehaviour {

	//private Animator animator;
	public GameObject spine;

	[HideInInspector] public Vector3 aimPos;
	public Camera cam;
	public Vector3 aoffset;

	public bool looksAtTarget = true;
	public bool aimsOnHorizontalPlane = true;

	public GameObject target;
	public bool followMouse = false;
	public LayerMask mouseLaysOn;

	public bool laserActivated = true;
	public GameObject laserSight;
	private LineRenderer laser;

	public float lerpValue = 1f;
	[HideInInspector] public Vector3 trueAimPos;

	 public Vector3 planePos; // point du plan horizontal




	void Start ()
	{
		//animator = GetComponent<Animator> ();
		cam = Camera.main;

		if (laserSight != null) {
			laser = laserSight.GetComponent<LineRenderer> ();
			laser.useWorldSpace = true;
		}else
			laserActivated=false;

		trueAimPos=transform.position+transform.forward*5f;


	}
	

    void LateUpdate ()
	{
		if (followMouse) {
			Ray ray = cam.ScreenPointToRay (Input.mousePosition);// Rayon caméra-position de la souris dans le monde
			Plane plane = new Plane (Vector3.up, planePos); //Plan où on lira l'intersection avec le rayon

			float distance;
			plane.Raycast (ray, out distance);

			RaycastHit hit;
			if (Physics.Raycast (ray, out hit, 1000f, mouseLaysOn))
				aimPos = hit.point+planePos;
			else
				aimPos = ray.GetPoint (distance); // position de la souris dans le world
			Debug.DrawLine (ray.origin, aimPos);
		} else {
			if (target != null) {
				if (aimsOnHorizontalPlane)
					aimPos = Vector3.ProjectOnPlane (target.transform.position, Vector3.up) + planePos;
				else
					aimPos = target.transform.position;

				
			} else {
				Debug.Log ("Erreur : aucune target");
			} 
		}

		trueAimPos = Vector3.Lerp (trueAimPos, aimPos, lerpValue * Time.deltaTime);


		//Orientation du buste vers le point visé
		if (looksAtTarget) {
			spine.transform.LookAt (trueAimPos);
			if (aimsOnHorizontalPlane) {
				Vector3 hrot = spine.transform.eulerAngles;
				hrot.x = 0;
				hrot.z = 0;
				spine.transform.eulerAngles = hrot;
			} 

			spine.transform.Rotate (aoffset);
			Debug.DrawLine (Vector3.ProjectOnPlane (transform.position, Vector3.up) + planePos.y * Vector3.up, trueAimPos);
		}

		//Laser
		if (laserActivated) {
			laser.SetPosition (0, laserSight.transform.position);
			RaycastHit hit;
			if (Physics.Raycast (laserSight.transform.position, trueAimPos - laserSight.transform.position, out hit))
				laser.SetPosition (1, hit.point);
			else
				laser.SetPosition (1, laserSight.transform.position + 100f * laserSight.transform.forward);
		}
		





	}

	public Vector3 AimDir ()
	{
		return (aimPos - (Vector3.ProjectOnPlane(transform.position,Vector3.up) + planePos.y*Vector3.up)).normalized;
	}

	public Vector3 TrueAimDir ()
	{
		return (trueAimPos - (Vector3.ProjectOnPlane(transform.position,Vector3.up) + planePos.y*Vector3.up)).normalized;
	}


}

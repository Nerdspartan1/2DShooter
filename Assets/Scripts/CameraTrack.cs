using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;

public class CameraTrack : MonoBehaviour {

	public GameObject target;
	private AimAt aiming;
	public Vector3 y_offset;
	private Quaternion originalRotation;
	[Range(0,5f)]public float stressingFactor;
	[Range(0,1f)]public float stress;
	public float maxYaw =10f;
	public float maxPitch = 10f;
	public float maxRoll= 10f;
	[Range(0.1f,3f)]public float destress = 0.5f;
	public float shakeSpeed = 1f;

	public PostProcessingProfile profile;

	void Start(){
		aiming = target.GetComponent<AimAt>();
		y_offset = Vector3.Project(transform.position - target.transform.position,Vector3.up);
		transform.eulerAngles = new Vector3(90,0,0);
		originalRotation  = transform.rotation;
		profile = GetComponent<PostProcessingBehaviour>().profile;
	}


	void LateUpdate ()
	{
		stress -= destress * Time.deltaTime;
		stress = Mathf.Clamp (stress, 0, 1);

		transform.position = (target.transform.position + aiming.trueAimPos) / 2 + y_offset;
		if (stress > 0) {
			float yaw = stress * maxYaw * 2*(Mathf.PerlinNoise (0, shakeSpeed*Time.time)-0.5f);
			float pitch = stress * maxPitch * 2*(Mathf.PerlinNoise (10, shakeSpeed*Time.time)-0.5f);
			float roll = stress * maxRoll * 2*(Mathf.PerlinNoise (20, shakeSpeed*Time.time)-0.5f);
			transform.rotation = originalRotation;
			transform.Rotate(yaw, pitch, roll);
		}

	}

	public void StressUp (float val)
	{
		stress += val*stressingFactor;
	}

	public void SetChromaticAberrationIntensity (float intensity)
	{
		ChromaticAberrationModel.Settings settings = new ChromaticAberrationModel.Settings();
		settings.intensity = intensity;
		profile.chromaticAberration.settings = settings;

	}
}

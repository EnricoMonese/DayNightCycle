using UnityEngine;
using System.Collections;
//[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour {

	public Light sun;
	public float secondsInFullDay = 120f;
	public float updateRateInSeconds = 5f;
	[Range(0,1)]
	public float currentTimeOfDay = 0.45f;
	public float timeMultiplier = 1f;
	[Space(10)]
	public Gradient nightDayColor;
	public float maxIntensity = 2f;
	public float minIntensity = 0f;
	public float minPoint = -0.2f;
	public float maxBounceIntensity = 1.0f;
	public float minBounceIntensity = 0.5f;
	public float maxAmbient = 1f;
	public float minAmbient = 0f;
	public float minAmbientPoint = -0.2f;
	public Gradient nightDayFogColor;
	public AnimationCurve fogDensityCurve;
	public float fogScale = 1f;
	public float exposureMultiplier = 1f;
	public float dayAtmosphereThickness = 0.4f;
	public float nightAtmosphereThickness = 0.87f;
	Skybox sky;
	Material skyMat;

	#if UNITY_EDITOR
	[HideInInspector]
	public bool showSettings = false;

	public void UpdateEditor () {
		UpdatePosition ();
		UpdateFX ();		
	}
	public void EditorSetup () {
		sun = GetComponent<Light>();
		skyMat = RenderSettings.skybox;
	}
	#endif
	void Start() {
		sun = GetComponent<Light>();
		skyMat = RenderSettings.skybox;
		InvokeRepeating ("UpdateCycle", updateRateInSeconds, updateRateInSeconds);
	}
	void UpdateCycle () {
		UpdatePosition();
		UpdateFX ();

		currentTimeOfDay += ((Time.deltaTime + updateRateInSeconds) / secondsInFullDay) * timeMultiplier;

		if (Input.GetKeyDown (KeyCode.O)) timeMultiplier *= 0.5f;
		if (Input.GetKeyDown (KeyCode.P)) timeMultiplier *= 2f;

		if (currentTimeOfDay >= 1) {
			currentTimeOfDay = 0;
		}
	}

	void UpdatePosition() {
		sun.transform.localRotation = Quaternion.Euler((currentTimeOfDay * 360f) - 90, 170, 0);
	}

	void UpdateFX () {
		float tRange = 1 - minPoint;
		float dot = Mathf.Clamp01 ((Vector3.Dot (sun.transform.forward, Vector3.down) - minPoint) / tRange);
		float i = ((maxIntensity - minIntensity) * dot) + minIntensity;
		sun.intensity = i;

		i = ((maxBounceIntensity - minBounceIntensity) * dot) + minBounceIntensity;
		sun.bounceIntensity = i;

		tRange = 1 - minAmbientPoint;
		dot = Mathf.Clamp01 ((Vector3.Dot (sun.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
		i = ((maxAmbient - minAmbient) * dot) + minAmbient;
		RenderSettings.ambientIntensity = i;

		sun.color = nightDayColor.Evaluate(dot);
		RenderSettings.ambientLight = sun.color;

		RenderSettings.fogColor = nightDayFogColor.Evaluate(dot);
		RenderSettings.fogDensity = fogDensityCurve.Evaluate(dot) * fogScale;

		i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
		skyMat.SetFloat ("_AtmosphereThickness", i);
		skyMat.SetFloat ("_Exposure", i * exposureMultiplier);
	}
}
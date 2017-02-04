using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class DayNightCycleSkybox : MonoBehaviour
{

	public Light sun;
	public Light moon;
	[Tooltip ("How many seconds make up a day in game time?")]
	public float secondsInFullDay = 120f;
	[Tooltip ("How often do we update the scene in real seconds? (only fires once during Start() function)")]
	public float updateRateInSeconds = 5f;
	[Tooltip ("Normalized time of day – noon is .5")]
	[Range (0, 1)]
	public float currentTimeOfDay = 0.45f;
	[Tooltip ("Normal speed of a day is 1, slow down the day cycle using < 1, speed up the day cycle > 1")]
	public float timeMultiplier = 1f;
	[Tooltip ("Left is midnight, right is noon")]
	[Space (10)]
	public Gradient nightDayColor;
	public float maxIntensity = 2f;
	public float minIntensity = 0f;
	[Tooltip ("Normalized time of day to hit minimum intensity")]
	public float minPoint = -0.2f;
	public float maxBounceIntensity = 1.0f;
	public float minBounceIntensity = 0.5f;
	public float maxAmbient = 1f;
	public float minAmbient = 0f;
	[Tooltip ("Normalized time of day to hit minimum ambient")]
	public float minAmbientPoint = -0.2f;
	[Tooltip ("Left is midnight, right is noon")]
	public Gradient nightDayFogColor;
	[Tooltip ("Left is midnight, right is noon")]
	public AnimationCurve fogDensityCurve;
	[Tooltip ("Multiply density curve by this value")]
	public float fogScale = 1f;
	public float exposureMultiplier = 1f;
	public float dayAtmosphereThickness = 0.4f;
	public float nightAtmosphereThickness = 0.87f;
	Skybox sky;
	Material skyMat;

	#if UNITY_EDITOR
	[HideInInspector]
	public bool showSettings = false;

	public void UpdateEditor ()
	{
		UpdatePosition ();
		UpdateFX ();		
	}

	public void EditorSetup ()
	{
		sun = GetComponent<Light> ();
		skyMat = RenderSettings.skybox;
	}
	#endif
	void Start ()
	{
		sun = GetComponent<Light> ();
		skyMat = RenderSettings.skybox;
		InvokeRepeating ("UpdateCycle", updateRateInSeconds, updateRateInSeconds);
	}

	void UpdateCycle ()
	{
		UpdatePosition ();
		UpdateFX ();

		currentTimeOfDay += ((Time.deltaTime + updateRateInSeconds) / secondsInFullDay) * timeMultiplier;

		if (currentTimeOfDay >= 1) {
			currentTimeOfDay = 0;
		}
	}

	void UpdatePosition ()
	{
		sun.transform.localRotation = Quaternion.Euler ((currentTimeOfDay * 360f) - 90, 170, 0);
	}

	void UpdateFX ()
	{
		float tRange = 1 - minPoint;
		float dot = Mathf.Clamp01 ((Vector3.Dot (sun.transform.forward, Vector3.down) - minPoint) / tRange);
		float i = ((maxIntensity - minIntensity) * dot) + minIntensity;
		sun.intensity = i;
		if (moon != null)
			moon.intensity = 1 - i;

		i = ((maxBounceIntensity - minBounceIntensity) * dot) + minBounceIntensity;
		sun.bounceIntensity = i;

		tRange = 1 - minAmbientPoint;
		dot = Mathf.Clamp01 ((Vector3.Dot (sun.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
		i = ((maxAmbient - minAmbient) * dot) + minAmbient;
		RenderSettings.ambientIntensity = i;

		sun.color = nightDayColor.Evaluate (dot);
		RenderSettings.ambientLight = sun.color;

		RenderSettings.fogColor = nightDayFogColor.Evaluate (dot);
		RenderSettings.fogDensity = fogDensityCurve.Evaluate (dot) * fogScale;

		i = ((dayAtmosphereThickness - nightAtmosphereThickness) * dot) + nightAtmosphereThickness;
		skyMat.SetFloat ("_AtmosphereThickness", i);
		skyMat.SetFloat ("_Exposure", i * exposureMultiplier);
	}
}
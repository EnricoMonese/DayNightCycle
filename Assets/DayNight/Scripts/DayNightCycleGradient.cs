using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEditor;

public class DayNightCycleGradient : MonoBehaviour
{

	public Gradient m_daytimeBackground;
	public Gradient m_nightimeBackground;
	public Material m_vertexShaderMaterial;
	public LayerMask mask = -1;

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

	public GameObject gradientPlane;

	Camera m_mainCamera;
	Camera m_backgroundCamera;

	#if UNITY_EDITOR
	[HideInInspector]
	public bool showSettings = false;

	public void UpdateEditor ()
	{
		UpdatePosition ();
		UpdateFX ();
	}

	public void MakePlaneBasedOnGradient ()
	{
		CreatePlaneMesh[] editors = (CreatePlaneMesh[])Resources.FindObjectsOfTypeAll (typeof(CreatePlaneMesh));
		if (editors.Length > 0) {
			gradientPlane = editors [0].MakeMyPlane (1, m_daytimeBackground.colorKeys.Length, 1, 1, m_daytimeBackground);
			SetPlaneMaterial (gradientPlane);
			SetUpBackgroundCamera ();
		} else {
			CreatePlaneMesh cpmesh = this.gameObject.AddComponent<CreatePlaneMesh> ();
			gradientPlane = cpmesh.MakeMyPlane (1, m_daytimeBackground.colorKeys.Length, 1, 1, m_daytimeBackground);
			SetPlaneMaterial (gradientPlane);
			SetUpBackgroundCamera ();
		}
	}

	#endif
	void Start ()
	{
		m_mainCamera = Camera.main;

		if (!m_mainCamera) {
			Debug.LogError ("Must tag a camera as the main camera for this to work!");
			return;
		}

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
		float intensity = ((maxIntensity - minIntensity) * dot) + minIntensity;
		sun.intensity = intensity;
		if (moon != null)
			moon.intensity = 1 - intensity;

		intensity = ((maxBounceIntensity - minBounceIntensity) * dot) + minBounceIntensity;
		sun.bounceIntensity = intensity;

		tRange = 1 - minAmbientPoint;
		dot = Mathf.Clamp01 ((Vector3.Dot (sun.transform.forward, Vector3.down) - minAmbientPoint) / tRange);
		intensity = ((maxAmbient - minAmbient) * dot) + minAmbient;
		RenderSettings.ambientIntensity = intensity;

		sun.color = nightDayColor.Evaluate (dot);
		RenderSettings.ambientLight = sun.color;

		RenderSettings.fogColor = nightDayFogColor.Evaluate (dot);
		RenderSettings.fogDensity = fogDensityCurve.Evaluate (dot) * fogScale;

		if (m_nightimeBackground.colorKeys.Length == m_daytimeBackground.colorKeys.Length) {

			Color[] colors = new Color[m_daytimeBackground.colorKeys.Length];

			for (int i = 0; i < colors.Length; i++) {

				float backgroundOffset = currentTimeOfDay - .5f; // this makes slider value .5f "noon" or 0 to be the daytime gradient
				backgroundOffset = Mathf.Abs (backgroundOffset) * 2; //by making all values positive, we gradually increase after hitting 0. Multiply by 2 insures that we end up at 1
				//when the slider is at 0 or 1, and 0 when the slider is at .5

				Color32 newColor = Color32.Lerp (m_daytimeBackground.colorKeys [i].color, m_nightimeBackground.colorKeys [i].color, backgroundOffset);
				colors [i] = newColor;
			}

			RenderSettings.ambientGroundColor = colors [0];
			RenderSettings.ambientEquatorColor = colors [colors.Length / 2];
			RenderSettings.ambientSkyColor = colors [colors.Length - 1];

			if (gradientPlane != null)
				UpdateVerticeColors (gradientPlane.GetComponent<MeshFilter> ().sharedMesh, colors);


			//Debug.Log (RenderSettings.ambientEquatorColor.ToString ());
		} else {

			Debug.LogWarning ("Gradients do not have matching number of color keys, not evaluating for ambient colors or updating the background - The daytime gradient has " + m_daytimeBackground.colorKeys.Length
			+ " color keys, and the night gradient has " + m_nightimeBackground.colorKeys.Length);
		}
	}

	#region Background Mesh Creation

	void SetPlaneMaterial (GameObject go)
	{
		Renderer mrendr = go.GetComponent<Renderer> ();
		mrendr.material = m_vertexShaderMaterial;
		UpdateVerticeColors (go.GetComponent<MeshFilter> ().sharedMesh, ColorsFromGradient (m_daytimeBackground));
	}

	Color[] ColorsFromGradient (Gradient g)
	{
		Color[] colors = new Color[g.colorKeys.Length];

		for (int c = 0; c < colors.Length; c++) {
			colors [c] = g.colorKeys [c].color;
		}

		return colors;
	}

	void SetUpBackgroundCamera ()
	{
		string backgroundCamName = "Background Camera";
		m_mainCamera = Camera.main;

		if (!m_mainCamera) {
			Debug.LogError ("Must tag a camera as the main camera for this to work!");
			return;
		}

		if (m_backgroundCamera != null)
			DestroyImmediate (m_backgroundCamera.gameObject);

		GameObject checkDoublecheck = GameObject.Find (backgroundCamName);

		if (checkDoublecheck != null)
			DestroyImmediate (checkDoublecheck);

		m_mainCamera.clearFlags = CameraClearFlags.Depth;
		m_mainCamera.cullingMask = ~mask.value;
		GameObject bgCam = new GameObject (backgroundCamName);
		bgCam.transform.SetParent (m_mainCamera.transform, false);
		m_backgroundCamera = bgCam.AddComponent<Camera> () as Camera;
		m_backgroundCamera.allowHDR = m_mainCamera.allowHDR;
		m_backgroundCamera.allowMSAA = m_mainCamera.allowMSAA;
		m_backgroundCamera.orthographic = m_mainCamera.orthographic;
		if (m_backgroundCamera.orthographic) {
			m_backgroundCamera.orthographicSize = m_mainCamera.orthographicSize;
		} else {
			m_backgroundCamera.fieldOfView = m_mainCamera.fieldOfView;
		}
		m_backgroundCamera.nearClipPlane = m_mainCamera.nearClipPlane;
		m_backgroundCamera.farClipPlane = m_mainCamera.farClipPlane;
		m_backgroundCamera.clearFlags = CameraClearFlags.Depth;
		m_backgroundCamera.depth = m_mainCamera.depth - 10;
		m_backgroundCamera.cullingMask = mask;

		gradientPlane.transform.SetParent (m_backgroundCamera.transform, false);

		gradientPlane.layer = 8;//This is the first user setable layer, which the example is named "background"

		ScaleToFitScreen (gradientPlane.transform);

	}

	void ScaleToFitScreen (Transform t)
	{
 
		float pos = (m_mainCamera.nearClipPlane + 0.01f);
 
		t.position = m_mainCamera.transform.position + m_mainCamera.transform.forward * pos;
 
		float h = Mathf.Tan (m_mainCamera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
 
		t.localScale = new Vector3 (h * m_mainCamera.aspect, h, 0f);
	}

	void UpdateVerticeColors (Mesh m, Color[] colors)
	{
		Vector3[] vertices = m.vertices;

		m.vertices = vertices;

		Color[] m_colors = new Color[vertices.Length];

		for (int i = 0; i < m_colors.Length; i++) {

			int c = i / 2;
			if (c >= colors.Length)
				c = colors.Length - 1;

			m_colors [i] = colors [c];
		}

		m.colors = m_colors;
	}

	#endregion

}

using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class DayNightCycleGradient : MonoBehaviour
{

	public Gradient m_backdropColor;
	public Shader m_shader;
	public LayerMask mask = -1;
	public Vector4 m_elevationsTopDown = new Vector4 (.577f, .3f, -.1f, -.3f);
	public float m_width = 1f;

	private Camera m_mainCamera;
	private Camera m_backgroundCamera;
	private Mesh msh;

	void Start ()
	{
		m_mainCamera = Camera.main;

		if (!m_mainCamera) {
			Debug.LogError ("Must tag a camera as the main camera for this to work!");
			return;
		}

		m_mainCamera.clearFlags = CameraClearFlags.Depth;
		m_mainCamera.cullingMask = ~mask.value;
		m_backgroundCamera = Instantiate (m_mainCamera, m_mainCamera.transform);
		Destroy (m_backgroundCamera.GetComponent<AudioListener> ());
		m_backgroundCamera.depth = m_mainCamera.depth - 1;
		m_backgroundCamera.cullingMask = mask;

		//msh = MakeBackdropMesh (m_backdropColor);

		Material mat = new Material (m_shader);

		GameObject gradientPlane = CreatePlane (m_backdropColor);
		gradientPlane.transform.SetParent (m_backgroundCamera.transform, false);

		gradientPlane.layer = 8;//This is the first user setable layer, which the example is named "background"

		ScaleToFitScreen (gradientPlane.transform);

		//InvokeRepeating ("UpdateColors", Random.Range (0f, LevelManager.instance.m_progress.smoothness), LevelManager.instance.m_progress.smoothness);
	}




	void UpdateColors ()
	{
//		float progress = .1f;
//
////		Color top = Color.Lerp (m_startColors [0], m_endColors [0], progress);
////		Color mid = Color.Lerp (m_startColors [1], m_endColors [1], progress);
////		Color horizon = Color.Lerp (m_startColors [2], m_endColors [2], progress);
////		Color bottom = Color.Lerp (m_startColors [3], m_endColors [3], progress);
////
////		if (msh != null)
////			msh.colors = new Color[8] { top, top, mid, mid, horizon, horizon, bottom, bottom };
////
//		//RenderSettings.ambientLight = mid;
	}

	void ScaleToFitScreen (Transform t)
	{
 
		float pos = (m_mainCamera.nearClipPlane + 0.01f);
 
		t.position = m_mainCamera.transform.position + m_mainCamera.transform.forward * pos;
 
		float h = Mathf.Tan (m_mainCamera.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
 
		t.localScale = new Vector3 (h * m_mainCamera.aspect, h, 0f);
	}


	GameObject CreatePlane (Gradient g)
	{

		int widthSegments = 1;
		int lengthSegments = g.colorKeys.Length;
		int width = 1;
		int length = 1;
		Material mat = new Material (m_shader);

		GameObject plane = new GameObject ("Plane", typeof(MeshFilter), typeof(MeshRenderer));
 
		plane.transform.position = Vector3.zero;
 
 
		MeshFilter meshFilter = plane.GetComponent<MeshFilter> ();
		Renderer mrendr = plane.GetComponent<Renderer> ();
		mrendr.material = mat;
		Mesh m = new Mesh ();
 
		m = new Mesh ();
		m.name = plane.name;

 
		int hCount2 = widthSegments + 1;
		int vCount2 = lengthSegments + 1;
		int numTriangles = widthSegments * lengthSegments * 6;

		int numVertices = hCount2 * vCount2;
 
		Vector3[] vertices = new Vector3[numVertices];
		Vector2[] uvs = new Vector2[numVertices];
		int[] triangles = new int[numTriangles];
		Vector4[] tangents = new Vector4[numVertices];
		Vector4 tangent = new Vector4 (1f, 0f, 0f, -1f);
 
		int index = 0;
		float uvFactorX = 1.0f / widthSegments;
		float uvFactorY = 1.0f / lengthSegments;
		float scaleX = (width * 1.0f) / (widthSegments * 1.0f);
		float scaleY = (length * 1.0f) / (lengthSegments * 1.0f);

		//Debug.Log (scaleX.ToString ("F4") + ", " + scaleY.ToString ("F4"));

		for (int y = 0; y < vCount2; y++) {
			for (int x = 0; x < hCount2; x++) {

				int keys = y;
				if (keys >= g.colorKeys.Length)
					keys = g.colorKeys.Length - 1;

				float yVector = y * scaleY - length / 2f;
				float yAltVector = g.colorKeys [keys].time - length / 2f;

				//Debug.Log ("YVector is " + yVector.ToString ("F4") + ", alt is " + yAltVector.ToString ("F4"));

				vertices [index] = new Vector3 (x * scaleX - width / 2f, yAltVector, 0.0f);

				tangents [index] = tangent;
				uvs [index++] = new Vector2 (x * uvFactorX, y * uvFactorY);
			}
		}

		index = 0;

		for (int y = 0; y < lengthSegments; y++) {
			for (int x = 0; x < widthSegments; x++) {
				triangles [index] = (y * hCount2) + x;
				triangles [index + 1] = ((y + 1) * hCount2) + x;
				triangles [index + 2] = (y * hCount2) + x + 1;
 
				triangles [index + 3] = ((y + 1) * hCount2) + x;
				triangles [index + 4] = ((y + 1) * hCount2) + x + 1;
				triangles [index + 5] = (y * hCount2) + x + 1;
				index += 6;
			}
		}

		Color[] m_colors = new Color[vertices.Length];

		for (int i = 0; i < m_colors.Length; i++) {

			int c = i / 2;
			if (c >= g.colorKeys.Length)
				c = g.colorKeys.Length - 1;

			m_colors [i] = g.colorKeys [c].color;
		}

		m.vertices = vertices;
		m.colors = m_colors;
		m.uv = uvs;
		m.triangles = triangles;
		m.tangents = tangents;
		meshFilter.sharedMesh = m;
		m.RecalculateBounds ();

		return plane;
	}

}

using UnityEngine;
using System.Collections;

 
public class CreatePlaneMesh : MonoBehaviour
{
 
	public enum Orientation
	{
		Horizontal,
		Vertical
	}

	public enum AnchorPoint
	{
		TopLeft,
		TopHalf,
		TopRight,
		RightHalf,
		BottomRight,
		BottomHalf,
		BottomLeft,
		LeftHalf,
		Center
	}


	public Orientation m_orientation = Orientation.Vertical;
	public AnchorPoint m_anchor = AnchorPoint.Center;
	public bool m_addCollider = false;
	public bool m_createAtOrigin = true;
	public bool m_twoSided = false;
	public string m_optionalName;

	int m_widthSegments = 1;
	int m_lengthSegments = 1;
	float m_width = 1.0f;
	float m_length = 1.0f;
 
	public Camera cam;
	GameObject plane;


	public GameObject MakeMyPlane (int widthSegments, int lengthSegments, float width, float length, Gradient gradient)
	{
		m_widthSegments = widthSegments;
		m_lengthSegments = lengthSegments;
		m_width = width;
		m_length = length;

		if (cam == null)
			cam = Camera.main;

		if (plane != null)
			DestroyImmediate (plane);

		plane = new GameObject ();
 
		if (!string.IsNullOrEmpty (m_optionalName))
			plane.name = m_optionalName;
		else
			plane.name = "BackgroundPlane";
 
		if (!m_createAtOrigin && cam)
			plane.transform.position = cam.transform.position + cam.transform.forward * 5.0f;
		else
			plane.transform.position = Vector3.zero;
 
		Vector2 anchorOffset;
		string anchorId;
		switch (m_anchor) {
		case AnchorPoint.TopLeft:
			anchorOffset = new Vector2 (-m_width / 2.0f, m_length / 2.0f);
			anchorId = "TL";
			break;
		case AnchorPoint.TopHalf:
			anchorOffset = new Vector2 (0.0f, m_length / 2.0f);
			anchorId = "TH";
			break;
		case AnchorPoint.TopRight:
			anchorOffset = new Vector2 (m_width / 2.0f, m_length / 2.0f);
			anchorId = "TR";
			break;
		case AnchorPoint.RightHalf:
			anchorOffset = new Vector2 (m_width / 2.0f, 0.0f);
			anchorId = "RH";
			break;
		case AnchorPoint.BottomRight:
			anchorOffset = new Vector2 (m_width / 2.0f, -m_length / 2.0f);
			anchorId = "BR";
			break;
		case AnchorPoint.BottomHalf:
			anchorOffset = new Vector2 (0.0f, -m_length / 2.0f);
			anchorId = "BH";
			break;
		case AnchorPoint.BottomLeft:
			anchorOffset = new Vector2 (-m_width / 2.0f, -m_length / 2.0f);
			anchorId = "BL";
			break;			
		case AnchorPoint.LeftHalf:
			anchorOffset = new Vector2 (-m_width / 2.0f, 0.0f);
			anchorId = "LH";
			break;			
		case AnchorPoint.Center:
		default:
			anchorOffset = Vector2.zero;
			anchorId = "C";
			break;
		}
 
		MeshFilter meshFilter = (MeshFilter)plane.AddComponent (typeof(MeshFilter));
		plane.AddComponent (typeof(MeshRenderer));

		string keypositions = "_";
		foreach (GradientColorKey gck in gradient.colorKeys) {
			keypositions += gck.time.ToString ("F1") + "_";
		}
 
		string planeAssetName = plane.name + keypositions + "_" + m_widthSegments + "x" + m_lengthSegments + "W" + m_width + "L" + m_length + (m_orientation == Orientation.Horizontal ? "H" : "V") + anchorId + ".asset";
		Mesh m = (Mesh)UnityEditor.AssetDatabase.LoadAssetAtPath ("Assets/DayNight/Editor/" + planeAssetName, typeof(Mesh));
 
		if (m == null) {
			m = new Mesh ();
			m.name = plane.name;
 
			int hCount2 = m_widthSegments + 1;
			int vCount2 = m_lengthSegments + 1;
			int numTriangles = m_widthSegments * m_lengthSegments * 6;
			if (m_twoSided) {
				numTriangles *= 2;
			}
			int numVertices = hCount2 * vCount2;
 
			Vector3[] vertices = new Vector3[numVertices];
			Vector2[] uvs = new Vector2[numVertices];
			int[] triangles = new int[numTriangles];
			Vector4[] tangents = new Vector4[numVertices];
			Vector4 tangent = new Vector4 (1f, 0f, 0f, -1f);
 
			int index = 0;
			float uvFactorX = 1.0f / m_widthSegments;
			float uvFactorY = 1.0f / m_lengthSegments;
			float scaleX = (m_width * 1.0f) / (m_widthSegments * 1.0f);
//			float scaleY = (m_length * 1.0f) / (m_lengthSegments * 1.0f);

			for (int y = 0; y < vCount2; y++) {
				for (int x = 0; x < hCount2; x++) {

					int keys = y;
					if (keys >= gradient.colorKeys.Length)
						keys = gradient.colorKeys.Length - 1;

					//float yVector = y * scaleY - length / 2f;//Original calculation for the Wiki
					float yAltVector = gradient.colorKeys [keys].time - length / 2f;

					//Debug.Log ("YVector is " + yVector.ToString ("F4") + ", alt is " + yAltVector.ToString ("F4"));
					if (m_orientation == Orientation.Horizontal) {
						vertices [index] = new Vector3 (x * scaleX - width / 2f, 0.0f, yAltVector);
					} else {
						vertices [index] = new Vector3 (x * scaleX - width / 2f, yAltVector, 0.0f);
					}

					tangents [index] = tangent;
					uvs [index++] = new Vector2 (x * uvFactorX, y * uvFactorY);
				}
			}
 
			index = 0;
			for (int y = 0; y < m_lengthSegments; y++) {
				for (int x = 0; x < m_widthSegments; x++) {
					triangles [index] = (y * hCount2) + x;
					triangles [index + 1] = ((y + 1) * hCount2) + x;
					triangles [index + 2] = (y * hCount2) + x + 1;
 
					triangles [index + 3] = ((y + 1) * hCount2) + x;
					triangles [index + 4] = ((y + 1) * hCount2) + x + 1;
					triangles [index + 5] = (y * hCount2) + x + 1;
					index += 6;
				}
				if (m_twoSided) {
					// Same tri vertices with order reversed, so normals point in the opposite direction
					for (int x = 0; x < m_widthSegments; x++) {
						triangles [index] = (y * hCount2) + x;
						triangles [index + 1] = (y * hCount2) + x + 1;
						triangles [index + 2] = ((y + 1) * hCount2) + x;
 
						triangles [index + 3] = ((y + 1) * hCount2) + x;
						triangles [index + 4] = (y * hCount2) + x + 1;
						triangles [index + 5] = ((y + 1) * hCount2) + x + 1;
						index += 6;
					}
				}
			}
 
			m.vertices = vertices;
			m.uv = uvs;
			m.triangles = triangles;
			m.tangents = tangents;
			m.RecalculateNormals ();
 
			UnityEditor.AssetDatabase.CreateAsset (m, "Assets/DayNight/Editor/" + planeAssetName);
			UnityEditor.AssetDatabase.SaveAssets ();
		}
 
		meshFilter.sharedMesh = m;
		m.RecalculateBounds ();
 
		if (m_addCollider)
			plane.AddComponent (typeof(BoxCollider));
 
		return plane;
	}
}
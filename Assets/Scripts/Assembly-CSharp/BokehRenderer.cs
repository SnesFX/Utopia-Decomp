using UnityEngine;

internal class BokehRenderer
{
	private Texture2D m_CurrentTexture;

	private Material m_FlareMaterial;

	private int m_CurrentWidth;

	private int m_CurrentHeight;

	private float m_CurrentRelativeScaleX;

	private float m_CurrentRelativeScaleY;

	public void RebuildMeshIfNeeded(int width, int height, float spriteRelativeScaleX, float spriteRelativeScaleY, ref Mesh[] meshes)
	{
		if (m_CurrentWidth == width && m_CurrentHeight == height && m_CurrentRelativeScaleX == spriteRelativeScaleX && m_CurrentRelativeScaleY == spriteRelativeScaleY && meshes != null)
		{
			return;
		}
		if (meshes != null)
		{
			Mesh[] array = meshes;
			foreach (Mesh obj in array)
			{
				Object.DestroyImmediate(obj, true);
			}
		}
		meshes = null;
		BuildMeshes(width, height, spriteRelativeScaleX, spriteRelativeScaleY, ref meshes);
	}

	public void BuildMeshes(int width, int height, float spriteRelativeScaleX, float spriteRelativeScaleY, ref Mesh[] meshes)
	{
		int num = 10833;
		int num2 = width * height;
		int num3 = Mathf.CeilToInt(1f * (float)num2 / (1f * (float)num));
		meshes = new Mesh[num3];
		int num4 = num2;
		m_CurrentWidth = width;
		m_CurrentHeight = height;
		m_CurrentRelativeScaleX = spriteRelativeScaleX;
		m_CurrentRelativeScaleY = spriteRelativeScaleY;
		int num5 = 0;
		for (int i = 0; i < num3; i++)
		{
			Mesh mesh = new Mesh();
			mesh.hideFlags = HideFlags.HideAndDontSave;
			int num6 = num4;
			if (num4 > num)
			{
				num6 = num;
			}
			num4 -= num6;
			Vector3[] vertices = new Vector3[num6 * 4];
			int[] triangles = new int[num6 * 6];
			Vector2[] array = new Vector2[num6 * 4];
			Vector2[] array2 = new Vector2[num6 * 4];
			Vector3[] normals = new Vector3[num6 * 4];
			Color[] colors = new Color[num6 * 4];
			float num7 = m_CurrentRelativeScaleX * (float)width;
			float num8 = m_CurrentRelativeScaleY * (float)height;
			for (int j = 0; j < num6; j++)
			{
				int num9 = num5 % width;
				int num10 = (num5 - num9) / width;
				SetupSprite(j, num9, num10, vertices, triangles, array, array2, normals, colors, new Vector2((float)num9 / (float)width, 1f - (float)num10 / (float)height), num7 * 0.5f, num8 * 0.5f);
				num5++;
			}
			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.colors = colors;
			mesh.uv = array;
			mesh.uv2 = array2;
			mesh.normals = normals;
			mesh.RecalculateBounds();
			mesh.UploadMeshData(true);
			meshes[i] = mesh;
		}
	}

	public void Clear(ref Mesh[] meshes)
	{
		if (meshes != null)
		{
			Mesh[] array = meshes;
			foreach (Mesh obj in array)
			{
				Object.DestroyImmediate(obj, true);
			}
		}
		meshes = null;
	}

	public void SetTexture(Texture2D texture)
	{
		m_CurrentTexture = texture;
		m_FlareMaterial.SetTexture("_MainTex", m_CurrentTexture);
	}

	public void SetMaterial(Material flareMaterial)
	{
		m_FlareMaterial = flareMaterial;
	}

	public void RenderFlare(RenderTexture brightPixels, RenderTexture destination, float intensity, ref Mesh[] meshes)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = destination;
		GL.Clear(true, true, Color.black);
		Matrix4x4 matrix = Matrix4x4.Ortho(0f, m_CurrentWidth, 0f, m_CurrentHeight, -1f, 1f);
		m_FlareMaterial.SetMatrix("_FlareProj", matrix);
		m_FlareMaterial.SetTexture("_BrightTexture", brightPixels);
		m_FlareMaterial.SetFloat("_Intensity", intensity);
		if (m_FlareMaterial.SetPass(0))
		{
			for (int i = 0; i < meshes.Length; i++)
			{
				Graphics.DrawMeshNow(meshes[i], Matrix4x4.identity);
			}
		}
		else
		{
			Debug.LogError("Can't render flare mesh");
		}
		RenderTexture.active = active;
	}

	public void SetupSprite(int idx, int x, int y, Vector3[] vertices, int[] triangles, Vector2[] uv0, Vector2[] uv1, Vector3[] normals, Color[] colors, Vector2 targetPixelUV, float halfWidth, float halfHeight)
	{
		int num = idx * 4;
		int num2 = idx * 6;
		triangles[num2] = num;
		triangles[num2 + 1] = num + 2;
		triangles[num2 + 2] = num + 1;
		triangles[num2 + 3] = num + 2;
		triangles[num2 + 4] = num + 3;
		triangles[num2 + 5] = num + 1;
		vertices[num] = new Vector3(0f - halfWidth + (float)x, 0f - halfHeight + (float)y, 0f);
		vertices[num + 1] = new Vector3(halfWidth + (float)x, 0f - halfHeight + (float)y, 0f);
		vertices[num + 2] = new Vector3(0f - halfWidth + (float)x, halfHeight + (float)y, 0f);
		vertices[num + 3] = new Vector3(halfWidth + (float)x, halfHeight + (float)y, 0f);
		Vector2 vector = targetPixelUV;
		colors[num] = new Color((0f - halfWidth) / (float)m_CurrentWidth + vector.x, (0f - halfHeight) * -1f / (float)m_CurrentHeight + vector.y, 0f, 0f);
		colors[num + 1] = new Color(halfWidth / (float)m_CurrentWidth + vector.x, (0f - halfHeight) * -1f / (float)m_CurrentHeight + vector.y, 0f, 0f);
		colors[num + 2] = new Color((0f - halfWidth) / (float)m_CurrentWidth + vector.x, halfHeight * -1f / (float)m_CurrentHeight + vector.y, 0f, 0f);
		colors[num + 3] = new Color(halfWidth / (float)m_CurrentWidth + vector.x, halfHeight * -1f / (float)m_CurrentHeight + vector.y, 0f, 0f);
		normals[num] = -Vector3.forward;
		normals[num + 1] = -Vector3.forward;
		normals[num + 2] = -Vector3.forward;
		normals[num + 3] = -Vector3.forward;
		uv0[num] = new Vector2(0f, 0f);
		uv0[num + 1] = new Vector2(1f, 0f);
		uv0[num + 2] = new Vector2(0f, 1f);
		uv0[num + 3] = new Vector2(1f, 1f);
		uv1[num] = targetPixelUV;
		uv1[num + 1] = targetPixelUV;
		uv1[num + 2] = targetPixelUV;
		uv1[num + 3] = targetPixelUV;
	}
}

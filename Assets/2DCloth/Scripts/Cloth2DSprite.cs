using UnityEngine;

namespace Cloth2D
{
	[ExecuteInEditMode()]
	[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
	public class Cloth2DSprite : MonoBehaviour
	{
		public Texture Sprite;
		public Vector2 Scale = Vector2.one;
		public Material SpriteMaterial;
		public float PixelPerMeter = 100f;
		public string SortingLayer;
		public int SortingOrder;
		[Range(1, 20)]
		public int Density = 3;
		public Color Color = Color.white;
		public Shader Shader;

		private Cloth2D cloth2D;
		private MeshRenderer meshRenderer;
		private MeshFilter meshFilter;
		private MaterialPropertyBlock propertyBlock;
		private const float VerticesZRange = 1f;
		private static readonly int MainTex = Shader.PropertyToID("_MainTex");

		private void Awake()
		{
			cloth2D = GetComponent<Cloth2D>();
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			CreateSpriteMaterial();
			CreateMesh();
		}

		/// <summary>
		/// Create Material for MeshRender component
		/// </summary>
		private void CreateSpriteMaterial()
		{
			if (SpriteMaterial == null)
			{
				if (Shader == null)
				{
					Shader = Shader.Find("2DCloth/VertexColor");
				}
				SpriteMaterial = new Material(Shader);
				meshRenderer.sharedMaterial = SpriteMaterial;
			}
		}

		/// <summary>
		/// Create the Mesh
		/// </summary>
		private void CreateMesh()
		{
			UpdateMesh();
			UpdateRenderParams();
		}

		/// <summary>
		/// Update the Mesh
		/// </summary>
		private void UpdateMesh()
		{
			if (Sprite == null)
			{
				return;
			}

			var mesh = new Mesh();
			var lossyScale = transform.lossyScale;
			var size = new Vector3(Sprite.width / PixelPerMeter * Scale.x * lossyScale.x, (Sprite.height / PixelPerMeter) * Scale.y * lossyScale.y);
			var sizeWithoutScale = new Vector3(Sprite.width / PixelPerMeter * Scale.x, (Sprite.height / PixelPerMeter) * Scale.y);
			sizeWithoutScale = new Vector2(Mathf.Abs(sizeWithoutScale.x), Mathf.Abs(sizeWithoutScale.y));
			size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
			var offset = sizeWithoutScale / 2f;
			int jointsHorizontalCount;
			int jointsVerticalCount;
			if (size.x == size.y)
			{
				jointsHorizontalCount = jointsVerticalCount = Density + 2;
			}
			else if (size.x > size.y)
			{
				jointsVerticalCount = Density + 2;
				jointsHorizontalCount = Mathf.RoundToInt(size.x / size.y * jointsVerticalCount);
			}
			else
			{
				jointsHorizontalCount = Density + 2;
				jointsVerticalCount = Mathf.RoundToInt(size.y / size.x * jointsHorizontalCount);
			}
			cloth2D.SetJointsCount(jointsHorizontalCount, jointsVerticalCount);
			var verticesCount = (jointsHorizontalCount + 1) * (jointsVerticalCount + 1);
			if (jointsHorizontalCount <= 0 || jointsVerticalCount <= 0 || verticesCount <= 0 || verticesCount >= 65535)
			{
				return;
			}

			var vertices = new Vector3[verticesCount];
			var triangles = new int[jointsHorizontalCount * jointsVerticalCount * 6];
			var uv = new Vector2[verticesCount];
			var colors = new Color[verticesCount];
			for (int y = 0, i = 0; y <= jointsVerticalCount; y++)
			{
				var z = VerticesZRange * (y / (float)jointsVerticalCount);
				for (var x = 0; x <= jointsHorizontalCount; x++, i++)
				{
					var zX = 0.1f * (x / (float)jointsHorizontalCount);

					vertices[i] = new Vector3(
						sizeWithoutScale.x * x / jointsHorizontalCount,
						sizeWithoutScale.y - sizeWithoutScale.y * y / jointsVerticalCount,
						0) - offset;
					uv[i] = new Vector3(x / (float) jointsHorizontalCount, 1f - y / (float) jointsVerticalCount, zX + z);
					colors[i] = Color;
				}
			}

			for (int y = 0, ti = 0, vi = 0; y < jointsVerticalCount; y++, vi++)
			{
				for (var x = 0; x < jointsHorizontalCount; x++, ti += 6, vi++)
				{
					triangles[ti + 0] = vi;
					triangles[ti + 1] = vi + 1;
					triangles[ti + 2] = vi + jointsHorizontalCount + 1;
					triangles[ti + 3] = vi + jointsHorizontalCount + 1;
					triangles[ti + 4] = vi + 1;
					triangles[ti + 5] = vi + jointsHorizontalCount + 2;
				}
			}

			mesh.vertices = vertices;
			mesh.triangles = triangles;
			mesh.uv = uv;
			mesh.colors = colors;
			meshFilter.sharedMesh = mesh;
		}

		/// <summary>
		/// Update Render Parameters (Material and MeshRender)
		/// </summary>
		private void UpdateRenderParams()
		{
			CreateSpriteMaterial();
			UpdateMeshRenderParams();
		}

		/// <summary>
		/// Update MeshRender parameters
		/// </summary>
		private void UpdateMeshRenderParams()
		{
			if (Sprite == null)
				return;
			
			if (propertyBlock == null)
			{
				propertyBlock = new MaterialPropertyBlock();
			}
			meshRenderer.sortingLayerName = SortingLayer;
			meshRenderer.sortingOrder = SortingOrder;
			meshRenderer.GetPropertyBlock(propertyBlock);
			propertyBlock.SetTexture(MainTex, Sprite);
			meshRenderer.SetPropertyBlock(propertyBlock);
		}

		/// <summary>
		/// Returns Cloth2D component
		/// </summary>
		/// <returns></returns>
		public Cloth2D GetCloth2D()
		{
			return cloth2D;
		}

		/// <summary>
		/// Force update Mesh, material and render parameters
		/// </summary>
		public void ForceUpdate()
		{
			UpdateMesh();
			UpdateRenderParams();
		}
	}
}
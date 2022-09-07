using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Cloth2D.Editor
{
	[CustomEditor(typeof(Cloth2DSprite))]
	public class Cloth2DSpriteInspector : UnityEditor.Editor
	{
		private SerializedProperty textureProp;
		private SerializedProperty scaleProp;
		private SerializedProperty materialProp;
		private SerializedProperty sortingOrderProp;
		private SerializedProperty pixelPerMeterProp;
		private SerializedProperty densityProp;
		private SerializedProperty colorProp;

		private Cloth2DSprite sprite;
		private string[] sortingLayerNames;
		private string currentSortingLayer = "Default";

		private const float MinPixelPerMeter = 1f;
		private const float MaxPixelPerMeter = 100f;
		private const float WidthOffset = 50f;
		private const int MinDensity = 1;
		private const int MaxDensity = 20;
		private const int SpriteHeight = 128;
		
		[MenuItem("Component/Rendering/2D Cloth Sprite", false, 32)]
		static void AddSoftSprite()
		{
			Selection.activeGameObject.AddComponent<Cloth2DSprite>();
		}

		void Awake()
		{
			Undo.undoRedoPerformed += UpdateSprite;
		}

		void OnEnable()
		{
			textureProp = serializedObject.FindProperty("Sprite");
			scaleProp = serializedObject.FindProperty("Scale");
			materialProp = serializedObject.FindProperty("SpriteMaterial");
			sortingOrderProp = serializedObject.FindProperty("SortingOrder");
			pixelPerMeterProp = serializedObject.FindProperty("PixelPerMeter");
			densityProp = serializedObject.FindProperty("Density");
			colorProp = serializedObject.FindProperty("Color");

			sortingLayerNames = new string[SortingLayer.layers.Length];
			for (var i = 0; i < SortingLayer.layers.Length; i++)
			{
				var layer = SortingLayer.layers[i];
				sortingLayerNames[i] = layer.name;
			}
			sprite = target as Cloth2DSprite;
			if (sprite != null)
			{
				currentSortingLayer = sprite.GetComponent<MeshRenderer>().sortingLayerName;
			}
		}

		private void UpdateSprite()
		{
			if (sprite != null)
			{
				try
				{
					sprite.Sprite = textureProp.objectReferenceValue as Texture;
					sprite.Scale = scaleProp.vector2Value;
					sprite.SpriteMaterial = materialProp.objectReferenceValue as Material;
					sprite.SortingLayer = currentSortingLayer;
					sprite.SortingOrder = sortingOrderProp.intValue;
					sprite.PixelPerMeter = pixelPerMeterProp.floatValue;
					sprite.Density = densityProp.intValue;
					sprite.Color = colorProp.colorValue;
					if (sprite.gameObject.activeInHierarchy)
					{
						sprite.ForceUpdate();
					}
				}
				catch
				{
					//ignore
				}
			}
		}

		private int GetLayerIndex(string layerToFind)
		{
			if (sortingLayerNames.Length == 0 || sortingLayerNames == null)
				return -1;
			for (var i = 0; i < sortingLayerNames.Length; i++)
			{
				if (sortingLayerNames[i].Equals(layerToFind))
					return i;
			}
			return -1;
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			sprite = target as Cloth2DSprite;
			if (textureProp.objectReferenceValue != null)
			{
				var rect = GUILayoutUtility.GetRect(EditorGUIUtility.currentViewWidth, SpriteHeight, GUILayout.Width(EditorGUIUtility.currentViewWidth - WidthOffset));
				GUI.DrawTexture(rect, (Texture) textureProp.objectReferenceValue, ScaleMode.ScaleToFit);
				EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.currentViewWidth - WidthOffset));
			}
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(textureProp, new GUIContent("Texture"));
			scaleProp.vector2Value = EditorGUILayout.Vector2Field("Scale", scaleProp.vector2Value);
			EditorGUILayout.PropertyField(materialProp, new GUIContent("Material"));
			var sortingLayerIndex = GetLayerIndex(currentSortingLayer);
			sortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", sortingLayerIndex, sortingLayerNames, GUILayout.ExpandWidth(true));
			currentSortingLayer = sortingLayerNames[sortingLayerIndex];
			EditorGUILayout.PropertyField(sortingOrderProp, new GUIContent("Sorting Order"));
			EditorGUILayout.Slider(pixelPerMeterProp, MinPixelPerMeter, MaxPixelPerMeter, new GUIContent("Pixel Per Meter"));
			EditorGUILayout.IntSlider(densityProp, MinDensity, MaxDensity, new GUIContent("Density"));
			colorProp.colorValue = EditorGUILayout.ColorField("Color", colorProp.colorValue);
			serializedObject.ApplyModifiedProperties();
			if (EditorGUI.EndChangeCheck())
			{
				if (!Application.isPlaying && sprite != null)
				{
					EditorUtility.SetDirty(sprite);
					EditorSceneManager.MarkSceneDirty(sprite.gameObject.scene);
				}
				UpdateSprite();
			}
			if (GUILayout.Button("Update"))
			{
				UpdateSprite();
				if (sprite.Sprite != null)
				{
					var cloth = sprite.GetCloth2D();
					if (cloth != null)
					{
						cloth.DestroyJoints();
						cloth.GenerateJoints();
					}
				}
			}
		}
	}
}
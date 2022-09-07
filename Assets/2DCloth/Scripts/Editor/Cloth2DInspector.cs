using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Cloth2D.Editor
{
    [CustomEditor(typeof(Cloth2D))]
    public class Cloth2DInspector : UnityEditor.Editor
    {
	    private SerializedProperty jointsTypeProp;
	    private SerializedProperty lockTopProp;
		private SerializedProperty lockBottomProp;
		private SerializedProperty lockLeftProp;
	    private SerializedProperty lockRightProp;
	    private SerializedProperty materialProp;
	    private SerializedProperty linearDragProp;
	    private SerializedProperty angularDragProp;
	    private SerializedProperty gravityScaleProp;
	    private SerializedProperty useAutoMassProp;
	    private SerializedProperty massTopProp;
	    private SerializedProperty massBottomProp;
	    private SerializedProperty autoConfigureDistanceProp;
	    private SerializedProperty autoConfigureConnectedAnchorProp;
	    private SerializedProperty maxDistanceOnlyProp;
	    private SerializedProperty enableCollisionProp;
	    private SerializedProperty dampingRatioProp;
	    private SerializedProperty frequencyProp;
	    private SerializedProperty jointsLayerProp;	    
	    private SerializedProperty jointsProp;
	    private SerializedProperty useInnerCollidersProp;
	    private SerializedProperty innerRadiusRatioProp;
	    private SerializedProperty innerMaterialProp;
	    private SerializedProperty innerCollidersLayerProp;

		private Cloth2D cloth;

		private const float WidthOffset = 50f;
		private const string UndoActionName = "ClothAdd";
		private const string RedoActionName = "ClothRem";

		[MenuItem("Component/Physics 2D/2D Cloth", false, 32)]
		static void AddCloth()
		{
			Selection.activeGameObject.AddComponent<Cloth2D>();
		}

		void Awake()
		{
			Undo.undoRedoPerformed += Generate;
			Undo.undoRedoPerformed += Remove;
		}

		void OnEnable()
		{
			jointsTypeProp = serializedObject.FindProperty("JointsType");
			lockTopProp = serializedObject.FindProperty("LockTop");
			lockBottomProp = serializedObject.FindProperty("LockBottom");
			lockLeftProp = serializedObject.FindProperty("LockLeft");
			lockRightProp = serializedObject.FindProperty("LockRight");
			materialProp = serializedObject.FindProperty("PhysicsMaterial");
			linearDragProp = serializedObject.FindProperty("LinearDrag");
			angularDragProp = serializedObject.FindProperty("AngularDrag");
			gravityScaleProp = serializedObject.FindProperty("GravityScale");
			useAutoMassProp = serializedObject.FindProperty("UseAutoMass");
			massTopProp = serializedObject.FindProperty("MassTop");
			massBottomProp = serializedObject.FindProperty("MassBottom");
			autoConfigureDistanceProp = serializedObject.FindProperty("AutoConfigureDistance");
			autoConfigureConnectedAnchorProp = serializedObject.FindProperty("AutoConfigureConnectedAnchor");
			maxDistanceOnlyProp = serializedObject.FindProperty("MaxDistanceOnly");
			enableCollisionProp = serializedObject.FindProperty("EnableCollision");
			dampingRatioProp = serializedObject.FindProperty("DampingRatio");
			frequencyProp = serializedObject.FindProperty("Frequency");
			jointsProp = serializedObject.FindProperty("joints");
			jointsLayerProp = serializedObject.FindProperty("JointsLayer");
			useInnerCollidersProp = serializedObject.FindProperty("UseInnerColliders");
			innerRadiusRatioProp = serializedObject.FindProperty("InnerRadiusRatio");
			innerMaterialProp = serializedObject.FindProperty("InnerPhysicsMaterial");
			innerCollidersLayerProp = serializedObject.FindProperty("InnerCollidersLayer");
		}

		private void UpdateCloth(bool regenerateCompletely)
		{
			if (cloth != null)
			{
				cloth.JointsType = (Cloth2D.JointType) jointsTypeProp.enumValueIndex;
				cloth.LockTop = lockTopProp.boolValue;
				cloth.LockBottom = lockBottomProp.boolValue;
				cloth.LockLeft = lockLeftProp.boolValue;
				cloth.LockRight = lockRightProp.boolValue;
				cloth.PhysicsMaterial = materialProp.objectReferenceValue as PhysicsMaterial2D;
				cloth.LinearDrag = linearDragProp.floatValue;
				cloth.AngularDrag = angularDragProp.floatValue;
				cloth.GravityScale = gravityScaleProp.floatValue;
				cloth.UseAutoMass = useAutoMassProp.boolValue;
				cloth.MassTop = massTopProp.floatValue;
				cloth.MassBottom = massBottomProp.floatValue;
				cloth.AutoConfigureDistance = autoConfigureDistanceProp.boolValue;
				cloth.MaxDistanceOnly = maxDistanceOnlyProp.boolValue;
				cloth.EnableCollision = enableCollisionProp.boolValue;
				cloth.DampingRatio = dampingRatioProp.floatValue;
				cloth.Frequency = frequencyProp.floatValue;
				cloth.JointsLayer = jointsLayerProp.intValue;
				cloth.UseInnerColliders = useInnerCollidersProp.boolValue;
				cloth.InnerRadiusRatio = innerRadiusRatioProp.floatValue;
				cloth.InnerPhysicsMaterial = innerMaterialProp.objectReferenceValue as PhysicsMaterial2D;
				cloth.InnerCollidersLayer = innerCollidersLayerProp.intValue;
				if (regenerateCompletely)
				{
					Remove();
					Generate();
					serializedObject.Update();
				}
				else
				{
					cloth.UpdateValues();
				}
			}
		}
		
		private void Generate()
		{
			if (cloth != null)
			{
				Undo.RecordObject(cloth, UndoActionName);
				var joints = cloth.GenerateJoints();
				foreach (var joint in joints)
				{
					Undo.RegisterCreatedObjectUndo(joint.gameObject, UndoActionName);
				}
				Undo.RecordObject(cloth, UndoActionName);
			}
		}

		private void Remove()
		{
			if (cloth != null)
			{
				Undo.RecordObject(cloth, RedoActionName);
				cloth.DestroyJoints();
				Undo.RecordObject(cloth, RedoActionName);
			}
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
 			cloth = target as Cloth2D;
            
            //joint type and lock
            EditorGUI.BeginChangeCheck();
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            EditorGUILayout.PropertyField(jointsTypeProp, new GUIContent("Joints Type"));
            EditorGUI.EndDisabledGroup();
            var jointsTypeChanged = EditorGUI.EndChangeCheck();
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.currentViewWidth - WidthOffset));
            EditorGUILayout.PropertyField(lockTopProp, new GUIContent("Lock top"));
			EditorGUILayout.PropertyField(lockBottomProp, new GUIContent("Lock bottom"));
			EditorGUILayout.PropertyField(lockLeftProp, new GUIContent("Lock left"));
            EditorGUILayout.PropertyField(lockRightProp, new GUIContent("Lock right"));
            EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.currentViewWidth - WidthOffset));

            //rigidbody
            EditorGUILayout.PropertyField(materialProp, new GUIContent("Physics Material 2D"));
            EditorGUILayout.PropertyField(linearDragProp, new GUIContent("Linear Drag"));
            EditorGUILayout.PropertyField(angularDragProp, new GUIContent("Angular Drag"));
            EditorGUILayout.PropertyField(gravityScaleProp, new GUIContent("Gravity Scale"));
            EditorGUILayout.PropertyField(useAutoMassProp, new GUIContent("Use Auto Mass"));
            if (!useAutoMassProp.boolValue)
            {
	            EditorGUILayout.PropertyField(massTopProp, new GUIContent("Mass Top Value"));
	            EditorGUILayout.PropertyField(massBottomProp, new GUIContent("Mass Bottom Value"));
            }
            EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.currentViewWidth - WidthOffset));
            
            //joints
            EditorGUILayout.PropertyField(autoConfigureDistanceProp, new GUIContent("AutoConfigure Distance"));
            EditorGUILayout.PropertyField(autoConfigureConnectedAnchorProp, new GUIContent("AutoConfigure Connected Anchor"));
            if (jointsTypeProp.enumValueIndex == (int) Cloth2D.JointType.DistanceJoint2D)
            {
	            EditorGUILayout.PropertyField(maxDistanceOnlyProp, new GUIContent("Max Distance Only"));
	            EditorGUILayout.PropertyField(enableCollisionProp, new GUIContent("Enable Collision"));
            }
            else if (jointsTypeProp.enumValueIndex == (int) Cloth2D.JointType.SpringJoint2D)
            {
	            EditorGUILayout.PropertyField(dampingRatioProp, new GUIContent("Damping Ratio"));
	            EditorGUILayout.PropertyField(frequencyProp, new GUIContent("Frequency"));
            }
            jointsLayerProp.intValue = EditorGUILayout.LayerField("Joints Layer", jointsLayerProp.intValue);
            EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.currentViewWidth - WidthOffset));

            //inner colliders
            EditorGUILayout.PropertyField(useInnerCollidersProp, new GUIContent("Use Inner Colliders"));
            if (useInnerCollidersProp.boolValue)
            {
	            EditorGUILayout.PropertyField(innerRadiusRatioProp, new GUIContent("Inner Radius Ratio"));
	            EditorGUILayout.PropertyField(innerMaterialProp, new GUIContent("Inner Material"));
	            innerCollidersLayerProp.intValue = EditorGUILayout.LayerField("Inner Colliders Layer", innerCollidersLayerProp.intValue);
            }
            EditorGUILayout.TextArea(string.Empty, GUI.skin.horizontalSlider, GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(EditorGUIUtility.currentViewWidth - WidthOffset));
            EditorGUILayout.PropertyField(jointsProp, new GUIContent("Joints"));
            if (EditorGUI.EndChangeCheck() || jointsTypeChanged)
            {
	            if (!Application.isPlaying && cloth != null)
	            {
		            EditorUtility.SetDirty(cloth);
		            EditorSceneManager.MarkSceneDirty(cloth.gameObject.scene);
	            }
	            UpdateCloth(jointsTypeChanged);
            }            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
			if (GUILayout.Button("Generate Joints"))
			{
				Generate();
				serializedObject.Update();
			}
			if (GUILayout.Button("Remove Joints"))
			{
				Remove();
				serializedObject.Update();
			}
			EditorGUI.EndDisabledGroup();
			serializedObject.ApplyModifiedProperties();
		}
    }
}
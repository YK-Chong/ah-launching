using System;
using UnityEngine;

namespace Cloth2D
{
    public class Cloth2D : MonoBehaviour, ICloth2D
    {
        public enum JointType
        {
            DistanceJoint2D,
            SpringJoint2D
        }

        #region Parameters

        public bool LockTop = true;
        public bool LockBottom;
        public bool LockLeft;
        public bool LockRight;

        #region Rigidbody2D

        public PhysicsMaterial2D PhysicsMaterial;
        public float LinearDrag;
        public float AngularDrag = 0.05f;
        public float GravityScale = 1f;
        public bool UseAutoMass = true;
        public float MassTop = 1f;
        public float MassBottom = 0.5f;
        
        #endregion

        #region Joints

        public JointType JointsType = JointType.DistanceJoint2D;

        //DistanceJoint2D
        public bool AutoConfigureDistance;
        public bool AutoConfigureConnectedAnchor;
        public bool MaxDistanceOnly = true;
        public bool EnableCollision = true;
        //SpringJoint2D
        public float DampingRatio;
        public float Frequency;

        #endregion

        public LayerMask JointsLayer = 0;
        
        public bool UseInnerColliders;
        public float InnerRadiusRatio = 0.5f;
        public PhysicsMaterial2D InnerPhysicsMaterial;
        public LayerMask InnerCollidersLayer = 0;

        #endregion

        [SerializeField]
        private Cloth2DJoint[] joints;
        private MeshFilter meshFilter;
        private Mesh sharedMesh;
        private Vector3[] jointsStartLocalPositions;
        private Vector3[] startVertices;
        private int jointsHorizontalCount;
        private int jointsVerticalCount;
        
        #region Unity

        void Awake()
        {
            CacheComponents();
            Generate(!HasJoints());
        }

        /// <summary>
        /// Update vertices position using joints data
        /// </summary>
        void FixedUpdate()
        {
            var positions = sharedMesh.vertices;
            for (var i = 0; i < positions.Length; i++)
            {
                positions[i] = startVertices[i];
            }
            
            for (var y = 0; y < jointsVerticalCount; y++)
            {
                for (var x = 0; x < jointsHorizontalCount; x++)
                {
                    var joint = joints[y * jointsHorizontalCount + x];
                    if (joint == null)
                        continue;
                        
                    var localPositionCurrent = joint.Transform.localPosition;
                    var currentJointsStartLocalPosition = jointsStartLocalPositions[y * jointsHorizontalCount + x];
                    var positionCurrent = localPositionCurrent - currentJointsStartLocalPosition;
                    
                    //upper left
                    var a = y * jointsHorizontalCount + x + y + 0;
                    //upper right
                    var b = y * jointsHorizontalCount + x + y + 1;
                    //bottom left
                    var c = y * jointsHorizontalCount + jointsHorizontalCount + x + y + 1;
                    //bottom right
                    var d = y * jointsHorizontalCount + jointsHorizontalCount + x + y + 2;

                    if ((x == 0 || x == jointsHorizontalCount - 1) && y > 0 && y < jointsVerticalCount - 1 || 
                        (y == 0 || y == jointsVerticalCount - 1) && x > 0 && x < jointsHorizontalCount - 1)
                    {
                        //two
                        if (x == 0)
                        {
                            //left
                            var positionUpper = joints[(y - 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y - 1) * jointsHorizontalCount + x];
                            var positionDown = joints[(y + 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y + 1) * jointsHorizontalCount + x];
                            positions[a] += (positionUpper + positionCurrent) / 4f;
                            positions[c] += (positionDown + positionCurrent) / 4f;
                        }
                        else if (y == 0)
                        {
                            //top
                            var positionLeft = joints[y * jointsHorizontalCount + x - 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x - 1];
                            var positionRight = joints[y * jointsHorizontalCount + x + 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x + 1];
                            positions[a] += (positionLeft + positionCurrent) / 4f;
                            positions[b] += (positionRight + positionCurrent) / 4f;
                        }
                        else if (x == jointsHorizontalCount - 1)
                        {
                            //right
                            var positionUpper = joints[(y - 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y - 1) * jointsHorizontalCount + x];
                            var positionDown = joints[(y + 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y + 1) * jointsHorizontalCount + x];
                            positions[b] += (positionUpper + positionCurrent) / 4f;
                            positions[d] += (positionDown + positionCurrent) / 4f;
                        }
                        else
                        {
                            //bottom
                            var positionLeft = joints[y * jointsHorizontalCount + x - 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x - 1];
                            var positionRight = joints[y * jointsHorizontalCount + x + 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x + 1];
                            positions[c] += (positionLeft + positionCurrent) / 4f;
                            positions[d] += (positionRight + positionCurrent) / 4f;
                        }
                    }
                    else if (x == 0 || x == jointsHorizontalCount - 1 || y == 0 || y == jointsVerticalCount - 1)
                    {
                        //one
                        if (x == 0 && y == 0)
                        {
                            //upper left
                            positions[a] += positionCurrent;
                            
                            //do smth with b:
                            var positionRight = joints[y * jointsHorizontalCount + x + 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x + 1];
                            positions[b] += (positionRight + positionCurrent) / 4f;
                            // do smth with c:
                            var positionDown = joints[(y + 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y + 1) * jointsHorizontalCount + x];
                            positions[c] += (positionDown + positionCurrent) / 4f;
                        }
                        else if (x == jointsHorizontalCount - 1 && y == 0)
                        {
                            //upper right
                            positions[b] += positionCurrent;
                            
                            //do smth with a:
                            var positionLeft = joints[y * jointsHorizontalCount + x - 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x - 1];
                            positions[a] += (positionLeft + positionCurrent) / 4f;
                            // do smth with d:
                            var positionDown = joints[(y + 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y + 1) * jointsHorizontalCount + x];
                            positions[d] += (positionDown + positionCurrent) / 4f;
                        }
                        else if (x == 0)
                        {
                            //bottom left
                            positions[c] += positionCurrent;
                            
                            //do smth with a:
                            var positionUpper = joints[(y - 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y - 1) * jointsHorizontalCount + x];
                            positions[a] += (positionUpper + positionCurrent) / 4f;
                            //do smth with d:
                            var positionRight = joints[y * jointsHorizontalCount + x + 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x + 1];
                            positions[d] += (positionRight + positionCurrent) / 4f;
                        }
                        else
                        {
                            //bottom right
                            positions[d] += positionCurrent;
                            
                            //do smth with b:
                            var positionUpper = joints[(y - 1) * jointsHorizontalCount + x].transform.localPosition - jointsStartLocalPositions[(y - 1) * jointsHorizontalCount + x];
                            positions[b] += (positionUpper + positionCurrent) / 4f;
                            //do smth with c:
                            var positionLeft = joints[y * jointsHorizontalCount + x - 1].transform.localPosition - jointsStartLocalPositions[y * jointsHorizontalCount + x - 1];
                            positions[c] += (positionLeft + positionCurrent) / 4f;
                        }
                    }
                    else
                    {
                        //four
                        var upperLeft = joints[(y - 1) * jointsHorizontalCount + x - 1].transform.localPosition;
                        var upperCenter = joints[(y - 1) * jointsHorizontalCount + x].transform.localPosition; 
                        var upperRight = joints[(y - 1) * jointsHorizontalCount + x + 1].transform.localPosition;
                        var middleLeft = joints[y * jointsHorizontalCount + x - 1].transform.localPosition;
                        var middleRight = joints[y * jointsHorizontalCount + x + 1].transform.localPosition;
                        var bottomLeft = joints[(y + 1) * jointsHorizontalCount + x - 1].transform.localPosition;
                        var bottomCenter = joints[(y + 1) * jointsHorizontalCount + x].transform.localPosition;
                        var bottomRight = joints[(y + 1) * jointsHorizontalCount + x + 1].transform.localPosition;
                        
                        var positionUpperLeft = upperLeft - jointsStartLocalPositions[(y - 1) * jointsHorizontalCount + x - 1];
                        var positionUpperCenter = upperCenter - jointsStartLocalPositions[(y - 1) * jointsHorizontalCount + x];
                        var positionUpperRight = upperRight - jointsStartLocalPositions[(y - 1) * jointsHorizontalCount + x + 1];
                        var positionMiddleLeft = middleLeft - jointsStartLocalPositions[y * jointsHorizontalCount + x - 1];
                        var positionMiddleRight = middleRight - jointsStartLocalPositions[y * jointsHorizontalCount + x + 1];
                        var positionBottomLeft = bottomLeft - jointsStartLocalPositions[(y + 1) * jointsHorizontalCount + x - 1];
                        var positionBottomCenter = bottomCenter - jointsStartLocalPositions[(y + 1) * jointsHorizontalCount + x];
                        var positionBottomRight = bottomRight - jointsStartLocalPositions[(y + 1) * jointsHorizontalCount + x + 1];

                        //if 3x3 -> div = 4
                        if (jointsHorizontalCount == 3 || jointsVerticalCount == 3)
                        {
                            positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 4f;
                            positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 4f;
                            positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 4f;
                            positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 4f;
                        }
                        else if (x == 1 || y == 1 || x == jointsHorizontalCount - 2 || y == jointsVerticalCount - 2)
                        {
                            if (x == 1 && y == 1)
                            {
                                //upper left
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 4f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 8f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 8f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 16f;
                            }
                            else if (x == jointsHorizontalCount - 2 && y == 1)
                            {
                                //upper right
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 8f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 4f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 16f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 8f;
                            }
                            else if (x == 1 && y == jointsVerticalCount - 2)
                            {
                                //bottom left
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 8f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 16f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 4f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 8f;
                            }
                            else if (x == jointsHorizontalCount - 2 && y == jointsVerticalCount - 2)
                            {
                                //bottom right
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 16f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 8f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 8f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 4f;
                            }
                            else if (x > 1 && x < jointsHorizontalCount - 2 && y == 1)
                            {
                                //top
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 8f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 8f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 16f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 16f;
                            }
                            else if (x == 1 && y > 1)
                            {
                                //left
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 8f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 16f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 8f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 16f;
                            }
                            else if (x == jointsHorizontalCount - 2 && y > 1)
                            {
                                //right
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 16f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 8f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 16f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 8f;
                            }
                            else 
                            {
                                //bottom
                                positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 16f;
                                positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 16f;
                                positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 8f;
                                positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 8f;
                            }
                        }
                        else if (x > 1 || y > 1 && x < jointsHorizontalCount - 2 || y < jointsVerticalCount - 2)
                        {
                            positions[a] += (positionUpperLeft + positionUpperCenter + positionMiddleLeft + positionCurrent) / 16f;
                            positions[b] += (positionUpperCenter + positionUpperRight + positionMiddleRight + positionCurrent) / 16f;
                            positions[c] += (positionMiddleLeft + positionBottomLeft + positionBottomCenter + positionCurrent) / 16f;
                            positions[d] += (positionMiddleRight + positionBottomCenter + positionBottomRight + positionCurrent) / 16f;
                        }
                    }
                }
            }
            sharedMesh.vertices = positions;
            sharedMesh.RecalculateBounds();
        }

        #endregion
        
        /// <summary>
        /// Set joints count
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetJointsCount(int width, int height)
        {
            jointsHorizontalCount = width;
            jointsVerticalCount = height;
        }

        /// <summary>
        /// Return all joints
        /// </summary>
        /// <returns></returns>
        public Cloth2DJoint[] GetAllJoints()
        {
            return joints;
        }
        
        /// <summary>
        /// Return joints from top border
        /// </summary>
        /// <returns></returns>
        public Cloth2DJoint[] GetTopJoints()
        {
            var topJoints = new Cloth2DJoint[jointsHorizontalCount];
            for (var i = 0; i < topJoints.Length; i++)
            {
                topJoints[i] = joints[i];
            }
            return topJoints;
        }
        
        /// <summary>
        /// Return joints from bottom border
        /// </summary>
        /// <returns></returns>
        public Cloth2DJoint[] GetBottomJoints()
        {
            var bottomJoints = new Cloth2DJoint[jointsHorizontalCount];
            for (var i = 0; i < bottomJoints.Length; i++)
            {
                bottomJoints[i] = joints[(jointsVerticalCount - 1) * jointsHorizontalCount + i];
            }
            return bottomJoints;
        }
        
        /// <summary>
        /// Return joints from left border
        /// </summary>
        /// <returns></returns>
        public Cloth2DJoint[] GetLeftJoints()
        {
            var leftJoints = new Cloth2DJoint[jointsVerticalCount];
            for (var i = 0; i < leftJoints.Length; i++)
            {
                leftJoints[i] = joints[i * jointsHorizontalCount];
            }
            return leftJoints;
        }
        
        /// <summary>
        /// Return joints from right border
        /// </summary>
        /// <returns></returns>
        public Cloth2DJoint[] GetRightJoints()
        {
            var rightJoints = new Cloth2DJoint[jointsVerticalCount];
            for (var i = 0; i < rightJoints.Length; i++)
            {
                rightJoints[i] = joints[i * jointsHorizontalCount + jointsHorizontalCount - 1];
            }
            return rightJoints;
        }

        /// <summary>
        /// Generate all joints
        /// </summary>
        /// <returns></returns>
        public Cloth2DJoint[] GenerateJoints()
        {
            CacheComponents();
            Generate(true);
            return joints;
        }

        /// <summary>
        /// Destroy all joints
        /// </summary>
        public void DestroyJoints()
        {
            if (joints == null)
                return;
            var copy = new Cloth2DJoint[joints.Length];
            Array.Copy(joints, copy, copy.Length);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "ClothRem");
#endif
            for (var i = copy.Length - 1; i >= 0; i--)
            {
                if (copy[i] == null)
                    continue;
                var item = copy[i].gameObject;
#if UNITY_EDITOR
                UnityEditor.Undo.DestroyObjectImmediate(item);
#else
                DestroyImmediate(item);
#endif
            }
        }
        
        /// <summary>
        /// Update joints values without recreating them
        /// </summary>
        public void UpdateValues()
        {
            if (HasJoints())
            {
                CacheComponents();
                Generate(false);
            }
        }

        /// <summary>
        /// Destroy inner colliders
        /// </summary>
        private void DestroyInnerColliders()
        {
            if (joints == null)
                return;
            var copy = new Cloth2DJoint[joints.Length];
            Array.Copy(joints, copy, copy.Length);
#if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "ClothRem");
#endif
            for (var i = copy.Length - 1; i >= 0; i--)
            {
                if (copy[i] == null || copy[i].InnerCollider2D == null)
                    continue;
                var item = copy[i].InnerCollider2D.gameObject;
#if UNITY_EDITOR
                UnityEditor.Undo.DestroyObjectImmediate(item);
#else
                DestroyImmediate(item);
#endif
            }
        }

        /// <summary>
        /// Cache components
        /// </summary>
        private void CacheComponents()
        {
            meshFilter = GetComponent<MeshFilter>();
            sharedMesh = meshFilter.sharedMesh;
            startVertices = sharedMesh.vertices;
        }
        
        /// <summary>
        /// Check if this Cloth2D component has joints
        /// </summary>
        /// <returns></returns>
        private bool HasJoints()
        {
            if (joints == null)
                return false;
            var hasJoints = true;
            foreach (var joint in joints)
            {
                if (joint == null)
                {
                    hasJoints = false;
                    break;
                }
            }
            return joints != null && !(joints.Length > 0 && !hasJoints || joints.Length == 0) && joints.Length == jointsHorizontalCount * jointsVerticalCount;
        }

        /// <summary>
        /// Create GameObject with Cloth2DJoint component and configure it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private Cloth2DJoint CreateJointObject(int id)
        {
            var jointGameObject = new GameObject("Joint_" + (id + 1));
            var joint = jointGameObject.AddComponent<Cloth2DJoint>();
            joint.Transform = jointGameObject.transform;
            joint.Transform.SetParent(transform);
            joint.Transform.localPosition = Vector3.zero;
            joint.Transform.localRotation = Quaternion.identity;
            joint.Transform.localScale = Vector3.one;
            joint.Rigidbody2D = jointGameObject.AddComponent<Rigidbody2D>();
            joint.Collider2D = jointGameObject.AddComponent<CircleCollider2D>();
            return joint;
        }
        
        /// <summary>
        /// Create inner collider GameObject and configure it
        /// </summary>
        /// <param name="jointTransform"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private CircleCollider2D CreateInnerCollider(Transform jointTransform, float radius)
        {
            var inner = new GameObject("Inner") {layer = InnerCollidersLayer};
            inner.transform.SetParent(jointTransform, true);
            inner.transform.localPosition = Vector3.zero;
            inner.transform.localRotation = Quaternion.identity;
            inner.transform.localScale = Vector3.one;
            var innerCollider2D = inner.AddComponent<CircleCollider2D>();
            innerCollider2D.radius = radius * InnerRadiusRatio;
            innerCollider2D.sharedMaterial = InnerPhysicsMaterial;
            return innerCollider2D;
        }
        
        /// <summary>
        /// Create Joint2D component and configure it
        /// </summary>
        /// <param name="cloth2DJoint"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private Joint2D CreateJoint(Cloth2DJoint cloth2DJoint, float distance)
        {
            if (JointsType == JointType.DistanceJoint2D)
            {
                var joint = cloth2DJoint.gameObject.AddComponent<DistanceJoint2D>();
                cloth2DJoint.Joints.Add(joint);
                joint.autoConfigureDistance = AutoConfigureDistance;
                joint.autoConfigureConnectedAnchor = AutoConfigureConnectedAnchor;
                joint.distance = distance;
                joint.maxDistanceOnly = MaxDistanceOnly;
                joint.enableCollision = EnableCollision;
                return joint;
            }
            if (JointsType == JointType.SpringJoint2D)
            {
                var joint = cloth2DJoint.gameObject.AddComponent<SpringJoint2D>();
                cloth2DJoint.Joints.Add(joint);
                joint.autoConfigureDistance = false;
                joint.autoConfigureConnectedAnchor = false;
                joint.distance = distance;
                joint.dampingRatio = DampingRatio;
                joint.frequency = Frequency;
                return joint;
            }
            return null;
        }

        /// <summary>
        /// Generate/Update all joints data
        /// </summary>
        /// <param name="regenerate"></param>
        private void Generate(bool regenerate)
        {
            if (regenerate)
            {
                DestroyJoints();
                joints = new Cloth2DJoint[jointsHorizontalCount * jointsVerticalCount];
            }
            jointsStartLocalPositions = new Vector3[joints.Length];
            var boundsFilterSizeScaled = Vector3.Scale(transform.localScale, meshFilter.sharedMesh.bounds.size);
            var distance = boundsFilterSizeScaled.x > boundsFilterSizeScaled.y ? boundsFilterSizeScaled.x / jointsHorizontalCount : boundsFilterSizeScaled.y / jointsVerticalCount;
            var radius = distance / 2f;
            var halfBounds = new Vector3(boundsFilterSizeScaled.x / 2f - radius, -boundsFilterSizeScaled.y / 2f + radius);
            var offset = new Vector3(boundsFilterSizeScaled.x / jointsHorizontalCount, boundsFilterSizeScaled.y / jointsVerticalCount);
            for (var y = 0; y < jointsVerticalCount; y++)
            {
                var newMass = MassTop - (MassTop - MassBottom) * (y / (jointsVerticalCount - 1f));
                for (var x = 0; x < jointsHorizontalCount; x++)
                {
                    var joint = joints[y * jointsHorizontalCount + x];
                    if (regenerate)
                    {
                        joint = CreateJointObject(y * jointsHorizontalCount + x);
                        joint.Transform.localPosition = new Vector3(offset.x * x, -offset.y * y, 0) - halfBounds;
                        joints[y * jointsHorizontalCount + x] = joint;
                    }
                    joint.gameObject.layer = JointsLayer;
                    
                    //rigidBody2D
                    joint.Collider2D.radius = radius;
                    joint.Rigidbody2D.sharedMaterial = PhysicsMaterial;
                    joint.Rigidbody2D.drag = LinearDrag;
                    joint.Rigidbody2D.angularDrag = AngularDrag;
                    joint.Rigidbody2D.gravityScale = GravityScale;
                    joint.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
                    jointsStartLocalPositions[y * jointsHorizontalCount + x] = joint.Transform.localPosition;
                    
                    //mass
                    joint.Rigidbody2D.useAutoMass = UseAutoMass;
                    if (!UseAutoMass)
                    {
                        joint.Rigidbody2D.mass = newMass;
                    }
  
                    //inner
                    if (UseInnerColliders)
                    {
                        if (joint.InnerCollider2D != null)
                        {
                            joint.InnerCollider2D.radius = radius * InnerRadiusRatio;
                            joint.InnerCollider2D.sharedMaterial = InnerPhysicsMaterial;
                            joint.InnerCollider2D.gameObject.layer = InnerCollidersLayer;
                        }
                        else
                        {
                            joint.InnerCollider2D = CreateInnerCollider(joint.Transform, radius);
                        }
                    }
                    else
                    {
                        DestroyInnerColliders();
                    }
                    
                    if (LockTop)
                    {
                        if (y == 0)
                        {
                            joint.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                    }
                    if (LockBottom)
                    {
                        if (y == jointsVerticalCount - 1)
                        {
                            joint.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                    }
                    if (LockLeft)
                    {
                        if (x == 0)
                        {
                            joint.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                    }
                    if (LockRight)
                    {
                        if (x == jointsHorizontalCount - 1)
                        {
                            joint.Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
                        }
                    }
                              
                    //create joints
                    if (regenerate)
                    {
                        var firstJoint = CreateJoint(joint, distance);
                        Joint2D secondJoint = null;
                        if (y > 0 && x > 0)
                        {
                            secondJoint = CreateJoint(joint, distance);
                        }
                        
                        //connect joints
                        if (x == 0 && y == 0)
                        {
                            firstJoint.enabled = false;
                        }
                        else if (x == 0 && y > 0)
                        {
                            var next = joints[(y - 1) * jointsHorizontalCount + x].Rigidbody2D;
                            firstJoint.connectedBody = next;
                        }
                        else if (x > 0)
                        {
                            var down = joints[y * jointsHorizontalCount + x - 1].Rigidbody2D;
                            firstJoint.connectedBody = down;
                            if (y > 0)
                            {
                                var next = joints[(y - 1) * jointsHorizontalCount + x].Rigidbody2D;
                                secondJoint.connectedBody = next;
                            }
                        }
                    }
                }
            }
        }
    }
}
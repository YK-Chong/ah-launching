using System.Collections.Generic;
using UnityEngine;

namespace Cloth2D
{
    public class Cloth2DJoint : MonoBehaviour
    {
        public List<Joint2D> Joints = new List<Joint2D>();
        public CircleCollider2D Collider2D;
        public CircleCollider2D InnerCollider2D;
        public Rigidbody2D Rigidbody2D;
        public Transform Transform;
    }
}
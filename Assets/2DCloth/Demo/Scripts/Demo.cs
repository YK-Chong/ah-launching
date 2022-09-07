using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cloth2D.Demo
{
    public class Demo : MonoBehaviour
    {
        public List<GameObject> ObjectsToMove;

        public Camera mainCamera;
        private Transform capturedObject;
        private TargetJoint2D targetJoint2D;
        
        void Awake()
        {
            Application.targetFrameRate = 60;
			if(mainCamera == null)
				mainCamera = Camera.main;
        }

	    void Update()
		{
			if (Manager.Instance.CurrentState != Manager.State.Start)
				return;

			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene(0);
				return;
			}

			if (Input.GetMouseButtonDown(0))
			{
				var position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
				var hits = new RaycastHit2D[ObjectsToMove.Count]; 
				Physics2D.RaycastNonAlloc(position, Vector2.zero, hits);
				foreach (var hit in hits)
				{
					if (hit.transform != null &&
					    (hit.transform.parent != null && ObjectsToMove.Contains(hit.transform.parent.gameObject) ||
					     ObjectsToMove.Contains(hit.transform.gameObject)))
					{
						capturedObject = hit.transform;
						targetJoint2D = capturedObject.gameObject.AddComponent<TargetJoint2D>();
						break;
					}
				}
			}

			if (Input.GetMouseButton(0))
			{
				if (capturedObject != null)
				{
					var position = mainCamera.ScreenToWorldPoint(Input.mousePosition);
					targetJoint2D.target = position;
				}
			}

			if (Input.GetMouseButtonUp(0))
			{
				if (capturedObject != null)
				{
					var body = capturedObject.GetComponent<Rigidbody2D>();
					if (body != null)
					{
						body.velocity = Vector3.zero;
						body.angularVelocity = 0f;
					}
					capturedObject = null;
				}

				if (targetJoint2D != null)
				{
					Destroy(targetJoint2D);
				}
			}
		}
    }
}
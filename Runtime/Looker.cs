using System;
using UnityEngine;

namespace YanickSenn.Controller.FirstPerson
{
    [DisallowMultipleComponent]
    public class Looker : MonoBehaviour {
        [SerializeField]
        private Transform cameraAnchor;
        
        [SerializeField]
        private float lookSensitivity;

        public Vector3 LookOrigin => GetLookOrigin();
        public Vector3 LookDirection => GetLookDirection();

        private Vector2 _lookRotation;

        private void Awake() {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UpdateLookInput(Vector2 lookInput) {
            if (lookInput.sqrMagnitude < Mathf.Epsilon) return;
            var mouseX = lookInput.x * lookSensitivity * Time.deltaTime;
            var mouseY = lookInput.y * lookSensitivity * Time.deltaTime;
            _lookRotation.x =  Mathf.Clamp(_lookRotation.x - mouseY, -60, 60);
            _lookRotation.y += mouseX;
    
            cameraAnchor.transform.localRotation = Quaternion.Euler(
                _lookRotation.x,
                _lookRotation.y,
                0f
            );
        }

        private Vector3 GetLookOrigin() {
            return cameraAnchor.transform.position;
        }

        private Vector3 GetLookDirection() {
            return cameraAnchor.transform.forward;
        }
        
        [Serializable]
        public struct AngleRange {
            public bool constrained;
            public float center;
            public float minOffset;
            public float maxOffset;
        }
    }
}
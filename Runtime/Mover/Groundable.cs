using UnityEngine;

namespace YanickSenn.Controller.FirstPerson.Mover {
    [DisallowMultipleComponent, RequireComponent(typeof(Rigidbody))]
    public class Groundable : MonoBehaviour {
        private Collider _collider;
        private Rigidbody _rigidbody;
        
        private bool _isGrounded;
        private Rigidbody _currentPlatform;

        public bool IsGrounded => _isGrounded;

        private void Awake() {
            _collider = GetComponent<Collider>();
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate() {
            UpdateGroundedState();
        }

        private void UpdateGroundedState() {
            var groundedRaycast = IsGroundedRaycast();
            if (groundedRaycast) {
                _isGrounded = true;
                return;
            }
            var groundedVelocity = IsGroundedVelocity();
            _isGrounded = groundedVelocity;
        }

        private bool IsGroundedRaycast() {
            float height = _collider.bounds.size.y;
            Vector3 rayStart = transform.position;
            return Physics.Raycast(rayStart, Vector3.down, height / 2 + 0.05f);
        }

        private bool IsGroundedVelocity() {
            return Mathf.Abs(_rigidbody.linearVelocity.y) < 0.01f;
        }
    }
}
using UnityEngine;
using UnityEngine.Serialization;

namespace YanickSenn.Controller.FirstPerson.Mover
{
    [DisallowMultipleComponent,
     RequireComponent(typeof(Rigidbody)),
     RequireComponent(typeof(Groundable)),
     RequireComponent(typeof(Looker))]
    public class RigidbodyMover : AbstractMover {
    
        [FormerlySerializedAs("walkingSpeed")] [SerializeField]
        private float walkingForce = 3500f;
        [FormerlySerializedAs("runningSpeed")] [SerializeField]
        private float runningForce = 5000f;
        [SerializeField]
        private float jumpForce = 15000;
        
        private Rigidbody _rigidbody;
        private Groundable _groundable;
        private Looker _looker;
        private bool _isRunning;

        public override bool IsGrounded => _groundable.IsGrounded;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _groundable = GetComponent<Groundable>();
            _looker = GetComponent<Looker>();

            _rigidbody.linearDamping = 10f;
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _rigidbody.interpolation = RigidbodyInterpolation.Extrapolate;
            _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        private void FixedUpdate() {
            if (IsGrounded) {
                _rigidbody.linearDamping = 10f;
                ApplyMovementInDirection();
            } else {
                _rigidbody.linearDamping = 0f;
            }
        }

        public override void Jump() {
            if (!IsGrounded) return;
            _rigidbody.AddRelativeForce(Vector3.up * jumpForce);
        }

        private void ApplyMovementInDirection() {
            var lookDirection = _looker.LookDirection;
            var forward = new Vector3(lookDirection.x, 0f, lookDirection.z).normalized;
            var right = new Vector3(forward.z, 0, -forward.x);
            var moveDirection = (forward * MoveInput.y + right * MoveInput.x).normalized;
            var translatedMoveDirection = transform.InverseTransformDirection(moveDirection);
            var moveForce = IsRunning ? runningForce : walkingForce;
            _rigidbody.AddRelativeForce(moveForce * translatedMoveDirection);
        }
    }
}
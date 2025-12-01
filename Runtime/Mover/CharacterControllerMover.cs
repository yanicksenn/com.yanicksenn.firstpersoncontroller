using System;
using UnityEngine;

namespace YanickSenn.Controller.FirstPerson.Mover
{
    [DisallowMultipleComponent, 
        RequireComponent(typeof(CharacterController)),
        RequireComponent(typeof(Looker))]
    public class CharacterControllerMover : AbstractMover {
        private CharacterController _characterController;
        private Looker _looker;

        private Vector3 _horizontalVelocity;
        private Vector3 _velocity;

        public override bool IsGrounded => _characterController.isGrounded;

        private void Awake() {
            _characterController = GetComponent<CharacterController>();
            _looker = GetComponent<Looker>();
        }

        private void FixedUpdate() {
            switch (MoverConfig) {
                case WalkingConfig walkingStrategy:
                    ApplyWalkingStrategy(walkingStrategy);
                    break;
                case SwimmingConfig swimmingStrategy:
                    ApplySwimmingStrategy(swimmingStrategy);
                    break;
            }
        }

        public override void Jump() {
            switch (MoverConfig) {
                case WalkingConfig walkingStrategy:
                    ApplyJump(walkingStrategy);
                    break;
            }
        }

        private void ApplyWalkingStrategy(WalkingConfig walkingConfig) {
            var isGrounded = _characterController.isGrounded;
            if (isGrounded && _velocity.y < 0) {
                _velocity.y = -2f;
            }

            if (isGrounded) {
                var currentSpeed = IsRunning ? walkingConfig.runningSpeed : walkingConfig.walkingSpeed;
                var lookDirection = _looker.LookDirection;
                var forward = new Vector3(lookDirection.x, 0f, lookDirection.z).normalized;
                var right = new Vector3(forward.z, 0, -forward.x);
                var moveDirection = (forward * MoveInput.y + right * MoveInput.x).normalized;
                _horizontalVelocity = moveDirection * currentSpeed;
            }
    
            _velocity.y += Physics.gravity.y * Time.fixedDeltaTime;

            var finalVelocity = _horizontalVelocity + new Vector3(0, _velocity.y, 0);
            _characterController.Move(finalVelocity * Time.fixedDeltaTime);
        }

        private void ApplySwimmingStrategy(SwimmingConfig swimmingConfig) {
            var lookDirection = _looker.LookDirection;
            var moveDirection = MoveInput.sqrMagnitude > Mathf.Epsilon ? lookDirection.normalized : Vector3.zero;
            _velocity = moveDirection * swimmingConfig.swimmingSpeed;
            _characterController.Move(_velocity * Time.fixedDeltaTime);
        }

        private void ApplyJump(WalkingConfig walkingConfig) {
            if (!IsGrounded) return;
            
            // Calculate the required upward velocity to reach the desired jumpHeight
            // This is derived from the physics formula: v = sqrt(h * -2 * g)
            _velocity.y = Mathf.Sqrt(walkingConfig.jumpHeight * -2f * Physics.gravity.y);
        }
    }
}
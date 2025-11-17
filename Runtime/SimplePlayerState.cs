using UnityEngine;
using UnityEngine.InputSystem;

namespace YanickSenn.Controller.FirstPerson {
    public class SimplePlayerState : IPlayerState, InputSystemActions.IPlayerActions {
        private readonly InputSystemActions _actions;
        private readonly AbstractPlayerController _playerController;

        public SimplePlayerState(AbstractPlayerController playerController) {
            _actions = new InputSystemActions();
            _playerController = playerController;
        }

        public void Enable() {
            _actions.Player.SetCallbacks(this);
            _actions.Player.Enable();
        }

        public void Disable() {
            _actions.Player.Disable();
        }

        public void OnMove(InputAction.CallbackContext context) {
            _playerController.Mover.DoIfPresent(mover => mover.MoveInput = context.ReadValue<Vector2>());
        }

        public void OnLook(InputAction.CallbackContext context) {
            _playerController.Looker.DoIfPresent(looker => looker.UpdateLookInput(context.ReadValue<Vector2>()));
        }

        public void OnSprint(InputAction.CallbackContext context) {
            _playerController.Mover.DoIfPresent(mover => mover.IsRunning = context.ReadValueAsButton());
        }

        public void OnJump(InputAction.CallbackContext context) {
            _playerController.Mover
                .Filter(_ => context.phase == InputActionPhase.Started)
                .DoIfPresent(mover => mover.Jump());
        }

        public void OnGrab(InputAction.CallbackContext context) {
            _playerController.Hand
                .Filter(_ => context.phase == InputActionPhase.Started)
                .DoIfPresent(hand => hand.Grab());
        }

        public void OnRelease(InputAction.CallbackContext context) {
            _playerController.Hand
                .Filter(_ => context.phase == InputActionPhase.Started)
                .DoIfPresent(hand => hand.Release());
        }

        public void OnThrow(InputAction.CallbackContext context) {
            _playerController.Hand
                .Filter(_ => context.phase == InputActionPhase.Started)
                .DoIfPresent(hand => hand.Throw());
        }
    }
}
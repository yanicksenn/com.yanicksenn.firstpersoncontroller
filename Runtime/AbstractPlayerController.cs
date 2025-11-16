using System.Collections.Generic;
using UnityEngine;
using YanickSenn.Controller.FirstPerson.Hand;
using YanickSenn.Utils;

namespace YanickSenn.Controller.FirstPerson
{
    [DisallowMultipleComponent]
    public abstract class AbstractPlayerController : MonoBehaviour {
        private readonly Stack<IPlayerState> _stateStack = new();

        [SerializeField] private Optional<Looker> looker;
        [SerializeField] private Optional<AbstractHand> hand;
        [SerializeField] private Optional<AbstractMover> mover;
        
        public Optional<Looker> Looker => looker;
        public Optional<AbstractHand> Hand => hand;
        public Optional<AbstractMover> Mover => mover;
        public IPlayerState CurrentPlayerState => _currentPlayerState;

        private IPlayerState _defaultPlayerState;
        private IPlayerState _currentPlayerState;

        private void Awake() {
            _defaultPlayerState = GetDefaultPlayerState();
            _currentPlayerState = _defaultPlayerState;
        }

        protected abstract IPlayerState GetDefaultPlayerState();

        private void OnEnable() {
            _currentPlayerState.Enable();
        }

        private void OnDisable() {
            _currentPlayerState.Disable();
        }

        private void FixedUpdate() {
            _currentPlayerState.FixedUpdate();
        }

        private void OnDrawGizmos() {
            _currentPlayerState?.OnDrawGizmos();
        }

        private void OnDrawGizmosSelected() {
            _currentPlayerState?.OnDrawGizmosSelected();
        }

        public void PushState(IPlayerState newState) {
            if (newState == null) return;
            _stateStack.Push(newState);
            _currentPlayerState.Disable();
            _currentPlayerState = newState;
            newState.Enable();
        }

        public void PopState() {
            if (_stateStack.Count == 0) return;
            var previousState = _stateStack.Pop();
            previousState.Disable();
            _currentPlayerState = _stateStack.Count == 0 ? _defaultPlayerState : _stateStack.Peek();
            _currentPlayerState.Enable();
        }
    }
}

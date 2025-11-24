using UnityEngine;

namespace YanickSenn.Controller.FirstPerson.Hand {
    [DisallowMultipleComponent,
     RequireComponent(typeof(Rigidbody)),
     RequireComponent(typeof(Collider))]
    public class Grabbable : MonoBehaviour
    {
        [SerializeField] private bool useCustomHoldingConfig;
        [SerializeField] private Vector3 holdingPosition;
        [SerializeField] private Quaternion holdingRotation = Quaternion.identity;
        
        private IState _currentState = new Idle();
        private Rigidbody _rigidbody;
        private Collider _collider;

        public Rigidbody Rigidbody => _rigidbody;
        public Collider Collider => _collider;

        private void Awake() {
            _rigidbody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void Grab(AbstractHand hand) {
            if (_currentState is Grabbed) {
                return;
            }

            _currentState = new Grabbed();
        }
    
        public void ReleaseWithForce(Vector3 force) {
            if (_currentState is not Grabbed) {
                return;
            }
        
            _currentState = new Idle();
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.AddForce(force);
        }

        private interface IState {}
        private class Idle : IState {}

        private class Grabbed : IState { }
    }
}
using YanickSenn.Utils;
using UnityEngine;
using YanickSenn.Utils.Control;
using YanickSenn.Utils.Snapshots;

namespace YanickSenn.Controller.FirstPerson.Hand
{
    public abstract class AbstractHand : MonoBehaviour
    {
        [SerializeField] private AbstractPlayerController playerController;
        [SerializeField] private float maxGrabbingDistance = 3;
        [SerializeField] private float throwForce = 10;

        private Optional<Looker> _looker;

        public AbstractPlayerController PlayerController => playerController;
        public float MaxGrabbingDistance => maxGrabbingDistance;
        public float ThrowForce => throwForce;

        private IHandState _currentHandState = new Idle();
        public IHandState CurrentHandState
        {
            get => _currentHandState;
            set {
                var oldState = _currentHandState;
                _currentHandState = value;
                
                // TODO: Make state handling more generic.
                switch (_currentHandState) {
                    case Idle:
                        if (oldState is Holding h1) {
                            OnRelease(h1);
                        }
                        break;
                    case Holding currentHolding:
                        if (oldState is Holding h2) {
                            if (h2.Grabbable != currentHolding.Grabbable) {
                                OnRelease(h2);
                                OnGrab(currentHolding);
                            }
                        } else {
                            OnGrab(currentHolding);
                        }
                        break;
                }
            }
        }

        private void Awake() {
            _looker = playerController.Looker;
        }

        private void OnDrawGizmos() {
            if (_looker.IsAbsent) return;
            var looker = _looker.Value;
            Gizmos.color = Color.red;
            var origin = looker.LookOrigin;
            var direction = looker.LookDirection;
            Gizmos.DrawLine(origin, origin + direction * MaxGrabbingDistance);
        }


        public bool Interact() {
            if (_looker.IsAbsent) return false;
            var looker = _looker.Value;
    
            if (CurrentHandState is Holding) {
                return false;
            }

            var origin = looker.LookOrigin;
            var direction = looker.LookDirection;
    
            return Physics.Raycast(origin, direction, out var hit, maxGrabbingDistance)
                && Interact(hit.collider.gameObject);
        }

        public bool Use() {
            if (CurrentHandState is not Holding holding) {
                return false;
            }

            if (!holding.Grabbable.TryGetComponent<Usable>(out var usable)) {
                return false;
            }
            
            usable.Use(this);
            return true;
        }

        public bool Release(float forceMultiplier = 0.0f) {
            if (_looker.IsAbsent) return false;
            var looker = _looker.Value;
            return Throw(throwForce * Mathf.Clamp01(forceMultiplier) * looker.LookDirection);
        }
        
        public abstract bool Interact(GameObject gameObject);
        public abstract bool Throw(Vector3 force);

        protected virtual void OnGrab(Holding holding) { }
        protected virtual void OnRelease(Holding holding) { }
        

        public interface IHandState {}
    
        public class Idle : IHandState {}

        public class Holding : IHandState {
            public Grabbable Grabbable { get; }
            public RigidbodySnapshot RigidbodySnapshot { get; }
            public ColliderSnapshot ColliderSnapshot { get; }
            public Holding(Grabbable grabbable) {
                Grabbable = grabbable;
                RigidbodySnapshot = RigidbodySnapshot.From(Grabbable.Rigidbody);
                ColliderSnapshot = ColliderSnapshot.From(Grabbable.Collider);
            }
        }
    }
}
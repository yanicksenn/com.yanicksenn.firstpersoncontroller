using UnityEngine;

namespace YanickSenn.Controller.FirstPerson.Hand
{
    [DisallowMultipleComponent]
    public class PhysicsHand : AbstractHand {
        private void FixedUpdate() {
            if (CurrentHandState is Holding) {
                Physics.SyncTransforms();
            }
        }

        public override bool Interact(GameObject gameObject) {
            if (PlayerController.Looker.IsAbsent) return false;
            var looker = PlayerController.Looker.Value;

            var canGrab = gameObject.TryGetComponent(out Grabbable grabbable)
                && grabbable.enabled
                && CurrentHandState is not Holding;

            if (canGrab) {
                CurrentHandState = new Holding(grabbable);
                grabbable.Grab(this);
                return true;
            }

            var canInteract = gameObject.TryGetComponent(out Interactable interactable) 
                && interactable.enabled;

            if (canInteract) {
                interactable.Interact(this);
                return true;
            }

            return false;
        }

        public override bool Throw(Vector3 force) {
            if (CurrentHandState is not Holding holding) return false;
            CurrentHandState = new Idle();
            holding.Grabbable.ReleaseWithForce(force);
            return true;
        }

        protected override void OnGrab(Holding holding) {
            var grabbable = holding.Grabbable;

            if (grabbable.UseCustomHoldingConfig) {
                grabbable.transform.SetParent(transform, false);
                grabbable.transform.localPosition = grabbable.HoldingPosition;
                grabbable.transform.localRotation = Quaternion.Euler(grabbable.HoldingRotation.eulerAngles * -1);
            } else {
                grabbable.transform.SetParent(transform);
                grabbable.transform.localPosition = Vector3.zero;
            }
            
            grabbable.Rigidbody.linearVelocity = Vector3.zero;
            grabbable.Rigidbody.angularVelocity = Vector3.zero;
            grabbable.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            grabbable.Rigidbody.isKinematic = true;
            grabbable.Collider.excludeLayers |= 1 << LayerMask.NameToLayer("Player");
            grabbable.Collider.enabled = false;
        }

        protected override void OnRelease(Holding holding) {
            var grabbable = holding.Grabbable;
            grabbable.transform.parent = null;
            holding.RigidbodySnapshot.ApplyTo(grabbable.Rigidbody);
            holding.ColliderSnapshot.ApplyTo(grabbable.Collider);
        }
    }
}
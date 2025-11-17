namespace YanickSenn.Controller.FirstPerson {
    public interface IPlayerState {
        void Enable() { } 
        void Disable() { }
        void Update() { }
        void FixedUpdate() { }

        void OnDrawGizmos() { }

        void OnDrawGizmosSelected() {
            OnDrawGizmos();
        }

        bool PermitSubState(IPlayerState playerState) {
            return true;
        }
    }
}
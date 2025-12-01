using System;
using UnityEngine;

namespace YanickSenn.Controller.FirstPerson {
    public abstract class AbstractMover : MonoBehaviour {
        public Vector2 MoveInput { get; set; }
        public bool IsRunning { get; set; }
        
        public IMoverConfig MoverConfig { get; set; }
        
        public abstract bool IsGrounded { get; }

        public abstract void Jump();
    }
    
    public interface IMoverConfig { }
    
    [Serializable]
    public class WalkingConfig : IMoverConfig {
        public float walkingSpeed = 5f;
        public float runningSpeed = 8f;
        public float jumpHeight = 1.5f;
    }
    
    [Serializable]
    public class SwimmingConfig : IMoverConfig {
        public float swimmingSpeed = 5f;
    }
}
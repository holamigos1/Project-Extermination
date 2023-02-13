using UnityEngine;

namespace Fragsurf.Movement
{
    public enum MoveType
    {
        None,
        Walk,
        Noclip, // not implemented
        Ladder // not implemented
    }

    public class MoveData
    {
        public bool angledLaddersEnabled = false;
        public bool cameraUnderwater = false;

        public bool climbingLadder = false;
        public bool crouching = false;
        public float crouchingHeight = 1f;
        public float crouchingSpeed = 10f;

        public float defaultHeight = 2f;
        public float fallingVelocity = 0f;
        public float forwardMove;
        public float gravityFactor = 1f;

        public bool grounded = false;
        public bool groundedTemp = false;
        public float horizontalAxis = 0f;
        public Vector3 ladderClimbDir = Vector3.up;
        public Vector3 ladderDirection = Vector3.forward;
        public Vector3 ladderNormal = Vector3.zero;
        public bool laddersEnabled = false;
        public Vector3 ladderVelocity = Vector3.zero;

        public Vector3 origin;
        ///// Fields /////

        public Transform playerTransform;

        public float rigidbodyPushForce = 1f;
        public float sideMove;

        public bool slidingEnabled = false;

        public float slopeLimit = 45f;
        public bool sprinting = false;
        public float stepOffset = 0f;
        public float surfaceFriction = 1f;
        public bool toggleCrouch = false;

        public bool underwater = false;
        public float upMove;

        public bool useStepOffset = false;
        public Vector3 velocity;
        public float verticalAxis = 0f;
        public Vector3 viewAngles;
        public Transform viewTransform;
        public Vector3 viewTransformDefaultLocalPos;
        public float walkFactor = 1f;
        public bool wishJump = false;
    }
}
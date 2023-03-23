using System;
using Movement.SourseMovment.TraceUtil;
using UnityEngine;

namespace Movement.SourseMovment.Movement
{
    [Serializable]
    public class SurfController
    {
        private MovementConfig _config;
        private float _deltaTime;
        private ISurfControllable _surfer;

        public Transform camera;
        public float cameraYPos = 0f;
        public bool crouching;
        private float crouchLerp;

        private readonly float frictionMult = 1f;

        ///// Methods /////

        private Vector3 groundNormal = Vector3.up;

        public bool jumping;
        ///// Fields /////

        [HideInInspector] public Transform playerTransform;
        private float slideDelay;
        private Vector3 slideDirection = Vector3.forward;

        private float slideSpeedCurrent;

        public float speed;

        private bool uncrouchDown;
        private bool wasSliding;

        /// <summary>
        /// </summary>
        public void ProcessMovement(ISurfControllable surfer, MovementConfig config, float deltaTime)
        {
            // cache instead of passing around parameters
            _surfer = surfer;
            _config = config;
            _deltaTime = deltaTime;

            if (_surfer.MoveData.laddersEnabled && !_surfer.MoveData.climbingLadder)
                // Look for ladders
                LadderCheck(new Vector3(1f, 0.95f, 1f),
                    _surfer.MoveData.velocity * Mathf.Clamp(Time.deltaTime * 2f, 0.025f, 0.25f));

            if (_surfer.MoveData.laddersEnabled && _surfer.MoveData.climbingLadder)
            {
                LadderPhysics();
            }
            else if (!_surfer.MoveData.underwater)
            {
                if (_surfer.MoveData.velocity.y <= 0f)
                    jumping = false;

                // apply gravity
                if (_surfer.GroundObject == null)
                {
                    _surfer.MoveData.velocity.y -= _surfer.MoveData.gravityFactor * _config.Gravity * _deltaTime;
                    _surfer.MoveData.velocity.y += _surfer.BaseVelocity.y * _deltaTime;
                }

                // input velocity, check for ground
                CheckGrounded();
                CalculateMovementVelocity();
            }
            else
            {
                // Do underwater logic
                UnderwaterPhysics();
            }

            var yVel = _surfer.MoveData.velocity.y;
            _surfer.MoveData.velocity.y = 0f;
            _surfer.MoveData.velocity = Vector3.ClampMagnitude(_surfer.MoveData.velocity, _config.MaxVelocity);
            speed = _surfer.MoveData.velocity.magnitude;
            _surfer.MoveData.velocity.y = yVel;

            if (_surfer.MoveData.velocity.sqrMagnitude == 0f)
            {
                // Do collisions while standing still
                SurfPhysics.ResolveCollisions(_surfer.SurfCollider, ref _surfer.MoveData.origin,
                    ref _surfer.MoveData.velocity, _surfer.MoveData.rigidbodyPushForce, 1f, _surfer.MoveData.stepOffset,
                    _surfer);
            }
            else
            {
                var maxDistPerFrame = 0.2f;
                var velocityThisFrame = _surfer.MoveData.velocity * _deltaTime;
                var velocityDistLeft = velocityThisFrame.magnitude;
                var initialVel = velocityDistLeft;
                while (velocityDistLeft > 0f)
                {
                    var amountThisLoop = Mathf.Min(maxDistPerFrame, velocityDistLeft);
                    velocityDistLeft -= amountThisLoop;

                    // increment origin
                    var velThisLoop = velocityThisFrame * (amountThisLoop / initialVel);
                    _surfer.MoveData.origin += velThisLoop;

                    // don't penetrate walls
                    SurfPhysics.ResolveCollisions(_surfer.SurfCollider, ref _surfer.MoveData.origin,
                        ref _surfer.MoveData.velocity, _surfer.MoveData.rigidbodyPushForce, amountThisLoop / initialVel,
                        _surfer.MoveData.stepOffset, _surfer);
                }
            }

            _surfer.MoveData.groundedTemp = _surfer.MoveData.grounded;

            _surfer = null;
        }

        /// <summary>
        /// </summary>
        private void CalculateMovementVelocity()
        {
            switch (_surfer.MoveType)
            {
                case MoveType.Walk:

                    if (_surfer.GroundObject == null)
                    {
                        /*
                        // AIR MOVEMENT
                        */

                        wasSliding = false;

                        // apply movement from input
                        _surfer.MoveData.velocity += AirInputMovement();

                        // let the magic happen
                        SurfPhysics.Reflect(ref _surfer.MoveData.velocity, _surfer.SurfCollider, _surfer.MoveData.origin,
                            _deltaTime);
                    }
                    else
                    {
                        /*
                        //  GROUND MOVEMENT
                        */

                        // Sliding
                        if (!wasSliding)
                        {
                            slideDirection = new Vector3(_surfer.MoveData.velocity.x, 0f, _surfer.MoveData.velocity.z)
                                .normalized;
                            slideSpeedCurrent = Mathf.Max(_config.maximumSlideSpeed,
                                new Vector3(_surfer.MoveData.velocity.x, 0f, _surfer.MoveData.velocity.z).magnitude);
                        }
                        
                        if (_surfer.MoveData.velocity.magnitude > _config.minimumSlideSpeed &&
                            _surfer.MoveData.slidingEnabled && _surfer.MoveData.crouching && slideDelay <= 0f)
                        {
                            if (!wasSliding)
                                slideSpeedCurrent = Mathf.Clamp(slideSpeedCurrent * _config.slideSpeedMultiplier,
                                    _config.minimumSlideSpeed, _config.maximumSlideSpeed);
                            
                            wasSliding = true;
                            SlideMovement();
                            return;
                        }

                        if (slideDelay > 0f)
                            slideDelay -= _deltaTime;

                        if (wasSliding)
                            slideDelay = _config.slideDelay;

                        wasSliding = false;

                        var fric = crouching ? _config.crouchFriction : _config.Friction;
                        var accel = crouching ? _config.crouchAcceleration : _config.acceleration;
                        var decel = crouching ? _config.crouchDeceleration : _config.deceleration;

                        // Get movement directions
                        var forward = Vector3.Cross(groundNormal, -playerTransform.right);
                        var right = Vector3.Cross(groundNormal, forward);

                        var speed = _surfer.MoveData.sprinting ? _config.sprintSpeed : _config.walkSpeed;
                        if (crouching)
                            speed = _config.crouchSpeed;

                        Vector3 _wishDir;

                        // Jump and friction
                        if (_surfer.MoveData.wishJump)
                        {
                            ApplyFriction(0.0f, true, true);
                            Jump();
                            return;
                        }

                        ApplyFriction(1.0f * frictionMult, true, true);

                        var forwardMove = _surfer.MoveData.verticalAxis;
                        var rightMove = _surfer.MoveData.horizontalAxis;

                        _wishDir = forwardMove * forward + rightMove * right;
                        _wishDir.Normalize();
                        var moveDirNorm = _wishDir;

                        var forwardVelocity = Vector3.Cross(groundNormal,
                            Quaternion.AngleAxis(-90, Vector3.up) * new Vector3(_surfer.MoveData.velocity.x, 0f,
                                _surfer.MoveData.velocity.z));

                        // Set the target speed of the player
                        var _wishSpeed = _wishDir.magnitude;
                        _wishSpeed *= speed;

                        // Accelerate
                        var yVel = _surfer.MoveData.velocity.y;
                        Accelerate(_wishDir, _wishSpeed, accel * Mathf.Min(frictionMult, 1f), false);

                        var maxVelocityMagnitude = _config.MaxVelocity;
                        _surfer.MoveData.velocity = Vector3.ClampMagnitude(
                            new Vector3(_surfer.MoveData.velocity.x, 0f, _surfer.MoveData.velocity.z),
                            maxVelocityMagnitude);
                        _surfer.MoveData.velocity.y = yVel;

                        // Calculate how much slopes should affect movement
                        var yVelocityNew = forwardVelocity.normalized.y *
                                           new Vector3(_surfer.MoveData.velocity.x, 0f, _surfer.MoveData.velocity.z)
                                               .magnitude;

                        // Apply the Y-movement from slopes
                        _surfer.MoveData.velocity.y = yVelocityNew * (_wishDir.y < 0f ? 1.2f : 1.0f);
                        var removableYVelocity = _surfer.MoveData.velocity.y - yVelocityNew;
                    }

                    break;
            } // END OF SWITCH STATEMENT
        }

        private void UnderwaterPhysics()
        {
            _surfer.MoveData.velocity = Vector3.Lerp(_surfer.MoveData.velocity, Vector3.zero,
                _config.underwaterVelocityDampening * _deltaTime);

            // Gravity
            if (!CheckGrounded())
                _surfer.MoveData.velocity.y -= _config.underwaterGravity * _deltaTime;

            // Swimming upwards
            if (Input.GetButton("Jump"))
                _surfer.MoveData.velocity.y += _config.swimUpSpeed * _deltaTime;

            var fric = _config.underwaterFriction;
            var accel = _config.underwaterAcceleration;
            var decel = _config.underwaterDeceleration;

            ApplyFriction(1f, true, false);

            // Get movement directions
            var forward = Vector3.Cross(groundNormal, -playerTransform.right);
            var right = Vector3.Cross(groundNormal, forward);

            var speed = _config.underwaterSwimSpeed;

            Vector3 _wishDir;

            var forwardMove = _surfer.MoveData.verticalAxis;
            var rightMove = _surfer.MoveData.horizontalAxis;

            _wishDir = forwardMove * forward + rightMove * right;
            _wishDir.Normalize();
            var moveDirNorm = _wishDir;

            var forwardVelocity = Vector3.Cross(groundNormal,
                Quaternion.AngleAxis(-90, Vector3.up) *
                new Vector3(_surfer.MoveData.velocity.x, 0f, _surfer.MoveData.velocity.z));

            // Set the target speed of the player
            var _wishSpeed = _wishDir.magnitude;
            _wishSpeed *= speed;

            // Accelerate
            var yVel = _surfer.MoveData.velocity.y;
            Accelerate(_wishDir, _wishSpeed, accel, false);

            var maxVelocityMagnitude = _config.MaxVelocity;
            _surfer.MoveData.velocity = Vector3.ClampMagnitude(
                new Vector3(_surfer.MoveData.velocity.x, 0f, _surfer.MoveData.velocity.z), maxVelocityMagnitude);
            _surfer.MoveData.velocity.y = yVel;

            var yVelStored = _surfer.MoveData.velocity.y;
            _surfer.MoveData.velocity.y = 0f;

            // Calculate how much slopes should affect movement
            var yVelocityNew = forwardVelocity.normalized.y *
                               new Vector3(_surfer.MoveData.velocity.x, 0f, _surfer.MoveData.velocity.z).magnitude;

            // Apply the Y-movement from slopes
            _surfer.MoveData.velocity.y = Mathf.Min(Mathf.Max(0f, yVelocityNew) + yVelStored, speed);

            // Jumping out of water
            var movingForwards = playerTransform.InverseTransformVector(_surfer.MoveData.velocity).z > 0f;
            var waterJumpTrace = TraceBounds(playerTransform.position,
                playerTransform.position + playerTransform.forward * 0.1f, SurfPhysics.groundLayerMask);
            if (waterJumpTrace.hitCollider != null &&
                Vector3.Angle(Vector3.up, waterJumpTrace.planeNormal) >= _config.SlopeLimit &&
                Input.GetButton("Jump") && !_surfer.MoveData.cameraUnderwater && movingForwards)
                _surfer.MoveData.velocity.y = Mathf.Max(_surfer.MoveData.velocity.y, _config.JumpForce);
        }

        private void LadderCheck(Vector3 colliderScale, Vector3 direction)
        {
            if (_surfer.MoveData.velocity.sqrMagnitude <= 0f)
                return;

            var foundLadder = false;

            var hits = Physics.BoxCastAll(_surfer.MoveData.origin,
                Vector3.Scale(_surfer.SurfCollider.bounds.size * 0.5f, colliderScale),
                Vector3.Scale(direction, new Vector3(1f, 0f, 1f)), Quaternion.identity, direction.magnitude,
                SurfPhysics.groundLayerMask, QueryTriggerInteraction.Collide);
            foreach (var hit in hits)
            {
                var ladder = hit.transform.GetComponentInParent<Ladder>();
                if (ladder != null)
                {
                    var allowClimb = true;
                    var ladderAngle = Vector3.Angle(Vector3.up, hit.normal);
                    if (_surfer.MoveData.angledLaddersEnabled)
                    {
                        if (hit.normal.y < 0f)
                        {
                            allowClimb = false;
                        }
                        else
                        {
                            if (ladderAngle <= _surfer.MoveData.slopeLimit)
                                allowClimb = false;
                        }
                    }
                    else if (hit.normal.y != 0f)
                    {
                        allowClimb = false;
                    }

                    if (allowClimb)
                    {
                        foundLadder = true;
                        if (_surfer.MoveData.climbingLadder == false)
                        {
                            _surfer.MoveData.climbingLadder = true;
                            _surfer.MoveData.ladderNormal = hit.normal;
                            _surfer.MoveData.ladderDirection = -hit.normal * direction.magnitude * 2f;

                            if (_surfer.MoveData.angledLaddersEnabled)
                            {
                                var sideDir = hit.normal;
                                sideDir.y = 0f;
                                sideDir = Quaternion.AngleAxis(-90f, Vector3.up) * sideDir;

                                _surfer.MoveData.ladderClimbDir = Quaternion.AngleAxis(90f, sideDir) * hit.normal;
                                _surfer.MoveData.ladderClimbDir *=
                                    1f / _surfer.MoveData.ladderClimbDir.y; // Make sure Y is always 1
                            }
                            else
                            {
                                _surfer.MoveData.ladderClimbDir = Vector3.up;
                            }
                        }
                    }
                }
            }

            if (!foundLadder)
            {
                _surfer.MoveData.ladderNormal = Vector3.zero;
                _surfer.MoveData.ladderVelocity = Vector3.zero;
                _surfer.MoveData.climbingLadder = false;
                _surfer.MoveData.ladderClimbDir = Vector3.up;
            }
        }

        private void LadderPhysics()
        {
            _surfer.MoveData.ladderVelocity = _surfer.MoveData.ladderClimbDir * _surfer.MoveData.verticalAxis * 6f;

            _surfer.MoveData.velocity = Vector3.Lerp(_surfer.MoveData.velocity, _surfer.MoveData.ladderVelocity,
                Time.deltaTime * 10f);

            LadderCheck(Vector3.one, _surfer.MoveData.ladderDirection);

            var floorTrace = TraceToFloor();
            if (_surfer.MoveData.verticalAxis < 0f && floorTrace.hitCollider != null &&
                Vector3.Angle(Vector3.up, floorTrace.planeNormal) <= _surfer.MoveData.slopeLimit)
            {
                _surfer.MoveData.velocity = _surfer.MoveData.ladderNormal * 0.5f;
                _surfer.MoveData.ladderVelocity = Vector3.zero;
                _surfer.MoveData.climbingLadder = false;
            }

            if (_surfer.MoveData.wishJump)
            {
                _surfer.MoveData.velocity = _surfer.MoveData.ladderNormal * 4f;
                _surfer.MoveData.ladderVelocity = Vector3.zero;
                _surfer.MoveData.climbingLadder = false;
            }
        }

        private void Accelerate(Vector3 wishDir, float wishSpeed, float acceleration, bool yMovement)
        {
            // Initialise variables
            float _addSpeed;
            float _accelerationSpeed;
            float _currentSpeed;

            // again, no idea
            _currentSpeed = Vector3.Dot(_surfer.MoveData.velocity, wishDir);
            _addSpeed = wishSpeed - _currentSpeed;

            // If you're not actually increasing your speed, stop here.
            if (_addSpeed <= 0)
                return;

            // won't bother trying to understand any of this, really
            _accelerationSpeed = Mathf.Min(acceleration * _deltaTime * wishSpeed, _addSpeed);

            // Add the velocity.
            _surfer.MoveData.velocity.x += _accelerationSpeed * wishDir.x;
            if (yMovement) _surfer.MoveData.velocity.y += _accelerationSpeed * wishDir.y;
            _surfer.MoveData.velocity.z += _accelerationSpeed * wishDir.z;
        }

        private void ApplyFriction(float t, bool yAffected, bool grounded)
        {
            // Initialise variables
            var _vel = _surfer.MoveData.velocity;
            float _speed;
            float _newSpeed;
            float _control;
            float _drop;

            // Set Y to 0, speed to the magnitude of movement and drop to 0. I think drop is the amount of speed that is lost, but I just stole this from the internet, idk.
            _vel.y = 0.0f;
            _speed = _vel.magnitude;
            _drop = 0.0f;

            var fric = crouching ? _config.crouchFriction : _config.Friction;
            var accel = crouching ? _config.crouchAcceleration : _config.acceleration;
            var decel = crouching ? _config.crouchDeceleration : _config.deceleration;

            // Only apply friction if the player is grounded
            if (grounded)
            {
                // i honestly have no idea what this does tbh
                _vel.y = _surfer.MoveData.velocity.y;
                _control = _speed < decel ? decel : _speed;
                _drop = _control * fric * _deltaTime * t;
            }

            // again, no idea, but comments look cool
            _newSpeed = Mathf.Max(_speed - _drop, 0f);
            if (_speed > 0.0f)
                _newSpeed /= _speed;

            // Set the end-velocity
            _surfer.MoveData.velocity.x *= _newSpeed;
            if (yAffected) _surfer.MoveData.velocity.y *= _newSpeed;
            _surfer.MoveData.velocity.z *= _newSpeed;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private Vector3 AirInputMovement()
        {
            Vector3 wishVel, wishDir;
            float wishSpeed;

            GetWishValues(out wishVel, out wishDir, out wishSpeed);

            if (_config.clampAirSpeed && wishSpeed != 0f && wishSpeed > _config.MaxSpeed)
            {
                wishVel = wishVel * (_config.MaxSpeed / wishSpeed);
                wishSpeed = _config.MaxSpeed;
            }

            return SurfPhysics.AirAccelerate(_surfer.MoveData.velocity, wishDir, wishSpeed, _config.airAcceleration,
                _config.airCap, _deltaTime);
        }

        /// <summary>
        /// </summary>
        /// <param name="wishVel"></param>
        /// <param name="wishDir"></param>
        /// <param name="wishSpeed"></param>
        private void GetWishValues(out Vector3 wishVel, out Vector3 wishDir, out float wishSpeed)
        {
            wishVel = Vector3.zero;
            wishDir = Vector3.zero;
            wishSpeed = 0f;

            Vector3 forward = _surfer.Forward,
                right = _surfer.Right;

            forward[1] = 0;
            right[1] = 0;
            forward.Normalize();
            right.Normalize();

            for (var i = 0; i < 3; i++)
                wishVel[i] = forward[i] * _surfer.MoveData.forwardMove + right[i] * _surfer.MoveData.sideMove;
            wishVel[1] = 0;

            wishSpeed = wishVel.magnitude;
            wishDir = wishVel.normalized;
        }

        /// <summary>
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="jumpPower"></param>
        private void Jump()
        {
            if (!_config.AutoBhop)
                _surfer.MoveData.wishJump = false;

            _surfer.MoveData.velocity.y += _config.JumpForce;
            jumping = true;
        }

        /// <summary>
        /// </summary>
        private bool CheckGrounded()
        {
            _surfer.MoveData.surfaceFriction = 1f;
            var movingUp = _surfer.MoveData.velocity.y > 0f;
            var trace = TraceToFloor();

            var groundSteepness = Vector3.Angle(Vector3.up, trace.planeNormal);

            if (trace.hitCollider == null || groundSteepness > _config.SlopeLimit ||
                (jumping && _surfer.MoveData.velocity.y > 0f))
            {
                SetGround(null);

                if (movingUp && _surfer.MoveType != MoveType.Noclip)
                    _surfer.MoveData.surfaceFriction = _config.airFriction;

                return false;
            }

            groundNormal = trace.planeNormal;
            SetGround(trace.hitCollider.gameObject);
            return true;
        }

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        private void SetGround(GameObject obj)
        {
            if (obj != null)
            {
                _surfer.GroundObject = obj;
                _surfer.MoveData.velocity.y = 0;
            }
            else
            {
                _surfer.GroundObject = null;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        private Trace TraceBounds(Vector3 start, Vector3 end, int layerMask)
        {
            return Tracer.TraceCollider(_surfer.SurfCollider, start, end, layerMask);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private Trace TraceToFloor()
        {
            var down = _surfer.MoveData.origin;
            down.y -= 0.15f;

            return Tracer.TraceCollider(_surfer.SurfCollider, _surfer.MoveData.origin, down, SurfPhysics.groundLayerMask);
        }

        public void Crouch(ISurfControllable surfer, MovementConfig config, float deltaTime)
        {
            _surfer = surfer;
            _config = config;
            _deltaTime = deltaTime;

            if (_surfer == null)
                return;

            if (_surfer.SurfCollider == null)
                return;

            var grounded = _surfer.GroundObject != null;
            var wantsToCrouch = _surfer.MoveData.crouching;

            var crouchingHeight = Mathf.Clamp(_surfer.MoveData.crouchingHeight, 0.01f, 1f);
            var heightDifference = _surfer.MoveData.defaultHeight - _surfer.MoveData.defaultHeight * crouchingHeight;

            if (grounded)
                uncrouchDown = false;

            // Crouching input
            if (grounded)
                crouchLerp = Mathf.Lerp(crouchLerp, wantsToCrouch ? 1f : 0f,
                    _deltaTime * _surfer.MoveData.crouchingSpeed);
            else if (!grounded && !wantsToCrouch && crouchLerp < 0.95f)
                crouchLerp = 0f;
            else if (!grounded && wantsToCrouch)
                crouchLerp = 1f;

            // Collider and position changing
            if (crouchLerp > 0.9f && !crouching)
            {
                // Begin crouching
                crouching = true;
                if (_surfer.SurfCollider.GetType() == typeof(BoxCollider))
                {
                    // Box collider
                    var boxCollider = (BoxCollider)_surfer.SurfCollider;
                    boxCollider.size = new Vector3(boxCollider.size.x, _surfer.MoveData.defaultHeight * crouchingHeight,
                        boxCollider.size.z);
                }
                else if (_surfer.SurfCollider.GetType() == typeof(CapsuleCollider))
                {
                    // Capsule collider
                    var capsuleCollider = (CapsuleCollider)_surfer.SurfCollider;
                    capsuleCollider.height = _surfer.MoveData.defaultHeight * crouchingHeight;
                }

                // Move position and stuff
                _surfer.MoveData.origin += heightDifference / 2 * (grounded ? Vector3.down : Vector3.up);
                foreach (Transform child in playerTransform)
                {
                    if (child == _surfer.MoveData.viewTransform)
                        continue;

                    child.localPosition = new Vector3(child.localPosition.x, child.localPosition.y * crouchingHeight,
                        child.localPosition.z);
                }

                uncrouchDown = !grounded;
            }
            else if (crouching)
            {
                // Check if the player can uncrouch
                var canUncrouch = true;
                if (_surfer.SurfCollider.GetType() == typeof(BoxCollider))
                {
                    // Box collider
                    var boxCollider = (BoxCollider)_surfer.SurfCollider;
                    var halfExtents = boxCollider.size * 0.5f;
                    var startPos = boxCollider.transform.position;
                    var endPos = boxCollider.transform.position +
                                 (uncrouchDown ? Vector3.down : Vector3.up) * heightDifference;

                    var trace = Tracer.TraceBox(startPos, endPos, halfExtents, boxCollider.contactOffset,
                        SurfPhysics.groundLayerMask);

                    if (trace.hitCollider != null)
                        canUncrouch = false;
                }
                else if (_surfer.SurfCollider.GetType() == typeof(CapsuleCollider))
                {
                    // Capsule collider
                    var capsuleCollider = (CapsuleCollider)_surfer.SurfCollider;
                    var point1 = capsuleCollider.center + Vector3.up * capsuleCollider.height * 0.5f;
                    var point2 = capsuleCollider.center + Vector3.down * capsuleCollider.height * 0.5f;
                    var startPos = capsuleCollider.transform.position;
                    var endPos = capsuleCollider.transform.position +
                                 (uncrouchDown ? Vector3.down : Vector3.up) * heightDifference;

                    var trace = Tracer.TraceCapsule(point1, point2, capsuleCollider.radius, startPos, endPos,
                        capsuleCollider.contactOffset, SurfPhysics.groundLayerMask);

                    if (trace.hitCollider != null)
                        canUncrouch = false;
                }

                // Uncrouch
                if (canUncrouch && crouchLerp <= 0.9f)
                {
                    crouching = false;
                    if (_surfer.SurfCollider.GetType() == typeof(BoxCollider))
                    {
                        // Box collider
                        var boxCollider = (BoxCollider)_surfer.SurfCollider;
                        boxCollider.size = new Vector3(boxCollider.size.x, _surfer.MoveData.defaultHeight,
                            boxCollider.size.z);
                    }
                    else if (_surfer.SurfCollider.GetType() == typeof(CapsuleCollider))
                    {
                        // Capsule collider
                        var capsuleCollider = (CapsuleCollider)_surfer.SurfCollider;
                        capsuleCollider.height = _surfer.MoveData.defaultHeight;
                    }

                    // Move position and stuff
                    _surfer.MoveData.origin += heightDifference / 2 * (uncrouchDown ? Vector3.down : Vector3.up);
                    foreach (Transform child in playerTransform)
                        child.localPosition = new Vector3(child.localPosition.x,
                            child.localPosition.y / crouchingHeight, child.localPosition.z);
                }

                if (!canUncrouch)
                    crouchLerp = 1f;
            }

            // Changing camera position
            if (!crouching)
                _surfer.MoveData.viewTransform.localPosition = Vector3.Lerp(
                    _surfer.MoveData.viewTransformDefaultLocalPos,
                    _surfer.MoveData.viewTransformDefaultLocalPos * crouchingHeight +
                    Vector3.down * heightDifference * 0.5f, crouchLerp);
            else
                _surfer.MoveData.viewTransform.localPosition = Vector3.Lerp(
                    _surfer.MoveData.viewTransformDefaultLocalPos - Vector3.down * heightDifference * 0.5f,
                    _surfer.MoveData.viewTransformDefaultLocalPos * crouchingHeight, crouchLerp);
        }

        private void SlideMovement()
        {
            // Gradually change direction
            slideDirection += new Vector3(groundNormal.x, 0f, groundNormal.z) * slideSpeedCurrent * _deltaTime;
            slideDirection = slideDirection.normalized;

            // Set direction
            var slideForward = Vector3.Cross(groundNormal, Quaternion.AngleAxis(-90, Vector3.up) * slideDirection);

            // Set the velocity
            slideSpeedCurrent -= _config.slideFriction * _deltaTime;
            slideSpeedCurrent = Mathf.Clamp(slideSpeedCurrent, 0f, _config.maximumSlideSpeed);
            slideSpeedCurrent -=
                (slideForward * slideSpeedCurrent).y * _deltaTime *
                _config.downhillSlideSpeedMultiplier; // Accelerate downhill (-y = downward, - * - = +)

            _surfer.MoveData.velocity = slideForward * slideSpeedCurrent;

            // Jump
            if (_surfer.MoveData.wishJump &&
                slideSpeedCurrent < _config.minimumSlideSpeed * _config.slideSpeedMultiplier)
                Jump();
        }
    }
}
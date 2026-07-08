using System;
using NaughtyAttributes;
using UnityEngine;
using static Workshop.Scaffolding.Nature.Scripts.Audio.AudioUtils;

namespace Workshop.Scaffolding.Nature.Scripts.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FPSController : MonoBehaviour
    {
        [SerializeField] 
        [BoxGroup("Movement Settings")]
        private float walkSpeed = 5.0f;

        [SerializeField] 
        [BoxGroup("Movement Settings")]
        private float sprintSpeed = 10.0f;

        [SerializeField]
        [BoxGroup("Movement Settings")]
        private float accelerationTime = 0.3f;

        [SerializeField] 
        [BoxGroup("Movement Settings")]
        private float decelerationTime = 0.2f;

        [SerializeField] 
        [BoxGroup("Movement Settings")]
        private float sprintTransitionTime = 0.5f;

        [SerializeField]
        [BoxGroup("Jump Settings")]
        private float jumpHeight = 1.5f;

        [SerializeField] 
        [BoxGroup("Jump Settings")]
        private float jumpCooldown = 0.1f;

        [SerializeField] 
        [BoxGroup("Camera Settings")]
        private Camera playerCamera;

        [SerializeField] 
        [BoxGroup("Camera Settings")]
        private float mouseSensitivity = 2.0f;

        [SerializeField] 
        [BoxGroup("Camera Settings")]
        private float verticalLookLimit = 80.0f;

        [SerializeField] 
        [BoxGroup("Ground collision settings")]
        private LayerMask groundLayers;

        [SerializeField]
        [BoxGroup("Physics settings")]
        private float gravity = 10f;

        [SerializeField]
        [BoxGroup("Footsteps settings")]
        private float stepLength = 2;

        [SerializeField]
        [BoxGroup("Footsteps settings")]
        private MonoBehaviour surfaceProviderBehaviour;

        [SerializeField]
        [BoxGroup("External components")]
        private CharacterController characterController;

        private IFootstepSurfaceProvider surfaceProvider;

        public event Action<AudioSurfaceType, float> OnFootstepDetected;
        public event Action OnJump;
    
        private float currentSpeed;
        private float targetSpeed;
        private float verticalRotation;
        private float verticalVelocity;
        private float speedSmoothVelocity;
        private float currentSpeedPercent;
        private float jumpCooldownTimer;

        private Vector3 moveDirection;
        private bool isGrounded;
        private bool wasGroundedLastFrame;
        private bool didJumpThisFrame;

        private float distanceTravelled;
        private Vector3 lastPosition;

        private bool shouldRotate = true;

        private void Start()
        {
            // Lock cursor to center of screen.
            // Cursor.lockState = CursorLockMode.Locked;
            // Cursor.visible = false;

            // Initialize speed.
            currentSpeed = 0f;
            targetSpeed = 0f;

            lastPosition = transform.position;

            surfaceProvider = surfaceProviderBehaviour as IFootstepSurfaceProvider;
            if (surfaceProviderBehaviour != null && surfaceProvider == null)
            {
                Debug.LogWarning(
                    $"{nameof(surfaceProviderBehaviour)} on {name} does not implement {nameof(IFootstepSurfaceProvider)}.",
                    this);
            }
        }

        private void Update()
        {
            UpdateGroundedState();
            HandleJump();
            HandleMouseLook();
            HandleMovement();
            HandleFootsteps();

            // Update jump cooldown.
            if (jumpCooldownTimer > 0) jumpCooldownTimer -= Time.deltaTime;

            // Remember grounded state for next frame.
            wasGroundedLastFrame = isGrounded;
        }

        private void HandleMouseLook()
        {
            if (!shouldRotate) return;
            
            // Horizontal rotation (rotate player as well).
            var mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            transform.Rotate(Vector3.up, mouseX);

            // Vertical rotation (camera only).
            var mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            verticalRotation -= mouseY;
            verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        }

        private void HandleJump()
        {
            didJumpThisFrame = false;

            if (!Input.GetKeyDown(KeyCode.Space))         return;
            if (!isGrounded || !(jumpCooldownTimer <= 0)) return;

            // Calculate jump velocity (v = sqrt(2gh)).
            verticalVelocity = Mathf.Sqrt(2f * gravity * jumpHeight);
            didJumpThisFrame = true;
            jumpCooldownTimer = jumpCooldown;
            OnJump?.Invoke();
        }

        private void HandleMovement()
        {
            // Get input.
            var inputX = Input.GetAxisRaw("Horizontal");
            var inputZ = Input.GetAxisRaw("Vertical");

            // Calculate move direction relative to player orientation.
            var forward = transform.forward * inputZ;
            var right = transform.right * inputX;
            var inputDirection = (forward + right).normalized;

            // Determine target speed based on sprint input.
            var isSprinting = Input.GetKey(KeyCode.LeftShift) && inputZ > 0;
            var maxSpeed = isSprinting ? sprintSpeed : walkSpeed;

            // If there's input, set target speed, otherwise target is 0.
            targetSpeed = inputDirection.magnitude > 0.1f ? maxSpeed : 0f;

            // Choose appropriate smoothing time based on accelerating, decelerating, or sprinting.
            float smoothTime;
            if (targetSpeed > currentSpeed) smoothTime = accelerationTime;
            else if (targetSpeed < currentSpeed && targetSpeed > 0) smoothTime = sprintTransitionTime;
            else smoothTime = decelerationTime;

            // Smoothly adjust current speed.
            currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);

            // Calculate sprint percentage for FMOD parameter.
            currentSpeedPercent = Mathf.InverseLerp(0, sprintSpeed, currentSpeed);

            // Set move direction (keep direction even when stopping).
            if (inputDirection.magnitude > 0.1f) moveDirection = inputDirection;

            // Compute horizontal movement.
            var velocity = moveDirection * currentSpeed;

            // Apply gravity.
            if (isGrounded && verticalVelocity < 0 && !didJumpThisFrame)
            {
                verticalVelocity = -2f; // Small negative value to keep player grounded.
            }
            else
            {
                verticalVelocity -= gravity * Time.deltaTime;
            }

            // Apply vertical movement.
            velocity.y = verticalVelocity;

            // Move the character.
            characterController.Move(velocity * Time.deltaTime);
        }

        private void HandleFootsteps()
        {
            if (!isGrounded || !(currentSpeed > 0.1f)) return;

            // Calculate horizontal movement using projection on horizontal plane.
            var horizontalMovement = Vector3.ProjectOnPlane(transform.position - lastPosition, Vector3.up);
            distanceTravelled += horizontalMovement.magnitude;

            // Check if we should play a footstep.
            if (distanceTravelled >= stepLength)
            {
                var footstepSurface = GetCurrentSurface();
                if (footstepSurface != AudioSurfaceType.None)
                {
                    OnFootstepDetected?.Invoke(footstepSurface, currentSpeedPercent);
                }
                distanceTravelled -= stepLength;
            }

            lastPosition = transform.position;
        }

        private void UpdateGroundedState()
        {
            isGrounded = characterController.isGrounded;

            // Detect landing (for FMOD footstep/landing sound).
            if (isGrounded && !wasGroundedLastFrame && verticalVelocity < -2f)
            {
                // Player has just landed. (note: impact force is unused).
                var impactForce = Mathf.Abs(verticalVelocity);
            }
        }

        private AudioSurfaceType GetCurrentSurface()
        {
            if (surfaceProvider == null) return AudioSurfaceType.None;

            if (Physics.Raycast(transform.position, Vector3.down, out var hit, 2f, groundLayers))
            {
                return surfaceProvider.GetSurface(hit.collider, hit.point);
            }

            return AudioSurfaceType.None;
        }
        
        public void SetCursorLockState(bool isLocked)
        {
            Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible   = !isLocked;
            shouldRotate     = isLocked;
        }
    }
}
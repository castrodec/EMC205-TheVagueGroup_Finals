#if ENABLE_UNITY_2D_ANIMATION && ENABLE_UNITY_COLLECTIONS

using UnityEngine;

namespace TBG3DOpenWorldExample
{
    [RequireComponent(typeof(CharacterController))]
    public class Basic3rdPersonController : MonoBehaviour
    {
        public Transform cameraTransform;
        public float orbitDistance = 5f;
        public float moveSpeed = 5f;
        public float jumpHeight = 2f;
        public float gravity = -9.81f;

        private Animator animator;
        private CharacterController characterController;
        private Vector3 velocity;
        private bool isGrounded;
        private Vector3 scale;

        private void Start()
        {
            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            scale = transform.localScale;

            // Unlock framerate
            Application.targetFrameRate = -1;
        }

        private void Update()
        {
            // Orbit camera on the mouse
            var mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            cameraTransform.eulerAngles = new Vector3(cameraTransform.eulerAngles.x - mouseDelta.y, cameraTransform.eulerAngles.y + mouseDelta.x, 0f);

            // Rotate the character based on the camera's rotation.
            transform.rotation = Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f);

            // Move the character based on the camera's forward direction.
            var moveDirection = cameraTransform.forward * Input.GetAxis("Vertical") + cameraTransform.right * Input.GetAxis("Horizontal");
            moveDirection.y = 0f;
            moveDirection.Normalize();
            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

            // Jump if the player presses the jump button and is grounded.
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }

            // Apply gravity.
            velocity.y += gravity * Time.deltaTime;
            characterController.Move(velocity * Time.deltaTime);

            // Check if the character is grounded.
            isGrounded = characterController.isGrounded;

            // Update animation
            animator.SetBool("isRunning", moveDirection.magnitude > 0.1f);
            var relativeXVelocity = Vector3.Dot(moveDirection, Camera.main.transform.right);
            if (Mathf.Abs(relativeXVelocity) > 0.1f)
            {
                transform.localScale = new Vector3(scale.x * -Mathf.Sign(relativeXVelocity), scale.y, scale.z);
            }
        }

        private void OnDrawGizmosSelected()
        {
            // Draw a sphere to show the orbit distance.
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, orbitDistance);
        }

        private void OnDrawGizmos()
        {
            // Draw a line to show the direction the character is facing.
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 2f);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            // If the character hits something while moving downwards, stop the character.
            if (velocity.y < 0f)
            {
                velocity.y = 0f;
            }
        }

        private void LateUpdate()
        {
            // Move the camera to the orbit distance behind the character.
            cameraTransform.position = transform.position - cameraTransform.forward * orbitDistance;
        }
    }
}

#endif
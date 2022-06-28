using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float speed;

    [SerializeField]
    private Transform cameraTransform;

    [SerializeField]
    private float jumpSpeed;

    [SerializeField]
    private float jumpButtonGracePeriod;

    [SerializeField]
    private float rotationSpeed;


    private Animator animator;
    private CharacterController characterController;
    private float ySpeed;
    private float? lastGroundTime;
    private float? jumpButtonPressedTime;
    private bool isJumping;
    private bool isGrounded;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
            // get horizontal and vertical input and convert it to movement
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");
            Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);
            // use the camera to get movement, player move in the camera direction
            movementDirection = Quaternion.AngleAxis(cameraTransform.rotation.eulerAngles.y, Vector3.up) * movementDirection;
            float magnitude = Mathf.Clamp01(movementDirection.magnitude) * speed;
            movementDirection.Normalize();
            // jump and gravity
            ySpeed += Physics.gravity.y * Time.deltaTime;

            if (movementDirection != Vector3.zero)
            {
                animator.SetBool("Movement", true);
                Quaternion toRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
            }
            else
            {
                animator.SetBool("Movement", false);
            }

            if (characterController.isGrounded)
            {
                lastGroundTime = Time.time;
            }

            if (Input.GetButtonDown("Jump"))
            {
                jumpButtonPressedTime = Time.time;
            }
            if (Time.time - lastGroundTime <= jumpButtonGracePeriod)
            {
                ySpeed = -0.8f;
                animator.SetBool("IsGrounded", true);
                isGrounded = true;
                animator.SetBool("IsJumping", false);
                isJumping = false;
                animator.SetBool("IsFalling", false);

                if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
                {
                    ySpeed = jumpSpeed;
                    animator.SetBool("IsJumping", true);
                    isJumping = true;
                    jumpButtonPressedTime = null;
                    lastGroundTime = null;
                }
            }
            else
            {
                animator.SetBool("IsGrounded", false);
                isGrounded = false;

                if ((isJumping && ySpeed < 0) || ySpeed < -2)
                {
                    animator.SetBool("IsFalling", true);
                }
            }

            Vector3 velocity = movementDirection * magnitude;
            velocity = adjustVelocityToSlope(velocity);
            velocity.y += ySpeed;

            characterController.Move(velocity * Time.deltaTime);
    }
    private void OnApplicationFocus(bool focusStatus)
    {
        if (focusStatus)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    private Vector3 adjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);
        
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 0.2f))
        {
            var slopeRotate = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            var adjustedVelocity = slopeRotate * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }
        return velocity;
    }
}

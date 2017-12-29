using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public Transform playerCam, character, centerPoint;
    public Animator animator;
    CharacterController controller;

    private float mouseX, mouseY;
    public float mouseSensitivity = 10f;
    public float mouseYPosition = 1f;

    private float moveFB, moveLR;
    public float walkSpeed = 2f;
    public float runSpeed = 6f;
    public float jumpForce = 10f;
    public float gravityForce = -10f;


    float turnSmoothVelocity;
    public float speedSmoothTime = 0.2f;
    public float speedSmoothVelocity = 0.2f;
    public float turnSmoothTime = 0.2f;
    public float airControlPercent = 0.3f;
    float currentSpeed;

    Vector3 movement;

    float moveY;
    float velocityY;

    private float zoom;
    public float zoomSpeed = 2;

    public float zoomMin = -2f;
    public float zoomMax = -10f;

    public float rotationSpeed = 5f;

    void Start()
    {
		//Setting the zoom from the player to be -3 units
        zoom = -3;
		//Getting animator component
        animator = GetComponent<Animator>();
		//Getting controller component
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool running = Input.GetKey(KeyCode.LeftShift);
		//Getting the inputs
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        //Normalize the input Vector2
		Vector2 inputDir = input.normalized;
	
		//Move method
        Move(inputDir, running);

		//Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

		//Zoom Input
        zoom += Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

		//Zoom max min/max distance to the CenterView
        if (zoom > zoomMin)
            zoom = zoomMin;

        if (zoom < zoomMax)
            zoom = zoomMax;

		//Apply the zoom
        playerCam.transform.localPosition = new Vector3(0, 0, zoom);

		//Moving the camera while the right click is on hold
        if (Input.GetMouseButton(1))
        {
            mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
            mouseY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        }

		//Min/max angle
        mouseY = Mathf.Clamp(mouseY, -60f, 60f);
		//Camera look at the CenterPoint
        playerCam.LookAt(centerPoint);
		//Rotating the CenterPoint to mouseY,mouseX and setting its position to character position
        centerPoint.localRotation = Quaternion.Euler(mouseY, mouseX, 0);
        centerPoint.position = new Vector3(character.position.x, character.position.y + mouseYPosition, character.position.z);

        //Modify animator variables based on Character Current Speed 
        float animationSpeedPercent = ((running) ? currentSpeed / runSpeed : currentSpeed / walkSpeed * .5f);
        animator.SetFloat("speedPercent", animationSpeedPercent, speedSmoothTime, Time.deltaTime);
    }


	//Smoothing the movement/rotation speed and get based on air control percent
    float GetModifiedSmoothTime(float smoothTime)
    {
        if (controller.isGrounded)
        {
            return smoothTime;
        }

        if (airControlPercent == 0)
        {
            return float.MaxValue;
        }
        return smoothTime / airControlPercent;
    }


	//Moving method
    void Move(Vector2 inputDir, bool running)
    {
		//Rotation based on camera
        if (inputDir != Vector2.zero)
        {
            float targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg + centerPoint.eulerAngles.y;
            transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref turnSmoothVelocity, GetModifiedSmoothTime(turnSmoothTime));
        }

		//Setting target speed based on runSpeed/walkSpeed multiply with the distance of the Vector2 inputDir
        float targetSpeed = ((running) ? runSpeed : walkSpeed) * inputDir.magnitude;
		//Smoothing the time between currentSpeed and target speed
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, GetModifiedSmoothTime(speedSmoothTime));

		//Gravity
        velocityY += Time.deltaTime * gravityForce;
        Vector3 velocity = transform.forward * currentSpeed + Vector3.up * velocityY;

		//Applying movement
        controller.Move(velocity * Time.deltaTime);
		//Setting new currentSpeed
        currentSpeed = new Vector2(controller.velocity.x, controller.velocity.z).magnitude;
		
		//Setting velocity on Y axis to be 0 while the character is grounded
        if (controller.isGrounded)
        {
            velocityY = 0;
        }

    }

	//Jump Method
    void Jump()
    {
        if (controller.isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(-2 * gravityForce * jumpForce);
            velocityY = jumpVelocity;
        }
    }
}
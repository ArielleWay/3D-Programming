using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerController : MonoBehaviour
{
    PlayerInputs playerInputs;
    public float speed = 5f;
    public float gravity = -9.01f;
    public float jumpHeight = 2f;

    public CharacterController controller;
    public Animator animator;

    private Vector3 velocity;
    private bool isGrounded;

    private Vector2 currentMovementInput;
    private Vector3 currentMovement;
    private bool isMovementPressed;
    private bool isRunningInput;
    private bool isRunning;

    public float rotationFactorPerFrame = 1f;    

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    // Update is called once per frame
    void Update()
    {

        controller.Move(currentMovement * Time.deltaTime);
        Debug.Log(currentMovement);
        animator.SetBool("isWalking", currentMovement != Vector3.zero);
        isGrounded = controller.isGrounded;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Debug.Log(playerInputs.CharacterController.Run.wantsInitialStateCheck);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);


        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetLocation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetLocation, rotationFactorPerFrame * Time.deltaTime);
        }
    }

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        controller = GetComponent<CharacterController>();
        //animator = GetComponent<Animator>();

        playerInputs.CharacterController.Move.started += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
       
        };

        playerInputs.CharacterController.Move.canceled += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            currentMovement.x = currentMovementInput.x;
            currentMovement.z = currentMovementInput.y;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
    

        };

        playerInputs.CharacterController.Run.started += context =>
        {
            isRunningInput = context.ReadValue<bool>();
            isRunning = isRunningInput;
        };

        playerInputs.CharacterController.Run.canceled += context =>
        {
            isRunningInput = context.ReadValue<bool>();
            isRunning = isRunningInput;

        };

        
    }

    private void OnEnable()
    {
        playerInputs.CharacterController.Enable();
    }

    private void OnDisable()
    {
        playerInputs.CharacterController.Disable();
    }
}

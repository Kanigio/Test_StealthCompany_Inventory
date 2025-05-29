using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class GO_PlayerController_FirstPerson : MonoBehaviour
{
    
    public GO_PlayerCharacter_FirstPerson OwnerCharacter;
   
    private PlayerInputActions inputActions;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector2 ScrollInput;
    private bool UseInput;
    private bool InteractInput;
    private bool jumpPressed;
    
    private bool usePressed;
    private bool interactPressed;
    private float scrollY;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => Jump();

        inputActions.Player.Use.performed += ctx => Use();
        inputActions.Player.Interact.performed += ctx => Interact();

        inputActions.Player.Scroll.performed += ctx =>
        {
            scrollY = ctx.ReadValue<Vector2>().y;
            HandleScroll(scrollY);
        };
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }
    
    private void Update()
    {
        Move(moveInput);
        Look(lookInput);
    }

    private void Move(Vector2 direction)
    {
        OwnerCharacter.Move(direction);
    }

    private void Look(Vector2 look)
    {
        // Usually mouse look: X = yaw, Y = pitch (optional for camera)
        float mouseSensitivity = 0.7f;
        float yaw = look.x * mouseSensitivity;
        float pitch = look.y * mouseSensitivity;
        OwnerCharacter.Look(pitch, yaw);
    }
    private void Jump()
    {
        OwnerCharacter.Jump();
    }
    
    private void Use()
    {
        OwnerCharacter.UseItem();
    }

    private void Interact()
    {
        OwnerCharacter.TryInteract();
    }

    private void HandleScroll(float deltaY)
    {
        if (Mathf.Approximately(deltaY, 0f)) return;

        bool scrollForward = deltaY > 0;
        OwnerCharacter.ScrollItems(scrollForward);
    }
}

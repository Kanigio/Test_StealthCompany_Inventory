using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GO_PlayerCharacter_FirstPerson : MonoBehaviour, I_Interactor_FirstPerson
{
    
    public GO_PlayerController_FirstPerson Controller;
    
    public GO_PlayerToolBar_Inventories ScrollToolbar;
    
    // Jump force multiplier
    public float JumpForce = 5f;
    
    [SerializeField] private float groundCheckDistance = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    // Movement speed multiplier
    public float MoveSpeed = 5f;
    
    [SerializeField] private Transform cameraTransform;
    private float cameraPitch = 0f;
    
    private Rigidbody rb;

    // Reference to the Inventory
    public GO_Inventory_Inventories Inventory;

    // Move called by the Player Controller
    public void Move(Vector2 direction)
    {
        // Convert local input to world direction
        Vector3 move = transform.right * direction.x + transform.forward * direction.y;
        move *= MoveSpeed * Time.deltaTime;

        // Apply movement
        Vector3 newPosition = rb.position + move;
        rb.MovePosition(newPosition);
    }

    // Look around with pitch (x rotation) and yaw (y rotation)
    public void Look(float pitch, float yaw)
    {
        // Rotate the character's transform horizontally (yaw)
        transform.Rotate(Vector3.up * yaw);

        // rotate only the camera on X (vertical - pitch)
        cameraPitch -= pitch;
        cameraPitch = Mathf.Clamp(cameraPitch, -85f, 85f);

        cameraTransform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    // Jump method
    public void Jump()
    {
        if (!IsGrounded()) return;

        rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }
    
    private bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.up * 0.1f; // sposta leggermente in alto per evitare self-hit
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
    }

    // Interact with objects implementing I_Interactable_FirstPerson
    public void Interact(GameObject interactable)
    {
        // Check The Other Object if Implements The Interactable Interface
        var interactableComponent = interactable.GetComponent<I_Interactable_FirstPerson>();
        if (interactableComponent != null)
        {
            interactableComponent.Interact(this.GameObject());
        }
    }

    public void InteractWith(GameObject interactable)
    {
        
    }
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void UseItem()
    {
        if (ScrollToolbar != null)
        {
            ScrollToolbar.UseSelectedItem();
        }
    }

    public void ScrollItems(bool bForward)
    {
        ScrollToolbar.ScrollItems(!bForward);
    }
}
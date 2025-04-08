using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    [Header("Attack Settings")]
    public float attackCooldown = 0.5f;
    private bool canAttack = true;

    [Header("Equip Settings")]
    private bool isEquipped = false;
    private bool canToggleEquip = true;
    public float toggleCooldown = 0.5f;

    public float rotationFactorPerFrame = 1f;

    // Inventory Integration
    public InventoryManager inventoryManager;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        controller.Move(currentMovement * Time.deltaTime);
        //Debug.Log(currentMovement);
        animator.SetBool("isWalking", currentMovement != Vector3.zero);
        isGrounded = controller.isGrounded;
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

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

        // Set isRunning based on left shift key press
        bool isRunningNow = Input.GetKey(KeyCode.LeftShift) && isMovementPressed;
        animator.SetBool("isRunning", isRunningNow);
        //Debug.Log("Is Running: " + isRunningNow);

        if (Input.GetKeyDown(KeyCode.F))
        {
            PickupItem();
            Debug.Log("Picked Up");
        }

        // Example: Drop item on press 'D'
        if (Input.GetKeyDown(KeyCode.V))
        {
            DropItem();
        }
    }

    private void Awake()
    {
        playerInputs = new PlayerInputs();
        controller = GetComponent<CharacterController>();

        playerInputs.CharacterController.Move.started += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            float movementSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2f : speed / 2f; // Adjust walk speed
            currentMovement.x = currentMovementInput.x * movementSpeed;
            currentMovement.z = currentMovementInput.y * movementSpeed;
            isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        };

        playerInputs.CharacterController.Move.canceled += context =>
        {
            currentMovementInput = context.ReadValue<Vector2>();
            float movementSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * 2f : speed / 2f; // Adjust walk speed
            currentMovement.x = currentMovementInput.x * movementSpeed;
            currentMovement.z = currentMovementInput.y * movementSpeed;
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

        playerInputs.CharacterController.Attack.started += context => PerformAttack();
        playerInputs.CharacterController.Equip.started += context => ToggleEquip();
    }

    private void PerformAttack()
    {
        if (canAttack)
        {
            animator.SetTrigger("isAttacking");
            StartCoroutine(AttackCooldown());
            // Optional: Stop movement during attack
            // currentMovement = Vector3.zero;
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void ToggleEquip()
    {
        if (!canToggleEquip) return;

        if (isEquipped)
        {
            UnequipWeapon();
        }
        else
        {
            EquipWeapon();
        }

        StartCoroutine(EquipCooldown());
    }

    private void EquipWeapon()
    {
        isEquipped = true;
        animator.SetLayerWeight(animator.GetLayerIndex("EquipLayer"), 1f);
        animator.SetLayerWeight(animator.GetLayerIndex("UnequipLayer"), 0f);
        animator.SetBool("isEquipping", true);
        animator.SetTrigger("isEquipping");
        Debug.Log("equipped!");
        //Debug.Log("Equip Layer Index: " + EquipLayer);
        // Optional: Stop movement during equip
        // currentMovement = Vector3.zero;
    }

    private void UnequipWeapon()
    {
        isEquipped = false;
        animator.SetLayerWeight(animator.GetLayerIndex("EquipLayer"), 0f);
        animator.SetLayerWeight(animator.GetLayerIndex("UnequipLayer"), 1f);
        animator.SetBool("isUnequipping", true);
        animator.SetTrigger("isUnequipping");
        Debug.Log("unequipped!");
        //Debug.Log("Unequip Layer Index: " + UnequipLayer);
        // Optional: Stop movement during unequip
        // currentMovement = Vector3.zero;
    }

    private IEnumerator EquipCooldown()
    {
        canToggleEquip = false;
        yield return new WaitForSeconds(toggleCooldown);
        canToggleEquip = true;
    }

    private void OnEnable()
    {
        playerInputs.CharacterController.Enable();
    }

    private void OnDisable()
    {
        playerInputs.CharacterController.Disable();
    }

    // Call these from animation events at the end of equip/unequip animations
    private void OnEquipComplete()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("EquipLayer"), 0f);
        animator.SetBool("isEquipping", false);
    }

    private void OnUnequipComplete()
    {
        animator.SetLayerWeight(animator.GetLayerIndex("UnequipLayer"), 0f);
        animator.SetBool("isUnequipping", false);
    }

    private void PickupItem()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 1f);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Item"))
            {
                string itemName = hitCollider.gameObject.name;
                inventoryManager.AddItem(itemName);
                Destroy(hitCollider.gameObject); // Remove the item from the scene
                Debug.Log($"Picked up {itemName}");
                return;
            }
        }
    }

    private void DropItem()
    {
        // Example: Drop the first item in the inventory
        if (inventoryManager.inventory.Count > 0)
        {
            var firstItem = inventoryManager.inventory.First().Value;
            inventoryManager.RemoveItem(firstItem.itemName);
            // Instantiate the item at the player's position
            GameObject itemObject = Instantiate(Resources.Load<GameObject>(firstItem.itemName), transform.position, Quaternion.identity);
            Debug.Log($"Dropped {firstItem.itemName}");
        }
    }
}

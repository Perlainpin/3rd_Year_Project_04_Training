 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class HeroController : MonoBehaviour
{
    private PlayerInput action;

    private InputAction moveAction;
    private InputAction lookAction;


    private Rigidbody rb;

    private float speed = 6f;
    private float jumpForce = 6f;

    public float sensX = 1f;
    public float sensY = 1f;


    private void Awake()
    {
        action = new PlayerInput();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveAction = action.Player.Move;
        moveAction.Enable();
        lookAction = action.Player.Look;
        lookAction.Enable();
        action.Player.Jump.performed += OnJump;
        action.Player.Jump.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();

        action.Player.Jump.performed -= OnJump;
        action.Player.Jump.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    private void FixedUpdate()
    {
        Vector2 moveDir = moveAction.ReadValue<Vector2>();
        Vector3 vel = rb.velocity;
        vel.x = speed * moveDir.x;
        vel.z = speed * moveDir.y;
        rb.velocity = vel;

        Vector2 lookDir = lookAction.ReadValue<Vector2>();

        float mouseX = lookAction.ReadValue<Vector2>().x * sensX * Time.deltaTime;
        float mouseY = lookAction.ReadValue<Vector2>().y * sensY * Time.deltaTime;
    }


}

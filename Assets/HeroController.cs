 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;

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


    public GameObject Gun;
    public GameObject Launcher;
    private GameObject ActiveGun;
    private bool gunInHand = true;


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

        action.Player.SwitchWeapon.performed += OnSwitch;
        action.Player.SwitchWeapon.Enable();

        action.Player.Fire.Enable();

        action.Player.Reload.performed += OnReload;
        action.Player.Reload.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();

        action.Player.Jump.performed -= OnJump;
        action.Player.Jump.Disable();

        action.Player.SwitchWeapon.performed -= OnSwitch;
        action.Player.SwitchWeapon.Disable();

        action.Player.Fire.Disable();

        action.Player.Reload.performed -= OnReload;
        action.Player.Reload.Disable();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnSwitch(InputAction.CallbackContext context)
    {
        gunInHand = !gunInHand;
    }

    private void OnReload(InputAction.CallbackContext context)
    {
        if (ActiveGun.GetComponent<Firing>().bulletsLeft < ActiveGun.GetComponent<Firing>().magazineSize && !ActiveGun.GetComponent<Firing>().reloading)
            ActiveGun.GetComponent<Firing>().Reload();
        
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

        if (gunInHand)
        {
            Gun.SetActive(true);
            Launcher.SetActive(false);

            ActiveGun = Gun;
        }

        else if (!gunInHand)
        {
            Launcher.SetActive(true);
            Gun.SetActive(false);

            ActiveGun = Launcher;
        }

        if (action.Player.Fire.ReadValue<float>() > 0)
        {

            if (ActiveGun.GetComponent<Firing>().readyToShoot && !ActiveGun.GetComponent<Firing>().reloading && ActiveGun.GetComponent<Firing>().bulletsLeft <= 0)
            {
                ActiveGun.GetComponent<Firing>().Reload();
            }

            if (ActiveGun.GetComponent<Firing>().readyToShoot && !ActiveGun.GetComponent<Firing>().reloading && ActiveGun.GetComponent<Firing>().bulletsLeft > 0)
            {
                ActiveGun.GetComponent<Firing>().bulletsShot = 0;

                ActiveGun.GetComponent<Firing>().Shoot();
            }
        }
    }
}
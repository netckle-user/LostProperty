using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public CharacterController controller;
    [Range(0, 10)]
    public float jumpSpeed = 0;
    [Range(0, 10)]
    public float moveSpeed = 0;

    public Transform lookObj;
    [Range(0, 10)]
    public float rotationSpeed = 0;

    public CinemachineCamera cam;

    [Range(0, 10)]
    public float lookObjHeight;

    private Vector2 input;
    private float ySpeed = 0;
    private bool jumpFlag = false;
    private bool isRunning = false;

    private Vector2 look;

    public Animator animator;

    private void Start()
    {
        lookObj.rotation = Quaternion.identity;
    }

    public void Update()
    {
        lookObj.position = transform.position + Vector3.up * lookObjHeight;



        ySpeed += Physics.gravity.y * Time.deltaTime;
        if (jumpFlag)
        {
            ySpeed = jumpSpeed;
            jumpFlag = false;
        }

        if (!jumpFlag || input != Vector2.zero)
            Move(input);

        animator.SetBool("isMoving", input != Vector2.zero);
        animator.SetBool("isRunning", isRunning);
    }

    private void LateUpdate()
    {
        Look();
    }

    public void FixedUpdate()
    {
        if (IsGrounded())
        {
            ySpeed = -0.8f;
        }
    }

    private void Look()
    {
        var curRot = lookObj.rotation.eulerAngles;

        var xRot = curRot.x + look.y * rotationSpeed * Time.deltaTime; 
        var yRot = curRot.y + look.x * rotationSpeed * Time.deltaTime;

        //xRot = Mathf.Clamp(xRot, -85.0f, 85.0f);

        lookObj.rotation = Quaternion.Euler(xRot, yRot, 0);
        
        if (input != Vector2.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(getLookDirection(input)), 0.1f);
        }
    }

    public void OnSprint(InputValue value)
    {
        isRunning = value.isPressed;
    }

    public void OnLook(InputValue value)
    {
        look = value.Get<Vector2>();        
    }

    public void Move(Vector2 input)
    {
        Vector3 direction = cam.transform.forward * input.y + cam.transform.right * input.x;
        direction.y = ySpeed;
        controller.Move(direction * Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, controller.height / 2 + 0.1f);
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && IsGrounded())
        {
            jumpFlag = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * (controller.height / 2 + 0.1f));
    }

    Vector3 getLookDirection(Vector2 input)
    {
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0f;

        return (camForward * input.y + cam.transform.right * input.x).normalized;
    }
}

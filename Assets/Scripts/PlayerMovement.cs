using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController controller;

    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 4f;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isGrounded;
    bool isMoving;

    private Vector3 lastPosition = new Vector3(0f, 0f, 0f);

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }


    void Update()
    {
        // Ground check
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Resetting the default velocity
        //if (isGrounded && velocity.y < 0f)
        //{
        //    velocity.y = -2f;
        //}

        // Getting inputs
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        // Creating moving vector
        Vector3 move = transform.right * x + transform.forward * z; 

        // Actually move
        controller.Move(move * speed * Time.deltaTime);

        // Check if player can jump
        if (Input.GetButton("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(-2 * gravity * jumpHeight);
        }

        // Falling down
        velocity.y += gravity * Time.deltaTime;

        // Executing the jump
        controller.Move(velocity * Time.deltaTime);

        if (lastPosition != transform.position && isGrounded)
        {
            isMoving = true;
        }
        else
        {
            isMoving = false;
        }

        lastPosition = transform.position;

    }
}

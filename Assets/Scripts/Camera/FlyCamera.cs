using UnityEngine;

[RequireComponent(typeof(Camera))]
public class FlyCamera : MonoBehaviour
{
    public float acceleration = 50; // how fast you accelerate
    public float accSprintMultiplier = 4; // how much faster you go when "sprinting"
    public float lookSensitivity = 1; // mouse look sensitivity
    public float dampingCoefficient = 5; // how quickly you break to a halt after you stop your input
    public bool focusOnEnable = true; // whether or not to focus and lock cursor immediately on enable

    public KeyCode moveForward = KeyCode.Z;
    public KeyCode moveBack = KeyCode.S;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode moveLeft = KeyCode.Q;
    public KeyCode moveUp = KeyCode.Space;
    public KeyCode moveDown = KeyCode.LeftControl;
    public KeyCode moveSprint = KeyCode.LeftShift;


    private Vector3 velocity; // current velocity

    private static bool Focused
    {
        get => Cursor.lockState == CursorLockMode.Locked;
        set
        {
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = value == false;
        }
    }

    private void Update()
    {
        // Input
        if (Focused)
            UpdateInput();
        else if (Input.GetMouseButtonDown(0))
            Focused = true;

        // Physics
        velocity = Vector3.Lerp(velocity, Vector3.zero, dampingCoefficient * Time.deltaTime);
        transform.position += velocity * Time.deltaTime;
    }

    private void OnEnable()
    {
        if (focusOnEnable) Focused = true;
    }

    private void OnDisable()
    {
        Focused = false;
    }

    private void UpdateInput()
    {
        // Position
        velocity += GetAccelerationVector() * Time.deltaTime;

        // Rotation
        var mouseDelta = lookSensitivity * new Vector2(Input.GetAxis("Mouse X"), -Input.GetAxis("Mouse Y"));
        var rotation = transform.rotation;
        var horiz = Quaternion.AngleAxis(mouseDelta.x, Vector3.up);
        var vert = Quaternion.AngleAxis(mouseDelta.y, Vector3.right);
        transform.rotation = horiz * rotation * vert;

        // Leave cursor lock
        if (Input.GetKeyDown(KeyCode.Escape))
            Focused = false;
    }

    private Vector3 GetAccelerationVector()
    {
        Vector3 moveInput = default;

        void AddMovement(KeyCode key, Vector3 dir)
        {
            if (Input.GetKey(key))
                moveInput += dir;
        }

        AddMovement(moveForward, Vector3.forward);
        AddMovement(moveBack, Vector3.back);
        AddMovement(moveRight, Vector3.right);
        AddMovement(moveLeft, Vector3.left);
        AddMovement(moveUp, Vector3.up);
        AddMovement(moveDown, Vector3.down);
        var direction = transform.TransformVector(moveInput.normalized);

        if (Input.GetKey(moveSprint))
            return direction * (acceleration * accSprintMultiplier); // "sprinting"
        return direction * acceleration; // "walking"
    }
}
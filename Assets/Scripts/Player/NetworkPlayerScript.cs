using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerScript : NetworkBehaviour
{
    public float speed = 5f;
    public InputActionReference moveAction;
    public InputActionReference lookAction;
    public float mouseSensitivity=0.5f;
    public Camera playerCamera;

    private GameObject mainCamera;
    private GameObject lobbyCanvas;

    private CharacterController controller;
    private float pitch = 0f;

    private Animator animator;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        lobbyCanvas = GameObject.FindGameObjectWithTag("LobbyCanvas");
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (!IsOwner) return;

        Vector2 input = moveAction.action.ReadValue<Vector2>();
        Vector3 movement = new Vector3(input.x, 0, input.y).normalized;
        movement = transform.TransformDirection(movement);
        controller.Move(movement * speed * Time.deltaTime);

        animator.SetFloat("MoveX", input.x);
        animator.SetFloat("MoveY", input.y);

        HandleCameraLook();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            playerCamera.gameObject.SetActive(false);
            return;
        }

        mainCamera.SetActive(false);
        lobbyCanvas.SetActive(false);

        playerCamera.gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void HandleCameraLook()
    {
        Vector2 look = lookAction.action.ReadValue<Vector2>();

        float mouseX = look.x * mouseSensitivity;
        float mouseY = look.y * mouseSensitivity;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -80f, 80f);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    [Header("References")]
    public Transform pitchController;
    public Camera playerCamara;
    [Range(30, 120)] public float fieldOfView = 90f;
    public float mSmoothChange = 0.3f;
    private float runningFieldOfView = 60f;
    private CharacterController characterController;

    [Header("Movement")]
    public float minSpeed = 4;
    public float moveSpeed = 4;
    public float runSpeed = 7;
    public float jumpSpeed = 5;

    [Header("Rotation")]
    public float rotationSpeed = 5;
    public bool invertPitch;
    [Range(-89, 89)] public float mMinPitch = -89;
    [Range(-89, 89)] public float mMaxPitch = 89;

    private float _mYaw; //Izq derecha
    private float _mPitch; //Arriba abajo
    private Vector2 _mLookDirection;
    private Vector2 _mMoveDirection;
    private bool _mIsRunning = false;
    private float _mVerticalSpeed;
    private bool isGrounded = true;

    // Start is called before the first frame update
    void Start()
    {
        _mYaw = transform.rotation.y;
        _mPitch = pitchController.localRotation.x;

        Cursor.lockState = CursorLockMode.Locked;

        characterController = GetComponent<CharacterController>();

        SoundManager.Instance.PlayAmbience();
    }

    // Update is called once per frame
    void Update()
    {
        // ---------------------------------------Camara-----------------------------------------------------------------------
        // Movimiento rectilíneo uniforme: X = X0 + Velocidad * Tiempo;
        _mYaw += _mLookDirection.x * rotationSpeed * Time.deltaTime;
        _mPitch += _mLookDirection.y * rotationSpeed * Time.deltaTime;
        _mPitch = Math.Clamp(_mPitch, mMinPitch, mMaxPitch);

        // Rotación local para la cabeza y arma (por el controller), global para personaje entero
        transform.rotation = Quaternion.Euler(0.0f, _mYaw, 0.0f);
        pitchController.localRotation = Quaternion.Euler(_mPitch * (invertPitch ? -1.0f : 1.0f), 0.0f, 0.0f);

        // Field of view
        runningFieldOfView = (fieldOfView - 30f) < 0f ? 0f : fieldOfView - 30f;
        playerCamara.fieldOfView = fieldOfView;

        // --------------------------------------Movimiento--------------------------------------------------------------------
        Vector3 finalDirection = (transform.forward * _mMoveDirection.y + transform.right * _mMoveDirection.x) * moveSpeed * Time.deltaTime;
        finalDirection.y = 0;
        characterController.Move(finalDirection);

        // ----------------------------------------Salto-----------------------------------------------------------------------
        //_mVerticalSpeed += Physics.gravity.y * Time.deltaTime * 2;

        // nueva gravedad para que no caiga al instante
        if (isGrounded && _mVerticalSpeed < 0f)
        {
            _mVerticalSpeed = -2f; // fuerza hacia abajo para mantener pegado al suelo
        }
        _mVerticalSpeed += Physics.gravity.y * Time.deltaTime;

        Vector3 verticalMove = new Vector3(0, _mVerticalSpeed, 0) * Time.deltaTime;
        CollisionFlags flags = characterController.Move(verticalMove);

        isGrounded = (flags & CollisionFlags.CollidedBelow) != 0;


        // Convertir de velocidad vertical en desplazamiento vertical
        finalDirection.y = _mVerticalSpeed * Time.deltaTime;
        CollisionFlags collisionFlags = characterController.Move(finalDirection);
        isGrounded = (collisionFlags & CollisionFlags.CollidedBelow) != 0;
        if(isGrounded && _mVerticalSpeed > 0.0f)
        {
            _mVerticalSpeed = 0.0f;
        }

        // Speed and Speed POV
        // Lerp = interpolacion del punto entre dos limites
        if (_mIsRunning)
        {
            moveSpeed = runSpeed;
            playerCamara.fieldOfView = Mathf.Lerp(playerCamara.fieldOfView, runningFieldOfView, mSmoothChange);
        }
        else
        {
            moveSpeed = minSpeed;
            playerCamara.fieldOfView = Mathf.Lerp(runningFieldOfView, playerCamara.fieldOfView, mSmoothChange);
        }
    }

    // ----------------------------------Camara con OnLook----------------------------------------------
    // Para utilizar Invoke unity events y no Send messages en Player input
    public void OnLook(InputAction.CallbackContext context)
    {
        _mLookDirection = context.ReadValue<Vector2>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _mMoveDirection = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            _mIsRunning = true;
        }
        else if(context.canceled)
        {
            _mIsRunning = false;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (isGrounded && context.performed)
        {
            _mVerticalSpeed = jumpSpeed;
            isGrounded = false;
        }
    }

    public void SetYaw(float yaw)
    {
        _mYaw = yaw;
    }

    public void SetPitch(float pitch)
    {
        _mPitch = invertPitch ? -pitch : pitch;
    }

    public void ApplyRotationImmediate()
    {
        transform.rotation = Quaternion.Euler(0f, _mYaw, 0f);
        pitchController.localRotation =
            Quaternion.Euler(_mPitch, 0f, 0f);
    }


}

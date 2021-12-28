using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    public static readonly int IsWalkingHash = Animator.StringToHash("isWalking");
    public static readonly int IsRunningHash = Animator.StringToHash("isRunning");
    public static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    public static readonly int JumpCountHash = Animator.StringToHash("jumpCount");
    public static readonly int IsFallingHash = Animator.StringToHash("isFalling");

    // Reference variables
    PlayerInput _playerInput;
    CharacterController _characterController;
    Animator _animator;
    Camera _camera;

    // Player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isJumpPressed = false;

    // Constants
    [SerializeField] float _walkSpeed = 15f;
    [SerializeField] float _runSpeed = 30f;
    [SerializeField] float _midAirSpeed = 15f;
    float _groundedGravity = -1f;
    float _fallingGravity = -9.8f;
    float _rotationFactorPerFrame = 10.0f;

    // Jumping variables
    float _maxJumpHeight = 2.0f;
    float _maxJumpTime = 0.5f;
    float _jumpPressTimer = 0f;
    int _jumpCount = 0;
    int _maxJumpCount = 3;
    float[] _initiailsjumpVelocities;
    float[] _jumpGravities;
    Coroutine _currentJumpCountResetRoutine = null;

    PlayerBaseState _currentState;
    PlayerStateFactory _state;

    // Getters and Setters
    public CharacterController CharacterController => _characterController;
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public bool IsJumpPressed => _isJumpPressed;
    public bool IsMovementPressed => _isMovementPressed;
    public bool IsRunPressed => _isRunPressed;
    public Animator Animator => _animator;
    public Coroutine CurrentJumpCountResetRoutine { get { return _currentJumpCountResetRoutine; } set { _currentJumpCountResetRoutine = value; } }
    public float[] InitialjumpVelocities => _initiailsjumpVelocities;
    public float[] JumpGravities => _jumpGravities;
    public int JumpCount { get { return _jumpCount; } set { _jumpCount = value; } }
    public int MaxJumpCount => _maxJumpCount;
    public float JumpPressTimer => _jumpPressTimer;
    public float GroundedGravity => _groundedGravity;
    public float FallingGravity => _fallingGravity;
    public float WalkSpeed => _walkSpeed;
    public float RunSpeed => _runSpeed;
    public float MidAirSpeed => _midAirSpeed;
    public Vector3 CurrentMovement => _currentMovement;
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = Mathf.Max(value, _fallingGravity); } }
    public Vector3 AppliedMovementXZ 
    { 
        get { return new Vector3(_appliedMovement.x, 0, _appliedMovement.z); } 
        set { _appliedMovement = new Vector3(value.x, _appliedMovement.y, value.z); } }

    private void Awake()
    {
        // Set reference variables
        _playerInput = new PlayerInput();
        _characterController = this.GetComponent<CharacterController>();
        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;

        // Setup state
        this._state = new PlayerStateFactory(this);
        this._currentState = this._state.Grounded();
        this._currentState.EnterState();

        // Set the player input callbacks
        _playerInput.CharacterControls.Move.started += this.OnMovementInput;
        _playerInput.CharacterControls.Move.performed += this.OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += this.OnMovementInput;

        _playerInput.CharacterControls.Run.started += this.OnRun;
        _playerInput.CharacterControls.Run.canceled += this.OnRun;

        _playerInput.CharacterControls.Jump.started += this.OnJump;
        _playerInput.CharacterControls.Jump.canceled += this.OnJump;

        _initiailsjumpVelocities = new float[_maxJumpCount];
        _jumpGravities = new float[_maxJumpCount + 1];

        this.SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;

        for (int i = 0; i < _maxJumpCount; i++)
        {
            float jumpOffset = i * 1.1f;
            float timeOffset = 1 + i * .1f;

            _jumpGravities[i + 1] = (-2 * (_maxJumpHeight + jumpOffset)) / Mathf.Pow(timeToApex * timeOffset, 2);
            _initiailsjumpVelocities[i] = (2 * (_maxJumpHeight + jumpOffset)) / timeToApex * timeOffset;
        }

        _jumpGravities[0] = _jumpGravities[1];
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        _jumpPressTimer = Time.time;
    }

    void OnRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log($"_appliedMovement: {_appliedMovement}");
        this.HandleMovement();
        this.HandleRotation();
        _characterController.Move(_appliedMovement * Time.deltaTime);
        this._currentState.UpdateStates();
    }

    void HandleMovement()
    {
        Vector3 foward = _camera.transform.rotation * Vector3.forward;
        Vector3 right = _camera.transform.rotation * Vector3.right;

        foward.y = 0f;
        right.y = 0f;
        foward.Normalize();
        right.Normalize();

        _currentMovement = foward * _currentMovementInput.y + right * _currentMovementInput.x;
    }

    void HandleRotation()
    {
        Quaternion currentRotation = this.transform.rotation;

        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_currentMovement);

            this.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }

    }

    private void OnEnable() => _playerInput.CharacterControls.Enable();
    private void OnDisable() => _playerInput.CharacterControls.Disable();
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerContext : Context
{
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
    [SerializeField] float _walkSpeed = 6f;
    [SerializeField] float _runSpeed = 12f;
    //[SerializeField] float _midAirSpeed = 0f;
    [SerializeField] float _maxGravity = -15f;
    [SerializeField] float _groundedGravity = -0.1f;
    [SerializeField] float _fallingGravity = -9.8f;
    [SerializeField] float _rotationFactorPerFrame = 10.0f;

    // Jumping variables
    [SerializeField] float _maxJumpHeight = 2.0f;
    [SerializeField] float _maxJumpTime = 0.5f;
    [SerializeField] float _jumpHeightOffset = 1.1f;
    [SerializeField] float _jumpTimeOffset = .1f;

    float _jumpPressTimer = 0f;
    int _jumpCount = 0;
    int _maxJumpCount = 3;
    float[] _initiailsjumpVelocities;
    float[] _jumpGravities;
    Coroutine _currentJumpCountResetRoutine = null;

    PlayerStateFactory _stateFactory;

    public Text _superStateText;
    public Text _subStateText;

    // Getters and Setters
    public CharacterController CharacterController => _characterController;
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
    public Vector3 CurrentMovement => _currentMovement;
    public float AppliedMovementY { get { return _appliedMovement.y; } set { _appliedMovement.y = Mathf.Max(value, _maxGravity); } }
    public Vector3 AppliedMovementXZ { get { return new Vector3(_appliedMovement.x, 0, _appliedMovement.z); } set { _appliedMovement = new Vector3(value.x, _appliedMovement.y, value.z); } }

    private void Awake()
    {
        // Set reference variables
        _playerInput = new PlayerInput();
        _characterController = this.GetComponent<CharacterController>();
        _animator = this.GetComponent<Animator>();
        _camera = Camera.main;

        // Setup state
        _stateFactory = new PlayerStateFactory(this);
        this.CurrentState = this._stateFactory.Fall();
        this.CurrentState.EnterState();

        // Set the player input callbacks
        _playerInput.CharacterControls.Move.started += this.OnMovementInput;
        _playerInput.CharacterControls.Move.performed += this.OnMovementInput;
        _playerInput.CharacterControls.Move.canceled += this.OnMovementInput;

        _playerInput.CharacterControls.Run.started += this.OnRun;
        _playerInput.CharacterControls.Run.canceled += this.OnRun;

        _playerInput.CharacterControls.Jump.started += this.OnJump;
        _playerInput.CharacterControls.Jump.canceled += this.OnJump;

        _playerInput.CharacterControls.ResetJumpVariables.started += _ => this.SetupJumpVariables();

        _initiailsjumpVelocities = new float[_maxJumpCount];
        _jumpGravities = new float[_maxJumpCount + 1];

        this.SetupJumpVariables();
    }

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;

        for (int i = 0; i < _maxJumpCount; i++)
        {
            float jumpOffset = i * _jumpHeightOffset;
            float timeOffset = 1 + i * _jumpTimeOffset;

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
        this.CurrentState.UpdateStates();
        this.UpdateStateTexts();
    }

    void HandleMovement()
    {
        if (_isMovementPressed)
        {
            Vector3 foward = _camera.transform.rotation * Vector3.forward;
            Vector3 right = _camera.transform.rotation * Vector3.right;
            foward.y = 0f;
            right.y = 0f;

            foward.Normalize();
            right.Normalize();

            _currentMovement = foward * _currentMovementInput.y + right * _currentMovementInput.x;
        }
    }

    void HandleRotation()
    {
        if (_isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(_currentMovement);
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }

    }

    private void UpdateStateTexts()
    {
        (string subStateText, string superStateText) = this.CurrentState.GetStateTextValues();
        _superStateText.text = superStateText ?? "No Super State";
        _subStateText.text = subStateText ?? "No Sub State";
    }

    private void OnEnable() => _playerInput.CharacterControls.Enable();
    private void OnDisable() => _playerInput.CharacterControls.Disable();
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController), typeof(CharacterPhysics), typeof(Animator))]
public class PlayerContext : Context
{
    // Reference variables

    // Player input values
    Vector2 _currentMovementInput;
    Vector3 _currentMovement;
    Vector3 _appliedMovement;
    bool _isMovementPressed;
    bool _isRunPressed;
    bool _isJumpPressed;

    // Constants
    [SerializeField] float _walkSpeed = 6f;
    [SerializeField] float _runSpeed = 12f;
    [SerializeField] float _maxGravity = -15f;
    [SerializeField] float _groundedGravity = -0.1f;
    [SerializeField] float _fallingGravity = -9.8f;
    [SerializeField] float _rotationFactorPerFrame = 10.0f;

    // Jumping variables
    float _maxJumpHeight = 2.0f;
    float _maxJumpTime = 0.5f;
    float _jumpHeightOffset = 1.1f;
    float _jumpTimeOffset = .1f;
    float _jumpPressTimer = 0f;
    int _maxJumpCount = 3;
    float[] _initiailsjumpVelocities;
    float[] _jumpGravities;

    PlayerStateFactory _stateFactory;

    public Text _superStateText;
    public Text _subStateText;

    // Getters and Setters
    PlayerInput PlayerInput { get; set; }
    public CharacterController CharacterController { get; private set; }
    public CharacterPhysics CharacterPhysics { get; private set; }
    public Animator Animator { get; private set; }
    public bool IsJumpPressed => _isJumpPressed;
    public bool IsMovementPressed => _isMovementPressed;
    public bool IsRunPressed => _isRunPressed;
    public float[] InitialjumpVelocities => _initiailsjumpVelocities;
    public float[] JumpGravities => _jumpGravities;
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
        this.PlayerInput = new PlayerInput();
        this.CharacterController = this.GetComponent<CharacterController>();
        this.CharacterPhysics = this.GetComponent<CharacterPhysics>();
        this.Animator = this.GetComponent<Animator>();

        // Setup state
        _stateFactory = new PlayerStateFactory(this);
        this.CurrentState = this._stateFactory.GetState(PlayerState.Fall);
        this.CurrentState.EnterState();

        // Set the player input callbacks
        this.PlayerInput.CharacterControls.Move.started += this.OnMovementInput;
        this.PlayerInput.CharacterControls.Move.performed += this.OnMovementInput;
        this.PlayerInput.CharacterControls.Move.canceled += this.OnMovementInput;

        this.PlayerInput.CharacterControls.Run.started += this.OnRun;
        this.PlayerInput.CharacterControls.Run.canceled += this.OnRun;

        this.PlayerInput.CharacterControls.Jump.started += this.OnJump;
        this.PlayerInput.CharacterControls.Jump.canceled += this.OnJump;

        this.PlayerInput.CharacterControls.ResetJumpVariables.started += _ => this.SetupJumpVariables();

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
        this.CharacterPhysics.UpdatePhysics();
        // Debug.Log($"_appliedMovement: {_appliedMovement}");
        this.HandleMovement();
        this.HandleRotation();
        this.CurrentState.UpdateStates();
        this.CharacterController.Move(_appliedMovement * Time.deltaTime);
        this.UpdateStateTexts();
    }

    void HandleMovement()
    {
        if (_isMovementPressed)
        {
            Vector3 foward = Camera.main.transform.rotation * Vector3.forward;
            Vector3 right = Camera.main.transform.rotation * Vector3.right;
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

    private void OnEnable() => this.PlayerInput.CharacterControls.Enable();
    private void OnDisable() => this.PlayerInput.CharacterControls.Disable();
}

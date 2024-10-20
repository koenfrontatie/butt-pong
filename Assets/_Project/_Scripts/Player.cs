using KVDW.Extensions;
using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    public Action OnBallHit;
    public Action OnBallCatch;

    [SerializeField] private InputType _inputType;
    [SerializeField] private KeyCode _leftKey, _rightKey;
    [SerializeField] private float _speed = 9f;
    
    private PlayerState _playerState;
    private float[] _moveInput = new float[2];
    private readonly float[] _moveRange = { -5, 5 };
    private SerialReader _serialReader;
    private float _aimAngle;
    private Vector3 _aimDirection;
    private Transform _indicator, _arc;
    private Vector3 _initialPosition;

    private void Start()
    {
        _serialReader = FindObjectOfType<SerialReader>();
        _playerState = PlayerState.Normal;
        if (transform.Find("Indicator").TryGetComponent(out _indicator)) _indicator.gameObject.Disable();
        if (transform.Find("Arc").TryGetComponent(out _arc)) _indicator.gameObject.Disable();

        _initialPosition = transform.localPosition;
    }

    void Update()
    {
        HandleInput();
    }

    private void HandleInput()
    {
        var balance = GetInputBalanceValue();

        switch (_playerState)
        {
            case PlayerState.Normal:
                Vector3 movement = balance * transform.right;
                Vector3 newPosition = transform.localPosition + movement;
                float distanceFromStart = Vector3.Dot(newPosition - _initialPosition, transform.right);
                float clampedDistance = Mathf.Clamp(distanceFromStart, _moveRange[0], _moveRange[1]);
                transform.localPosition = _initialPosition + clampedDistance * transform.right;
                break;

            case PlayerState.Aiming:
                float angleAdjustment = balance.Remap(-1, 1, -_speed, _speed) * -5f;
                _aimAngle = Mathf.Clamp(_aimAngle + angleAdjustment, -45, 45);
                _aimDirection = Quaternion.Euler(0, 0, _aimAngle) * transform.up;

                if (_indicator) _indicator.localEulerAngles = new Vector3(0, 0, _aimAngle);
                break;
        }
    }
    private float GetInputBalanceValue()
    {
        switch (_inputType)
        {
            case InputType.Keyboard:
                _moveInput[0] = Input.GetKey(_leftKey) ? 1 : 0;
                _moveInput[1] = Input.GetKey(_rightKey) ? 1 : 0;
                break;

            case InputType.Balance:
                if (_serialReader == null) break;
                // will have to be updated to match the balance board and two ctrls
                _moveInput[0] = _serialReader.InputValues[0];
                _moveInput[1] = _serialReader.InputValues[1];
                break;
        }

        return (_moveInput[0] - _moveInput[1]) * Time.deltaTime * _speed;
    }

    public Vector3 GetAimDirection() => _aimDirection;
    public void SetAiming(bool aiming)
    {
        _aimAngle = 0;
        _playerState = aiming ? PlayerState.Aiming : PlayerState.Normal;
        if(_indicator) _indicator.gameObject.SetActive(aiming);
        if (_arc) _arc.gameObject.SetActive(aiming);
    }

    public float[] MoveInput
    {
        get { return _moveInput; }
    }

//#if UNITY_EDITOR
//    void OnDrawGizmos()
//    {
//        if (_playerState == PlayerState.Aiming)
//        {
//            Gizmos.color = Color.white;
//            Gizmos.DrawRay(transform.position, _aimDirection * 3f);
//        }
//    }
//#endif

}

public enum InputType
{
    Keyboard,
    Balance
}

public enum PlayerState
{
    Normal,
    Aiming
}

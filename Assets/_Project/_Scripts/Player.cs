using KVDW.Extensions;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private InputType _inputType;
    [SerializeField] private KeyCode _upKey, _downKey;
    [SerializeField] private float _speed = 9f;
    private static readonly float[] _HEIGHTRANGE = { -5, 5 };
    private SerialReader _serialReader;
    private float[] _moveInput = new float[2];
    private bool _aiming;
    private float _aimAngle;
    private float orientation;

    private void Start()
    {
        _serialReader = FindObjectOfType<SerialReader>();
        orientation = transform.position.GetAngle2D(Vector2.zero);
    }

    void Update()
    {
        HandleInput(GetInputBalanceValue());
    }

    float GetInputBalanceValue()
    {
        switch (_inputType)
        {
            case InputType.Keyboard:
                _moveInput[0] = Input.GetKey(_upKey) ? 1 : 0;
                _moveInput[1] = Input.GetKey(_downKey) ? 1 : 0;
                break;

            case InputType.Balance:
                if (_serialReader == null) break;

                _moveInput[0] = _serialReader.InputValues[0];
                _moveInput[1] = _serialReader.InputValues[1];
                break;
        }

        return (_moveInput[0] - _moveInput[1]) * Time.deltaTime * _speed;
    }

    void HandleInput(float balance)
    {
        if (!_aiming)
        {
            float newY = Mathf.Clamp(transform.position.y + balance, _HEIGHTRANGE[0], _HEIGHTRANGE[1]);
            transform.position = new Vector3(transform.position.x, newY);
        }
        else
        {
            _aimAngle = Mathf.Clamp(_aimAngle + balance.Remap(-1, 1, -_speed, _speed) * 2f, -45, 45);
            float angleMultiplier = transform.position.x < 0 ? 1f : -1f;
            transform.localRotation = Quaternion.Euler(0, 0, _aimAngle * angleMultiplier);
        }
    }

    public void SetAiming(bool aiming)
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        _aimAngle = 0;
        _aiming = aiming;
    }
}

public enum InputType
{
    Keyboard,
    Balance
}
//using KVDW.Extensions;
//using UnityEngine;

//public class Player: MonoBehaviour
//{    
//    [SerializeField] private InputType _inputType;
//    [SerializeField] PlayerIndex _playerIndex;
//    [SerializeField] private KeyCode _upKey, _downKey;
//    [SerializeField] private float _speed = 9f;
//    private static readonly float[] _HEIGHTRANGE = { -5, 5 };
//    private SerialReader _serialReader;
//    private float[] _moveInput = new float[2];
//    private bool _aiming;
//    private float _aimAngle;
//    private float orientation;

//    private void Start()
//    {
//        _serialReader = FindObjectOfType<SerialReader>();
//        orientation = transform.position.GetAngle2D(Vector2.zero);
//    }

//    void Update()
//    {
//        HandleInput(GetInputBalanceValue());
//    }

//    float GetInputBalanceValue()
//    {
//        switch (_inputType)
//        {
//            case InputType.Keyboard:
//                _moveInput[0] = Input.GetKey(_upKey) ? 1 : 0;
//                _moveInput[1] = Input.GetKey(_downKey) ? 1 : 0;
//                break;

//            case InputType.Balance:
//                if (_serialReader == null) break;

//                if (_playerIndex == PlayerIndex.Player1)
//                {
//                    _moveInput[0] = _serialReader.InputValues[0];
//                    _moveInput[1] = _serialReader.InputValues[1];
//                }
//                else
//                {
//                    _moveInput[0] = _serialReader.InputValues[2];
//                    _moveInput[1] = _serialReader.InputValues[3];
//                }
//                break;
//        }

//        return (_moveInput[0] - _moveInput[1]) * Time.deltaTime * _speed;
//    }
//    void HandleInput(float balance)
//    {
//        if (!_aiming)
//        {
//            float newY = Mathf.Clamp(transform.position.y + balance, _HEIGHTRANGE[0], _HEIGHTRANGE[1]);
//            transform.position = new Vector3(transform.position.x, newY);
//        }
//        else
//        {
//            _aimAngle = Mathf.Clamp(_aimAngle + balance.Remap(-1, 1, -_speed, _speed) * 2f, -45, 45);

//            if(_playerIndex == PlayerIndex.Player1)
//            {
//                transform.localRotation = Quaternion.Euler(0, 0, _aimAngle);
//            } else
//            {
//                transform.localRotation = Quaternion.Euler(0, 0, _aimAngle * -1f);
//            }
//        }
//    }

//    public void SetAiming(bool aiming)
//    {
//        transform.localRotation = Quaternion.Euler(Vector3.zero);
//        _aimAngle = 0;
//        _aiming = aiming;
//    }
//}

//public enum InputType
//{
//    Keyboard,
//    Balance
//}

//public enum PlayerIndex
//{
//    Player1,
//    Player2
//}

using UnityEngine;

public class Player: MonoBehaviour
{
    [SerializeField] private InputType _inputType;
    [SerializeField] PlayerIndex _playerIndex;
    [SerializeField] private KeyCode _upKey, _downKey;
    [SerializeField] private float _speed = 9f;
    private static readonly float[] _HEIGHTRANGE = { -5, 5 };
    private SerialReader _serialReader;
    private float[] _moveInput = new float[2];

    private void Start()
    {
        _serialReader = FindObjectOfType<SerialReader>();
    }

    void Update()
    {
        switch (_inputType)
        {
            case InputType.Keyboard:
                _moveInput[0] = Input.GetKey(_upKey) ? 1 : 0;
                _moveInput[1] = Input.GetKey(_downKey) ? 1 : 0;
                break;

            case InputType.Balance:
                if (_serialReader == null) break;

                if (_playerIndex == PlayerIndex.Player1)
                {
                    _moveInput[0] = _serialReader.InputValues[0];
                    _moveInput[1] = _serialReader.InputValues[1];
                }
                else
                {
                    _moveInput[0] = _serialReader.InputValues[2];
                    _moveInput[1] = _serialReader.InputValues[3];
                }
                break;
        }

        float balance = (_moveInput[0] - _moveInput[1]) * Time.deltaTime * _speed;
        float newY = Mathf.Clamp(transform.position.y + balance, _HEIGHTRANGE[0], _HEIGHTRANGE[1]);
        transform.position = new Vector3(transform.position.x, newY);
    }
}

public enum InputType
{
    Keyboard,
    Balance
}

public enum PlayerIndex
{
    Player1,
    Player2
}

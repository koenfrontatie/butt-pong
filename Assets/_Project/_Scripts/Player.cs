using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private KeyCode _upKey, _downKey;
    [SerializeField]
    private float _speed = 2f;

    private float[] _heightRange;
    private float _heightOffset;

    void Start()
    {
        _heightRange = new float[2];
        _heightRange[0] = 5;
        _heightRange[1] = -5;
    }

    void Update()
    {
        float newY = transform.position.y;

        var increment = _heightRange[1] - _heightRange[0] * .01f;

        if (Input.GetKey(_upKey))
        {
            newY = Mathf.Clamp(newY + _speed * Time.deltaTime, _heightRange[1], _heightRange[0]);
        }

        if (Input.GetKey(_downKey))
        {
            newY = Mathf.Clamp(newY - _speed * Time.deltaTime, _heightRange[1], _heightRange[0]);
        }

        transform.position = new Vector2(transform.position.x, newY);
    }
}

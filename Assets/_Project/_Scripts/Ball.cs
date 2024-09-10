using UnityEngine;
using System.Collections;
public class Ball : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;
    //private RectTransform _rectTransform;

    [SerializeField]
    private float _speed, _superSpeed;

    bool _superCharged;


    void Start()
    {
        //_rectTransform = GetComponent<RectTransform>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void ResetPosition()
    {
        transform.position = Vector2.zero;
        _rigidbody2D.velocity = Vector2.zero;

        if(GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

        var dir = Random.Range(0, 2);

        float randomAngle = Random.Range(-45f, 45f);
        float angleInRadians = randomAngle * Mathf.Deg2Rad;

        float xSpeed = _speed * Mathf.Cos(angleInRadians);
        float ySpeed = _speed * Mathf.Sin(angleInRadians);

        if (Random.Range(0, 2) == 0)
        {
            xSpeed = -xSpeed;
        }

        _rigidbody2D.velocity = new Vector2(xSpeed, ySpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.gameObject.name == "RightBound")
        {
            GameManager.Instance.OnPointScored(Vector2.right);
            ResetPosition();
        }

        if (collision.transform.gameObject.name == "LeftBound")
        {
            GameManager.Instance.OnPointScored(Vector2.up);
            ResetPosition();
        }

        if (collision.transform.gameObject.name == "ButtCenter")
        {
            //GameManager.Instance.OnPointScored(Vector2.up);
            //ResetPosition();

            if (collision.transform.parent.TryGetComponent<Player>(out var player))
            {
                _rigidbody2D.velocity = Vector2.zero;
                
                StartCoroutine(CatchAndShoot(player));

            }
        }
    }

    private IEnumerator CatchAndShoot(Player player)
    {
        _rigidbody2D.velocity = Vector2.zero;

        _superCharged = true;

        transform.parent = player.transform;

        bool isPlayer1 = player.transform.position.x < 0;

        if (isPlayer1)
        {
            transform.localPosition = Vector2.zero + Vector2.right * .5f;
        }
        else
        {
            transform.localPosition = Vector2.zero + Vector2.left * .5f;
        }

        yield return new WaitForSeconds(1f);

        transform.parent = player.transform.parent;

        if (isPlayer1)
        {
            _rigidbody2D.velocity = Vector2.right * _superSpeed;
        }
        else
        {
            _rigidbody2D.velocity = Vector2.left * _superSpeed;
        }
    }

    public void OnGameplayStart()
    {
        StartCoroutine(DelayedReset(1f));
    }
    private IEnumerator DelayedReset(float time)
    {
        yield return new WaitForSeconds(time);
        ResetPosition();
    }
}

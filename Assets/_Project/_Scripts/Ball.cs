using UnityEngine;

using System.Collections;
public class Ball : MonoBehaviour
{
    private Rigidbody2D _rigidbody2D;

    [SerializeField]
    private float _speed, _superSpeed;

    bool _superCharged;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    public void ResetPosition()
    {
        transform.position = Vector2.zero;
        _rigidbody2D.velocity = Vector2.zero;

        if (GameManager.Instance.State != GameState.Playing)
        {
            return;
        }

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
            var player = collision.transform.GetComponentInParent<Player>();
            if (player)
            {
                player.OnBallCatch?.Invoke();
                _rigidbody2D.velocity = Vector2.zero;
                StartCoroutine(CatchAndShoot(player));
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.gameObject.name == "Cheek")
        {
            var player = collision.transform.GetComponentInParent<Player>();
            if (player)
            {
                player.OnBallHit?.Invoke();
            }
        }
    }

    private IEnumerator CatchAndShoot(Player player)
    {
        player.SetAiming(true);

        _rigidbody2D.velocity = Vector2.zero;
        transform.parent = player.transform;
        transform.localPosition = new Vector3(0, 1.2f, 0);

        yield return new WaitForSeconds(1f);

        transform.parent = player.transform.parent;

        _rigidbody2D.velocity = _superSpeed * player.GetAimDirection();

        player.SetAiming(false);
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
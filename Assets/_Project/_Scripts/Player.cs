using UnityEngine;
using System;

public class Player : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private int playerNumber = 1;
    [SerializeField] private float speed = 4f;
    [SerializeField] private Vector2 moveRange = new Vector2(-5f, 5f);
    [SerializeField] private float inputDifferenceThreshold = 0.5f; // Minimum difference needed


    public Action OnBallHit;
    public Action OnBallCatch;

    private Vector3 initialPosition;
    private PlayerState playerState = PlayerState.Normal;
    private float aimAngle;
    private Vector3 aimDirection;
    private Vector2 currentInput;
    private Transform indicator;
    private Transform arc;
    private PlayerCalibration calibration;
    private SerialReader serialReader;

    public Vector2 MoveInput => currentInput; // For animator

    void Start()
    {
        initialPosition = transform.localPosition;
        calibration = GetComponent<PlayerCalibration>();
        serialReader = FindObjectOfType<SerialReader>();

        if (transform.Find("Indicator").TryGetComponent(out indicator))
            indicator.gameObject.SetActive(false);
        if (transform.Find("Arc").TryGetComponent(out arc))
            arc.gameObject.SetActive(false);
    }

    void Update()
    {
        // Calibration input check
        if ((playerNumber == 1 && Input.GetKeyDown(KeyCode.Alpha1)) ||
            (playerNumber == 2 && Input.GetKeyDown(KeyCode.Alpha2)))
        {
            calibration.StartCalibration();
            return;
        }

        // Get and process input
        if (serialReader.TryGetRawInput(playerNumber, out float leftValue, out float rightValue))
        {
            currentInput = calibration.GetCalibratedInput(new Vector2(leftValue, rightValue));

            if (playerState == PlayerState.Normal)
            {
                // Calculate difference between inputs
                float inputDifference;
                if (playerNumber == 1)
                {
                    inputDifference = currentInput.x - currentInput.y;
                }
                else // Player 2
                {
                    inputDifference = currentInput.y - currentInput.x;
                }

                // Only move if the difference is significant enough
                float movement = 0f;
                if (Mathf.Abs(inputDifference) > inputDifferenceThreshold)
                {
                    movement = inputDifference * speed * Time.deltaTime;
                }

                Vector3 newPosition = transform.localPosition + new Vector3(0, movement, 0);
                float distanceFromStart = newPosition.y - initialPosition.y;
                float clampedDistance = Mathf.Clamp(distanceFromStart, moveRange.x, moveRange.y);
                transform.localPosition = new Vector3(initialPosition.x, initialPosition.y + clampedDistance, initialPosition.z);
            }
        }
    }

    public void SetAiming(bool aiming)
    {
        aimAngle = 0;
        playerState = aiming ? PlayerState.Aiming : PlayerState.Normal;

        if (indicator)
            indicator.gameObject.SetActive(aiming);
        if (arc)
            arc.gameObject.SetActive(aiming);
    }

    public Vector3 GetAimDirection() => aimDirection;
}

public enum PlayerState
{
    Normal,
    Aiming
}
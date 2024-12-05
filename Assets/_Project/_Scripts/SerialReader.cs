using UnityEngine;
using System;

public class SerialReader : MonoBehaviour
{
    public static SerialReader Instance { get; private set; }

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = true;  // Set to true by default for debugging

    private UnitySerialPort _serialPort;
    private float[] InputValues = new float[4];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (TryGetComponent(out _serialPort))
        {
            UnitySerialPort.SerialDataParseEvent += OnSerialDataParse;
            Debug.Log("SerialReader initialized successfully");
        }
        else
        {
            Debug.LogError("No SerialPort component found!");
        }
    }

    void OnDestroy()
    {
        if (_serialPort != null)
        {
            UnitySerialPort.SerialDataParseEvent -= OnSerialDataParse;
        }
    }

    void OnSerialDataParse(string[] data, string rawData)
    {
        if (data == null || data.Length == 0)
        {
            Debug.LogWarning("Received null or empty data");
            return;
        }

        Debug.Log($"Raw serial data received: {rawData}"); // Log raw data
        string pData = data[0].Trim();
        Debug.Log($"Processed data string: {pData}"); // Log processed string

        try
        {
            string[] values = pData.Split('.');
            Debug.Log($"Split values count: {values.Length}"); // Log number of values

            // Log each value before parsing
            for (int i = 0; i < values.Length; i++)
            {
                Debug.Log($"Value {i}: {values[i]}");
            }

            // Parse all values (up to 4)
            for (int i = 0; i < Mathf.Min(values.Length, 4); i++)
            {
                if (float.TryParse(values[i], out float value))
                {
                    InputValues[i] = value;
                    Debug.Log($"Successfully parsed value {i}: {value}");
                }
                else
                {
                    Debug.LogWarning($"Failed to parse value {i}: {values[i]}");
                }
            }

            // Log current state of InputValues
            Debug.Log($"Current InputValues: P1L={InputValues[0]}, P1R={InputValues[1]}, P2L={InputValues[2]}, P2R={InputValues[3]}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error parsing serial data: {ex.Message}");
        }
    }

    public bool TryGetRawInput(out float leftValue, out float rightValue)
    {
        leftValue = 0f;
        rightValue = 0f;

        if (InputValues.Length < 2)
        {
            Debug.LogWarning("InputValues array too small");
            return false;
        }

        leftValue = InputValues[0];
        rightValue = InputValues[1];

        if (showDebugLog)
        {
            Debug.Log($"Getting P1 input - Left: {leftValue}, Right: {rightValue}");
        }

        return true;
    }

    public bool TryGetRawInput(int playerIndex, out float leftValue, out float rightValue)
    {
        leftValue = 0f;
        rightValue = 0f;

        int startIndex = (playerIndex - 1) * 2;
        if (startIndex < 0 || startIndex + 1 >= InputValues.Length)
        {
            Debug.LogWarning($"Invalid player index: {playerIndex}");
            return false;
        }

        leftValue = InputValues[startIndex];
        rightValue = InputValues[startIndex + 1];

        if (showDebugLog)
        {
            Debug.Log($"Getting P{playerIndex} input - Left: {leftValue}, Right: {rightValue}");
        }

        return true;
    }

    // Helper method to see all values in the Inspector
    void OnGUI()
    {
        if (!showDebugLog) return;

        GUILayout.BeginArea(new Rect(10, 10, 300, 100));
        GUILayout.Label("Serial Input Values:");
        GUILayout.Label($"P1: L={InputValues[0]:F1}, R={InputValues[1]:F1}");
        GUILayout.Label($"P2: L={InputValues[2]:F1}, R={InputValues[3]:F1}");
        GUILayout.EndArea();
    }
}
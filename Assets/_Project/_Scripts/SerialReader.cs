//using System;
//using System.Net;
//using UnityEngine;

//public class SerialReader : MonoBehaviour
//{
//    private UnitySerialPort _serialPort;

//    // Booleans representing serial inputs
//    public bool one, two, three, four;

//    public Player Player1, Player2;

//    void Start()
//    {
//        if (transform.TryGetComponent(out _serialPort))
//        {
//            UnitySerialPort.SerialDataParseEvent += OnSerialDataParse;
//        }
//    }

//    void Update()
//    {
//        if(one)
//        {
//            Player1.MoveUp();

//            if (GameManager.Instance.State != GameState.Playing) GameManager.Instance.UpdateGameState(GetNextEnumValue(GameManager.Instance.State));
//        }

//        if(two)
//        {
//            Player1.MoveDown();

//            if (GameManager.Instance.State != GameState.Playing) GameManager.Instance.UpdateGameState(GetNextEnumValue(GameManager.Instance.State));
//        }

//        if(three)
//        {
//            Player2.MoveUp();

//            if (GameManager.Instance.State != GameState.Playing) GameManager.Instance.UpdateGameState(GetNextEnumValue(GameManager.Instance.State));
//        }

//        if(four)
//        {
//            Player2.MoveDown();

//            if (GameManager.Instance.State != GameState.Playing) GameManager.Instance.UpdateGameState(GetNextEnumValue(GameManager.Instance.State));
//        }
//    }

//    void OnDestroy()
//    {
//        if (_serialPort != null)
//        {
//            UnitySerialPort.SerialDataParseEvent -= OnSerialDataParse;
//        }
//    }

//    void OnSerialDataParse(string[] data, string rawData)
//    {
//        // Check if data contains at least one element to avoid out of bounds errors
//        if (data == null || data.Length == 0) return;

//        // Trim the data to remove any unwanted whitespace or newline characters
//        string pData = data[0].Trim();

//        // Process the parsed serial data

//        switch (pData)
//        {
//            case "1":
//                one = true;
//                break;
//            case "2":
//                two = true;
//                break;
//            case "3":
//                three = true;
//                break;
//            case "4":
//                four = true;
//                break;
//            case "x1":
//                one = false;
//                break;
//            case "x2":
//                two = false;
//                break;
//            case "x3":
//                three = false;
//                break;
//            case "x4":
//                four = false;
//                break;
//            default:
//                Debug.LogWarning($"Unexpected data received: {pData}");
//                break;
//        }
//    }

//    public static Enum GetNextEnumValue(Enum currentValue)
//    {
//        // Get the values of the enum
//        MyEnum[] enumValues = (MyEnum[])Enum.GetValues(typeof(MyEnum));

//        // Get the index of the current value
//        int currentIndex = Array.IndexOf(enumValues, currentValue);

//        // Calculate the next index using modulo
//        int nextIndex = (currentIndex + 1) % enumValues.Length;

//        // Return the next enum value
//        return enumValues[nextIndex];
//    }

//}

using System;
//using System.Diagnostics;
using UnityEngine;

public class SerialReader : MonoBehaviour
{
    private UnitySerialPort _serialPort;

    // Booleans representing serial inputs
    public bool one, two, three, four;

    public Player Player1, Player2;

    float menuTimer;

    void Start()
    {
        if (transform.TryGetComponent(out _serialPort))
        {
            UnitySerialPort.SerialDataParseEvent += OnSerialDataParse;
        }
    }

    void Update()
    {
        if (one)
        {
            Player1.MoveUp();

            MenuBehaviour();
        }

        if (two)
        {
            Player1.MoveDown();

            MenuBehaviour();
        }

        if (three)
        {
            Player2.MoveUp();

            MenuBehaviour();
        }

        if (four)
        {
            Player2.MoveDown();

            MenuBehaviour();
        }
    }

    void MenuBehaviour()
    {
        if (GameManager.Instance.State != GameState.Playing && Time.time > menuTimer + 1f)
        {
            // Get the next enum value
            GameManager.Instance.UpdateGameState(GetNextEnumValue(GameManager.Instance.State));
        }

        if (GameManager.Instance.State == GameState.Playing)
        {
            menuTimer = Time.time;
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
        if (data == null || data.Length == 0) return;

        string pData = data[0].Trim();

        switch (pData)
        {
            case "1":
                one = true;
                break;
            case "2":
                two = true;
                break;
            case "3":
                three = true;
                break;
            case "4":
                four = true;
                break;
            case "x1":
                one = false;
                break;
            case "x2":
                two = false;
                break;
            case "x3":
                three = false;
                break;
            case "x4":
                four = false;
                break;
            default:
                Debug.LogWarning($"Unexpected data received: {pData}");
                break;
        }
    }

    // Corrected GetNextEnumValue method for GameState
    public static GameState GetNextEnumValue(GameState currentValue)
    {
        // Get the values of the GameState enum
        GameState[] enumValues = (GameState[])Enum.GetValues(typeof(GameState));

        // Get the index of the current value
        int currentIndex = Array.IndexOf(enumValues, currentValue);

        // Calculate the next index using modulo to loop back
        int nextIndex = (currentIndex + 1) % enumValues.Length;

        // Return the next GameState value
        return enumValues[nextIndex];
    }
}

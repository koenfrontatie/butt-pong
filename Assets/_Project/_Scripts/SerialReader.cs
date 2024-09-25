using UnityEngine;

public class SerialReader : MonoBehaviour
{
    private UnitySerialPort _serialPort;

    // Booleans representing serial inputs
    public bool one, two, three, four;

    void Start()
    {
        if (transform.TryGetComponent(out _serialPort))
        {
            UnitySerialPort.SerialDataParseEvent += OnSerialDataParse;
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
        // Check if data contains at least one element to avoid out of bounds errors
        if (data == null || data.Length == 0) return;

        // Trim the data to remove any unwanted whitespace or newline characters
        string pData = data[0].Trim();

        // Process the parsed serial data
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
            case "-1":
                one = false;
                break;
            case "-2":
                two = false;
                break;
            case "-3":
                three = false;
                break;
            case "-4":
                four = false;
                break;
            default:
                Debug.LogWarning($"Unexpected data received: {pData}");
                break;
        }
    }

}


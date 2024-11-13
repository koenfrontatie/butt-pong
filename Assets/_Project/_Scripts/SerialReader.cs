using System;
using UnityEngine;
using KVDW.Extensions;
using System.Collections.Generic;
using System.Linq;

public class SerialReader : MonoBehaviour
{
    public float[] InputValues = new float[4];
    private UnitySerialPort _serialPort;
    
    private float[] _serialInputValues = new float[4];
    private float[] _smoothedValues = new float[4];
    public float _minimumCompensation = 200f;

    private List<float[]> _inputsToAverage = new List<float[]>();
    private int maxAverageValues = 5;

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

    private float[] AddValueGetAverage(float[] newValue)
    {
        // Ensure newValue has exactly 4 elements by padding with default values if necessary
        if (newValue.Length < 4)
        {
            Array.Resize(ref newValue, 4);
        }

        _inputsToAverage.Add(newValue);

        // Maintain only the last maxAverageValues entries
        if (_inputsToAverage.Count > maxAverageValues)
        {
            _inputsToAverage.RemoveAt(0);
        }

        var averagedInput = new float[4];

        // Calculate the average for each of the four elements
        for (int i = 0; i < 4; i++)
        {
            averagedInput[i] = _inputsToAverage
                .Where(values => values.Length > i) // Only consider arrays that have a value at this index
                .Select(values => values[i])
                .Average();
        }

        return averagedInput;
    }

    void OnSerialDataParse(string[] data, string rawData)
    {
        if (data == null || data.Length == 0) return;

        string pData = data[0].Trim();

        try
        {
            // Split and parse input data, handling potential format issues
            string[] splitData = pData.Split('.');
            List<float> parsedValues = new List<float>();

            foreach (string value in splitData)
            {
                if (float.TryParse(value, out float parsedValue))
                {
                    parsedValues.Add(parsedValue);
                }
                else
                {
                    //Debug.Log($"Warning: Unable to parse '{value}' as a float.");
                    //parsedValues.Add(0f); // Add default value if parsing fails
                }
            }

            _serialInputValues = parsedValues.ToArray();
            if (_serialInputValues.Length < 4)
            {
                Array.Resize(ref _serialInputValues, 4); // Pad with 0s if fewer than 4 values
            }

            _smoothedValues = AddValueGetAverage(_serialInputValues);

            for (int i = 0; i < _smoothedValues.Length && i < InputValues.Length; i++)
            {
                InputValues[i] = _smoothedValues[i].Remap(0, 1023, 0, 1);
            }
        }
        catch (Exception ex)
        {
            //Debug.Log($"Error parsing serial data: {ex.Message}");
        }
    }
}

using System;
using UnityEngine;
using KVDW.Extensions;

public class SerialReader : MonoBehaviour
{
    public float[] InputValues = new float[4];
    private UnitySerialPort _serialPort;
    private float[] _serialInputValues = new float[4];
    private float[] _serialInputCeilings = new float[] { 1f, 1f, 1f, 1f };

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
        if (data == null || data.Length == 0) return;

        string pData = data[0].Trim();

        float[] parsedValues = Array.ConvertAll(pData.Split('.'), float.Parse);

        for (int i = 0; i < parsedValues.Length && i < InputValues.Length; i++)
        {
            _serialInputCeilings[i] = parsedValues[i] > _serialInputCeilings[i] ? parsedValues[i] : _serialInputCeilings[i];

            // this normalizes the input values to a range of 0 to 1
            InputValues[i] = parsedValues[i].Remap(0, _serialInputCeilings[i], 0, 1);
        }
    }
}

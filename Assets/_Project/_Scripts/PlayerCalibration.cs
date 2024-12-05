using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class PlayerCalibration : MonoBehaviour
{
    [Header("Calibration Settings")]
    [SerializeField] private float neutralDuration = 3f;
    [SerializeField] private float maxPressureDuration = 3f;
    [SerializeField] private TextMeshProUGUI instructionText;

    [Header("Default Calibration Values")]
    [SerializeField] private float defaultNeutralPressure = 150f;
    [SerializeField] private float defaultMaxPressure = 750f;
    [SerializeField] private bool useDefaultCalibration = true;

    private enum CalibrationPhase
    {
        None,
        NeutralPosition,
        MaximumPressure
    }

    private Vector2 neutralPressure;
    private Vector2 maxPressure;
    private CalibrationPhase currentPhase;
    private float phaseTimer;
    private List<Vector2> calibrationSamples = new List<Vector2>();
    private bool hasCustomCalibration;

    private void Start()
    {
        if (useDefaultCalibration)
        {
            SetDefaultCalibration();
        }
    }

    private void SetDefaultCalibration()
    {
        neutralPressure = new Vector2(defaultNeutralPressure, defaultNeutralPressure);
        maxPressure = new Vector2(defaultMaxPressure, defaultMaxPressure);
        hasCustomCalibration = false;
        Debug.Log($"Using default calibration - Neutral: {defaultNeutralPressure}, Max: {defaultMaxPressure}");
    }

    public void StartCalibration()
    {
        Debug.Log("Starting Neutral Position Calibration...");
        currentPhase = CalibrationPhase.NeutralPosition;
        phaseTimer = neutralDuration;
        calibrationSamples.Clear();

        if (instructionText)
        {
            instructionText.gameObject.SetActive(true);
            instructionText.text = "Stand normally...";
        }
    }

    void Update()
    {
        if (currentPhase == CalibrationPhase.None) return;

        phaseTimer -= Time.deltaTime;

        if (SerialReader.Instance.TryGetRawInput(out float leftValue, out float rightValue))
        {
            calibrationSamples.Add(new Vector2(leftValue, rightValue));
        }

        UpdateCalibrationUI();

        if (phaseTimer <= 0)
        {
            CompleteCurrentPhase();
        }
    }

    void UpdateCalibrationUI()
    {
        if (!instructionText) return;

        switch (currentPhase)
        {
            case CalibrationPhase.NeutralPosition:
                instructionText.text = $"Stand normally... {Mathf.Ceil(phaseTimer)}";
                break;
            case CalibrationPhase.MaximumPressure:
                instructionText.text = $"Lean with maximum comfortable pressure... {Mathf.Ceil(phaseTimer)}";
                break;
        }
    }

    void CompleteCurrentPhase()
    {
        if (calibrationSamples.Count == 0)
        {
            Debug.LogWarning("No samples collected!");
            return;
        }

        switch (currentPhase)
        {
            case CalibrationPhase.NeutralPosition:
                neutralPressure = new Vector2(
                    calibrationSamples.Average(s => s.x),
                    calibrationSamples.Average(s => s.y)
                );

                currentPhase = CalibrationPhase.MaximumPressure;
                phaseTimer = maxPressureDuration;
                calibrationSamples.Clear();

                if (instructionText)
                    instructionText.text = "Now lean with maximum comfortable pressure";
                break;

            case CalibrationPhase.MaximumPressure:
                maxPressure = new Vector2(
                    calibrationSamples.Max(s => s.x),
                    calibrationSamples.Max(s => s.y)
                );

                CompleteCalibration();
                hasCustomCalibration = true;
                break;
        }
    }

    void CompleteCalibration()
    {
        Debug.Log($"Calibration complete!\nNeutral: {neutralPressure}\nMax: {maxPressure}");
        currentPhase = CalibrationPhase.None;
        calibrationSamples.Clear();

        if (instructionText)
        {
            instructionText.text = "Calibration complete!";
            Invoke("HideText", 2f);
        }
    }

    public Vector2 GetCalibratedInput(Vector2 rawInput)
    {
        if (!hasCustomCalibration && useDefaultCalibration)
        {
            // Simple linear mapping for default calibration
            return new Vector2(
                Mathf.InverseLerp(defaultNeutralPressure, defaultMaxPressure, rawInput.x),
                Mathf.InverseLerp(defaultNeutralPressure, defaultMaxPressure, rawInput.y)
            );
        }

        // Custom calibration mapping
        float normalizedLeft = Mathf.InverseLerp(
            neutralPressure.x,
            maxPressure.x,
            rawInput.x
        );

        float normalizedRight = Mathf.InverseLerp(
            neutralPressure.y,
            maxPressure.y,
            rawInput.y
        );

        return new Vector2(
            Mathf.Clamp01(normalizedLeft),
            Mathf.Clamp01(normalizedRight)
        );
    }

    void HideText()
    {
        if (instructionText)
            instructionText.gameObject.SetActive(false);
    }

    // Optional: Method to reset to default calibration
    public void ResetToDefaultCalibration()
    {
        SetDefaultCalibration();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        // Simple debug display in editor
        if (UnityEngine.Input.GetKey(KeyCode.Tab))
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 100));
            GUILayout.Label($"Using {(hasCustomCalibration ? "Custom" : "Default")} Calibration");
            GUILayout.Label($"Neutral: {neutralPressure}");
            GUILayout.Label($"Max: {maxPressure}");
            GUILayout.EndArea();
        }
    }
#endif
}
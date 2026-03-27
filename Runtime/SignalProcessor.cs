using System;
using System.Collections.Generic;
using UnityEngine;

namespace BrainFlowBiosignals
{
    /// <summary>
    /// Processes raw biosignal data into usable normalized values.
    /// Handles smoothing, thresholding, and calibration.
    /// </summary>
    public class SignalProcessor : MonoBehaviour
    {
        [Header("Normalization")]
        [Tooltip("Minimum expected raw signal value")]
        [SerializeField] private float minRawValue = 0f;
        
        [Tooltip("Maximum expected raw signal value")]
        [SerializeField] private float maxRawValue = 1000f;

        [Header("Smoothing")]
        [Tooltip("Number of samples to average for smoothing")]
        [SerializeField] private int smoothingWindowSize = 5;

        [Header("Threshold")]
        [Tooltip("Minimum normalized value to register as 'active'")]
        [SerializeField] private float activationThreshold = 0.3f;

        [Header("Calibration")]
        [SerializeField] private bool autoCalibrate = true;
        [SerializeField] private float calibrationDuration = 3f;

        public float NormalizedValue { get; private set; }
        public float SmoothedValue { get; private set; }
        public bool IsActive => SmoothedValue >= activationThreshold;
        public float ActivationStrength => Mathf.Clamp01((SmoothedValue - activationThreshold) / (1f - activationThreshold));

        public event Action OnActivated;
        public event Action OnDeactivated;

        private Queue<float> smoothingBuffer;
        private bool wasActive;
        private bool isCalibrating;
        private float calibrationStartTime;
        private float calibrationMin = float.MaxValue;
        private float calibrationMax = float.MinValue;

        private void Awake()
        {
            smoothingBuffer = new Queue<float>(smoothingWindowSize);
        }

        /// <summary>
        /// Process a raw signal value and update normalized/smoothed outputs.
        /// </summary>
        public void ProcessSignal(float rawValue)
        {
            // Handle calibration
            if (isCalibrating)
            {
                UpdateCalibration(rawValue);
            }

            // Normalize
            NormalizedValue = Mathf.InverseLerp(minRawValue, maxRawValue, Mathf.Abs(rawValue));

            // Smooth
            smoothingBuffer.Enqueue(NormalizedValue);
            if (smoothingBuffer.Count > smoothingWindowSize)
            {
                smoothingBuffer.Dequeue();
            }

            float sum = 0f;
            foreach (float val in smoothingBuffer)
            {
                sum += val;
            }
            SmoothedValue = sum / smoothingBuffer.Count;

            // Check activation state changes
            bool isNowActive = IsActive;
            if (isNowActive && !wasActive)
            {
                OnActivated?.Invoke();
            }
            else if (!isNowActive && wasActive)
            {
                OnDeactivated?.Invoke();
            }
            wasActive = isNowActive;
        }

        /// <summary>
        /// Start calibration to auto-detect min/max signal range.
        /// </summary>
        public void StartCalibration()
        {
            isCalibrating = true;
            calibrationStartTime = Time.time;
            calibrationMin = float.MaxValue;
            calibrationMax = float.MinValue;
            Debug.Log("[SignalProcessor] Calibration started. Flex and relax your muscle...");
        }

        private void UpdateCalibration(float rawValue)
        {
            float absValue = Mathf.Abs(rawValue);
            calibrationMin = Mathf.Min(calibrationMin, absValue);
            calibrationMax = Mathf.Max(calibrationMax, absValue);

            if (Time.time - calibrationStartTime >= calibrationDuration)
            {
                FinishCalibration();
            }
        }

        private void FinishCalibration()
        {
            isCalibrating = false;
            
            // Add some margin
            float range = calibrationMax - calibrationMin;
            minRawValue = calibrationMin - (range * 0.1f);
            maxRawValue = calibrationMax + (range * 0.1f);
            
            Debug.Log($"[SignalProcessor] Calibration complete. Range: {minRawValue:F2} - {maxRawValue:F2}");
        }

        /// <summary>
        /// Manually set the signal range for normalization.
        /// </summary>
        public void SetRange(float min, float max)
        {
            minRawValue = min;
            maxRawValue = max;
        }

        /// <summary>
        /// Set the activation threshold (0-1).
        /// </summary>
        public void SetThreshold(float threshold)
        {
            activationThreshold = Mathf.Clamp01(threshold);
        }

        /// <summary>
        /// Clear the smoothing buffer.
        /// </summary>
        public void Reset()
        {
            smoothingBuffer.Clear();
            NormalizedValue = 0f;
            SmoothedValue = 0f;
            wasActive = false;
        }
    }
}

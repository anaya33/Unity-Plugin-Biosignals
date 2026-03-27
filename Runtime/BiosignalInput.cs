using System;
using UnityEngine;

namespace BrainFlowBiosignals
{
    /// <summary>
    /// High-level input abstraction for biosignals.
    /// Use this like you would Input.GetAxis() or Input.GetButton().
    /// </summary>
    public class BiosignalInput : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BrainFlowManager brainFlowManager;
        [SerializeField] private SignalProcessor signalProcessor;

        [Header("Channel Selection")]
        [Tooltip("Which EMG channel to use (0-indexed)")]
        [SerializeField] private int emgChannelIndex = 0;

        [Header("Auto Start")]
        [SerializeField] private bool startStreamingOnEnable = true;
        [SerializeField] private bool calibrateOnStart = false;

        /// <summary>
        /// Current EMG value normalized 0-1 (smoothed).
        /// </summary>
        public float EMGValue => signalProcessor != null ? signalProcessor.SmoothedValue : 0f;

        /// <summary>
        /// Raw normalized value before smoothing.
        /// </summary>
        public float EMGValueRaw => signalProcessor != null ? signalProcessor.NormalizedValue : 0f;

        /// <summary>
        /// True if EMG exceeds activation threshold.
        /// </summary>
        public bool IsFlexing => signalProcessor != null && signalProcessor.IsActive;

        /// <summary>
        /// Strength of flex above threshold (0-1).
        /// </summary>
        public float FlexStrength => signalProcessor != null ? signalProcessor.ActivationStrength : 0f;

        /// <summary>
        /// Fired when muscle activation begins.
        /// </summary>
        public event Action OnFlexStart;

        /// <summary>
        /// Fired when muscle activation ends.
        /// </summary>
        public event Action OnFlexEnd;

        private void OnEnable()
        {
            if (brainFlowManager == null)
            {
                brainFlowManager = FindObjectOfType<BrainFlowManager>();
            }

            if (signalProcessor == null)
            {
                signalProcessor = GetComponent<SignalProcessor>();
                if (signalProcessor == null)
                {
                    signalProcessor = gameObject.AddComponent<SignalProcessor>();
                }
            }

            if (brainFlowManager != null)
            {
                brainFlowManager.OnEMGDataReceived += HandleEMGData;
            }

            if (signalProcessor != null)
            {
                signalProcessor.OnActivated += HandleActivated;
                signalProcessor.OnDeactivated += HandleDeactivated;
            }

            if (startStreamingOnEnable && brainFlowManager != null)
            {
                brainFlowManager.StartStreaming();
            }

            if (calibrateOnStart && signalProcessor != null)
            {
                signalProcessor.StartCalibration();
            }
        }

        private void OnDisable()
        {
            if (brainFlowManager != null)
            {
                brainFlowManager.OnEMGDataReceived -= HandleEMGData;
            }

            if (signalProcessor != null)
            {
                signalProcessor.OnActivated -= HandleActivated;
                signalProcessor.OnDeactivated -= HandleDeactivated;
            }
        }

        private void HandleEMGData(float[] emgValues)
        {
            if (emgValues.Length == 0) return;
            
            int index = Mathf.Clamp(emgChannelIndex, 0, emgValues.Length - 1);
            signalProcessor.ProcessSignal(emgValues[index]);
        }

        private void HandleActivated()
        {
            OnFlexStart?.Invoke();
        }

        private void HandleDeactivated()
        {
            OnFlexEnd?.Invoke();
        }

        /// <summary>
        /// Start calibration for the signal processor.
        /// </summary>
        public void Calibrate()
        {
            signalProcessor?.StartCalibration();
        }
    }
}

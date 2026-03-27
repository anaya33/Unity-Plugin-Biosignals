using System;
using System.Threading;
using UnityEngine;
using brainflow;
using brainflow.math;

namespace BrainFlowBiosignals
{
    /// <summary>
    /// Core manager for BrainFlow board connection and data streaming.
    /// Attach to a GameObject in your scene to start receiving biosignal data.
    /// </summary>
    public class BrainFlowManager : MonoBehaviour
    {
        [Header("Board Configuration")]
        [Tooltip("BrainFlow board ID. Use -1 for Synthetic board (testing), 0 for Cyton, etc.")]
        [SerializeField] private int boardId = (int)BoardIds.SYNTHETIC_BOARD;
        
        [Tooltip("Serial port for the board (e.g., COM3 on Windows, /dev/ttyUSB0 on Linux)")]
        [SerializeField] private string serialPort = "";
        
        [Header("Streaming Settings")]
        [Tooltip("How often to read data from the board (in seconds)")]
        [SerializeField] private float readInterval = 0.05f;
        
        [Header("Debug")]
        [SerializeField] private bool logDataToConsole = true;

        public event Action<double[,]> OnDataReceived;
        public event Action<float[]> OnEMGDataReceived;
        
        public bool IsStreaming { get; private set; }
        public int SamplingRate { get; private set; }
        public int[] EMGChannels { get; private set; }

        private BoardShim boardShim;
        private float lastReadTime;
        private bool isInitialized;

        private void Start()
        {
            InitializeBoard();
        }

        private void Update()
        {
            if (!IsStreaming) return;
            
            if (Time.time - lastReadTime >= readInterval)
            {
                ReadData();
                lastReadTime = Time.time;
            }
        }

        private void OnDestroy()
        {
            StopStreaming();
            ReleaseBoard();
        }

        /// <summary>
        /// Initialize the BrainFlow board connection.
        /// </summary>
        public void InitializeBoard()
        {
            if (isInitialized) return;

            try
            {
                BoardShim.enable_dev_board_logger();
                
                BrainFlowInputParams inputParams = new BrainFlowInputParams();
                if (!string.IsNullOrEmpty(serialPort))
                {
                    inputParams.serial_port = serialPort;
                }

                boardShim = new BoardShim(boardId, inputParams);
                boardShim.prepare_session();
                
                SamplingRate = BoardShim.get_sampling_rate(boardId);
                EMGChannels = BoardShim.get_emg_channels(boardId);
                
                isInitialized = true;
                Debug.Log($"[BrainFlowManager] Board initialized. ID: {boardId}, Sampling Rate: {SamplingRate}Hz");
                
                if (EMGChannels.Length > 0)
                {
                    Debug.Log($"[BrainFlowManager] EMG Channels: {string.Join(", ", EMGChannels)}");
                }
            }
            catch (BrainFlowException e)
            {
                Debug.LogError($"[BrainFlowManager] Failed to initialize board: {e.Message}");
            }
        }

        /// <summary>
        /// Start streaming data from the board.
        /// </summary>
        public void StartStreaming()
        {
            if (!isInitialized)
            {
                Debug.LogError("[BrainFlowManager] Board not initialized. Call InitializeBoard() first.");
                return;
            }

            if (IsStreaming) return;

            try
            {
                boardShim.start_stream();
                IsStreaming = true;
                lastReadTime = Time.time;
                Debug.Log("[BrainFlowManager] Streaming started.");
            }
            catch (BrainFlowException e)
            {
                Debug.LogError($"[BrainFlowManager] Failed to start streaming: {e.Message}");
            }
        }

        /// <summary>
        /// Stop streaming data from the board.
        /// </summary>
        public void StopStreaming()
        {
            if (!IsStreaming) return;

            try
            {
                boardShim.stop_stream();
                IsStreaming = false;
                Debug.Log("[BrainFlowManager] Streaming stopped.");
            }
            catch (BrainFlowException e)
            {
                Debug.LogError($"[BrainFlowManager] Failed to stop streaming: {e.Message}");
            }
        }

        /// <summary>
        /// Release the board session. Call this when done.
        /// </summary>
        public void ReleaseBoard()
        {
            if (!isInitialized) return;

            try
            {
                boardShim.release_session();
                isInitialized = false;
                Debug.Log("[BrainFlowManager] Board session released.");
            }
            catch (BrainFlowException e)
            {
                Debug.LogError($"[BrainFlowManager] Failed to release board: {e.Message}");
            }
        }

        private void ReadData()
        {
            try
            {
                int dataCount = boardShim.get_board_data_count();
                if (dataCount == 0) return;

                double[,] data = boardShim.get_board_data();
                OnDataReceived?.Invoke(data);

                // Extract EMG data if available
                if (EMGChannels.Length > 0)
                {
                    float[] emgValues = new float[EMGChannels.Length];
                    int lastSampleIndex = data.GetLength(1) - 1;
                    
                    for (int i = 0; i < EMGChannels.Length; i++)
                    {
                        emgValues[i] = (float)data[EMGChannels[i], lastSampleIndex];
                    }
                    
                    OnEMGDataReceived?.Invoke(emgValues);

                    if (logDataToConsole)
                    {
                        Debug.Log($"[BrainFlowManager] EMG: {string.Join(", ", emgValues)}");
                    }
                }
            }
            catch (BrainFlowException e)
            {
                Debug.LogError($"[BrainFlowManager] Failed to read data: {e.Message}");
            }
        }

        /// <summary>
        /// Get the latest EMG value for a specific channel (normalized 0-1).
        /// </summary>
        public float GetNormalizedEMG(int channelIndex, float minValue = 0f, float maxValue = 1000f)
        {
            if (!IsStreaming || EMGChannels.Length == 0) return 0f;

            try
            {
                double[,] data = boardShim.get_current_board_data(1);
                if (data.GetLength(1) == 0) return 0f;

                int channel = EMGChannels[Mathf.Clamp(channelIndex, 0, EMGChannels.Length - 1)];
                float rawValue = (float)data[channel, 0];
                
                return Mathf.InverseLerp(minValue, maxValue, Mathf.Abs(rawValue));
            }
            catch
            {
                return 0f;
            }
        }
    }
}

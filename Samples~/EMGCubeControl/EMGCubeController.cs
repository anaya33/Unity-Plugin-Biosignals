using UnityEngine;
using BrainFlowBiosignals;

namespace BrainFlowBiosignals.Samples
{
    /// <summary>
    /// Sample script: Flex your muscle to move a cube up.
    /// This demonstrates the basic EMG → Unity object control pipeline.
    /// </summary>
    public class EMGCubeController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BiosignalInput biosignalInput;
        [SerializeField] private Transform targetCube;

        [Header("Movement Settings")]
        [Tooltip("Maximum height the cube can reach")]
        [SerializeField] private float maxHeight = 3f;
        
        [Tooltip("Starting height of the cube")]
        [SerializeField] private float baseHeight = 0f;
        
        [Tooltip("How fast the cube responds to EMG changes")]
        [SerializeField] private float responsiveness = 5f;

        [Header("Visual Feedback")]
        [SerializeField] private bool changeColorOnFlex = true;
        [SerializeField] private Color restColor = Color.white;
        [SerializeField] private Color flexColor = Color.green;

        private Vector3 startPosition;
        private Renderer cubeRenderer;
        private float currentHeight;

        private void Start()
        {
            if (targetCube == null)
            {
                targetCube = transform;
            }

            startPosition = targetCube.position;
            cubeRenderer = targetCube.GetComponent<Renderer>();

            if (biosignalInput == null)
            {
                biosignalInput = FindObjectOfType<BiosignalInput>();
            }

            if (biosignalInput != null)
            {
                biosignalInput.OnFlexStart += HandleFlexStart;
                biosignalInput.OnFlexEnd += HandleFlexEnd;
            }
        }

        private void OnDestroy()
        {
            if (biosignalInput != null)
            {
                biosignalInput.OnFlexStart -= HandleFlexStart;
                biosignalInput.OnFlexEnd -= HandleFlexEnd;
            }
        }

        private void Update()
        {
            if (biosignalInput == null) return;

            // Map EMG value to height
            float targetHeight = baseHeight + (biosignalInput.EMGValue * maxHeight);
            currentHeight = Mathf.Lerp(currentHeight, targetHeight, Time.deltaTime * responsiveness);

            // Apply position
            Vector3 newPosition = startPosition;
            newPosition.y = startPosition.y + currentHeight;
            targetCube.position = newPosition;

            // Update color based on flex strength
            if (changeColorOnFlex && cubeRenderer != null)
            {
                cubeRenderer.material.color = Color.Lerp(restColor, flexColor, biosignalInput.EMGValue);
            }
        }

        private void HandleFlexStart()
        {
            Debug.Log("[EMGCubeController] Flex detected!");
        }

        private void HandleFlexEnd()
        {
            Debug.Log("[EMGCubeController] Flex released.");
        }
    }
}

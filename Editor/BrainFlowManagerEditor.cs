using UnityEngine;
using UnityEditor;

namespace BrainFlowBiosignals.Editor
{
    [CustomEditor(typeof(BrainFlowManager))]
    public class BrainFlowManagerEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            BrainFlowManager manager = (BrainFlowManager)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
            
            GUI.enabled = false;
            EditorGUILayout.Toggle("Is Streaming", manager.IsStreaming);
            EditorGUILayout.IntField("Sampling Rate", manager.SamplingRate);
            GUI.enabled = true;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Controls", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            
            GUI.enabled = Application.isPlaying && !manager.IsStreaming;
            if (GUILayout.Button("Start Streaming"))
            {
                manager.StartStreaming();
            }

            GUI.enabled = Application.isPlaying && manager.IsStreaming;
            if (GUILayout.Button("Stop Streaming"))
            {
                manager.StopStreaming();
            }
            
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            EditorGUILayout.HelpBox(
                "Board IDs:\n" +
                "-1 = Synthetic (for testing)\n" +
                "0 = Cyton\n" +
                "1 = Ganglion\n" +
                "2 = Cyton + Daisy\n" +
                "See BrainFlow docs for full list.",
                MessageType.Info
            );
        }
    }
}

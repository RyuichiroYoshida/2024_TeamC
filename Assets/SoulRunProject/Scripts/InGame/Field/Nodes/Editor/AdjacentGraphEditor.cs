using SoulRunProject.Common;
using UnityEngine;
using UnityEditor;

namespace SoulRunProject.Editor
{
    [CustomEditor(typeof(AdjacentGraph))]
    public class AdjacentGraphEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Process"))
            {
                var graph = target as AdjacentGraph;
                var processor = new AdjacentGraphProcessor(graph);
                processor.Run();
            }
        }
    }
}

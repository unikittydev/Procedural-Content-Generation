using UnityEditor;
using UnityEngine.UIElements;

namespace PCG.Editor.Graph
{
    public class PCGEditorWindow : EditorWindow
    {
        [MenuItem("Window/PCG Graph")]
        static void Create()
        {
            PCGEditorWindow window = GetWindow<PCGEditorWindow>("PCG Graph");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddStyles();
        }

        private void AddGraphView()
        {
            var graphView = new PCGGraphView(this);
            rootVisualElement.Add(graphView);
            graphView.StretchToParentSize();
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("PCG Graph/PCGGraphVariables.uss");
        }
    }
}
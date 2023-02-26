using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    [CustomPropertyDrawer(typeof(UniformFloatGenerator))]
    [CustomPropertyDrawer(typeof(UniformDoubleGenerator))]
    [CustomPropertyDrawer(typeof(UniformIntGenerator))]
    public class UniformGeneratorPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var tree = (VisualTreeAsset)EditorGUIUtility.Load("Generators/UniformGeneratorPropertyDrawer.uxml");

            var drawer = tree.CloneTree();
            drawer.Query<PropertyField>().ForEach(field => field.label = string.Empty);

            TryDisableMissingFields(property, drawer, "min");
            TryDisableMissingFields(property, drawer, "max");
            TryDisableMissingFields(property, drawer, "mean");

            return drawer;
        }

        private void TryDisableMissingFields(SerializedProperty root, VisualElement drawer, string name)
        {
            if (root.FindPropertyRelative(name) == null)
                drawer.Q<VisualElement>(name).SetDisplay(false);
        }
    }
}

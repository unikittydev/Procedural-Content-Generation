using UnityEditor;
using UnityEngine.UIElements;

namespace PCG.Generation.Editor
{
    [CustomPropertyDrawer(typeof(IObjectProvider<>), true)]
    [CustomPropertyDrawer(typeof(ICollectionProvider<>), true)]
    public class ProviderDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();
            
            SerializedProperty copy = property.Copy();
            if (!copy.Next(true))
                return container;

            do
                GeneratorEditorUtility.DrawPropertyField(copy, container);
            while (copy.Next(false));

            return container;
        }
    }
}
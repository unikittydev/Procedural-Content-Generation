using UnityEditor;
using UnityEngine.UIElements;

namespace PCG.Editor.Graph
{
    public static class StyleUtils
    {
        public static void AddStyleSheets(this VisualElement element, params string[] styleSheetNames)
        {
            foreach (var styleSheetName in styleSheetNames)
            {
                StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load(styleSheetName);
                element.styleSheets.Add(styleSheet);
            }
        }

        public static VisualElement AddClasses(this VisualElement element, params string[] classNames)
        {
            foreach (var className in classNames)
                element.AddToClassList(className);

            return element;
        }
    }
}

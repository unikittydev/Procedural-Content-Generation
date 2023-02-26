using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Editor.Graph
{
    public class PCGGraphView : GraphView
    {
        private PCGEditorWindow editorWindow;
        
        public PCGGraphView(PCGEditorWindow window)
        {
            editorWindow = window;
            
            AddGridBackground();
            AddManipulators();
            AddStyles();
            
            AddElement(AddTestNode(Vector2.zero));
            AddElement(AddTestNode(Vector2.zero));
            AddElement(AddTestNode(Vector2.zero));
        }

        private Node AddTestNode(Vector2 position)
        {
            TestNode node = new TestNode();
            node.Init(position);
            node.Draw();

            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>(ports.Where(port =>
            {
                if (startPort.node == port.node)
                    return false;
                if (startPort.direction == port.direction)
                    return false;
                // @ Проверка на циклы
                
                return true;
            }));
            
            return compatiblePorts;
        }

        private void AddGridBackground()
        {
            var bg = new GridBackground();
            bg.StretchToParentSize();
            
            Insert(0, bg);
        }
        
        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
        }

        private void AddStyles()
        {
            this.AddStyleSheets("PCG Graph/PCGGraphView.uss");
        }
    }
}
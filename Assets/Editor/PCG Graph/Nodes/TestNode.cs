using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Editor.Graph
{
    public class TestNode : Node
    {
        public virtual void Init(Vector2 position)
        {
            SetPosition(new Rect(position, Vector2.zero));
        }
        
        public virtual void Draw()
        {
            var titleField = new TextField("Test Node");
            titleContainer.Insert(0, titleField);
            
            Port inputPort1 = CreatePort(this, "Test 1", Orientation.Horizontal, Direction.Input, Port.Capacity.Single);
            Port inputPort2 = CreatePort(this, "Test 2", Orientation.Vertical, Direction.Input, Port.Capacity.Single);
            Port outPort1 = CreatePort(this, "Test 3", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
            Port outPort2 = CreatePort(this, "Test 4", Orientation.Vertical, Direction.Output, Port.Capacity.Multi);

            inputContainer.Add(inputPort1);
            inputContainer.Add(inputPort2);
            outputContainer.Add(outPort1);
            outputContainer.Add(outPort2);
        }
        
        public Port CreatePort(TestNode node, string portName, Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }
    }
}
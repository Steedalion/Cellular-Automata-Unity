using UnityEngine;

public class ControlNode : Node
{
    public bool active;
    public Node right, above;

    public ControlNode(Vector3 position, bool active, float squareSize) : base(position)
    {
        this.active = active;
        above = new Node(position + Vector3.forward * squareSize * 0.5f);
        right = new Node(position + Vector3.right * squareSize * 0.5f);
    }
}
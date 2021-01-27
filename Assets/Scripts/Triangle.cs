readonly struct Triangle
{
    public readonly int vertexA;
    public readonly int vertexB;
    public readonly int vertexC;
    public readonly int[] vertices;

    public Triangle(int vertexA, int vertexB, int vertexC)
    {
        this.vertexA = vertexA;
        this.vertexB = vertexB;
        this.vertexC = vertexC;
            
        vertices = new int[3];
        vertices[0] = this.vertexA;
        vertices[1] = this.vertexB;
        vertices[2] = this.vertexC;
    }

    public int this[int i] => vertices[i];

    public bool Contains(int vertexIndex)
    {
        return vertexIndex == vertexA || vertexIndex ==vertexB || vertexIndex ==vertexC;
    }
}
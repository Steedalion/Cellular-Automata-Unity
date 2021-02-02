public class Square
{
    public ControlNode topLeft, topRight, bottomLeft , bottomRight;
    public Node topMiddle, bottomMiddle, centerLeft, centreRight;
    public int configuration;
    // int depicting active nodes; 
    /// <summary>
    /// 1   2
    ///
    /// 4   3 = 1234
    ///
    /// e.g.
    /// 0   1
    ///
    /// 1   0 = 0101 = 1+2^2 = 5
    /// 
    /// </summary>
    /// <param name="topLeft"></param>
    /// <param name="topRight"></param>
    /// <param name="bottomLeft"></param>
    /// <param name="bottomRight"></param>
        

    public Square(ControlNode topLeft, ControlNode topRight, ControlNode bottomLeft, ControlNode bottomRight)
    {
        this.topLeft = topLeft;
        this.topRight = topRight;
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
        this.topMiddle = topLeft.right;
        this.bottomMiddle = bottomLeft.right;
        this.centerLeft = bottomLeft.above;
        this.centreRight = bottomRight.above;

        configuration += (this.topLeft.active)?8:0;
        configuration += (this.topRight.active)?4:0;
        configuration += (this.bottomRight.active)?2:0;
        configuration += (this.bottomLeft.active)?1:0;
            
    }
}
namespace VoronoiLib.Structures
{
    public class VPoint
    {
        public double X { get; }
        public double Y { get; }

        public static implicit operator UnityEngine.Vector2(VPoint p)
        {
            return new UnityEngine.Vector2((float)p.X, (float)p.Y);
        }

        internal VPoint(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}

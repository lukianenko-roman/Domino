using System.Windows;

namespace Domino
{
    public class Bone : DependencyObject
    {
        internal int PointsQty1 { get; private set; }
        internal int PointsQty2 { get; private set; }
        internal int SumOfPoints { get; private set; }
        internal bool AreHalfsEquil { get; private set; }
        private protected Bone() { }
        internal Bone(int pointsQty1, int pointsQty2)
        {
            PointsQty1 = pointsQty1;
            PointsQty2 = pointsQty2;
            SumOfPoints = pointsQty1 + pointsQty2;
            AreHalfsEquil = PointsQty1 == PointsQty2;
        }
    }
}
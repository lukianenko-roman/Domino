using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Domino
{
    abstract class UserOrOppenentZone : BonesZone
    {
        internal static MainWindow mainWindow;
        internal static Canvas canvasForAnimation;
        private protected bool isNeedToTransferMove;
        private protected BonesZone zoneTo;
        private protected BoneGraphics bone;
        private protected bool isToEnd;
        private protected bool HasAlreadyMadeMove { get; set; } = false;

        internal static readonly DependencyProperty NameProperty;
        internal string Name
        {
            get { return (string)GetValue(NameProperty); }
            set { SetValue(NameProperty, value); }
        }
        internal int Score { get; set; }
        internal virtual bool IsTurnToMove { get; set; }
        internal int FirstBoneIndexToMove { get; set; } = -1;
        internal static int DelayInMs { get; set; }

        internal static readonly DependencyProperty BackgroundBrushProperty;
        internal Brush BackgroundBrush
        {
            get { return (Brush)GetValue(BackgroundBrushProperty); }
            private protected set { SetValue(BackgroundBrushProperty, value); }
        }
        static UserOrOppenentZone()
        {
            BackgroundBrushProperty = DependencyProperty.Register("BackgroundBrush", typeof(Brush), typeof(UserOrOppenentZone));
            NameProperty = DependencyProperty.Register("Name", typeof(string), typeof(UserOrOppenentZone));
        }
        internal UserOrOppenentZone() : base() { }
        internal override void Reset()
        {
            base.Reset();
            FirstBoneIndexToMove = -1;
            BackgroundBrush = Brushes.AliceBlue;
            HasAlreadyMadeMove = false;
        }
        internal void SetBackgroundBrush(Brush brush)
        {
            BackgroundBrush = brush;
        }
        private protected override void StartAnimation(BonesZone zoneTo, BoneGraphics bone, bool isToEnd, bool isNeedToTransferMove, System.Windows.Point startPoint)
        {
            this.isNeedToTransferMove = isNeedToTransferMove;
            this.bone = bone;
            this.zoneTo = zoneTo;
            this.isToEnd = isToEnd;

            if (bone.AreHalfsEquil)
                bone.State = StateOfBone.OpenVertical;
            else
            {
                if (BonesRepository.PlayedZone.Bones.Count == 0)
                    bone.State = StateOfBone.OpenHorizontal;
                else
                {
                    if (isToEnd)
                        bone.State = BonesRepository.PlayedZone.PointsRightSide == bone.PointsQty1 ? StateOfBone.OpenHorizontal : StateOfBone.OpenHorizontalTransposed;
                    else
                        bone.State = BonesRepository.PlayedZone.PointsLeftSide == bone.PointsQty2 ? StateOfBone.OpenHorizontal : StateOfBone.OpenHorizontalTransposed;
                }
            }

            var windowPoint = mainWindow.PointFromScreen(new System.Windows.Point(0, 0));
            var borderPoint = mainWindow.border.PointFromScreen(new System.Windows.Point(0, 0));
            borderPoint.Y -= mainWindow.borderRest.ActualHeight / 2.0;
            bone.BoneGrid.Margin = new Thickness(Math.Abs(startPoint.X) - Math.Abs(windowPoint.X), Math.Abs(startPoint.Y) - Math.Abs(windowPoint.Y) - 23, 0, 0);
            canvasForAnimation.Children.Add(bone.BoneGrid);

            Storyboard myStoryboard = new Storyboard();
            myStoryboard.Completed += MyStoryboard_Completed;
            ObjectAnimationUsingKeyFrames animation = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTarget(animation, bone.BoneGrid);
            Storyboard.SetTargetProperty(animation, new PropertyPath("Margin"));

            System.Windows.Point target = BonesRepository.PlayedZone.GetTargetPoint(isToEnd, bone.AreHalfsEquil);
            if (target.X == -1 && target.Y == -1)
            {
                target = new System.Windows.Point(
                Math.Floor(Math.Abs(borderPoint.X) + Math.Abs(mainWindow.border.ActualWidth / 2)) - Math.Min(bone.BoneGrid.Width, bone.BoneGrid.Height),
                Math.Floor(Math.Abs(borderPoint.Y) + Math.Abs(mainWindow.border.ActualHeight / 2)) - Math.Min(bone.BoneGrid.Width, bone.BoneGrid.Height) * (bone.AreHalfsEquil ? 1 : 0.5));
            }

            double diffX = Math.Abs(startPoint.X) - target.X;
            double diffY = Math.Abs(startPoint.Y) - target.Y;
            for (int count = 1; count < 101; count++)
            {
                double left = bone.BoneGrid.Margin.Left - (diffX / 100.0 * count);
                double top = bone.BoneGrid.Margin.Top - (diffY / 100.0 * count);
                DiscreteObjectKeyFrame keyFrame = new DiscreteObjectKeyFrame(new
                                Thickness(left, top, 0, 0), TimeSpan.FromMilliseconds(0.01 * DelayInMs * count));
                animation.KeyFrames.Add(keyFrame);
            }
            myStoryboard.Children.Add(animation);
            myStoryboard.Begin();
        }
        private void MyStoryboard_Completed(object sender, EventArgs e)
        {
            canvasForAnimation.Children.Remove(bone.BoneGrid);
            zoneTo.AddBone(bone, isToEnd);
            HasAlreadyMadeMove = false;
            if (isNeedToTransferMove) BonesRepository.NextMove(Bones.Count > 0);
        }

    }
}
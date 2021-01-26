using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Domino
{
    class UserBonesZone : UserOrOppenentZone // зона с костями пользователя
    {
        private static bool isShowNotificationTwoSides = true;

        private bool isTurnToMove;
        internal override bool IsTurnToMove
        {
            get { return isTurnToMove; }
            set
            {
                isTurnToMove = value;
                BackgroundBrush = BoneGraphics.SetBackgroundBrush(IsTurnToMove, IsPossibleMove());
                if (isTurnToMove && (BonesRepository.PlayedZone.PointsLeftSide == -1 || !IsPossibleMove()) && BonesRepository.RestZone.Bones.Count < 2 && FirstBoneIndexToMove == -1)
                    NextMoveWithDelay();
            }
        }
        internal static readonly DependencyProperty CurrentScoreProperty;
        internal string CurrentScore
        {
            get { return (string)GetValue(CurrentScoreProperty); }
            set { SetValue(CurrentScoreProperty, value); }
        }
        static UserBonesZone() { CurrentScoreProperty = DependencyProperty.Register("CurrentScore", typeof(string), typeof(UserOrOppenentZone)); }
        internal UserBonesZone() : base() { Name = "User"; }
        private async void NextMoveWithDelay()
        {
            await Task.Delay(DelayInMs);
            BonesRepository.NextMove(Bones.Count > 0);
        }
        private bool IsPossibleMove()
        {
            if (FirstBoneIndexToMove != -1) return true;
            for (int i = 0; i < Bones.Count; i++)
                if (Bones[i].PointsQty1 == BonesRepository.PlayedZone.PointsLeftSide ||
                    Bones[i].PointsQty2 == BonesRepository.PlayedZone.PointsLeftSide ||
                    Bones[i].PointsQty1 == BonesRepository.PlayedZone.PointsRightSide ||
                    Bones[i].PointsQty2 == BonesRepository.PlayedZone.PointsRightSide)
                    return true;
            return false;
        }
        internal override void AddBone(BoneGraphics bone, bool isToEnd = true)
        {
            bone.State = StateOfBone.OpenVerticalWithMargin;
            bone.BoneGrid.MouseDown += UserBoneGrid_MouseDown;
            AddingBone(bone);
        }
        internal void UserBoneGrid_MouseDown(object sender, MouseEventArgs e) // клик на своих костях
        {
            if (!IsTurnToMove || HasAlreadyMadeMove) return;
            Grid grid = (Grid)sender;
            BoneGraphics bone = BonesRepository.UserZone.Bones[grid.Name.Length == 2 ? int.Parse($"{grid.Name[^1]}") : int.Parse($"{grid.Name[^2]}{grid.Name[^1]}")];

            var boneLocation = bone.BoneGrid.PointToScreen(new Point(0, 0));

            if (BonesRepository.PlayedZone.Bones.Count == 0)
            {
                //если первая кость
                if (FirstBoneIndexToMove != -1)
                {
                    if ((bone.AreHalfsEquil && bone.PointsQty1 == Bones[FirstBoneIndexToMove].PointsQty1 && bone.PointsQty2 == Bones[FirstBoneIndexToMove].PointsQty2) || (!Bones[FirstBoneIndexToMove].AreHalfsEquil && bone.SumOfPoints == Bones[FirstBoneIndexToMove].SumOfPoints))
                    {
                        FirstBoneIndexToMove = -1;
                        TransferBoneAfterClick(grid, this, BonesRepository.PlayedZone, true, boneLocation);
                    }
                }
            }
            else
            {
                if (((bone.PointsQty1 == BonesRepository.PlayedZone.PointsLeftSide && bone.PointsQty2 == BonesRepository.PlayedZone.PointsRightSide) ||
                    (bone.PointsQty2 == BonesRepository.PlayedZone.PointsLeftSide && bone.PointsQty1 == BonesRepository.PlayedZone.PointsRightSide)) &&
                    (!bone.AreHalfsEquil) &&
                    (BonesRepository.PlayedZone.PointsLeftSide != BonesRepository.PlayedZone.PointsRightSide))
                {
                    //если выбранная кость может быть поставлена в оба конца
                    bool isLeftButton = e.LeftButton == MouseButtonState.Pressed;
                    bool isRigthButton = e.RightButton == MouseButtonState.Pressed;
                    if (isShowNotificationTwoSides)
                    {
                        WindowNotificationTwoSidesOK windowNotificationTwoSidesOK = new WindowNotificationTwoSidesOK { Owner = mainWindow };
                        if (windowNotificationTwoSidesOK.ShowDialog() == true)
                        {
                            isShowNotificationTwoSides = windowNotificationTwoSidesOK.isShowThisAgain;
                            if (windowNotificationTwoSidesOK.isPutBoneDueToChoise)
                                TransferDueToChoise(grid, isLeftButton, isRigthButton, boneLocation);
                        }
                    }
                    else
                        TransferDueToChoise(grid, isLeftButton, isRigthButton, boneLocation);
                }
                else
                {
                    if (bone.PointsQty1 == BonesRepository.PlayedZone.PointsRightSide || bone.PointsQty2 == BonesRepository.PlayedZone.PointsRightSide)
                        TransferBoneAfterClick(grid, this, BonesRepository.PlayedZone, true, boneLocation);
                    else
                    {
                        if (bone.PointsQty1 == BonesRepository.PlayedZone.PointsLeftSide || bone.PointsQty2 == BonesRepository.PlayedZone.PointsLeftSide)
                            TransferBoneAfterClick(grid, this, BonesRepository.PlayedZone, true, boneLocation, false);
                    }
                }
            }
        }
        private void TransferDueToChoise(Grid grid, bool isLeftButton, bool isRigthButton, System.Windows.Point boneLocation)
        {
            if (isLeftButton)
                TransferBoneAfterClick(grid, this, BonesRepository.PlayedZone, true, boneLocation, false);
            else
                if (isRigthButton)
                TransferBoneAfterClick(grid, this, BonesRepository.PlayedZone, true, boneLocation);
        }
        internal void RestBoneGrid_MouseDown(object sender, MouseEventArgs e) // клик на "базаре" (неразобранных костях)
        {
            if (!IsTurnToMove || IsPossibleMove()) return;
            if (BonesRepository.RestZone.Bones.Count > 1)
            {
                Grid grid = (Grid)sender;
                TransferBoneAfterClick(grid, BonesRepository.RestZone, BonesRepository.UserZone, false, grid.PointFromScreen(new Point(0, 0)));
                BackgroundBrush = BoneGraphics.SetBackgroundBrush(IsTurnToMove, IsPossibleMove());
            }
            if (BonesRepository.RestZone.Bones.Count == 1)
            {
                BonesRepository.RestZone.SetOpenState();
                AddCountersInRepository(BonesRepository.RestZone.Bones[0].PointsQty1, BonesRepository.RestZone.Bones[0].PointsQty2);
                if (!IsPossibleMove()) BonesRepository.NextMove(Bones.Count > 0);
            }
        }
        private void TransferBoneAfterClick(Grid grid, BonesZone zoneFrom, BonesZone zoneTo, bool isNeedToTransferMove, System.Windows.Point boneLocation, bool isToEnd = true)
        {
            if (isNeedToTransferMove) HasAlreadyMadeMove = true;
            zoneFrom.TransferBone(zoneTo, grid.Name.Length == 2 ? int.Parse($"{grid.Name[^1]}") : int.Parse($"{grid.Name[^2]}{grid.Name[^1]}"), isToEnd, isNeedToTransferMove, boneLocation);
        }
    }
}
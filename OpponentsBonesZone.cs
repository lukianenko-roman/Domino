using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Domino
{
    class OpponentsBonesZone : UserOrOppenentZone // зоны оппонентов
    {

        private StateOfBone State { get; set; }
        private bool isTurnToMove;
        internal override bool IsTurnToMove
        {
            get { return isTurnToMove; }
            set
            {
                isTurnToMove = value;
                BackgroundBrush = BoneGraphics.SetBackgroundBrush(IsTurnToMove, GetIndexOfMaxElement(BonesRepository.PlayedZone.PointsLeftSide, BonesRepository.PlayedZone.PointsRightSide) != -1);
                if (isTurnToMove)
                    MakeMoveWithDelay();
            }
        }

        internal OpponentsBonesZone(StateOfBone state) : base() { State = state; }

        private async void MakeMoveWithDelay()
        {
            await Task.Delay(DelayInMs);
            if (!TryMakeMove())
            {
                if (TryGetBoneFromRest())
                    IsTurnToMove = true;
                else
                    BonesRepository.NextMove(Bones.Count > 0);
            }
        }
        internal override void AddBone(BoneGraphics bone, bool isToEnd = true)
        {
            bone.State = State;
            AddingBone(bone);
        }
        private int GetIndexOfMaxElement(int PLS, int PRS)
        {
            if (FirstBoneIndexToMove != -1) return FirstBoneIndexToMove;
            return Bones.Select((value, index) => new { value, index = index + 1 })
                .Where(b => b.value.PointsQty1 == PLS || b.value.PointsQty2 == PLS || b.value.PointsQty1 == PRS || b.value.PointsQty2 == PRS)
                .OrderByDescending(b => b.value.SumOfPoints)
                .Select(pair => pair.index)
                .FirstOrDefault() - 1;
        }
        private bool TryMakeMove()
        {
            if (FirstBoneIndexToMove != -1)
            {
                MakeMoveFromSelectedBone();
                return true;
            }
            int indexOfMaxElement = GetIndexOfMaxElement(BonesRepository.PlayedZone.PointsLeftSide, BonesRepository.PlayedZone.PointsRightSide);
            if (indexOfMaxElement == -1)
                return false;
            else
            {
                var boneLocation = Bones[indexOfMaxElement].BoneGrid.PointFromScreen(new Point(0, 0));

                if ((Bones[indexOfMaxElement].PointsQty1 == BonesRepository.PlayedZone.PointsLeftSide || Bones[indexOfMaxElement].PointsQty2 == BonesRepository.PlayedZone.PointsLeftSide) &&
                    (Bones[indexOfMaxElement].PointsQty1 == BonesRepository.PlayedZone.PointsRightSide || Bones[indexOfMaxElement].PointsQty2 == BonesRepository.PlayedZone.PointsRightSide) &&
                    (BonesRepository.PlayedZone.PointsLeftSide != BonesRepository.PlayedZone.PointsRightSide))
                {
                    if (BonesRepository.PlayedZone.PointsLeftSide > BonesRepository.PlayedZone.PointsRightSide)
                        TransferBone(BonesRepository.PlayedZone, indexOfMaxElement, false, true, boneLocation);
                    else
                        TransferBone(BonesRepository.PlayedZone, indexOfMaxElement, true, true, boneLocation);
                }
                else
                {
                    if (Bones[indexOfMaxElement].PointsQty1 == BonesRepository.PlayedZone.PointsLeftSide || Bones[indexOfMaxElement].PointsQty2 == BonesRepository.PlayedZone.PointsLeftSide)
                        TransferBone(BonesRepository.PlayedZone, indexOfMaxElement, false, true, boneLocation);
                    else
                        TransferBone(BonesRepository.PlayedZone, indexOfMaxElement, true, true, boneLocation);
                }
                return true;
            }
        }

        private void MakeMoveFromSelectedBone()
        {
            TransferBone(BonesRepository.PlayedZone, FirstBoneIndexToMove, true, true, Bones[FirstBoneIndexToMove].BoneGrid.PointFromScreen(new Point(0, 0)));
            FirstBoneIndexToMove = -1;
        }
        private bool TryGetBoneFromRest()
        {
            if (BonesRepository.RestZone.Bones.Count > 1)
            {
                Random random = new Random();
                int r = random.Next(0, BonesRepository.RestZone.Bones.Count);
                BonesRepository.RestZone.TransferBone(this, r, true, false, BonesRepository.RestZone.Bones[r].BoneGrid.PointFromScreen(new Point(0, 0)));

                if (BonesRepository.RestZone.Bones.Count == 1)
                {
                    BonesRepository.RestZone.SetOpenState();
                    AddCountersInRepository(BonesRepository.RestZone.Bones[0].PointsQty1, BonesRepository.RestZone.Bones[0].PointsQty2);
                }
                BackgroundBrush = BoneGraphics.SetBackgroundBrush(IsTurnToMove, GetIndexOfMaxElement(BonesRepository.PlayedZone.PointsLeftSide, BonesRepository.PlayedZone.PointsRightSide) != -1);
                return true;
            }
            else return false;
        }

    }
}
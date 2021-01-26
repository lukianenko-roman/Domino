using System;
using System.Windows;

namespace Domino
{
    class PlayedBonesZone : BonesZone // зона с костями (в центре приложения), которыми ходили в течении раунда
    {
        private readonly int BonesHalfsOnOneLine = 20;
        internal int PointsLeftSide { get; set; }
        internal int PointsRightSide { get; set; }
        internal override void Reset()
        {
            base.Reset();
            PointsLeftSide = -1;
            PointsRightSide = -1;
        }
        internal override void AddBone(BoneGraphics bone, bool isAddAtEnd = true)
        {
            bool isTranspose = false;
            if (Bones.Count != 0)
                if (isAddAtEnd)
                {
                    if (Bones[^1].PointsQty1 == bone.PointsQty1)
                        isTranspose = false;
                    else
                    {
                        if (Bones[^1].PointsQty2 == bone.PointsQty1)
                            isTranspose = false;
                        else
                            isTranspose = true;
                    }
                }
                else
                {
                    if (Bones[0].PointsQty1 == bone.PointsQty2)
                        isTranspose = false;
                    else
                    {
                        if (Bones[0].PointsQty2 == bone.PointsQty2)
                            isTranspose = false;
                        else
                            isTranspose = true;
                    }
                }
            bone.State = bone.AreHalfsEquil ? StateOfBone.OpenVertical : isTranspose ? StateOfBone.OpenHorizontalTransposed : StateOfBone.OpenHorizontal;

            if (Bones.Count == 0)
            {
                PointsRightSide = bone.State == StateOfBone.OpenHorizontalTransposed ? bone.PointsQty1 : bone.PointsQty2;
                PointsLeftSide = bone.State == StateOfBone.OpenHorizontalTransposed ? bone.PointsQty2 : bone.PointsQty1;

                Bones.Add(bone);
                for (int i = 0; i < (PointsLeftSide == PointsRightSide ? BonesHalfsOnOneLine : BonesHalfsOnOneLine - 1); i++) AddUnvisibleGrid();
                Grids[BonesHalfsOnOneLine / 2 - (BonesHalfsOnOneLine % 2 == 0 ? 1 : 0)] = bone.BoneGrid;
            }
            else
            {
                if (isAddAtEnd)
                    PointsRightSide = bone.State == StateOfBone.OpenHorizontalTransposed ? bone.PointsQty1 : bone.PointsQty2;
                else
                    PointsLeftSide = bone.State == StateOfBone.OpenHorizontalTransposed ? bone.PointsQty2 : bone.PointsQty1;
                AddingBone(bone, isAddAtEnd);
            }
        }
        private int GetIndexOfLastVisibleGrid(bool isFromEnd)
        {
            if (isFromEnd)
            {
                for (int i = Grids.Count - 1; i >= 0; i--)
                    if (Grids[i].Visibility != Visibility.Hidden)
                        return i;
            }
            else
                for (int i = 0; i < Grids.Count; i++)
                    if (Grids[i].Visibility != Visibility.Hidden)
                        return i;
            return 0;
        }
        private void AddingBone(BoneGraphics bone, bool isToEnd)
        {
            Bones.Insert(isToEnd ? Bones.Count : 0, bone);
            int shift = 0;
            if (IsNeedToAddNewLines(bone.AreHalfsEquil, isToEnd))
                for (int i = 0; i < BonesHalfsOnOneLine; i++)
                {
                    AddUnvisibleGrid();
                    AddUnvisibleGrid(false);
                }
            int indexOfLastVisibleGrid = GetIndexOfLastVisibleGrid(isToEnd);
            if (IsNeedToAddEmptyBoneBefore(bone.AreHalfsEquil, isToEnd, indexOfLastVisibleGrid))
                shift = isToEnd ? 1 : -1;
            Grids[indexOfLastVisibleGrid + (isToEnd ? 1 : -1) + shift] = bone.BoneGrid;
            if (!bone.AreHalfsEquil) Grids.RemoveAt(isToEnd ? Grids.Count - 1 : 0);
        }
        private bool IsNeedToAddEmptyBoneBefore(bool isEquelBone, bool isToEnd, int indexOfLastVisibleGrid)
        {
            if (!isEquelBone)
            {
                if (isToEnd)
                {
                    if ((Grids.Count - indexOfLastVisibleGrid) % BonesHalfsOnOneLine == 2)
                        return true;
                }
                else
                    if (indexOfLastVisibleGrid % BonesHalfsOnOneLine == 1)
                    return true;
            }
            return false;
        }
        private bool IsNeedToAddNewLines(bool isEquelBone, bool isToEnd)
        {
            if (isToEnd)
            {
                if (GetIndexOfLastVisibleGrid(isToEnd) > Grids.Count - (isEquelBone ? 2 : 3))
                    return true;
            }
            else
                if (GetIndexOfLastVisibleGrid(isToEnd) < (isEquelBone ? 1 : 2))
                return true;
            return false;
        }
        private void AddUnvisibleGrid(bool isToEnd = true)
        {
            Grids.Insert(isToEnd ? GetIndexOfLastVisibleGrid(true) + (Grids.Count > 0 ? 1 : 0) : GetIndexOfLastVisibleGrid(false), new BoneGraphics { State = StateOfBone.Unvisible }.BoneGrid);
        }
        internal protected System.Windows.Point GetTargetPoint(bool isToEnd, bool isEquel)
        {
            int indexLastVisibleBone = GetIndexOfLastVisibleGrid(isToEnd);
            return new System.Windows.Point()
            {
                X = Grids.Count == 0 ? -1 : Math.Abs(Grids[indexLastVisibleBone].PointFromScreen(new System.Windows.Point(0, 0)).X) + Math.Min(Grids[indexLastVisibleBone].Width, Grids[indexLastVisibleBone].Height) * (isToEnd ? (Bones[^1].AreHalfsEquil ? 1.0 : 2.0) : (isEquel ? -1.0 : -2.0)),
                Y = Grids.Count == 0 ? -1 : Math.Abs(Grids[indexLastVisibleBone].PointFromScreen(new System.Windows.Point(0, 0)).Y) + Math.Max(Grids[indexLastVisibleBone].Width, Grids[indexLastVisibleBone].Height) / 4.0 * (isEquel ? -1.0 : (Bones[isToEnd ? ^1 : 0].AreHalfsEquil ? 1.0 : 0.0))
            };
        }
    }
}
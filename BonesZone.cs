using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Domino
{
    abstract class BonesZone : DependencyObject // абстрактный класс для зон приложения, где будут располагаться кости
    {
        internal ObservableCollection<BoneGraphics> Bones { get; set; }
        internal ObservableCollection<Grid> Grids { get; set; }

        internal BonesZone()
        {
            Bones = new ObservableCollection<BoneGraphics>();
            Grids = new ObservableCollection<Grid>();
        }
        internal virtual void Reset()
        {
            Bones.Clear();
            Grids.Clear();
        }
        internal void SetOpenState()
        {
            for (int i = Bones.Count - 1; i > -1; i--)
            {
                BoneGraphics bg = Bones[i];
                RemoveAtIndex(i);
                switch (bg.State)
                {
                    case StateOfBone.ClosedHorizontal:
                        bg.State = StateOfBone.OpenHorizontal;
                        break;
                    case StateOfBone.ClosedVertical:
                        bg.State = StateOfBone.OpenVertical;
                        break;
                    case StateOfBone.ClosedVerticalWithMargin:
                        bg.State = StateOfBone.OpenVerticalWithMargin;
                        break;
                }
                AddingBone(bg);
            }
        }
        internal virtual void TransferBone(BonesZone zoneTo, int index, bool isToEnd, bool isNeedToTransferMove, System.Windows.Point startPoint)
        {
            BoneGraphics boneTmp = Bones[index];
            if (zoneTo.GetType() == BonesRepository.PlayedZone.GetType())
                AddCountersInRepository(boneTmp.PointsQty1, boneTmp.PointsQty2);
            RemoveAtIndex(index);
            StartAnimation(zoneTo, boneTmp, isToEnd, isNeedToTransferMove, startPoint);
            if (!isNeedToTransferMove) zoneTo.AddBone(boneTmp, isToEnd);
        }
        private protected virtual void StartAnimation(BonesZone zoneTo, BoneGraphics bone, bool isToEnd, bool isNeedToTransferMove, System.Windows.Point startPoint) { }
        private protected void AddCountersInRepository(int points1, int points2)
        {
            BonesRepository.counterOfPlayedPoints[points1]++;
            BonesRepository.counterOfPlayedPoints[points2]++;
        }
        private protected virtual void AddingBone(BoneGraphics bone)
        {
            Bones.Add(bone);
            Grids.Add(bone.BoneGrid);
            Grids[^1].Name = $"G{Grids.Count - 1}";
        }
        internal abstract void AddBone(BoneGraphics bone, bool isToEnd = true);
        private protected void RemoveAtIndex(int index)
        {
            Grids[index].Name = null;
            Bones.RemoveAt(index);
            Grids.RemoveAt(index);
            for (int i = 0; i < Grids.Count; i++)
                Grids[i].Name = $"G{i}";
        }

    }
}
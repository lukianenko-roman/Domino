using System.Windows;

namespace Domino
{
    class RestBonesZone : BonesZone // зона "базара" (неразобранные кости)
    {
        internal static readonly DependencyProperty RestGridHeightProperty;
        internal string RestGridHeight
        {
            get { return (string)GetValue(RestGridHeightProperty); }
            set { SetValue(RestGridHeightProperty, value); }
        }
        static RestBonesZone() { RestGridHeightProperty = DependencyProperty.Register("RestGridHeight", typeof(string), typeof(RestBonesZone)); }
        internal RestBonesZone() : base() { RestGridHeight = "0*"; }
        internal override void AddBone(BoneGraphics bone, bool isToEnd = true)
        {
            if (Bones.Count == 0) RestGridHeight = ".8*";
            bone.State = StateOfBone.ClosedVertical;
            bone.BoneGrid.MouseDown += BonesRepository.UserZone.RestBoneGrid_MouseDown;
            AddingBone(bone);
        }
        internal override void Reset()
        {
            base.Reset();
            RestGridHeight = "0*";
        }
        internal void CopyAllBones(BoneGraphics[] bonesFrom)
        {
            for (int i = 0; i < bonesFrom.Length; i++)
                AddBone(bonesFrom[i]);
        }
        internal override void TransferBone(BonesZone zoneTo, int index, bool isToEnd, bool isNeedToTransferMove, System.Windows.Point startPoint)
        {
            base.TransferBone(zoneTo, index, isToEnd, isNeedToTransferMove, startPoint);
            if (Bones.Count == 0) RestGridHeight = "0*";
        }
    }
}
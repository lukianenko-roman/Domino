using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Domino
{
    struct RowColumnIndexer
    {
        public byte rowIndex;
        public byte columnIndex;
    }
    internal enum StateOfBone
    {
        ClosedHorizontal,
        ClosedVertical,

        OpenVertical,
        OpenHorizontal,
        OpenHorizontalTransposed,

        ClosedVerticalWithMargin,
        OpenVerticalWithMargin,

        Unvisible
    }
    // класс для визуального представления игральной кости
    // визуальное представление состоит из Grid (BoneGrid), у которого в Children добавляются границы и необходимые точки (points)
    // представление кости меняется состоянием (State) в зависимости от расположения кости и текущего состояния раунда/игры
    public class BoneGraphics : Bone
    {
        private static readonly Dictionary<int, RowColumnIndexer[]> pointsIndexesHorizontal; // указатель индексов расположения точек на горизонтальной кости
        private static readonly Dictionary<int, RowColumnIndexer[]> pointsIndexesVertical; // указатель индексов расположения точек на вертикальной кости
        private static readonly Style[] stylesPoint;

        private static readonly SolidColorBrush[] colorsForPoints;
        internal static bool IsDifferentColorOfPoints { get; set; }

        private StateOfBone stateOfBone;
        internal StateOfBone State
        {
            get
            {
                return stateOfBone;
            }
            set
            {
                stateOfBone = value;
                switch (value)
                {
                    case StateOfBone.ClosedHorizontal:
                        SetClosedBone("Horizontal", (6, 3), false);
                        break;
                    case StateOfBone.ClosedVertical:
                        SetClosedBone("Vertical", (3, 6), false);
                        break;
                    case StateOfBone.OpenHorizontal:
                        SetOpenView("Horizontal", (6, 3), false, false, false, false);
                        break;
                    case StateOfBone.OpenHorizontalTransposed:
                        SetOpenView("Horizontal", (6, 3), false, true, false, false);
                        break;
                    case StateOfBone.OpenVertical:
                        SetOpenView("Vertical", (3, 6), true, false, false, false);
                        break;
                    case StateOfBone.Unvisible:
                        SetOpenView("Vertical", (3, 6), true, false, true, false);
                        break;
                    case StateOfBone.ClosedVerticalWithMargin:
                        SetClosedBone("Vertical", (3, 6), true);
                        break;
                    case StateOfBone.OpenVerticalWithMargin:
                        SetOpenView("Vertical", (3, 6), true, false, false, true);
                        break;
                }
            }
        }
        internal static readonly DependencyProperty BoneGridProperty;
        internal Grid BoneGrid
        {
            get { return (Grid)GetValue(BoneGridProperty); }
            set { SetValue(BoneGridProperty, value); }
        }
        private Border borderGrid;
        private Border borderMid;
        private List<Ellipse> points;

        static BoneGraphics()
        {
            BoneGridProperty = DependencyProperty.Register("BoneGrid", typeof(Grid), typeof(BoneGraphics));

            pointsIndexesHorizontal = new Dictionary<int, RowColumnIndexer[]>();
            pointsIndexesVertical = new Dictionary<int, RowColumnIndexer[]>();

            RowColumnIndexer rc00 = new RowColumnIndexer { rowIndex = 0, columnIndex = 0 };
            RowColumnIndexer rc01 = new RowColumnIndexer { rowIndex = 0, columnIndex = 1 };
            RowColumnIndexer rc02 = new RowColumnIndexer { rowIndex = 0, columnIndex = 2 };
            RowColumnIndexer rc10 = new RowColumnIndexer { rowIndex = 1, columnIndex = 0 };
            RowColumnIndexer rc11 = new RowColumnIndexer { rowIndex = 1, columnIndex = 1 };
            RowColumnIndexer rc12 = new RowColumnIndexer { rowIndex = 1, columnIndex = 2 };
            RowColumnIndexer rc20 = new RowColumnIndexer { rowIndex = 2, columnIndex = 0 };
            RowColumnIndexer rc21 = new RowColumnIndexer { rowIndex = 2, columnIndex = 1 };
            RowColumnIndexer rc22 = new RowColumnIndexer { rowIndex = 2, columnIndex = 2 };

            pointsIndexesHorizontal.Add(0, new RowColumnIndexer[] { });
            pointsIndexesHorizontal.Add(1, new RowColumnIndexer[] { rc11 });
            pointsIndexesHorizontal.Add(2, new RowColumnIndexer[] { rc00, rc22 });
            pointsIndexesHorizontal.Add(3, new RowColumnIndexer[] { rc00, rc11, rc22 });
            pointsIndexesHorizontal.Add(4, new RowColumnIndexer[] { rc00, rc02, rc20, rc22 });
            pointsIndexesHorizontal.Add(5, new RowColumnIndexer[] { rc00, rc02, rc11, rc20, rc22 });
            pointsIndexesHorizontal.Add(6, new RowColumnIndexer[] { rc00, rc01, rc02, rc20, rc21, rc22 });

            pointsIndexesVertical.Add(0, new RowColumnIndexer[] { });
            pointsIndexesVertical.Add(1, new RowColumnIndexer[] { rc11 });
            pointsIndexesVertical.Add(2, new RowColumnIndexer[] { rc02, rc20 });
            pointsIndexesVertical.Add(3, new RowColumnIndexer[] { rc02, rc11, rc20 });
            pointsIndexesVertical.Add(4, new RowColumnIndexer[] { rc00, rc02, rc20, rc22 });
            pointsIndexesVertical.Add(5, new RowColumnIndexer[] { rc00, rc02, rc11, rc20, rc22 });
            pointsIndexesVertical.Add(6, new RowColumnIndexer[] { rc00, rc10, rc20, rc02, rc12, rc22 });

            colorsForPoints = new SolidColorBrush[6] { new SolidColorBrush(Colors.DarkRed),
                                                    new SolidColorBrush(Colors.OrangeRed),
                                                    new SolidColorBrush(Colors.DarkGoldenrod),
                                                    new SolidColorBrush(Colors.Green),
                                                    new SolidColorBrush(Colors.Blue),
                                                    new SolidColorBrush(Colors.DarkViolet) };
            stylesPoint = new Style[7];
            for (int i = 0; i < stylesPoint.Length; i++)
            {
                Style style = new Style();
                style.Setters.Add(new Setter { Property = Ellipse.MarginProperty, Value = new Thickness(i) });
                stylesPoint[i] = style;
            }
        }

        internal BoneGraphics(int pointsQty1, int pointsQty2) : base(pointsQty1, pointsQty2) { SetNewBone(); }
        internal BoneGraphics() : base() { SetNewBone(); }

        private void SetOpenView(string VerticalOrHorizontal, (int, int) ColumnsAndRowsQty, bool isVertical, bool isTransposed, bool isUnvisible, bool isMargin)
        {
            SetNewBone();
            BoneGrid.SetResourceReference(Grid.StyleProperty, $"StyleOpenBone{VerticalOrHorizontal}" + (isMargin ? "WithMargin" : ""));
            borderGrid.SetResourceReference(Border.StyleProperty, $"StyleBorderBone{VerticalOrHorizontal}");
            borderMid.SetResourceReference(Border.StyleProperty, $"StyleBorderMid{VerticalOrHorizontal}");

            for (int i = 0; i < ColumnsAndRowsQty.Item1; i++)
                BoneGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < ColumnsAndRowsQty.Item2; i++)
                BoneGrid.RowDefinitions.Add(new RowDefinition());

            BoneGrid.Children.Add(borderGrid);
            BoneGrid.Children.Add(borderMid);
            SetPoints(isTransposed, isVertical);
            if (isUnvisible) { BoneGrid.Visibility = Visibility.Hidden; }
        }
        private void SetClosedBone(string VerticalOrHorizontal, (int, int) ColumnsAndRowsQty, bool isMargin)
        {
            SetNewBone();
            BoneGrid.SetResourceReference(Grid.StyleProperty, $"StyleClosedBone{VerticalOrHorizontal}" + (isMargin ? "WithMargin" : ""));
            borderGrid.SetResourceReference(Border.StyleProperty, $"StyleBorderBone{VerticalOrHorizontal}");
            borderMid.SetResourceReference(Border.StyleProperty, $"StyleBorderMid{VerticalOrHorizontal}");

            for (int i = 0; i < ColumnsAndRowsQty.Item1; i++)
                BoneGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < ColumnsAndRowsQty.Item2; i++)
                BoneGrid.RowDefinitions.Add(new RowDefinition());

            BoneGrid.Children.Add(borderGrid);
            BoneGrid.Children.Add(borderMid);
        }
        private void SetNewBone()
        {
            BoneGrid = new Grid();
            borderGrid = new Border();
            borderMid = new Border();
            points = new List<Ellipse>();
        }
        private void SetPoints(bool isTransposed, bool isVertical)
        {
            if (isVertical)
            {
                SetPointsForHalfBone(pointsIndexesVertical[PointsQty1], isVertical);
                SetPointsForHalfBone(pointsIndexesVertical[PointsQty2], isVertical, 3);
            }
            else
            {
                SetPointsForHalfBone(pointsIndexesHorizontal[isTransposed ? PointsQty2 : PointsQty1], isVertical);
                SetPointsForHalfBone(pointsIndexesHorizontal[isTransposed ? PointsQty1 : PointsQty2], isVertical, 3);
            }
        }
        private void SetPointsForHalfBone(RowColumnIndexer[] rowColumnIndexer, bool isVertical, int addParamiter = 0)
        {
            int iTo = rowColumnIndexer.Length;
            for (int i = 0; i < iTo; i++)
            {
                Ellipse ellipse = new Ellipse();
                Grid.SetRow(ellipse, rowColumnIndexer[i].rowIndex + (isVertical ? addParamiter : 0));
                Grid.SetColumn(ellipse, rowColumnIndexer[i].columnIndex + (isVertical ? 0 : addParamiter));
                points.Add(ellipse);
                BoneGrid.Children.Add(ellipse);
            }
        }
        internal void ReStylePoints(double width)
        {
            int j = Math.Min(stylesPoint.Length - 1, (int)Math.Floor(width == 0 ? 1000 : width / 200));
            for (int i = 0; i < points.Count; i++)
            {
                points[i].Style = stylesPoint[j];
                int points1 = stateOfBone == StateOfBone.OpenHorizontalTransposed ? PointsQty2 : PointsQty1;
                points[i].Fill = IsDifferentColorOfPoints ? (points1 > i ? colorsForPoints[Math.Max(0, (points1 == PointsQty1 ? PointsQty1 : PointsQty2) - 1)] : colorsForPoints[Math.Max(0, (points1 == PointsQty1 ? PointsQty2 : PointsQty1) - 1)]) : new SolidColorBrush(Colors.Black);
            }
        }
        internal static Brush SetBackgroundBrush(bool isTurnToMove, bool isPossibleMove)
        {
            return !isTurnToMove ? Brushes.AliceBlue : isPossibleMove ? Brushes.LightGreen : Brushes.Wheat;
        }
    }
}
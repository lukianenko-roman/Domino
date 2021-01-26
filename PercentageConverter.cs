using System;
using System.Globalization;
using System.Windows.Data;

namespace Domino
{
    public class PercentageConverter : IValueConverter // изменение размеров костей относительно ширины окна
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            for (int i = 0; i < BonesRepository.bones.Length; i++)
                BonesRepository.bones[i].ReStylePoints(System.Convert.ToDouble(value));
            return System.Convert.ToDouble(value) * System.Convert.ToDouble(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator == "." ? parameter.ToString() : parameter.ToString().Replace('.', ','));
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
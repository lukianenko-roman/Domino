using System.Windows;
using System.Windows.Controls;

namespace Domino
{
    /// <summary>
    /// Логика взаимодействия для WindowNotificationTwoSidesOK.xaml
    /// </summary>
    public partial class WindowNotificationTwoSidesOK : Window
    {
        internal bool isShowThisAgain = true;
        internal bool isPutBoneDueToChoise = false;
        public WindowNotificationTwoSidesOK()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.Name == "ButtonOK") isPutBoneDueToChoise = true;
            isShowThisAgain = !(bool)CheckBoxIsShowThisAgain.IsChecked;
            this.DialogResult = true;
        }
    }
}

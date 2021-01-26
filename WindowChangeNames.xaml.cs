using System.Windows;

namespace Domino
{
    /// <summary>
    /// Логика взаимодействия для WindowChangeNames.xaml
    /// </summary>
    public partial class WindowChangeNames : Window
    {
        internal string UserName;
        internal string OppLeftName;
        internal string OppTopName;
        internal string OppRightName;

        public WindowChangeNames(string userName, string oppLeftName, string oppTopName, string oppRightName)
        {
            InitializeComponent();

            UserName = userName;
            OppLeftName = oppLeftName;
            OppTopName = oppTopName;
            OppRightName = oppRightName;

            TextBoxUserName.Text = userName;
            TextBoxOppLeftName.Text = oppLeftName;
            TextBoxOppTopName.Text = oppTopName;
            TextBoxOppRightName.Text = oppRightName;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            UserName = TextBoxUserName.Text != "" ? TextBoxUserName.Text : UserName;
            OppLeftName = TextBoxOppLeftName.Text != "" ? TextBoxOppLeftName.Text : OppLeftName;
            OppTopName = TextBoxOppTopName.Text != "" ? TextBoxOppTopName.Text : OppTopName;
            OppRightName = TextBoxOppRightName.Text != "" ? TextBoxOppRightName.Text : OppRightName;
            this.DialogResult = true;
        }
    }
}

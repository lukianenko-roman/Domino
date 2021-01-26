using System.Windows;
using System.Windows.Controls;

namespace Domino
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetInitials();
            BonesRepository.SetBones();
        }
        internal void SetInitials() // изначальные привязки и инициализации
        {
            UserBonesItems.DataContext = BonesRepository.UserZone.Grids;
            OpponentTopBonesItems.DataContext = BonesRepository.OpponentTopZone.Grids;
            OpponentLeftBonesItems.DataContext = BonesRepository.OpponentLeftZone.Grids;
            OpponentRigthBonesItems.DataContext = BonesRepository.OpponentRightZone.Grids;
            PlayedBonesItems.DataContext = BonesRepository.PlayedZone.Grids;
            RestBonesItems.DataContext = BonesRepository.RestZone.Grids;

            UserGridCell.DataContext = BonesRepository.UserZone;
            OppLeftGridCell.DataContext = BonesRepository.OpponentLeftZone;
            OppTopGridCell.DataContext = BonesRepository.OpponentTopZone;
            OppRightGridCell.DataContext = BonesRepository.OpponentRightZone;
            RestZoneGrid.DataContext = BonesRepository.RestZone;

            TextBoxCurrentScore.DataContext = BonesRepository.UserZone;

            TextBlockUserName.DataContext = BonesRepository.UserZone;
            TextBlockOppLeftName.DataContext = BonesRepository.OpponentLeftZone;
            TextBlockOppTopName.DataContext = BonesRepository.OpponentTopZone;
            TextBlockOppRightName.DataContext = BonesRepository.OpponentRightZone;

            UserOrOppenentZone.mainWindow = this;
            UserOrOppenentZone.canvasForAnimation = canvas;

            MenuItemSpeedN.IsChecked = true;
            MenuitemPoints100.IsChecked = true;
            MenuItemIsShowNames.IsChecked = true;
        }
        private void MenuItemSpeed_Checked(object sender, RoutedEventArgs e) // изменение скорости игры
        {
            MenuItem menuItem = (MenuItem)sender;
            UncheckAllMenuItemsExcept(menuItem, MenuItemSpeedVS, MenuItemSpeedS, MenuItemSpeedN, MenuItemSpeedF, MenuItemSpeedVF);
            switch (menuItem.Name)
            {
                case "MenuItemSpeedVS":
                    UserOrOppenentZone.DelayInMs = 4000;
                    break;
                case "MenuItemSpeedS":
                    UserOrOppenentZone.DelayInMs = 2000;
                    break;
                case "MenuItemSpeedN":
                    UserOrOppenentZone.DelayInMs = 1000;
                    break;
                case "MenuItemSpeedF":
                    UserOrOppenentZone.DelayInMs = 500;
                    break;
                case "MenuItemSpeedVF":
                    UserOrOppenentZone.DelayInMs = 250;
                    break;
            }
        }
        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            MenuItemsNewGameGroup.IsEnabled = false;
            MenuItemReset.IsEnabled = true;
            MenuItem clickedButton = (MenuItem)sender;
            switch (clickedButton.Name)
            {
                case "MenuItemNewGameOpp1":
                    BonesRepository.StartNewGame(1);
                    SetVisibilityForTextBlocks(Visibility.Visible, TextBlockUserName, TextBlockOppTopName);
                    break;
                case "MenuItemNewGameOpp2":
                    BonesRepository.StartNewGame(2);
                    SetVisibilityForTextBlocks(Visibility.Visible, TextBlockUserName, TextBlockOppLeftName, TextBlockOppTopName);
                    break;
                case "MenuItemNewGameOpp3":
                    BonesRepository.StartNewGame(3);
                    SetVisibilityForTextBlocks(Visibility.Visible, TextBlockUserName, TextBlockOppLeftName, TextBlockOppTopName, TextBlockOppRightName);
                    break;
                case "MenuItemNewGameOpp2x2":
                    BonesRepository.StartNewGame(0);
                    SetVisibilityForTextBlocks(Visibility.Visible, TextBlockUserName, TextBlockOppLeftName, TextBlockOppTopName, TextBlockOppRightName);
                    break;
            }
        }
        private void MenuItemReset_Click(object sender, RoutedEventArgs e)
        {
            BonesRepository.FullReset(true);
        }
        private void MenuItemExit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure?", "Exit", MessageBoxButton.YesNo) == MessageBoxResult.Yes) Close();
        }
        private void MenuItemLossAt_Checked(object sender, RoutedEventArgs e) // изменение максимального количества очков
        {
            MenuItem menuItem = (MenuItem)sender;
            UncheckAllMenuItemsExcept(menuItem, MenuitemPoints100, MenuitemPoints200, MenuitemPoints300);
            switch (menuItem.Name)
            {
                case "MenuitemPoints100":
                    BonesRepository.GameMaxScore = 100;
                    break;
                case "MenuitemPoints200":
                    BonesRepository.GameMaxScore = 200;
                    break;
                case "MenuitemPoints300":
                    BonesRepository.GameMaxScore = 300;
                    break;
            }
        }
        internal void SetVisibilityForTextBlocks(Visibility visibility, params TextBlock[] textBlocks)
        {
            if (BonesRepository.IsShowNames)
                foreach (TextBlock tb in textBlocks)
                    tb.Visibility = visibility;
        }
        private void UncheckAllMenuItemsExcept(MenuItem menuItemException, params MenuItem[] menuItems)
        {
            for (int i = 0; i < menuItems.Length; i++)
                if (menuItems[i] != menuItemException)
                    menuItems[i].IsChecked = false;
        }

        private void MenuItemDiffColors_Click(object sender, RoutedEventArgs e) // включение/выключение разных цветов точек на костях
        {
            MenuItem menuItem = (MenuItem)sender;
            BoneGraphics.IsDifferentColorOfPoints = menuItem.IsChecked;
            for (int i = 0; i < BonesRepository.bones.Length; i++)
                BonesRepository.bones[i].ReStylePoints(border.ActualWidth);
        }

        private void MenuItemIsShowNames_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = (MenuItem)sender;

            if (menuItem.IsChecked)
            {
                BonesRepository.IsShowNames = menuItem.IsChecked;
                switch (BonesRepository.activePlayers?.Count)
                {
                    case 2:
                        SetVisibilityForTextBlocks(Visibility.Visible, TextBlockUserName, TextBlockOppTopName);
                        break;
                    case 3:
                        SetVisibilityForTextBlocks(Visibility.Visible, TextBlockUserName, TextBlockOppLeftName, TextBlockOppTopName);
                        break;
                    case 4:
                        SetVisibilityForTextBlocks(Visibility.Visible, TextBlockUserName, TextBlockOppLeftName, TextBlockOppTopName, TextBlockOppRightName);
                        break;
                }
            }
            else
            {
                SetVisibilityForTextBlocks(Visibility.Hidden, TextBlockUserName, TextBlockOppLeftName, TextBlockOppTopName, TextBlockOppRightName);
                BonesRepository.IsShowNames = menuItem.IsChecked;
            }
        }
        private void MenuItemChangeNames_Click(object sender, RoutedEventArgs e)
        {
            WindowChangeNames windowChangeNames = new WindowChangeNames(BonesRepository.UserZone.Name, BonesRepository.OpponentLeftZone.Name, BonesRepository.OpponentTopZone.Name, BonesRepository.OpponentRightZone.Name) { Owner = this };
            if (windowChangeNames.ShowDialog() == true)
            {
                BonesRepository.UserZone.Name = windowChangeNames.UserName;
                BonesRepository.OpponentLeftZone.Name = windowChangeNames.OppLeftName;
                BonesRepository.OpponentTopZone.Name = windowChangeNames.OppTopName;
                BonesRepository.OpponentRightZone.Name = windowChangeNames.OppRightName;
                BonesRepository.UpdateCurrentScore();
            }
        }

        private void MenuItemAbout_Click(object sender, RoutedEventArgs e)
        {
            WindowAbout windowAbout = new WindowAbout() { Owner = this };
            windowAbout.ShowDialog();
        }
    }
}

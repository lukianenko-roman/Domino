using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Domino
{
    /// <summary>
    /// Логика взаимодействия для WindowScore.xaml
    /// </summary>
    public partial class WindowScore : Window
    {
        private static ObservableCollection<ScorePerRound> ScoresPerRounds { get; set; }
        private static int OpponentsQty { get; set; }
        public WindowScore()
        {
            InitializeComponent();
            DataGridScore.DataContext = ScoresPerRounds;

            ColumnUserPoints.Header = BonesRepository.UserZone.Name;
            ColumnOppLeftPoints.Header = BonesRepository.OpponentLeftZone.Name;
            ColumnOppTopPoints.Header = BonesRepository.OpponentTopZone.Name;
            ColumnOppRightPoints.Header = BonesRepository.OpponentRightZone.Name;

            switch (OpponentsQty)
            {
                case 0:
                    SetGridViewColumnUnvisible(ColumnUserPoints, ColumnOppLeftPoints, ColumnOppTopPoints, ColumnOppRightPoints, ColumnEmpty1);
                    break;
                case 1:
                    SetGridViewColumnUnvisible(ColumnOppLeftPoints, ColumnOppRightPoints, ColumnUserPairPoints, ColumnOppPairPoints, ColumnEmpty3);
                    break;
                case 2:
                    SetGridViewColumnUnvisible(ColumnOppRightPoints, ColumnUserPairPoints, ColumnOppPairPoints, ColumnEmpty3);
                    break;
                case 3:
                    SetGridViewColumnUnvisible(ColumnUserPairPoints, ColumnOppPairPoints, ColumnEmpty3);
                    break;
            }
        }
        internal static void SetNewScoreWindow(int opponentsQty)
        {
            ScoresPerRounds = new ObservableCollection<ScorePerRound>();
            OpponentsQty = opponentsQty;

            ScorePerRound.roundCounter = 0;
        }
        private void SetGridViewColumnUnvisible(params DataGridTextColumn[] columns)
        {
            foreach (DataGridTextColumn c in columns)
                c.Visibility = Visibility.Collapsed;
        }
        internal static void AddNewRoundScore(int equilScore)
        {
            AddNewRoundScore(equilScore, 0, 0, 0, 0);
        }
        internal static void AddNewRoundScore(int maxScore, List<int> indexes)
        {
            int user = 0, oppLeft = 0, oppTop = 0, oppRigth = 0;
            foreach (int i in indexes)

                switch (i)
                {
                    case 0:
                        user = maxScore;
                        break;
                    case 1:
                        if (OpponentsQty > 1)
                            oppLeft = maxScore;
                        else
                            oppTop = maxScore;
                        break;
                    case 2:
                        oppTop = maxScore;
                        break;
                    case 3:
                        oppRigth = maxScore;
                        break;
                }

            AddNewRoundScore(0, user, oppLeft, oppTop, oppRigth);
        }
        internal static void AddNewRoundScore(int equilScore, int userScore, int oppLeftScore, int oppTopScore = 0, int oppRightScore = 0)
        {
            if (ScoresPerRounds.Count > 1)
                ScoresPerRounds.RemoveAt(ScoresPerRounds.Count - 1);
            ScoresPerRounds.Add(new ScorePerRound(userScore, oppLeftScore, oppTopScore, oppRightScore, equilScore, ScoresPerRounds.Count > 0 ? ScoresPerRounds[^1].EqualScore : 0));
            ScoresPerRounds.Add(new ScoreTotal(ScoresPerRounds));
        }
    }
}

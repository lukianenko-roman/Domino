using System.Collections.ObjectModel;
using System.Linq;

namespace Domino
{
    class ScoreTotal : ScorePerRound
    {
        internal ScoreTotal(ObservableCollection<ScorePerRound> scoresPerRounds)
        {
            RoundNumber = "Total:";
            EqualScore = 0;
            UserScore = scoresPerRounds.Sum(s => s.UserScore);
            OppLeftScore = scoresPerRounds.Sum(s => s.OppLeftScore);
            OppTopScore = scoresPerRounds.Sum(s => s.OppTopScore);
            OppRightScore = scoresPerRounds.Sum(s => s.OppRightScore);
            UserPairScore = UserScore + OppTopScore;
            OpponentPairScore = OppLeftScore + OppRightScore;
        }
    }
}

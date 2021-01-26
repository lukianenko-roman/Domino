using System;
using System.Windows;
using System.Linq;

namespace Domino
{
    class ScorePerRound : DependencyObject
    {
        internal static int roundCounter;
        internal static readonly DependencyProperty RoundNumberProperty;
        internal static readonly DependencyProperty UserScoreProperty;
        internal static readonly DependencyProperty OppLeftScoreProperty;
        internal static readonly DependencyProperty OppTopScoreProperty;
        internal static readonly DependencyProperty OppRightScoreProperty;
        internal static readonly DependencyProperty EqualScoreProperty;
        internal static readonly DependencyProperty UserPairScoreProperty;
        internal static readonly DependencyProperty OpponentPairScoreProperty;

        internal string RoundNumber { get { return (string)GetValue(RoundNumberProperty); } set { SetValue(RoundNumberProperty, value); } }
        internal int UserScore { get { return (int)GetValue(UserScoreProperty); } set { SetValue(UserScoreProperty, value); } }
        internal int OppLeftScore { get { return (int)GetValue(OppLeftScoreProperty); } set { SetValue(OppLeftScoreProperty, value); } }
        internal int OppTopScore { get { return (int)GetValue(OppTopScoreProperty); } set { SetValue(OppTopScoreProperty, value); } }
        internal int OppRightScore { get { return (int)GetValue(OppRightScoreProperty); } set { SetValue(OppRightScoreProperty, value); } }
        internal int EqualScore { get { return (int)GetValue(EqualScoreProperty); } set { SetValue(EqualScoreProperty, value); } }
        internal int UserPairScore { get { return (int)GetValue(UserPairScoreProperty); } set { SetValue(UserPairScoreProperty, value); } }
        internal int OpponentPairScore { get { return (int)GetValue(OpponentPairScoreProperty); } set { SetValue(OpponentPairScoreProperty, value); } }
        static ScorePerRound()
        {
            RoundNumberProperty = DependencyProperty.Register("RoundNumber", typeof(string), typeof(ScorePerRound));
            UserScoreProperty = DependencyProperty.Register("UserScore", typeof(int), typeof(ScorePerRound));
            OppLeftScoreProperty = DependencyProperty.Register("OppLeftScore", typeof(int), typeof(ScorePerRound));
            OppTopScoreProperty = DependencyProperty.Register("OppTopScore", typeof(int), typeof(ScorePerRound));
            OppRightScoreProperty = DependencyProperty.Register("OppRightScore", typeof(int), typeof(ScorePerRound));
            EqualScoreProperty = DependencyProperty.Register("EqualScore", typeof(int), typeof(ScorePerRound));
            UserPairScoreProperty = DependencyProperty.Register("UserPairScore", typeof(int), typeof(ScorePerRound));
            OpponentPairScoreProperty = DependencyProperty.Register("OpponentPairScore", typeof(int), typeof(ScorePerRound));
        }
        internal ScorePerRound() { }
        internal ScorePerRound(int userScore, int oppLeftScore, int oppTopScore, int oppRightScore, int equilScore, int _lastEquilScore)
        {
            int lastEquilScore = _lastEquilScore;
            int playersWithNotZeroScoreInitial = new int[4] { userScore, oppLeftScore, oppTopScore, oppRightScore }.Count(c => c > 0);
            int playersWithNotZeroScoreLeft = playersWithNotZeroScoreInitial;

            RoundNumber = (++roundCounter).ToString();
            UserScore = userScore + (userScore == 0 || lastEquilScore == 0 ? 0 : AddLastEquilScore(lastEquilScore, playersWithNotZeroScoreInitial, ref playersWithNotZeroScoreLeft));
            OppLeftScore = oppLeftScore + (oppLeftScore == 0 || lastEquilScore == 0 ? 0 : AddLastEquilScore(lastEquilScore, playersWithNotZeroScoreInitial, ref playersWithNotZeroScoreLeft));
            OppTopScore = oppTopScore + (oppTopScore == 0 || lastEquilScore == 0 ? 0 : AddLastEquilScore(lastEquilScore, playersWithNotZeroScoreInitial, ref playersWithNotZeroScoreLeft));
            OppRightScore = oppRightScore + (oppRightScore == 0 || lastEquilScore == 0 ? 0 : AddLastEquilScore(lastEquilScore, playersWithNotZeroScoreInitial, ref playersWithNotZeroScoreLeft));

            EqualScore = equilScore + (equilScore == 0 ? 0 : lastEquilScore);
            UserPairScore = UserScore + OppTopScore;
            OpponentPairScore = OppLeftScore + OppRightScore;
        }
        private int AddLastEquilScore(int lastEquilScore, int playersWithNotZeroScore, ref int playersWithNotZeroScoreLeft)
        {
            if (playersWithNotZeroScoreLeft != 1)
            {
                playersWithNotZeroScoreLeft--;
                return (int)Math.Floor((double)lastEquilScore / playersWithNotZeroScore);
            }
            else
                return (int)Math.Ceiling((double)lastEquilScore / playersWithNotZeroScore);
        }
    }
}

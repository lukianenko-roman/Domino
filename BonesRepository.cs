using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Windows.Media;

namespace Domino
{
    internal static class BonesRepository // класс со всеми костями, со всеми зонами игроков, управление ходами, расчет результатов раундов
    {
        internal static BoneGraphics[] bones = new BoneGraphics[28]; // все игральные кости
        internal static int[] counterOfPlayedPoints = new int[7]; // подсчет, сколько раз половиной игральной кости сыграли за раунд (для расчета, есть ли возможные ходы, т.е. нет ли "рыбы")
        internal static int GameMaxScore { get; set; }
        internal static UserBonesZone UserZone { get; set; } = new UserBonesZone();
        internal static OpponentsBonesZone OpponentLeftZone { get; set; } = new OpponentsBonesZone(StateOfBone.ClosedHorizontal) { Name = "OppLeft" };
        internal static OpponentsBonesZone OpponentTopZone { get; set; } = new OpponentsBonesZone(StateOfBone.ClosedVerticalWithMargin) { Name = "OppTop" };
        internal static OpponentsBonesZone OpponentRightZone { get; set; } = new OpponentsBonesZone(StateOfBone.ClosedHorizontal) { Name = "OppRight" };
        internal static PlayedBonesZone PlayedZone { get; set; } = new PlayedBonesZone();
        internal static RestBonesZone RestZone { get; set; } = new RestBonesZone();
        internal static bool isGameActive;
        private static bool isPairGame; // игра в парах или нет
        internal static List<UserOrOppenentZone> activePlayers; // список активных игроков в текущей игре
        private static int activePlayerIndex;
        private static int equalScore = 0; // хранит результат предыдущего (или сумм предыдущих) раундов, если была ничья ("рыба")
        private static WindowScore windowScore;
        internal static bool IsShowNames { get; set; } = true;

        internal static int ActivePlayerIndex // индекс игрока, который ходит в текущий момент
        {
            get { return activePlayerIndex; }
            set
            {
                if (isGameActive)
                {
                    activePlayers[activePlayerIndex].IsTurnToMove = false;
                    if (value >= activePlayers.Count)
                        activePlayerIndex = 0;
                    else
                        activePlayerIndex = value;
                    activePlayers[activePlayerIndex].IsTurnToMove = true;
                }
            }
        }
        static BonesRepository() { }
        internal static void NextMove(bool isStillBones)
        {
            if (!isGameActive) return;
            if (isStillBones && !IsEndOfPossibleMoves())
                ActivePlayerIndex++;
            else
            {
                OpenAllBones(RestZone, OpponentLeftZone, OpponentRightZone, OpponentTopZone);
                AddScores();
                UpdateCurrentScore();
                ShowResults();
                if (!IsGameOver())
                {
                    ResetBetweenRounds();
                    StartNewRound();
                    FindFirstMover(false);
                    activePlayers[activePlayerIndex].IsTurnToMove = true;
                }
                else
                    FullReset(true);
            }
        }
        internal static void SetBones() // изначальная инициализация всех игральных костей
        {
            int k = 0;
            for (int i = 0; i < 7; i++)
                for (int j = i; j < 7; j++)
                {
                    bones[k] = new BoneGraphics(i, j);
                    k++;
                }
        }
        internal static void StartNewGame(int opponentsQty)
        {
            isGameActive = true;
            FullReset(false);
            switch (opponentsQty)
            {
                case 0:
                    isPairGame = true;
                    activePlayers = new List<UserOrOppenentZone> { UserZone, OpponentLeftZone, OpponentTopZone, OpponentRightZone };
                    break;
                case 1:
                    activePlayers = new List<UserOrOppenentZone> { UserZone, OpponentTopZone };
                    break;
                case 2:
                    activePlayers = new List<UserOrOppenentZone> { UserZone, OpponentLeftZone, OpponentTopZone };
                    break;
                case 3:
                    activePlayers = new List<UserOrOppenentZone> { UserZone, OpponentLeftZone, OpponentTopZone, OpponentRightZone };
                    break;
            }
            WindowScore.SetNewScoreWindow(opponentsQty);
            SetAllPlayersBackground(Brushes.AliceBlue, activePlayers.ToArray());
            StartNewRound();
            FindFirstMover();
            UpdateCurrentScore();
        }
        private static bool IsEndOfPossibleMoves()
        {
            if (counterOfPlayedPoints[PlayedZone.PointsLeftSide] == 8 && counterOfPlayedPoints[PlayedZone.PointsRightSide] == 8)
                return true;
            return false;
        }
        private static void OpenAllBones(params BonesZone[] bonesZone)
        {
            foreach (BonesZone zone in bonesZone)
                zone.SetOpenState();
        }
        private static void AddScores()
        {
            int[] scores = new int[activePlayers.Count];
            for (int i = 0; i < activePlayers.Count; i++)
            {
                for (int j = 0; j < activePlayers[i].Bones.Count; j++)
                    scores[i] += activePlayers[i].Bones[j].SumOfPoints;
                if (scores[i] == 0 && activePlayers[i].Bones.Count > 0)
                    scores[i] = 10;
            }
            if (isPairGame)
            {
                if (scores[0] + scores[2] > scores[1] + scores[3])
                {
                    UserZone.Score += scores[0] + (int)Math.Floor(equalScore / 2.0);
                    OpponentTopZone.Score += scores[2] + (int)Math.Ceiling(equalScore / 2.0);
                    equalScore = 0;
                    WindowScore.AddNewRoundScore(equalScore, scores[0], 0, scores[2], 0);
                }
                else
                {
                    if (scores[0] + scores[2] < scores[1] + scores[3])
                    {
                        OpponentLeftZone.Score += scores[1] + (int)Math.Floor(equalScore / 2.0);
                        OpponentRightZone.Score += scores[3] + (int)Math.Ceiling(equalScore / 2.0);
                        equalScore = 0;
                        WindowScore.AddNewRoundScore(equalScore, 0, scores[1], 0, scores[3]);
                    }
                    else
                    {
                        equalScore += scores[0] + scores[2];
                        WindowScore.AddNewRoundScore(equalScore);
                    }
                }
            }
            else
            {
                int maxScore = scores.Max();
                List<int> indexesWithMaxScore = new List<int>();
                for (int i = 0; i < activePlayers.Count; i++)
                    if (scores[i].Equals(maxScore))
                        indexesWithMaxScore.Add(i);
                if (indexesWithMaxScore.Count == activePlayers.Count)
                {
                    equalScore += scores[0];
                    WindowScore.AddNewRoundScore(equalScore);
                }
                else
                {
                    WindowScore.AddNewRoundScore(maxScore, indexesWithMaxScore);
                    int counter = equalScore;
                    for (int i = 0; i < indexesWithMaxScore.Count; i++)
                        if (i != indexesWithMaxScore.Count - 1)
                        {
                            activePlayers[indexesWithMaxScore[i]].Score += scores[indexesWithMaxScore[i]] + equalScore / indexesWithMaxScore.Count;
                            counter -= equalScore / indexesWithMaxScore.Count;
                        }
                        else
                        {
                            activePlayers[indexesWithMaxScore[i]].Score += scores[indexesWithMaxScore[i]] + counter;
                        }
                    equalScore = 0;
                }
            }
        }
        internal static void UpdateCurrentScore()
        {
            if (isPairGame)
            {
                UserZone.CurrentScore = $"Your pair: {UserZone.Score + OpponentTopZone.Score}\nOpponent pair: {OpponentLeftZone.Score + OpponentRightZone.Score}";
            }
            else
            {
                if (isGameActive)
                {
                    string str = "";
                    foreach (UserOrOppenentZone z in activePlayers)
                        str += $"{z.Name}: {z.Score}\n";
                    UserZone.CurrentScore = str;
                }
            }
        }
        private static void ShowResults()
        {
            windowScore = new WindowScore() { Owner = UserBonesZone.mainWindow };
            windowScore.ShowDialog();
        }
        private static bool IsGameOver()
        {
            if (isPairGame)
            {
                if (UserZone.Score + OpponentTopZone.Score < GameMaxScore && OpponentLeftZone.Score + OpponentRightZone.Score < GameMaxScore)
                    return false;
                else
                {
                    MessageBox.Show($"{(UserZone.Score + OpponentTopZone.Score > OpponentLeftZone.Score + OpponentRightZone.Score ? "Lose this time. Try again!" : "You win!")}");
                    isGameActive = false;
                    return true;
                }
            }
            else
            {
                if (activePlayers.Max(s => s.Score) >= GameMaxScore)
                {
                    MessageBox.Show($"{(UserZone.Score == activePlayers.Max(s => s.Score) ? "Lose this time. Try again!" : "You win!")}");
                    isGameActive = false;
                    return true;
                }
                else
                    return false;
            }
        }
        internal static void FullReset(bool isResetOnForm = false)
        {
            UserZone.CurrentScore = "";
            if (activePlayers?.Count > 0) ResetScores(activePlayers.ToArray());
            ResetBetweenRounds();
            activePlayers = new List<UserOrOppenentZone>();
            activePlayerIndex = 0;
            isPairGame = false;
            SetAllPlayersBackground(Brushes.White, UserZone, OpponentLeftZone, OpponentTopZone, OpponentRightZone);

            if (isResetOnForm)
            {
                UserOrOppenentZone.mainWindow.SetVisibilityForTextBlocks(Visibility.Hidden, UserOrOppenentZone.mainWindow.TextBlockUserName, UserOrOppenentZone.mainWindow.TextBlockOppLeftName, UserOrOppenentZone.mainWindow.TextBlockOppTopName, UserOrOppenentZone.mainWindow.TextBlockOppRightName);
                isGameActive = false;
                UserOrOppenentZone.mainWindow.MenuItemsNewGameGroup.IsEnabled = true;
                UserOrOppenentZone.mainWindow.MenuItemReset.IsEnabled = false;
            }
        }
        private static void SetAllPlayersBackground(Brush brush, params UserOrOppenentZone[] players)
        {
            foreach (UserOrOppenentZone player in players)
                player.SetBackgroundBrush(brush);
        }
        private static void ResetScores(params UserOrOppenentZone[] zones)
        {
            foreach (UserOrOppenentZone z in zones) z.Score = 0;
        }

        private static void ResetBetweenRounds()
        {
            counterOfPlayedPoints = new int[7];
            ResetZones(PlayedZone, RestZone);
            for (int i = 0; i < bones.Length; i++)
                bones[i].BoneGrid.Name = null;
        }
        private static void ResetZones(params BonesZone[] zones)
        {
            if (activePlayers?.Count > 0) foreach (UserOrOppenentZone z in activePlayers) z.Reset();
            foreach (BonesZone z in zones) z.Reset();
        }
        private static void StartNewRound()
        {
            SetStartBonesToEachPlayer();
        }
        private static void SetStartBonesToEachPlayer()
        {
            ShuffleBones();
            RestZone.CopyAllBones(bones);
            foreach (BonesZone zone in activePlayers)
                for (int i = 0; i < 7; i++)
                {
                    Random random = new Random();
                    int r = random.Next(0, RestZone.Bones.Count);
                    RestZone.TransferBone(zone, r, true, false, new Point(0, 0));
                }
        }
        private static void ShuffleBones()
        {
            Random random = new Random();
            for (int i = bones.Length - 1; i >= 1; i--)
            {
                int j = random.Next(0, i + 1);
                BoneGraphics tmp = bones[j];
                bones[j] = bones[i];
                bones[i] = tmp;
            }
        }
        private static void FindFirstMover(bool isNeedToSetMover = true)
        {
            for (int i = 1; i < 7; i++)
                for (int j = 0; j < activePlayers.Count; j++)
                {
                    if (!isNeedToSetMover) j = activePlayerIndex;
                    if (SetFirstMoverIfEquelHalfs(activePlayers[j], i))
                    {
                        if (isNeedToSetMover)
                            ActivePlayerIndex = j;
                        return;
                    }
                    if (!isNeedToSetMover) break;
                }
            SetFirstMoverIfNotEquelHalfs(isNeedToSetMover);
        }
        private static bool SetFirstMoverIfEquelHalfs(UserOrOppenentZone uooz, int i)
        {
            for (int j = 0; j < uooz.Bones.Count; j++)
                if (uooz.Bones[j].PointsQty1 == i && uooz.Bones[j].PointsQty2 == i)
                {
                    uooz.FirstBoneIndexToMove = j;
                    return true;
                }
            return false;
        }
        private static void SetFirstMoverIfNotEquelHalfs(bool isNeedToSetMover)
        {
            (int zone, int index) = (int.MaxValue, int.MaxValue);
            int minimumSum = int.MaxValue;
            for (int i = 0; i < activePlayers.Count; i++)
            {
                if (!isNeedToSetMover) i = activePlayerIndex;
                int minBoneSum = activePlayers[i].Bones.Min(m => m.SumOfPoints == 0 ? int.MaxValue : m.SumOfPoints);
                if (minBoneSum < minimumSum)
                {
                    minimumSum = minBoneSum;
                    zone = i;
                }
                if (!isNeedToSetMover) break;
            }
            for (int i = 0; i < activePlayers[zone].Bones.Count; i++)
                if (activePlayers[zone].Bones[i].SumOfPoints == minimumSum)
                {
                    index = i;
                    break;
                }

            activePlayers[zone].FirstBoneIndexToMove = index;

            if (isNeedToSetMover)
                ActivePlayerIndex = zone;
        }
    }
}
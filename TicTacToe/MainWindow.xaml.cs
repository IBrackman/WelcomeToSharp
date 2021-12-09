using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static TicTacToe.RoundModel;

namespace TicTacToe
{
    public partial class MainWindow : Window
    {
        internal GameModel Game { get; set; }
        internal Button[] Buttons { get; set; }
        internal Line Line { get; set; }
        private ISceneViewer CurrentScene { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Game = new GameModel();
            Buttons = new Button[9] {button1, button2, button3, button4, button5, button6, button7, button8, button9};

            startGame.Click += OnStart_Click;
            nextRound.Click += NextRound_Click;
            newGame.Click += NewGame_Click;
            restartGame.Click += RestartGame_Click;
            restartGame1.Click += RestartGame_Click;
            restartRound.Click += RestartRound_Click;
            undoBtn.Click += UndoBtn_Click;
            redoBtn.Click += RedoBtn_Click;

            foreach (var button in Buttons)
            {
                button.Click += Button_Click;
            }

            Line = new Line();
            field.Children.Add(Line);
            Line.Visibility = Visibility.Collapsed;
            Line.Stroke = Brushes.Black;

            CurrentScene = new WelcomeScene();
            SetView(Game);

            test1();
            test2();
            test3();
        }

        private void OnStart_Click(object sender, RoutedEventArgs e)
        {
            bool correct = int.TryParse(string.Join("", roundInput.Text.Where(char.IsDigit)), out int rounds);
            if (!correct || firstPlayer.Text.Replace(" ", "") == "" || secondPlayer.Text.Replace(" ", "") == "")
            {
                MessageBox.Show("Bad input!");
                return;
            }

            StartGame(rounds, firstPlayer.Text, secondPlayer.Text);
        }

        private void StartGame(int roundsCount, string name1, string name2)
        {
            Game.Start(roundsCount, name1, name2);
            CurrentScene = new RoundScene();
            SetView(Game);
        }

        void test1()
        {
            var game = new GameModel();
            game.Start(2, "Bill", "Bob");
            game.Step(0);
            game.Step(1);
            game.Undo();
            game.Redo();
            var x = game.Round.GetCell(0).Item;
            var y = game.Round.GetCell(1).Item;
        }

        void test2()
        {
            var game = new GameModel();
            game.Start(2, "Bill", "Bob");
            game.Step(0);
            game.Step(1);
            game.Step(3);
            game.Undo();
            game.Undo();
            game.Redo();
            game.Redo();
            game.Step(4);

            var x = game.Round.GetCell(0).Item;
            var y = game.Round.GetCell(1).Item;
            var z = game.Round.GetCell(3).Item;
            var w = game.Round.GetCell(4).Item;
        }

        void test3()
        {
            var game = new GameModel();
            game.Start(2, "Bill", "Bob");
          
            game.Step(0);
            var x = game.Round.GetCell(0).Item;

           
            game.Undo();
            x = game.Round.GetCell(0).Item;

            game.Redo();
            x = game.Round.GetCell(0).Item;

            game.Undo();
            x = game.Round.GetCell(0).Item;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            Game = new GameModel();
            Game.Init();
            SetView(Game);
        }

        private int GetIndex(Button btn)
        {
            for (var i = 0; i < Buttons.Length; i++)
            {
                if (Buttons[i] == btn)
                {
                    return i;
                }
            }

            throw new Exception("Not found");
        }

        private void NextRound_Click(object sender, RoutedEventArgs e)
        {
            Game.NextRound();
            SetView(Game);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var cellIndex = GetIndex((Button) sender);
            var gameChanged = Game.Step(cellIndex);
            if (gameChanged)
            {
                SetView(Game);
            }
        }

        private void RestartRound_Click(object sender, RoutedEventArgs e)
        {
            if (Game.RoundResult.WinnerIndex == 0)
                Game.Player1.Score--;
            else if (Game.RoundResult.WinnerIndex == 1) Game.Player2.Score--;
            Game.Round = new RoundModel();
            Game.RoundResult = new RoundResult();
            SetView(Game);
        }

        private void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            Game.Round = new RoundModel();
            Game.RoundResult = new RoundResult();
            Game.RoundNumber = 1;
            Game.Player1.Score = Game.Player2.Score = 0;
            Game.ActiveScene = 1;
            SetView(Game);
        }

        private void RedoBtn_Click(object sender, RoutedEventArgs e)
        {
            Game.Redo();

            SetView(Game);
        }

        private void UndoBtn_Click(object sender, RoutedEventArgs e)
        {
            Game.Undo();

            SetView(Game);
        }

        private void SetView(GameModel model)
        {
            if (model.ActiveScene == 0) CurrentScene = new WelcomeScene();

            if (model.ActiveScene == 1) CurrentScene = new RoundScene();

            if (model.ActiveScene == 2) CurrentScene = new EndGameScene();

            CurrentScene.Draw(this);
        }

        internal void DrawLine(int combination)
        {
            combination %= 8;
            var height = button1.Height;
            var width = button1.Width;
            int[][] LineIndexes =
            {
                new[] {0, 2}, new[] {3, 5}, new[] {6, 8}, new[] {0, 6}, new[] {1, 7}, new[] {2, 8}, new[] {0, 8},
                new[] {2, 6}
            };
            int StartIndex = LineIndexes[combination][0];
            int EndIndex = LineIndexes[combination][1];

            const int w = 90;
            const int a = w >> 1;
            const int b = w + a;
            const int c = 2 * w + a;
            int[] x = new[] {a, b, c, a, b, c, a, b, c};
            int[] y = new[] {a, a, a, b, b, b, c, c, c};

            Line.X1 = x[StartIndex];
            Line.Y1 = y[StartIndex];
            Line.X2 = x[EndIndex];
            Line.Y2 = y[EndIndex];
        }
    }

    public interface ISceneViewer
    {
        void Draw(MainWindow window);
    }

    public class WelcomeScene : ISceneViewer
    {
        public void Draw(MainWindow window)
        {
            window.scene01.Visibility = Visibility.Visible;
            window.scene02.Visibility = Visibility.Hidden;
            window.scene03.Visibility = Visibility.Hidden;
            window.firstPlayer.Text = string.Empty;
            window.secondPlayer.Text = string.Empty;
            window.roundInput.Text = string.Empty;
        }
    }

    public class RoundScene : ISceneViewer
    {
        public void Draw(MainWindow window)
        {
            window.scene01.Visibility = Visibility.Hidden;
            window.scene02.Visibility = Visibility.Visible;
            window.scene03.Visibility = Visibility.Hidden;

            window.firstGamerName.Content = $"{window.Game.Player1.Name}: ";
            window.firstGamerScore.Content = window.Game.Player1.Score;
            window.secondGamerName.Content = $"{window.Game.Player2.Name}: ";
            window.secondGamerScore.Content = window.Game.Player2.Score;
            window.currentRound.Content = $"{window.Game.RoundNumber}/{window.Game.RoundsCount}";

            for (int i = 0; i < window.Buttons.Length; i++)
            {
                var cell = window.Game.GetCell(i);
                window.Buttons[i].Content = cell.Content();
            }

            if (window.Game.RoundResult.Finished)
            {
                window.nextRound.Visibility = Visibility.Visible;
                window.firstGamerScore.Content = window.Game.Player1.Score.ToString();
                window.secondGamerScore.Content = window.Game.Player2.Score.ToString();

                if (window.Game.RoundResult.WinnerIndex >= 0)
                {
                    window.DrawLine(window.Game.RoundResult.WinCombination);
                    window.Line.Visibility = Visibility.Visible;
                }
            }
            else
            {
                window.nextRound.Visibility = Visibility.Collapsed;
                window.Line.Visibility = Visibility.Collapsed;
            }
        }
    }

    public class EndGameScene : ISceneViewer
    {
        public void Draw(MainWindow window)
        {
            window.scene01.Visibility = Visibility.Hidden;
            window.scene02.Visibility = Visibility.Hidden;
            window.scene03.Visibility = Visibility.Visible;
            window.result.Content = window.Game.GameResult;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WinApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GameModel model;

        public MainWindow()
        {
            InitializeComponent();
            InitHandlers();
            NewGame();
        }

        private void NewGame()
        {
            model = new GameModel();
            model.NewRound();
            SetView(model);
        }

        public void InitHandlers()
        {
            positive.Click += Positive_Click;
            negative.Click += Negative_Click;
            newGame.Click += NewGame_Click;
        }

        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void SetView(GameModel model)
        {    
            good.Content = model.GoodCounter.ToString();
            all.Content = model.AllCounter.ToString();
            testNumber.Content = model.Number.ToString();
            result.Content = model.RoundResult ? "Да" : "Нет";
            if (model.AllCounter > 0)
            {
                resultIs.Visibility = Visibility.Visible;
            }
            else
            {
                resultIs.Visibility = Visibility.Hidden;
                result.Content = string.Empty;
            }
           
        }

        private void Positive_Click(object sender, RoutedEventArgs e)
        {
            model.Check(0);
            model.NewRound();
            SetView(model);
        }

        private void Negative_Click(object sender, RoutedEventArgs e)
        {
            model.Check(1);
            model.NewRound();
            SetView(model);
        }
    }

    class GameModel
    {
        public int Number { get; private set; } = 0;
        public int GoodCounter { get; private set; } = 0;
        public int AllCounter { get; private set; } = 0;
        public bool RoundResult { get; private set; } = false;

        public void Check(int answer)
        {
            if (Number % 2 == answer)
            {
                GoodCounter++;
                RoundResult = true;
            }
            else
            {
                RoundResult = false;
            }
            AllCounter++;
        }

        public void NewRound()
        {
            var rnd = new Random();
            Number = rnd.Next();
        }
    }
}

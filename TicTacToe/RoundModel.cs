using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToe
{
    public class GameModel
    {
        private Stack<ICommand> undoHistory;
        private Stack<ICommand> redoHistory;

        public GameModel()
        {
            ActiveScene = 0;
            RoundsCount = 0;
            RoundNumber = 1;
            Round = new RoundModel();
            Run = false;
            undoHistory = new Stack<ICommand>();
            redoHistory = new Stack<ICommand>();
        }

        public int ActiveScene { get; set; }
        public int RoundsCount { get; set; }
        public int RoundNumber { get; set; }
        public RoundModel Round { get; set; }
        public RoundResult RoundResult { get; set; }
        public bool Run { get; set; }
        public Player Player1 { get; private set; }
        public Player Player2 { get; private set; }
        public string GameResult { get; set; }

        public Cell GetCell(int number)
        {
            return Round.GetCell(number);
        }

        public void Init()
        {
            RoundsCount = 0;
            RoundNumber = 1;
            ActiveScene = 0;
            Run = false;
        }

        public void Start(int roundsCount, string name1, string name2)
        {
            ActiveScene = 1;
            Run = true;
            RoundsCount = roundsCount;
            Player1 = new Player()
            {
                Name = name1,
                Score = 0
            };
            Player2 = new Player()
            {
                Name = name2,
                Score = 0
            };
            RoundNumber = 1;
            RoundResult = new RoundResult();
        }

        private bool IsValidStep(int index)
        {
            return Round.IsEmptyCell(index);
        }

        public bool Step(int index)
        {
            if (!Run) return false;
            if (!IsValidStep(index)) return false;
            if (RoundResult.Finished) return false;

            var cmd = new SmartBaseCommand(this, index);
            cmd.run();
            undoHistory.Push(cmd);
            redoHistory.Clear();
            Round.SwitchPlayer();

            return true;
        }

        public void Undo()
        {
            if (undoHistory.Count == 0) return;

            var cmd = undoHistory.Pop();
            redoHistory.Push(cmd);
            cmd.undo();
        }

        public void Redo()
        {
            if (redoHistory.Count == 0) return;

            var cmd = redoHistory.Pop();
            undoHistory.Push(cmd);
            cmd.run();
            Round.SwitchPlayer();
        }

        public void SwitchPlayer()
        {
            Round.SwitchPlayer();
        }

        public void NextRound()
        {
            RoundNumber++;
            Round = new RoundModel();
            RoundResult = new RoundResult();
        }
    }

    public struct State
    {
        public int LineIndex { get; set; }

        public int CurrentPlayer { get; set; }

        public RoundModel.CellType[] Field { get; set; }

        public State Clone()
        {
            return new State()
            {
                CurrentPlayer = CurrentPlayer,
                Field = (RoundModel.CellType[])Field.Clone(),
                LineIndex = LineIndex
            };
        }
    }

    public class Cell
    {

        public RoundModel.CellType Item { get; set; }

        private bool IsZero() => Item == RoundModel.CellType.Zero;

        private bool IsCross() => Item == RoundModel.CellType.X;

        public string Content()
        {
            string value = string.Empty;
            if (IsZero()) value = "O";
            if (IsCross()) value = "X";
            return value;
        }
    }

    public class RoundModel
    {
        public State state;

        public RoundModel()
        {
            state = new State()
            {
                Field = EmptyField(),
                CurrentPlayer = 0,
                LineIndex = 0
            };
        }

        public enum CellType
        {
            Empty,
            X,
            Zero
        }

        private CellType[] EmptyField()
        {
            return new CellType[9]
            {
                CellType.Empty,
                CellType.Empty,
                CellType.Empty,
                CellType.Empty,
                CellType.Empty,
                CellType.Empty,
                CellType.Empty,
                CellType.Empty,
                CellType.Empty
            };
        }

        public bool IsEmptyCell(int index)
        {
            return state.Field[index] == CellType.Empty;
        }

        public void Step(int index)
        {
            CellType cellType = state.CurrentPlayer == 0 ? CellType.X : CellType.Zero;
            state.Field[index] = cellType;
        }

        private int[][] Win =
        {
            new[] {0, 1, 2},
            new[] {3, 4, 5},
            new[] {6, 7, 8},
            new[] {0, 3, 6},
            new[] {1, 4, 7},
            new[] {2, 5, 8},
            new[] {0, 4, 8},
            new[] {2, 4, 6}
        };

        public RoundResult IsEnd(GameModel model)
        {
            var result = new RoundResult();
            
            if (Check(CellType.X))
            {
                result.WinnerIndex = model.RoundNumber % 2 == 1 ? 0 : 1;
                result.WinCombination = state.LineIndex;
                result.Finished = true;
                return result;
            }

            if (Check(CellType.Zero))
            {
                result.WinnerIndex = model.RoundNumber % 2 == 0 ? 0 : 1;
                result.WinCombination = state.LineIndex;
                result.Finished = true;
                return result;
            }

            result.WinnerIndex = -1;
            result.Finished = FieldIsFull();
            return result;
        }

        private bool FieldIsFull()
        {
            return state.Field.All(cell => cell != CellType.Empty);
        }

        private bool Check(CellType type)
        {
            int i = 0;
            foreach (var set in Win)
            {
                if (CheckWinSet(set, type))
                {
                    state.LineIndex = i;
                    return true;
                }

                i++;
            }
            return false;
        }

        private bool CheckWinSet(IReadOnlyList<int> set, CellType type)
        {
            return 
                (state.Field[set[0]] == type) &&
                (state.Field[set[1]] == type) &&
                (state.Field[set[2]] == type);
        }

        public void SwitchPlayer()
        {
            state.CurrentPlayer = (state.CurrentPlayer + 1) % 2;
        }

        public Cell GetCell(int number)
        {
            return new Cell()
            {
                Item = state.Field[number]
            };
        }

    }

    public class RoundResult
    {
        public RoundResult()
        {
            Finished = false;
            WinnerIndex = -1;
        }
        public int WinCombination { get; set; }
        public int WinnerIndex { get; set; }
        public bool Finished { get; set; }
    }

    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }

    public interface ICommand
    {

        void undo();

        void run();

    }

    public class SmartBaseCommand : ICommand
    {
        private GameModel Game;

        private int CellIndex;

        private RoundModel.CellType PrevSymbol;

        private int PrevPlayer;

        public SmartBaseCommand(GameModel game, int index)
        {
            this.Game = game;
            CellIndex = index;
        }

        public void undo()
        {
            Game.Round.state.CurrentPlayer = PrevPlayer;
            Game.Round.state.Field[CellIndex] = RoundModel.CellType.Empty;
        }

        public void run()
        {
            PrevPlayer = Game.Round.state.CurrentPlayer;
            PrevSymbol = Game.Round.state.Field[CellIndex];
            work();
        }

        private void work()
        {
            var round = Game.Round;
            round.Step(CellIndex);
            Game.RoundResult = Game.Round.IsEnd(Game);
            if (Game.RoundResult.Finished)
            {
                if (Game.RoundResult.WinnerIndex == 0) Game.Player1.Score++;
                else if (Game.RoundResult.WinnerIndex == 1) Game.Player2.Score++;

                if (Game.RoundNumber >= Game.RoundsCount)
                {
                    Game.ActiveScene = 2;
                    if (Game.Player1.Score == Game.Player2.Score)
                    {
                        Game.GameResult = "Ничья";
                    }
                    else
                    {
                        var winner = Game.Player1.Score > Game.Player2.Score ? Game.Player1.Name : Game.Player2.Name;
                        Game.GameResult = $"Победил {winner}";
                    }

                }
            }
        }
    }
}

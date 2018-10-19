using Checkers.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers.Agent
{
    public class Agent
    {
        private Random _rnd;
        private double _learn;
        public List<StateValue> States { get; }
        private double v;
        private List<StateValue> list;
        private IEnumerable<string> enumerable;

        public Agent(double learnRate, List<StateValue> previousStates)
        {
            _rnd = new Random();
            _learn = learnRate;
            States = previousStates;
        }

        public Move DoSomething(GameState state)
        {
            Move selectedMove = null;
            var rnd = _rnd.NextDouble();
            var currentState = States.FirstOrDefault(x => x.State.Equals(state.ToString()));
            if(currentState == null)
            {
                currentState = new StateValue { State = state.ToString(), Value = 0.5 };

                States.Add(currentState);
            }
            var allMoves = state.GetAllMoves();
            if (rnd < _learn)
            {
                selectedMove = allMoves[_rnd.Next(allMoves.Count())];
                return selectedMove;
            }
            var possibleStates = new List<StateValue>();
            foreach (var move in allMoves)
            {
                    var newState = state.MakeMove(move);
                    var newStateString = newState.ToString();
                    var found = States.FirstOrDefault(x => x.State.Equals(newStateString));
                    if (found == null)
                    {
                        PieceColor winner;

                        var value = newState.HasWinner(out winner) ? winner == PieceColor.White ? 1 : 0 : 0.5;
                        found = new StateValue
                        {
                            State = newStateString,
                            Value = value,
                        };
                        States.Add(found);
                    }

                found.Move = move;
                    possibleStates.Add(found);
            }
            

            var bestState = possibleStates.OrderByDescending(x => x.Value).First();
            currentState.Value = (currentState.Value + bestState.Value) / 2;
            return bestState.Move;
        }


    }
}

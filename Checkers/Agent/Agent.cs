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
        private List<StateValue> _states;
        private double v;
        private List<StateValue> list;

        public Agent(double learnRate, List<StateValue> previousStates)
        {
            _rnd = new Random();
            _learn = learnRate;
            _states = previousStates;
        }

        public Move DoSomething(GameState state)
        {
            Move selectedMove = null;
            var rnd = _rnd.NextDouble();
            var currentState = _states.FirstOrDefault(x => x.GameState.Equals(state.ToString()));
            if(currentState == null)
            {
                currentState = new StateValue { GameState = state.ToString(), Value = 0.5 };

                _states.Add(currentState);
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
                    var found = _states.FirstOrDefault(x => x.GameState.Equals(newStateString));
                    if (found == null)
                    {
                        PieceColor winner;

                        var value = newState.HasWinner(out winner) ? winner == PieceColor.White ? 1 : 0 : 0.5;
                        found = new StateValue
                        {
                            GameState = newStateString,
                            Value = value,
                        };
                        _states.Add(found);
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

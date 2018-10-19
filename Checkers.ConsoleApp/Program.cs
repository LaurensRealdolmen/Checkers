using Checkers;
using Checkers.Agent;
using Checkers.Game;
using System;
using System.Collections.Generic;
using Checkers.DataAccess.Dapper;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var repo = new StateRepository();

            var state = new GameState();
            var agent = new Agent(0.2, repo.GetStates());
            var gameCounter = 0;

            do
            {
                gameCounter++;
                var counter = 0;
                while (!state.HasWinner(out PieceColor winner))
                {

                    var move = agent.DoSomething(state);
                    if (move == null)
                        break;
                    counter++;
                    state = state.MakeMove(move);
                }


                System.Console.WriteLine($"done game {gameCounter}");
                state = new GameState();


                var items = agent.States;
                items.ForEach(i => repo.SaveState(i.State, i.Value));

            } while (gameCounter < 10);
        }
    }
}

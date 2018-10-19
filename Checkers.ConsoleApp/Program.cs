using Checkers;
using Checkers.Agent;
using Checkers.Game;
using System;
using System.Collections.Generic;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var state = new GameState();
            var agent = new Agent(0.2, new List<StateValue>());
            while(true)
            {
                var counter = 0;
                while(!state.HasWinner(out PieceColor winner))
                {

                    var move = agent.DoSomething(state);
                    if (move == null)
                        break;
                    counter++;
                        state = state.MakeMove(move);
                }
                System.Console.WriteLine("done game");
                state = new GameState();
            }
        }
    }
}

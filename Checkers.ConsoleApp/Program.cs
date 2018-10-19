using System;
using Checkers.Game;

namespace Checkers.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var gameState = new GameState();
            Console.WriteLine($"Game state overview: {gameState}.");
            Console.ReadKey();
        }
    }
}

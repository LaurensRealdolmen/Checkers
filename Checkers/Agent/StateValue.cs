using Checkers.Game;
using System;
using System.Collections.Generic;
using System.Text;

namespace Checkers

{
    public class StateValue
    {
        public string GameState { get; set; }
        public double Value { get; set; }
        public Move Move { get; set; }
    }
}

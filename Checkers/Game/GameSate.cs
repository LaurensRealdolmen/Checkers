using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Checkers.Game
{
    public class GameState
    {
        private readonly Board _board;
        private readonly Piece[,] _pieces;
        private readonly PieceColor _upNext;
        private readonly Tile _followUpTile;

        public GameState()
        {
            _board = new Board();
            _pieces = new Piece[_board.Tiles.GetLength(0), _board.Tiles.GetLength(1)];
            _upNext = PieceColor.Black;
            for (int i = 0; i < _board.Tiles.GetLength(0); i++)
            {
                for (int j = 0; j < _board.Tiles.GetLength(1); j++)
                {
                    if (i < 3 && _board.Tiles[i, j].Color == TileColor.Black)
                    {
                        var piece = new Piece(PieceColor.White);
                        _pieces[i, j] = piece;
                    }
                    if (i >= 5 && _board.Tiles[i, j].Color == TileColor.Black)
                    {
                        var piece = new Piece(PieceColor.Black);
                        _pieces[i, j] = piece;
                    }
                }
            }
        }

        public GameState(string inputString, PieceColor upNext, Tile followUpTile = null)
        {
            _board = new Board();
            _pieces = new Piece[_board.Tiles.GetLength(0), _board.Tiles.GetLength(1)];
            _upNext = upNext;
            _followUpTile = followUpTile;
            var positions = inputString.Split(';');
            foreach (var p in positions)
            {
                var pieceType = p.Split(':')[0];
                var color = int.Parse(pieceType.Split('.')[0]) == 0 ? PieceColor.Black : PieceColor.White;
                Piece piece = int.Parse(pieceType.Split('.')[1]) == 0 ? new Piece(color) : new King(color);
                var coords = p.Split(':')[1];
                _pieces[int.Parse(coords.Split(',')[0]), int.Parse(coords.Split(',')[1])] = piece;
            }
        }

        public Move[] GetAllMoves()
        {
            var result = new List<Move>();
            if (_followUpTile != null)
                return GetMovesForPiece(_followUpTile.X, _followUpTile.Y).Where(x => x.Overtake).ToArray();
            for (int i = 0; i < _pieces.GetLength(0); i++)
            {
                for (int j = 0; j < _pieces.GetLength(1); j++)
                {
                    if (_pieces[i, j] == null || _pieces[i, j].Color != _upNext) continue;
                    result.AddRange(GetMovesForPiece(i, j));
                }
            }
            if (result.Any(x => x.Overtake))
                return result.Where(x => x.Overtake).ToArray();
            return result.ToArray();
        }

        public Move[] GetMovesForPiece(int x, int y)
        {
            var result = new List<Move>();
            var piece = _pieces[x, y];
            var src = GetTile(x, y);
            if (piece != null && !(piece is King))
            {
                var left = GetTile(piece, x, y, Direction.Left);
                var right = GetTile(piece, x, y, Direction.Right);
                var lefBw = GetTile(piece, x, y, Direction.Left, true);
                var rightBw = GetTile(piece, x, y, Direction.Right, true);

                var leftMove = GetAvailableMove(src, piece, left, Direction.Left);
                var rightMove = GetAvailableMove(src, piece, right, Direction.Right);

                if (leftMove != null)
                    result.Add(leftMove);
                if (rightMove != null)
                    result.Add(rightMove);

                var backwardTakeLeft = GetBackWardTakeTIle(src, piece, lefBw, Direction.Left);
                var backwardTakeRight = GetBackWardTakeTIle(src, piece, rightBw, Direction.Right);

                if (backwardTakeLeft != null)
                    result.Add(backwardTakeLeft);
                if (backwardTakeRight != null)
                    result.Add(backwardTakeRight);
            }
            else if (piece is King)
            {
                return GetMovesForKng(piece, x, y);
            }
            return result.ToArray();
        }

        public GameState MakeMove(Move move)
        {
            var allMoves = GetAllMoves();
            if (allMoves.Contains(move))
            {
                Tile overtakeTile = null;
                if (move.Overtake)
                {
                    overtakeTile = FindOvertakenPiecePosition(move);
                }
                var builder = new StringBuilder();
                var playingPiece = _pieces[move.Source.X, move.Source.Y];
                for (int i = 0; i < _pieces.GetLength(0); i++)
                {
                    for (int j = 0; j < _pieces.GetLength(1); j++)
                    {
                        var piece = _pieces[i, j];
                        if (piece != null && !_board.Tiles[i, j].Equals(overtakeTile))
                        {
                            var x = i;
                            var y = j;
                            if (_board.Tiles[i, j].Equals(move.Source))
                            {
                                x = move.Destination.X;
                                y = move.Destination.Y;
                                if (piece.Color == PieceColor.White && move.Destination.X == 7 ||
                                    piece.Color == PieceColor.Black && move.Destination.X == 0)
                                    piece = new King(piece.Color);
                            }
                            builder.Append(piece.Color == PieceColor.White ? 1 : 0);
                            if (piece is King)
                                builder.Append(".1:");
                            else
                                builder.Append(".0:");
                            builder.Append($"{x},{y};");
                        }
                    }
                }
                var resultString = builder.ToString();
                resultString = resultString.Substring(0, resultString.Length - 1);
                return CalculateUpNext(resultString, move);
            }
            throw new ArgumentException("Move is not valid");
        }

        public bool HasWinner(out PieceColor winner) 
        {
            int blackCount = 0;
            int whiteCount = 0;
            for (int i = 0; i < _pieces.GetLength(0); i++)
            {
                for (int j = 0; j < _pieces.GetLength(1); j++)
                {
                    if (_pieces[i, j] == null) continue;
                    if (_pieces[i, j].Color == PieceColor.Black) blackCount++;
                    else whiteCount++;
                }
            }
            winner = PieceColor.Black;
            if (blackCount == 0) winner = PieceColor.White;
            return blackCount == 0 || whiteCount == 0;
        }

        public bool isTied()
        {
            return GetAllMoves().Count() == 0;
        }

        public bool HasFollowUpMove()
        {
            return _followUpTile != null;
        }

        private GameState CalculateUpNext(string inputString, Move move)
        {
            if (move.Overtake)
            {
                var newState = new GameState(inputString, _upNext);
                var followUpMoves = newState.GetMovesForPiece(move.Destination.X, move.Destination.Y);
                if (followUpMoves.Any(x => x.Overtake))
                {
                    return new GameState(inputString, _upNext, new Tile(move.Destination.X, move.Destination.Y, TileColor.Black));
                }
            }
            var color = _upNext == PieceColor.Black ? PieceColor.White : PieceColor.Black;
            return new GameState(inputString, color);
        }

        private Tile FindOvertakenPiecePosition(Move move)
        {
            var xDiff = move.Source.X - move.Destination.X;
            var yDiff = move.Source.Y - move.Destination.Y;
            var xFunc = GetFunc(xDiff);
            var yFunc = GetFunc(yDiff);
            return FindOvertakenPiecePositionRecursive(move.Source.X, move.Source.Y, xFunc, yFunc);
        }

        private Tile FindOvertakenPiecePositionRecursive(int x, int y, Func<int, int> xFunc, Func<int, int> yFunc)
        {
            var newX = xFunc(x);
            var newY = yFunc(y);
            if (_pieces[newX, newY] != null)
                return _board.Tiles[newX, newY];
            return FindOvertakenPiecePositionRecursive(newX, newY, xFunc, yFunc);
        }

        private Func<int, int> GetFunc(int i)
        {
            if (i < 0)
                return x => x + 1;
            else
                return x => x - 1;
        }

        private Tile GetTile(int x, int y)
        {
            if (x < 0 || x >= _board.Tiles.GetLength(0))
                return null;
            if (y < 0 || y >= _board.Tiles.GetLength(1))
                return null;
            return _board.Tiles[x, y];
        }

        private Tile GetTile(Piece piece, int x, int y, Direction direction, bool backwards = false)
        {
            var rowModifier = backwards ? -1 : 1;
            var nextX = piece.Color == PieceColor.White ? x + rowModifier : x - rowModifier;
            var nextY = direction == Direction.Right ? y + 1 : y - 1;
            return GetTile(nextX, nextY);
        }

        private Move[] GetMovesForKng(Piece piece, int x, int y)
        {
            var moves = new List<Move>();
            FindKingMovesRecursive(piece, x, y, moves, x, y, a => a + 1, b => b + 1);
            FindKingMovesRecursive(piece, x, y, moves, x, y, a => a - 1, b => b + 1);
            FindKingMovesRecursive(piece, x, y, moves, x, y, a => a + 1, b => b - 1);
            FindKingMovesRecursive(piece, x, y, moves, x, y, a => a - 1, b => b - 1);
            return moves.ToArray();
        }

        private void FindKingMovesRecursive(Piece piece, int srcX, int srcY, List<Move> moves, int x, int y, Func<int, int> xFunc, Func<int, int> yFunc, bool isStepOver = false)
        {
            var newX = xFunc(x);
            var newY = yFunc(y);
            if (newX == -1 || newX == _board.Tiles.GetLength(0) || newY == -1 || newY == _board.Tiles.GetLength(1))
                return;
            var pieceToCheck = _pieces[newX, newY];
            if (pieceToCheck == null)
            {
                moves.Add(new Move(GetTile(srcX, srcY), GetTile(newX, newY), isStepOver));
                FindKingMovesRecursive(piece, srcX, srcY, moves, newX, newY, xFunc, yFunc, isStepOver);
            }
            else if (pieceToCheck.Color == piece.Color)
            {
                return;
            }
            else if (!isStepOver)
            {
                FindKingMovesRecursive(piece, srcX, srcY, moves, newX, newY, xFunc, yFunc, true);
            }
        }

        private Move GetBackWardTakeTIle(Tile src, Piece executingPiece, Tile tileToCheck, Direction direction)
        {
            var rowModifier = executingPiece.Color == PieceColor.White ? -1 : 1;
            var columnModifier = direction == Direction.Left ? -1 : 1;
            if (tileToCheck != null)
            {
                var piece = _pieces[tileToCheck.X, tileToCheck.Y];
                if (piece == null)
                {
                    return null;
                }
                else if (piece.Color != executingPiece.Color)
                {
                    var tile = GetTile(tileToCheck.X + rowModifier, tileToCheck.Y + columnModifier);
                    if (tile != null)
                    {
                        var secondPiece = _pieces[tile.X, tile.Y];
                        if (secondPiece == null)
                            return new Move(src, tile, true);
                    }
                }
            }
            return null;
        }

        private Move GetAvailableMove(Tile src, Piece executingPiece, Tile tileToCheck, Direction direction)
        {
            var rowModifier = executingPiece.Color == PieceColor.White ? 1 : -1;
            var columnModifier = direction == Direction.Left ? -1 : 1;
            if (tileToCheck != null)
            {
                var piece = _pieces[tileToCheck.X, tileToCheck.Y];
                if (piece == null)
                {
                    return new Move(src, tileToCheck, false);
                }
                else if (piece.Color != executingPiece.Color)
                {
                    var tile = GetTile(tileToCheck.X + rowModifier, tileToCheck.Y + columnModifier);
                    if (tile != null)
                    {
                        var secondPiece = _pieces[tile.X, tile.Y];
                        if (secondPiece == null)
                            return new Move(src, tile, true);
                    }
                }
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is GameState))
                return false;
            var state = obj as GameState;
            if (state._pieces.GetLength(0) != _pieces.GetLength(0) || state._pieces.GetLength(1) != _pieces.GetLength(1))
                return false;
            for (int i = 0; i < _pieces.GetLength(0); i++)
            {
                for (int j = 0; j < _pieces.GetLength(1); j++)
                {
                    if ((_pieces[i, j] != null && state._pieces[i, j] == null) ||
                        (_pieces[i, j] == null && state._pieces[i, j] != null) ||
                        (_pieces[i, j] is King && !(state._pieces[i, j] is King)) ||
                        (!(_pieces[i,j] is King) && state._pieces[i,j] is King))
                        return false;
                }
            }
            return state._upNext == _upNext;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.Append(_upNext == PieceColor.Black ? "B$" : "W$");
            for (int i = 0; i < _pieces.GetLength(0); i++)
            {
                for (int j = 0; j < _pieces.GetLength(1); j++)
                {
                    if (_pieces[i, j] == null) continue;
                    builder.Append(_pieces[i, j].Color == PieceColor.White ? 1 : 0);
                    builder.Append(_pieces[i, j] is King ? ".1:" : ".0:");
                    builder.Append($"{i},{j};");
                }
            }
            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public override int GetHashCode()
        {
            var hashCode = -547111983;
            hashCode = hashCode * -1521134295 + EqualityComparer<Board>.Default.GetHashCode(_board);
            hashCode = hashCode * -1521134295 + EqualityComparer<Piece[,]>.Default.GetHashCode(_pieces);
            hashCode = hashCode * -1521134295 + _upNext.GetHashCode();
            return hashCode;
        }
    }

    public enum Direction
    {
        Left, Right
    }
}

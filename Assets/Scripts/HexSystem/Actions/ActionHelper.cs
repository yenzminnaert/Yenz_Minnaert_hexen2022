using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DAE.BoardSystem;

namespace DAE.HexSystem.Actions
{
    class ActionHelper<TCard, TPiece> where TPiece : IPiece where TCard : ICard
    {    

        private Board<IHex, TPiece> _board;
        private Grid<IHex> _grid;
        private TPiece _piece;
        private IHex _position;
        private CardType _card;

        private List<IHex> _validPositions = new List<IHex>();

        public ActionHelper(Board<IHex, TPiece> board, Grid<IHex> grid, IHex position, TPiece piece, CardType card)
        {
            _board = board;
            this._piece = piece;
            this._grid = grid;
            _position = position;
            this._card = card;
        }

        

        public delegate bool Validator(Board<IHex, TPiece> board, Grid<IHex> grid, TPiece piece, IHex position);
        public static bool IsEmptyTile(Board<IHex, TPiece> board, Grid<IHex> grid, TPiece piece, IHex position)
        {
            return !board.TryGetPieceAt(position, out _);
        }              
        public ActionHelper<TCard, TPiece> SelectSIngle(params Validator[] validators)
        {
            _validPositions.Add(_position);
            return this;
        }


        public ActionHelper<TCard, TPiece> StraightAction(int xOffset, int yOffset, int numTiles = int.MaxValue, params Validator[] validators)
        {
            if (!_board.TryGetPositionOf(_piece, out var position))
                return this;

            if (!_grid.TryGetCoordinateOf(position, out var coordinate))
                return this;

            var nextXCoordinate = coordinate.x + xOffset;
            var nextYCoordinate = coordinate.y + yOffset;

            var hasNextPosition = _grid.TryGetPositionAt(nextXCoordinate, nextYCoordinate, out var nextPosition);
            int step = 0;

            while (hasNextPosition && step <= numTiles)
            {

                var isOk = validators.All((v) => v(_board, _grid, _piece, nextPosition));
                if (!isOk)
                    return this;

           
                _validPositions.Add(nextPosition);       

                nextXCoordinate = coordinate.x + ((step + 1) * xOffset);
                nextYCoordinate = coordinate.y + ((step + 1) * yOffset);

                hasNextPosition = _grid.TryGetPositionAt(nextXCoordinate, nextYCoordinate, out nextPosition);

                step++;
            }

            return this;


        }
        public ActionHelper<TCard, TPiece> TargetedStraightAction(int xOffset, int yOffset, int numTiles = int.MaxValue, params Validator[] validators)
        {
            List<IHex> TempvalidPositions = new List<IHex>();

            if (!_board.TryGetPositionOf(_piece, out var positionPlayer))
                return this;

            if (!_grid.TryGetCoordinateOf(positionPlayer, out var coordinate))
                return this;

            var nextXCoordinate = coordinate.x + xOffset;
            var nextYCoordinate = coordinate.y + yOffset;

            var hasNextPosition = _grid.TryGetPositionAt(nextXCoordinate, nextYCoordinate, out var nextPosition);
            int step = 0;

            while (hasNextPosition && step < numTiles)
            {

                var isOk = validators.All((v) => v(_board, _grid, _piece, nextPosition));
                if (!isOk)
                    return this;

                //var hasPiece = _board.TryGetPieceAt(nextPosition, out var nextPiece);
                //if (!hasPiece)
                //{
                TempvalidPositions.Add(nextPosition);
                //}
                //else
                //{
                //    ////detect other pieces shit
                //    //if (nextPiece.PlayerID == _piece.PlayerID)
                //    //    return this;

                //    TempvalidPositions.Add(nextPosition);
                //    return this;
                //}

                nextXCoordinate = coordinate.x + ((step + 1) * xOffset);
                nextYCoordinate = coordinate.y + ((step + 1) * yOffset);

                hasNextPosition = _grid.TryGetPositionAt(nextXCoordinate, nextYCoordinate, out nextPosition);

                step++;
            }

            if (TempvalidPositions.Contains(_position))
            {
                foreach (var pos in TempvalidPositions)
                {
                    _validPositions.Add(pos);
                }
            }

            return this;
        }
        public ActionHelper<TCard, TPiece> TargetedPlusSides(int xOffset, int yOffset, int direction, int numTiles = int.MaxValue, params Validator[] validators)
        {
            List<IHex> TempvalidPositions = new List<IHex>();

            if (!_board.TryGetPositionOf(_piece, out var positionPlayer))
                return this;

            if (!_grid.TryGetCoordinateOf(positionPlayer, out var coordinate))
                return this;

            var nextXCoordinate = coordinate.x + xOffset;
            var nextYCoordinate = coordinate.y + yOffset;

            var hasNextPosition = _grid.TryGetPositionAt(nextXCoordinate, nextYCoordinate, out var nextPosition);

            var isOk = validators.All((v) => v(_board, _grid, _piece, nextPosition));
            if (!isOk)
                return this;

            TempvalidPositions.Add(nextPosition);

            if (TempvalidPositions.Contains(_position))
            {
                foreach (var pos in TempvalidPositions)
                {
                    _validPositions.Add(pos);
                }
                
            }
            else
                return this;

            var othertile1 = GetNextDirectionDown(direction);
            var nextXCoordinate1 = coordinate.x + (int)othertile1.x;
            var nextYCoordinate1 = coordinate.y + (int)othertile1.y;            
            if (_grid.TryGetPositionAt(nextXCoordinate1, nextYCoordinate1, out var nextPosition1))
            {
                var isOk1 = validators.All((v) => v(_board, _grid, _piece, nextPosition1));
                if (!isOk1)
                    return this;

                _validPositions.Add(nextPosition1);
            }

            var othertile2 = GetNextDirectionUp(direction);
            var nextXCoordinate2 = coordinate.x + (int)othertile2.x;
            var nextYCoordinate2 = coordinate.y + (int)othertile2.y;
            if (_grid.TryGetPositionAt(nextXCoordinate2, nextYCoordinate2, out var nextPosition2))
            {
                var isOk2 = validators.All((v) => v(_board, _grid, _piece, nextPosition));
                if (!isOk2)
                    return this;
                _validPositions.Add(nextPosition2);
            }
            
          
            return this;
        }

        #region directionmethods
        internal ActionHelper<TCard, TPiece> TargetedPlusSides(int numTiles = int.MaxValue, params Validator[] validators)
     => TargetedPlusSides((int)_directions[0].x, (int)_directions[0].y, 0, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargetedPlusSides1(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedPlusSides((int)_directions[1].x, (int)_directions[1].y, 1, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargetedPlusSides2(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedPlusSides((int)_directions[2].x, (int)_directions[2].y, 2, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargetedPlusSides3(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedPlusSides((int)_directions[3].x, (int)_directions[3].y, 3, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargetedPlusSides4(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedPlusSides((int)_directions[4].x, (int)_directions[4].y, 4, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargetedPlusSides5(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedPlusSides((int)_directions[5].x, (int)_directions[5].y, 5, numTiles, validators);


        internal ActionHelper<TCard, TPiece> Direction0(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_directions[0].x, (int)_directions[0].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> Direction1(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_directions[1].x, (int)_directions[1].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> Direction2(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_directions[2].x, (int)_directions[2].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> Direction3(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_directions[3].x, (int)_directions[3].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> Direction4(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_directions[4].x, (int)_directions[4].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> Direction5(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_directions[5].x, (int)_directions[5].y, numTiles, validators);

        internal ActionHelper<TCard, TPiece> TargettedDirection0(int numTiles = int.MaxValue, params Validator[] validators)
        => TargetedStraightAction((int)_directions[0].x, (int)_directions[0].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargettedDirection1(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedStraightAction((int)_directions[1].x, (int)_directions[1].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargettedDirection2(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedStraightAction((int)_directions[2].x, (int)_directions[2].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargettedDirection3(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedStraightAction((int)_directions[3].x, (int)_directions[3].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargettedDirection4(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedStraightAction((int)_directions[4].x, (int)_directions[4].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> TargettedDirection5(int numTiles = int.MaxValue, params Validator[] validators)
         => TargetedStraightAction((int)_directions[5].x, (int)_directions[5].y, numTiles, validators);


        internal ActionHelper<TCard, TPiece> DiagonalDirection0(int numTiles = int.MaxValue, params Validator[] validators)
        => StraightAction((int)_diagonalDirections[0].x, (int)_diagonalDirections[0].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> DiagonalDirection1(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_diagonalDirections[1].x, (int)_diagonalDirections[1].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> DiagonalDirection2(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_diagonalDirections[2].x, (int)_diagonalDirections[2].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> DiagonalDirection3(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_diagonalDirections[3].x, (int)_diagonalDirections[3].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> DiagonalDirection4(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_diagonalDirections[4].x, (int)_diagonalDirections[4].y, numTiles, validators);
        internal ActionHelper<TCard, TPiece> DiagonalDirection5(int numTiles = int.MaxValue, params Validator[] validators)
         => StraightAction((int)_diagonalDirections[5].x, (int)_diagonalDirections[5].y, numTiles, validators);

        #endregion

        public Vector2 GetNextDirectionDown(int currentDirection)
        {

            if (currentDirection - 1 == -1)
            {
                return _directions[5];
            }
            else return _directions[currentDirection - 1];
        }

        public Vector2 GetNextDirectionUp(int currentDirection)
        {

            if (currentDirection + 1 == 6 )
            {
                return _directions[0];
            }
            else return _directions[currentDirection + 1];
        }

        public Vector2[] _directions =
            new Vector2[6]{new Vector2(1,0), new Vector2(1,-1), new Vector2(0,-1),
            new Vector2(-1,0), new Vector2(-1,1), new Vector2(0,1)};

        public Vector2[] _diagonalDirections =
            new Vector2[6] { new Vector2(+2, -1), new Vector2(+1, -2), new Vector2(-1, -1),
            new Vector2(-2, +1), new Vector2(-1, +2), new Vector2(+1, +1) };

        #region hexmath stuf




        public Vector2 HexAdd(Vector2 hexA, Vector2 HexB)
        {
            return new Vector2(hexA.x + HexB.x, hexA.y + HexB.y);
        }

        public Vector2 HexSubstract(Vector2 hexA, Vector2 HexB)
        {
            return new Vector2(hexA.x - HexB.x, hexA.y - HexB.y);
        }

        public Vector2 HexMultiply(Vector2 hexA, Vector2 HexB)
        {
            return new Vector2(hexA.x * HexB.x, hexA.y * HexB.y);
        }

        public int HexLenght(Vector2 hex)
        {
            return (int)((Mathf.Abs(hex.x) + (Mathf.Abs(hex.y)) + (-(Mathf.Abs(hex.x) - (Mathf.Abs(hex.x))))));
        }

        public int HexDistance(Vector2 hexA, Vector2 hexB)
        {
            return HexLenght(HexSubstract(hexA, hexB));
        }

        public Vector2 HexDirection(int direction)
        {
            return _directions[direction];
        }

        public Vector2 HexDiagonalDirection(int diagonalDirection)
        {
            return _diagonalDirections[diagonalDirection];
        }

        public Vector2 HexNeighbour(Vector3 hex, int direction)
        {
            return HexAdd(hex, HexDirection(direction));
        }


        #endregion
        internal List<IHex> Collect()
        {
            return _validPositions;
        }
    }
}
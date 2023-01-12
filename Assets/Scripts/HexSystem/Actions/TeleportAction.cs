using DAE.BoardSystem;
using DAE.HexSystem;
using DAE.HexSystem.Actions;
using DAE.ReplaySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.HexSystem.Actions
{

    class TeleportAction<TCard, TPiece> : ActionBase<TCard, TPiece> where TPiece : IPiece where TCard : ICard
    {

        public TeleportAction(ReplayManager replayManager) : base(replayManager)
        {

        }

        public override void ExecuteAction(Board<IHex, TPiece> board, Grid<IHex> grid, IHex position, TPiece piece, CardType card)
        {

            board.TryGetPositionOf(piece, out var fromPosition);

            board.Move(piece, position);

            Action forward = () =>
            {              

                board.Move(piece, position);
            };

            Action backward = () =>
            {
                board.Move(piece, fromPosition);                
            };




            ReplayManager.Execute(new DelegateReplayCommandMove(forward, backward));
        }

        public override List<IHex> IsolatedPositions(Board<IHex, TPiece> board, Grid<IHex> grid, IHex position, TPiece piece, CardType card)
        {
            ActionHelper<TCard, TPiece> actionHelper = new ActionHelper<TCard, TPiece>(board, grid, position, piece, card);
            actionHelper.SelectSIngle(ActionHelper<TCard, TPiece>.IsEmptyTile);

            return actionHelper.Collect();
        }

        public override List<IHex> Validpositions(Board<IHex, TPiece> board, Grid<IHex> grid, IHex position, TPiece piece, CardType card)
        {
            ActionHelper<TCard, TPiece> actionHelper = new ActionHelper<TCard, TPiece>(board, grid, position, piece, card);
            actionHelper.SelectSIngle(ActionHelper<TCard, TPiece>.IsEmptyTile);

            return actionHelper.Collect();

        }
    }
}

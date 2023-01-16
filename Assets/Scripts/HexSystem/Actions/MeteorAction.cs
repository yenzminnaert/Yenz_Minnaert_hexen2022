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
    class MeteorAction<TCard, TPiece> : ActionBase<TCard, TPiece> where TPiece : IPiece where TCard : ICard
    {
        public MeteorAction(ReplayManager replayManager) : base(replayManager)
        {

        }

        public override void ExecuteAction(Board<IHex, TPiece> board, Grid<IHex> grid, IHex position, TPiece piece, CardType card)
        {
            var templist = new List<IHex>();
            var templistenemies = new List<TPiece>();

            foreach (var hex in IsolatedPositions(board, grid, position, piece, card))
            {
                if (board.TryGetPieceAt(hex, out var enemy))
                {
                    board.Take(enemy);

                    templist.Add(hex);
                    templistenemies.Add(enemy);
                }
            }

            Action forward = () =>
            {
                for (int i = 0; i <= templist.Count - 1; i++)
                {
                    var hex = templist[i];
                    var piece = templistenemies[i];
                    board.Take(piece);
                }
            };

            Action backward = () =>
            {
                for (int i = 0; i <= templist.Count - 1; i++)
                {
                    var hex = templist[i];
                    var piece = templistenemies[i];
                    board.Place(piece, hex);
                }
            };
        }

        public override List<IHex> IsolatedPositions(Board<IHex, TPiece> board, Grid<IHex> grid, IHex position, TPiece piece, CardType card)
        {
            ActionHelper<TCard, TPiece> actionHelperPartual = new ActionHelper<TCard, TPiece>(board, grid, position, piece, card);

            actionHelperPartual.Meteor0(10)
                       .Meteor1(10)
                       .Meteor2(10)
                       .Meteor3(10)
                       .Meteor4(10)
                       .Meteor5(10);

            return actionHelperPartual.Collect();
        }

        // preview attack
        public override List<IHex> Validpositions(Board<IHex, TPiece> board, Grid<IHex> grid, IHex position, TPiece piece, CardType card)
        {
            ActionHelper<TCard, TPiece> actionHelper = new ActionHelper<TCard, TPiece>(board, grid, position, piece, card);

            actionHelper.Meteor0(10)
                       .Meteor1(10)
                       .Meteor2(10)
                       .Meteor3(10)
                       .Meteor4(10)
                       .Meteor5(10);

            return actionHelper.Collect();

        }
    }
}

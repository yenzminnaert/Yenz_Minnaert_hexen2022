using DAE.BoardSystem;
using DAE.Gamesystem;
using DAE.HexSystem;
using DAE.StateSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAE.GameSystem.GameStates
{
    class GamePlayState : GameStateBase
    {

        private ActionManager<Card, Piece> _actionManager;
        private Board<IHex, Piece> _board;
        private Deck _deck;

        public GamePlayState(StateMachine<GameStateBase> stateMachine, Board<IHex, Piece> board, ActionManager<Card, Piece> moveManager, PlayerHand playerhand, Deck deck) : base(stateMachine)
        {

            _deck = deck;
            _actionManager = moveManager;
            _board = board;

            _deck.EqualizeDecks();
            _deck.ShuffleCurrentDeck();
            _deck.DrawCard();
            _deck.DrawCard();
            _deck.DrawCard();
            _deck.DrawCard();
            _deck.DrawCard();
            _deck.InstantiateHandGOs();
        }


        internal override void Backward()
        {
            StateMachine.MoveToState(GameState.ReplayState);
        }

        internal override void HighLightNew(Piece piece, Hex position, Card card)
        {
            var validpositions = _actionManager.ValidPisitionsFor(piece, position, card._cardType);
            var IsolatedPositions = _actionManager.IsolatedValidPisitionsFor(piece, position, card._cardType);

            if (!validpositions.Contains(position))
            {
                foreach (var hex in validpositions)
                {
                    hex.Activate();
                }
            }

            if (IsolatedPositions.Contains(position))
            {
                foreach (var hex in IsolatedPositions)
                {
                    hex.Activate();
                }
            };
        }

        internal override void UnHighlightOld(Piece piece, Hex position, Card card)
        {
            var validpositions = _actionManager.ValidPisitionsFor(piece, position, card._cardType);
            var IsolatedPositions = _actionManager.IsolatedValidPisitionsFor(piece, position, card._cardType);

            foreach (var hex in validpositions)
            {
                hex.Deactivate();
            }

            foreach (var hex in IsolatedPositions)
            {
                position.Deactivate();
            }
        }

        internal override void OnDrop(Piece piece, Hex position, Card card)
        {
            var validpositions = _actionManager.ValidPisitionsFor(piece, position, card._cardType);
            var IsolatedPositions = _actionManager.IsolatedValidPisitionsFor(piece, position, card._cardType);

            if (IsolatedPositions.Contains(position))
            {
                _actionManager.Action(piece, position, card._cardType);

                _deck.ExecuteCard(card);
            }

            foreach (var hex in validpositions)
            {
                hex.Deactivate();
            }
        }
    }

}

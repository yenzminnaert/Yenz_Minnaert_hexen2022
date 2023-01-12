using DAE.BoardSystem;
using DAE.HexSystem.Actions;
using DAE.Commons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DAE.ReplaySystem;

namespace DAE.HexSystem
{
    public class ActionManager<TCard,TPiece> where TPiece : IPiece where TCard : ICard
    {
  
        private MultiValueDictionary<CardType, ICheckPosition<TCard, TPiece>> _actions = new MultiValueDictionary<CardType, ICheckPosition<TCard, TPiece>>();
        private readonly Board<IHex, TPiece> _board;
        private readonly Grid<IHex> _grid;
        private readonly ReplayManager _replayManager;

        public ActionManager(Board<IHex, TPiece> board, Grid<IHex> grid, ReplayManager replayManager)
        {
            //cardtype?
            _board = board;
            _grid = grid;
            _replayManager = replayManager;

            InitializeMoves();
        }

        

        private void InitializeMoves()
        {
            
            _actions.Add(CardType.Beam, new LaserBeamAction<TCard, TPiece>(_replayManager));

            _actions.Add(CardType.Thunderclap, new ThunderClapAction<TCard, TPiece>(_replayManager));

            _actions.Add(CardType.Teleport, new TeleportAction<TCard, TPiece>(_replayManager));

            _actions.Add(CardType.Push, new CleaveAction<TCard, TPiece>(_replayManager));                

        }

        public List<IHex> ValidPisitionsFor(TPiece piece, IHex position, CardType cardType)
        {
            return _actions[cardType]
                .Where(m => m.CanExecute(_board, _grid, position, piece, cardType))
                .SelectMany(m => m.Validpositions(_board, _grid, position, piece, cardType)/*.Contains(position)*/)
                .ToList();
        }

        public List<IHex> IsolatedValidPisitionsFor(TPiece piece, IHex position, CardType cardType)
        {
            return _actions[cardType]
                .Where(m => m.CanExecute(_board, _grid, position, piece, cardType))
                .SelectMany(m => m.IsolatedPositions(_board, _grid, position, piece, cardType)/*.Contains(position)*/)
                .ToList();
        }

        public void Action(TPiece piece, IHex position, CardType cardType)
        {
            _actions[cardType]
            .Where(m => m.CanExecute(_board, _grid, position, piece, cardType))
            .First(m => m.IsolatedPositions(_board, _grid, position, piece, cardType).Contains(position))
            .ExecuteAction(_board, _grid, position, piece, cardType);
        }
       
    }
    }

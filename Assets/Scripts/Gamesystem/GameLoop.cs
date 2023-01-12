using DAE.BoardSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using DAE.HexSystem;
using DAE.StateSystem;
using DAE.GameSystem.GameStates;
using DAE.ReplaySystem;
namespace DAE.Gamesystem
{
    public class GameLoop : MonoBehaviour
    {
        [SerializeField]
        private PositionHelper _positionHelper;
        [SerializeField]
        private GenerateBoard _generateboard;
        [SerializeField]
        private Transform _boardParent;

        [Header("GenerateBoard")]
        public int Rows = 8;
        public int Columns = 8;
        public int Tileradius = 1;
        public GameObject hex;
        public GenerationShapes MapShape = GenerationShapes.Hexagon;

        public Deck _deckview;
        public PlayerHand _playerhand;
        private ActionManager<Card, Piece> _actionManager;

        public Card _currentCard;

        private Grid<IHex> _grid;
        private Board<IHex, Piece> _board;

        public Piece Player;

        private StateMachine<GameStateBase> _gameStateMachine;

        void Start()
        {
            _positionHelper.TileRadius = Tileradius;
            _generateboard.GenerateBoardView(Rows, Columns, 1, MapShape, _positionHelper, hex, _boardParent);

            _grid = new Grid<IHex>(Rows, Columns);
            ConnectGrid(_grid);
            _board = new Board<IHex, Piece>();
            ConnectPiece(_grid, _board);

            var replayManager = new ReplayManager();

            _actionManager = new ActionManager<Card, Piece>(_board, _grid, replayManager);
            _deckview.InitiReplayManager(replayManager);

            _gameStateMachine = new StateMachine<GameStateBase>();
            _gameStateMachine.Register(GameState.GamePlayState, new GamePlayState(_gameStateMachine, _board, _actionManager, _playerhand, _deckview));
            _gameStateMachine.Register(GameState.ReplayState, new ReplayState(_gameStateMachine, replayManager));

            _gameStateMachine.InitialState = GameState.GamePlayState;                    

            BoardListereners();
        }



     


        private void BoardListereners()
        {
            _board.moved += (s, e) =>
            {
                if (_grid.TryGetCoordinateOf(e.ToPosition, out var toCoordinate))
                {
                    var worldPosition = _positionHelper.ToWorldPosition(_boardParent, toCoordinate.x, toCoordinate.y);

                    e.Piece.MoveTo(worldPosition);
                }
            };

            _board.placed += (s, e) =>
            {

                if (_grid.TryGetCoordinateOf(e.ToPosition, out var toCoordinate))
                {
                    var worldPosition = _positionHelper.ToWorldPosition(_boardParent, toCoordinate.x, toCoordinate.y);


                    e.Piece.Place(worldPosition);
                }

            };

            _board.taken += (s, e) =>
            {
                e.Piece.Taken();
            };
        }

        private void ConnectGrid(Grid<IHex> grid)
        {
            var hexes = FindObjectsOfType<Hex>();
            foreach (var hex in hexes)
            {

                hex.Dropped += (s, e) => _gameStateMachine.CurrentState.OnDrop(Player, hex, e.Card);
                hex.Entered += (s, e) => _gameStateMachine.CurrentState.HighLightNew(Player,hex, e.Card);
                hex.Exitted += (s, e) => _gameStateMachine.CurrentState.UnHighlightOld(Player,hex, e.Card);               


                var gridpos = _positionHelper.ToGridPosition(_boardParent, hex.transform.position);

                grid.Register((int)gridpos.x, (int)gridpos.y, hex);

                hex.gameObject.name = $"tile {(int)gridpos.x},{(int)gridpos.y}";
            }
        }

        private void ConnectPiece(Grid<IHex> grid, Board<IHex, Piece> board)
        {
            var pieces = FindObjectsOfType<Piece>();
            foreach (var piece in pieces)
            {
                var gridpos = _positionHelper.ToGridPosition(_boardParent, piece.transform.position);
                if (grid.TryGetPositionAt((int)gridpos.x, (int)gridpos.y, out var position))
                {                 
                    board.Place(piece, position);
                }
            }
        }

        public void Forward()
                 => _gameStateMachine.CurrentState.Forward();
        public void Backward()
                 => _gameStateMachine.CurrentState.Backward();


    }

}

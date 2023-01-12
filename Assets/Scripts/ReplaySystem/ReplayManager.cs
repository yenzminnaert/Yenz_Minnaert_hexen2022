using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DAE.ReplaySystem
{

    public class ReplayManager
    {
        private List<IReplayableCommand> _commands = new List<IReplayableCommand>();
        private int _currentPosition = -1;

        public bool IsAtEnd => (_currentPosition + 1) == _commands.Count;

        public void Execute(IReplayableCommand command)
        {
            ForwardToEnd();

            _commands.Add(command);
            _currentPosition += 1;
            _commands[_currentPosition].Forward();

        }

        public void ForwardToEnd()
        {

            

            while (!IsAtEnd)
            {
                Forward();
            }
        }

        public bool Forward()
        {
            if (!IsAtEnd)
            {
                _currentPosition += 1;

                var nextCommand = _commands[_currentPosition];
                nextCommand.Forward();

                return true;
            }

            return false;
        }

        public void Backward()
        {
            if (_currentPosition >= 0)
            {
                var currentCommand = _commands[_currentPosition];
                currentCommand.Backward();

                _currentPosition -= 1;
            }

        }
    }
}

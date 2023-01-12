using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DAE.HexSystem;
using System.Linq;
using DAE.ReplaySystem;
using System;
using Random = UnityEngine.Random;

namespace DAE.Gamesystem
{
    

    public class Deck : MonoBehaviour, IDeck<CardData>
    {
        protected ReplayManager ReplayManager;



        //[SerializeField]
        //private int _maxdecksize;

        [SerializeField]
        private List<CardData> _currentDeckList;

        [SerializeField]
        private List<CardData> _startingDeckList;
        
        private List<CardData> _temporaryCardList;

        [SerializeField]
        private List<CardData> _playerhandList;

        [SerializeField]
        private List<CardData> _discardList;

        [SerializeField]
        private GameObject CardBase;

        public GameObject HandView;

        private List<CardData> templist = new List<CardData>();
        private List<CardData> tempDrawnlist = new List<CardData>();


        //shuffle shit, generate new deck etc
        //public int DeckSize => _decksize;
        public List<CardData> CurrentDeckList => _currentDeckList;
        public List<CardData> StartingDecklist => _startingDeckList;      

        public List<CardData> TemporaryCardsList => _temporaryCardList;

        public List<CardData> PlayerHandList => _playerhandList;

        public List<CardData> DiscardList => _discardList;

       public void InitiReplayManager(ReplayManager replayManager)
        {
            ReplayManager = replayManager;
         
        }

        public void DrawCard()
        {
            _playerhandList.Add(CurrentDeckList[0]);        
            CurrentDeckList.RemoveAt(0);            
        }

        public void EqualizeDecks()        
        {         
            CurrentDeckList.AddRange(StartingDecklist);
        }
        public List<CardData> ShuffleCurrentDeck()
        {
            return _currentDeckList.OrderBy(x => Random.value).ToList();
        }

        //for prototype im working on
        public List<CardData> ShuffleStartingDeck()
        {
            return _startingDeckList.OrderBy(x => Random.value).ToList();
        }

        public void ExecuteCard(Card cardo)
        {       
            
            templist.Add(cardo.CardData);
            
            cardo.Used();
            ClearHandGO();
            InstantiateHandGOs();

            Action forward = () =>
            {
                _playerhandList.Add(CurrentDeckList[0]);
                tempDrawnlist.Add(CurrentDeckList[0]);
                CurrentDeckList.RemoveAt(0);

                _discardList.Add(templist[templist.Count - 1]);
                _playerhandList.Remove(templist[templist.Count-1]);

                ClearHandGO();
                InstantiateHandGOs();
            };


            Action backward = () =>
            {
                _playerhandList.Remove(tempDrawnlist[tempDrawnlist.Count - 1]);
                _currentDeckList.Insert(0,(tempDrawnlist[tempDrawnlist.Count - 1]));


                _playerhandList.Add(_discardList[_discardList.Count-1]);              
                _discardList.RemoveAt(_discardList.Count - 1);       
                
                ClearHandGO();
                InstantiateHandGOs();              
            };

           
            ReplayManager.Execute(new DelegateReplayCommandMove(forward, backward));
        }

        public void ClearHandGO()
        {
            int childs = HandView.transform.childCount;
            for (int i = childs -1 ; i >= 0; i--)
            {
                Destroy(HandView.transform.GetChild(i).gameObject);
            }
                    
        }

        public void InstantiateHandGOs()
        {
            foreach (var handCard in _playerhandList)
            {
                var cardobject = Instantiate(CardBase, HandView.transform);                
                cardobject.GetComponent<Card>().InitializeCard(handCard);
            }

        }

      

    }


}


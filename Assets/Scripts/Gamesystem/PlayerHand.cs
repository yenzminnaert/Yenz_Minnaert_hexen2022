﻿
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using DAE.HexSystem;

namespace DAE.Gamesystem
{

    public class PlayerHand : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
    {        
              
        public void OnPointerEnter(PointerEventData eventData)
        {
            // === triggers when hovering on card deck

            Debug.Log("OnPointerEnter");
            if (eventData.pointerDrag == null)
                return;

            Card d = eventData.pointerDrag.GetComponent<Card>();
            if (d != null)
            {
                d.placeholderParent = this.transform;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // === triggers when hovering off card deck

            Debug.Log("OnPointerExit");
            if (eventData.pointerDrag == null)
                return;

            Card d = eventData.pointerDrag.GetComponent<Card>();
            if (d != null && d.placeholderParent == this.transform)
            {
                d.placeholderParent = d.parentToReturnTo;
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            // === triggers when dropping card

            Debug.Log(eventData.pointerDrag.name + " was dropped on " + gameObject.name);

            Card d = eventData.pointerDrag.GetComponent<Card>();
            if (d != null)
            {
                d.parentToReturnTo = this.transform;
            }
        }

    }
}

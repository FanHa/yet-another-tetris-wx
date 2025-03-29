using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace UI.Inventories
{
    public class InventoryItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image itemImage;
        public Model.InventoryItem Data;

        private Outline outline;

        public event Action<InventoryItem> 
            OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

        public void Awake()
        {
            Deselect();
        }

        public void Start()
        {
            outline = GetComponent<Outline>();
        }


        public void Deselect()
        {
            // todo
        }

        public void SetData(Model.InventoryItem item , Sprite sprite)
        {
            Data = item;
            itemImage.sprite = sprite;
        }

        public void Select()
        {
            // todo
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }


    }
}

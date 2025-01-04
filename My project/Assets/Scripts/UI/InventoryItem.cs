using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace UI {
    public class InventoryItem : MonoBehaviour, IPointerClickHandler, 
        IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        [SerializeField] private Image itemImage;
        [SerializeField] private Model.InventoryItem inventoryItemSO;

        private Outline outline;

        public event Action<InventoryItem> 
            OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;
        private bool empty = true;

        public void Awake()
        {
            ResetData();
            Deselect();
        }

        public void Start()
        {
            outline = GetComponent<Outline>();
        }

        public void ResetData()
        {
            itemImage.gameObject.SetActive(false);
            empty = true;
        }

        public void Deselect()
        {
            // todo
        }

        public void SetData(Sprite sprite)
        {
            itemImage.sprite = sprite;
            itemImage.gameObject.SetActive(true);
            empty = false;
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

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty) return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {

        }
    }
}

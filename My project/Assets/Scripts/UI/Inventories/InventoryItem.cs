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
        public Model.UnitInventoryItem Data;

        private Outline outline;

        public event Action<InventoryItem> 
            OnItemClicked;

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

        public void SetData(Model.UnitInventoryItem item , Sprite sprite)
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
            }
            else
            {
                OnItemClicked?.Invoke(this);
            }
        }


    }
}

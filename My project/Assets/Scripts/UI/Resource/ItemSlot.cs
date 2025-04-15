using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Model.Tetri;
using UnityEngine.UI;


namespace UI.Resource {
    public class ItemSlot : MonoBehaviour, IPointerClickHandler, 
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Action<ItemSlot> OnItemClicked; // 定义点击事件
        public Action<ItemSlot> OnItemBeginDrag; // 定义开始拖动事件
        public Action<ItemSlot> OnItemEndDrag; // 定义结束拖动事件
        public Action<ItemSlot> OnRotateRequested; // 定义旋转请求事件


        [SerializeField] private Tetri tetri; // Tetri数据
        [SerializeField] private Transform previewParent; 
        [SerializeField] private Button RotateButton; // 旋转按钮

        private ItemSlot() { } // 私有构造函数，防止直接实例化

        private void Awake()
        {
            RotateButton.onClick.AddListener(() =>
            {
                OnRotateRequested?.Invoke(this); // 触发旋转请求事件
            });
        }
        public void Initialize(Tetri tetri, GameObject preview)
        {
            SetTetri(tetri);
            foreach (Transform child in previewParent)
            {
                Destroy(child.gameObject);
            }
            
            preview.transform.SetParent(previewParent, false);
            if (tetri.Type == Tetri.TetriType.Character)
            {
                RotateButton.gameObject.SetActive(false); // 隐藏旋转按钮
            }
            else
            {
                RotateButton.gameObject.SetActive(true); // 显示旋转按钮
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            OnItemClicked?.Invoke(this);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        private void SetTetri(Tetri tetri)
        {
            this.tetri = tetri;
            
        }
        public Tetri GetTetri()
        {
            return tetri;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // todo 为什么要加一个空的OnDrag函数？不然不能成功拖动
        }

    }
}
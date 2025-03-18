using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Model.Tetri;


namespace UI.TetrisResource {
    public class TetrisResourceItem : MonoBehaviour, IPointerClickHandler, 
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Action<TetrisResourceItem> OnItemClicked; // 定义点击事件
        public Action<TetrisResourceItem> OnItemBeginDrag; // 定义开始拖动事件
        public Action<TetrisResourceItem> OnItemEndDrag; // 定义结束拖动事件

        [SerializeField] private Tetri tetri; // Tetri数据
        [SerializeField] private Transform previewParent; 

        private TetrisResourceItem() { } // 私有构造函数，防止直接实例化
        public void Initialize(Tetri tetri, GameObject preview)
        {
            SetTetri(tetri);
            preview.transform.SetParent(previewParent, false);
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

        private void SetTetri(Tetri newTetri)
        {
            tetri = newTetri;
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
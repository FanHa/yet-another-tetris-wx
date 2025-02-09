using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Model.Tetri;
using UnityEngine.UI;
using UnityEngine.Tilemaps;

namespace UI.TetrisResource {
    public class TetrisResourceItem : MonoBehaviour, IPointerClickHandler, 
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Action<TetrisResourceItem> OnItemClicked; // 定义点击事件
        public Action<TetrisResourceItem> OnItemBeginDrag; // 定义开始拖动事件
        public Action<TetrisResourceItem> OnItemEndDrag; // 定义结束拖动事件
        // public Transform GridParent => gridParent; // 只读属性，允许外部获取

        [SerializeField] private Tetri tetri; // Tetri数据
        [SerializeField] private GameObject tetriBrickGroupPrefab;
        [SerializeField] private GameObject tetriBrickPrefab; 
        [SerializeField] private Transform gridParent; // 4x4网格的父对象

        private TetriCellTypeSpriteMapping spriteMapping; // TetriCellTypeSpriteMapping实例



        private TetrisResourceItem() { } // 私有构造函数，防止直接实例化
        public void Initialize(Tetri tetri, TetriCellTypeSpriteMapping spriteMapping)
        {
            SetTetri(tetri);
            this.spriteMapping = spriteMapping;
            GameObject tetriInstance = CreateGridImages();
            tetriInstance.transform.SetParent(gridParent, false);
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

        public GameObject CreateGridImages(Vector2? gridSize = null)
        {
            GameObject tetriBrickGrouprInstance = Instantiate(tetriBrickGroupPrefab);
            // 获取GridLayoutGroup组件
            GridLayoutGroup gridLayoutGroup = tetriBrickGrouprInstance.GetComponent<GridLayoutGroup>();
            if (gridLayoutGroup != null)
            {
                if (gridSize != null) {
                    gridLayoutGroup.cellSize = gridSize.Value;
                }
            }

            // 创建4x4网格的Image
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject imageObject = Instantiate(tetriBrickPrefab, tetriBrickGrouprInstance.transform);
                    Image image = imageObject.GetComponent<Image>();

                    // 根据Tetri的形状设置Image的显示
                    TetriCell cell = tetri.Shape[i, j];
                    if (cell is not TetriCellEmpty)
                    {
                        Sprite sprite = spriteMapping.GetSprite(cell.GetType());
                        if (sprite != null)
                        {
                            image.sprite = sprite;
                            image.color = Color.green; // 设置为白色表示有方块
                        }
                        else
                        {
                            image.color = Color.black; // 设置为黑色表示有方块但没有找到对应的sprite
                        }
                    }
                    else
                    {
                        image.color = Color.clear; // 设置为透明表示没有方块
                    }
                }
            }

            return tetriBrickGrouprInstance;
        }

        
    }
}
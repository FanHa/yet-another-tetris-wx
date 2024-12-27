using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Model;
using UnityEngine.UI;

namespace UI {
    public class TetrisResourceItem : MonoBehaviour, IPointerClickHandler, 
        IBeginDragHandler, IEndDragHandler, IDragHandler
    {
        public Action<TetrisResourceItem> OnItemClicked; // 定义点击事件
        public Action<TetrisResourceItem> OnItemBeginDrag; // 定义开始拖动事件
        public Action<TetrisResourceItem> OnItemEndDrag; // 定义结束拖动事件
        [SerializeField] private Tetri tetri; // Tetri数据
        [SerializeField] private GameObject imagePrefab; // 用于显示Tetri形状的Image预制件
        [SerializeField] private Transform gridParent; // 4x4网格的父对象



        private TetrisResourceItem() { } // 私有构造函数，防止直接实例化
        public static TetrisResourceItem CreateInstance(GameObject prefab, Transform parent, Tetri tetri)
        {
            GameObject instance = Instantiate(prefab, parent);
            TetrisResourceItem resourceItem = instance.GetComponent<TetrisResourceItem>();
            resourceItem.SetTetri(tetri);
            resourceItem.CreateGridImages();
            return resourceItem;
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

        public void SetTetri(Tetri newTetri)
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

        private void CreateGridImages()
        {
            // 清空现有的子对象
            foreach (Transform child in gridParent)
            {
                Destroy(child.gameObject);
            }

            // 创建4x4网格的Image
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    GameObject imageObject = Instantiate(imagePrefab, gridParent);
                    Image image = imageObject.GetComponent<Image>();

                    // 根据Tetri的形状设置Image的显示
                    if (i < tetri.shape.GetLength(0) && j < tetri.shape.GetLength(1) && tetri.shape[i, j] != 0)
                    {
                        image.color = Color.black; // 设置为黑色表示有方块
                    }
                    else
                    {
                        image.color = Color.clear; // 设置为透明表示没有方块
                    }
                }
            }
        }
    }
}
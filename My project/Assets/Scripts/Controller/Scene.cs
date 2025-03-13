using System;
using System.Collections;
using System.Collections.Generic;
using Model;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controller
{
    public class Scene : MonoBehaviour
    {
        [SerializeField] private Transform battleFieldTransform;
        [SerializeField] private GameObject tetrisResourcePanel; // 新增引用
        [SerializeField] private GameObject operationTable; // 新增引用
        [SerializeField] private Model.Inventory inventory; // 新增引用
        public event Action OnSwitchToOperationPhase;

        private BattleField battleField; // 添加对 BattleField 的引用

        void Awake()
        {
            // 找到当前物体上的 BattleField 组件
            battleField = GetComponent<BattleField>();
            if (battleField == null)
            {
                Debug.LogWarning("BattleField component not found on the same GameObject.");
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void SwitchToBattlePhase()
        {
            if (battleFieldTransform != null)
            {
                Camera.main.transform.position = new Vector3(battleFieldTransform.position.x, battleFieldTransform.position.y, Camera.main.transform.position.z);                
                battleField.StartSpawningUnits();
                tetrisResourcePanel.SetActive(false);
                operationTable.SetActive(false);
            }
            else
            {
                Debug.LogWarning("battleFieldTransform is not set.");
            }
        }

        public void SwitchToOperationPhase()
        {
            battleField.StopSpawningUnits();
            Camera.main.transform.position = new Vector3(0, 0, -10);
            tetrisResourcePanel.SetActive(true);
            operationTable.SetActive(true);

            // 触发事件
            OnSwitchToOperationPhase?.Invoke();
        }

        // 新增方法
        private List<Model.InventoryItem> GenerateItemsFromTable()
        {
            List<Model.InventoryItem> items = new List<Model.InventoryItem>();
            // 根据 operationTable 数据生成新的 inventoryItem
            // 这里假设 operationTable 有一个方法 GetItems() 返回 List<InventoryItem>
            // items = operationTable.GetComponent<OperationTable>().GetItems();
            return items;
        }
    }
}

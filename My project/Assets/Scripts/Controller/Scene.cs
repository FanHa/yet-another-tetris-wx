using System;
using System.Collections.Generic;
using UnityEngine;

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

        public void SwitchToOperationPhase()
        {
            battleField.StopSpawningUnits();
            Camera.main.transform.position = new Vector3(0, 0, -10);
            tetrisResourcePanel.SetActive(true);
            operationTable.SetActive(true);

            // 触发事件
            OnSwitchToOperationPhase?.Invoke();
        }

    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tetri;
using System.Linq;
using Units;

namespace Controller {
    [RequireComponent(typeof(UI.Inventories.UnitInventoryView))]
    public class UnitInventoryController : MonoBehaviour
    {
        private UI.Inventories.UnitInventoryView unitInventoryView;
        [SerializeField] private Model.UnitInventoryModel inventoryData;
        [SerializeField] private Units.UnitFactory unitFactory;

        public event Action<Unit> OnUnitClicked;

        private void Awake()
        {
            unitInventoryView = GetComponent<UI.Inventories.UnitInventoryView>();
        }
        private void Start()
        {
            inventoryData.OnDataChanged += HandleDataChange;
            unitInventoryView.OnUnitClicked += HandleUnitClicked;
        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        public void RefreshInventoryFromInfluenceGroups(List<CharacterInfluenceGroup> characterGroups)
        {
            var items = new List<UnitInventoryItem>();
            foreach (var group in characterGroups)
            {
                var item = new UnitInventoryItem(
                    group.Character,
                    group.InfluencedCells
                );
                items.Add(item);
            }
            inventoryData.ResetInventoryData(items);

        }


        private void HandleDataChange(List<Model.UnitInventoryItem> inventoryState)
        {
            var unitList = new List<Units.Unit>();

            foreach (Model.UnitInventoryItem item in inventoryState)
            {
                Units.Unit unit = unitFactory.CreateUnit(item);
                if (unit != null)
                {
                    unitList.Add(unit);
                }
            }
            unitInventoryView.UpdateData(unitList);
        }
    }
}
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
        [SerializeField] private Model.UnitInventoryModel playerUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel enemyUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel trainGroundUnitInventoryDataA;
        [SerializeField] private Model.UnitInventoryModel trainGroundUnitInventoryDataB;
        [SerializeField] private Model.TrainGround.Setup trainGroundSetup;
        [SerializeField] private Units.UnitFactory unitFactory;
        [SerializeField] private TetriCellFactory tetriCellFactory;

        public event Action<Unit> OnUnitClicked;

        private void Awake()
        {
            unitInventoryView = GetComponent<UI.Inventories.UnitInventoryView>();
        }
        private void Start()
        {
            playerUnitInventoryData.OnDataChanged += HandleDataChange;
            unitInventoryView.OnUnitClicked += HandleUnitClicked;
        }

        private void HandleUnitClicked(Unit unit)
        {
            OnUnitClicked?.Invoke(unit);
        }

        public void RefreshInventoryFromInfluenceGroups(List<CharacterInfluenceGroup> characterGroups)
        {
            var items = new List<CharacterInfluenceGroup>();
            foreach (var group in characterGroups)
            {
                var item = new CharacterInfluenceGroup(
                    group.Character,
                    group.InfluencedCells
                );
                items.Add(item);
            }
            playerUnitInventoryData.ResetInventoryData(items);

        }

        public void SetEnemyInventoryData(List<CharacterInfluenceGroup> enemyData)
        {
            enemyUnitInventoryData.ResetInventoryData(enemyData);
        }

        public void PrepareTrainGroundUnitInventory()
        {
            var itemsA = new List<CharacterInfluenceGroup>();
            foreach (var unitConfig in trainGroundSetup.FactionAUnits)
            {
                var characterCell = tetriCellFactory.CreateCharacterCell(unitConfig.characterId);
                var tetriCells = new List<Model.Tetri.Cell>();
                if (unitConfig.tetriCellIds != null)
                {
                    foreach (var cellId in unitConfig.tetriCellIds)
                    {
                        var cell = tetriCellFactory.CreateCell(cellId);
                        if (cell != null)
                            tetriCells.Add(cell);
                    }
                }
                var item = new CharacterInfluenceGroup(characterCell, tetriCells);
                itemsA.Add(item);
            }
            trainGroundUnitInventoryDataA.ResetInventoryData(itemsA);

            var itemsB = new List<CharacterInfluenceGroup>();
            foreach (var unitConfig in trainGroundSetup.FactionBUnits)
            {
                var characterCell = tetriCellFactory.CreateCharacterCell(unitConfig.characterId);
                var tetriCells = new List<Model.Tetri.Cell>();
                if (unitConfig.tetriCellIds != null)
                {
                    foreach (var cellId in unitConfig.tetriCellIds)
                    {
                        var cell = tetriCellFactory.CreateCell(cellId);
                        if (cell != null)
                            tetriCells.Add(cell);
                    }
                }
                var item = new CharacterInfluenceGroup(characterCell, tetriCells);
                itemsB.Add(item);
            }
            trainGroundUnitInventoryDataB.ResetInventoryData(itemsB);
        }


        private void HandleDataChange(List<Model.CharacterInfluenceGroup> inventoryState)
        {
            var unitList = new List<Units.Unit>();

            foreach (Model.CharacterInfluenceGroup item in inventoryState)
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
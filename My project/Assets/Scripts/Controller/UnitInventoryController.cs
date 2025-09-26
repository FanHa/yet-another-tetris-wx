using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tetri;
using System.Linq;
using Units;

namespace Controller {
    public class UnitInventoryController : MonoBehaviour
    {
        [SerializeField] private Model.UnitInventoryModel playerUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel enemyUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel trainGroundUnitInventoryDataA;
        [SerializeField] private Model.UnitInventoryModel trainGroundUnitInventoryDataB;
        [SerializeField] private Model.TrainGround.Setup trainGroundSetup;
        [SerializeField] private Units.UnitFactory unitFactory;
        [SerializeField] private TetriCellFactory tetriCellFactory;

        private void Awake()
        {
        }
        private void Start()
        {
            
        }


        public void SetEnemyInventoryData(List<CharacterInfluenceGroup> enemyData)
        {
            enemyUnitInventoryData.ResetInventoryData(enemyData);
        }

        public void SetPlayerInventoryData(List<CharacterInfluenceGroup> playerData)
        {
            playerUnitInventoryData.ResetInventoryData(playerData);
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

    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using Model.Tetri;
using System.Linq;
using Units;

namespace Model {
    [CreateAssetMenu]
    public class UnitInventoryService : ScriptableObject
    {
        [SerializeField] private Model.UnitInventoryModel playerUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel enemyUnitInventoryData;
        [SerializeField] private Model.UnitInventoryModel trainGroundUnitInventoryDataA;
        [SerializeField] private Model.UnitInventoryModel trainGroundUnitInventoryDataB;
        [SerializeField] private Model.TrainGround.Setup trainGroundSetup;
        [SerializeField] private Units.UnitFactory unitFactory;
        [SerializeField] private TetriCellFactory tetriCellFactory;

        public void SetEnemyInventoryData(List<CharacterPlacement> enemyData)
        {
            enemyUnitInventoryData.ResetInventoryData(enemyData);
        }

        public void SetPlayerInventoryData(List<CharacterPlacement> playerData)
        {
            playerUnitInventoryData.ResetInventoryData(playerData);
        }

        public void PrepareTrainGroundUnitInventory()
        {
            var itemsA = new List<CharacterPlacement>();
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
                CharacterInfluence influence = new CharacterInfluence(characterCell, tetriCells, null);
                var item = new CharacterPlacement(influence, Vector3.zero);
                itemsA.Add(item);
            }
            trainGroundUnitInventoryDataA.ResetInventoryData(itemsA);

            var itemsB = new List<CharacterPlacement>();
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
                CharacterInfluence influence = new CharacterInfluence(characterCell, tetriCells, null);
                var item = new CharacterPlacement(influence, Vector3.zero);
                itemsB.Add(item);
            }
            trainGroundUnitInventoryDataB.ResetInventoryData(itemsB);
        }

    }
}
using NUnit.Framework;
using Model.Rewards;
using Model.Tetri;
using System;
using System.Collections.Generic;
using UnityEngine;
using Model;
using System.Linq;

namespace Tests.Model.Rewards
{
    public class RewardTests
    {
        private TetrisResources tetrisResources;
        private TetrisFactory tetrisFactory;

        [SetUp]
        public void Setup()
        {
            // 初始化 TetrisResources 和 TetrisFactory
            tetrisResources = ScriptableObject.CreateInstance<TetrisResources>();
            tetrisFactory = new TetrisFactory();

            // 初始化 TetrisResources 的未使用 Tetri 列表
            var initialTetris = new List<Tetri>
            {
                tetrisFactory.CreateTShape(),
                tetrisFactory.CreateIShape(),
                tetrisFactory.CreateOShape()
            };
            tetrisResources.InitialUnusedTetris(initialTetris);
        }

        [Test]
        public void AddTetriReward_ShouldAddTetriWithRandomCell()
        {
            // Arrange
            var tetri = tetrisFactory.CreateTShape();
            var cell = new Attack(); // 示例 Cell 类型
            var addTetriReward = new AddTetri(tetri, cell);

            // Act
            var resultTetri = addTetriReward.GetTetri();

            // Assert
            Assert.IsNotNull(resultTetri);
           
            Assert.IsTrue(resultTetri.GetOccupiedPositions().Count > 0, "Tetri should have occupied positions.");
            var container = resultTetri.Shape.Cast<Cell>();
            Assert.IsTrue(resultTetri.Shape.Cast<Cell>().Contains(cell), "Tetri should contain the specified cell.");
        }

        [Test]
        public void UpgradeTetriReward_ShouldUpgradePaddingCell()
        {
            // Arrange
            var tetri = tetrisFactory.CreateTShape();
            var paddingCell = new Padding();
            tetri.SetCell(1, 1, paddingCell); // 设置一个 PaddingCell
            var newCell = new Padding(); // 示例升级后的 Cell 类型
            var upgradeTetriReward = new UpgradeTetri(tetri, new Vector2Int(1, 1), newCell);

            // Act
            upgradeTetriReward.Apply();

            // Assert
            Assert.AreEqual(newCell, tetri.Shape[1, 1], "The cell at the target position should be upgraded.");
        }

        [Test]
        public void NewCharacterReward_ShouldCreateCharacterTetri()
        {
            // Arrange
            var character = new Circle(); // 示例 Character 类型
            var newCharacterReward = new NewCharacter(character);

            // Act
            var resultTetri = newCharacterReward.GetTetri();
            var resultCharacter = newCharacterReward.GetCharacter();

            // Assert
            Assert.IsNotNull(resultTetri);
            Assert.IsNotNull(resultCharacter);
            Assert.AreEqual(character, resultCharacter, "The character should match the one passed to the reward.");
            Assert.AreEqual(character, resultTetri.Shape[1, 1], "The character should be placed at the correct position in the Tetri.");
        }

    }
}
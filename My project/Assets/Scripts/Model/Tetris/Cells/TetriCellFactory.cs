using System;
using System.Collections.Generic;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Bootstrap/Factories/Tetri Cell Factory")]
    public class TetriCellFactory : ScriptableObject
    {
        [SerializeField] private CellDatabase cellDatabase;

        public Cell CreatePadding()
        {
            CellDefinition definition = cellDatabase?.GetPaddingDefinition();
            return CreateCell(definition);
        }

        public Cell CreateCell(string cellId)
        {
            if (string.IsNullOrWhiteSpace(cellId))
            {
                throw new ArgumentException("Cell id is null or empty.", nameof(cellId));
            }

            var definition = cellDatabase != null ? cellDatabase.GetDefinition(cellId) : throw new InvalidOperationException("CellDatabase is missing.");
            return CreateCell(definition);
        }

        public Character CreateCharacterCell(string definitionId)
        {
            if (string.IsNullOrWhiteSpace(definitionId))
            {
                throw new ArgumentException("Character definition id is null or empty.", nameof(definitionId));
            }

            var definition = cellDatabase != null ? cellDatabase.GetDefinition(definitionId) : throw new InvalidOperationException("CellDatabase is missing.");
            return CreateCharacterCell(definition as CharacterDefinition ?? throw new ArgumentException($"Definition '{definitionId}' is not a CharacterDefinition.", nameof(definitionId)));
        }

        public Character CreateCharacterCell(CharacterDefinition definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition));
            }

            var character = new Character();
            character.Initialize(definition);
            return character;
        }

        public Cell CreateCell(CellDefinition definition)
        {
            var cell = definition switch
            {
                SkillBackedCellDefinition => new SkillCell(),
                _ => new Cell()
            };
            if (definition != null)
            {
                cell.Initialize(definition);
            }
            return cell;
        }

        public Cell Clone(Cell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException(nameof(cell));
            }

            if (cell is Character character)
            {
                return CreateCharacterCell(character.DefinitionData);
            }

            var clone = cell is SkillCell ? new SkillCell() : new Cell();
            if (cell.Definition != null)
            {
                clone.Initialize(cell.Definition);
            }

            clone.Level = cell.Level;
            return clone;
        }
    }
}


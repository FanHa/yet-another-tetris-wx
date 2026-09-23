using UnityEngine;

namespace Operation
{
    [CreateAssetMenu(menuName = "Game Data/Bootstrap/Factories/Operation Tetri Factory")]
    public class TetriFactory : ScriptableObject
    {
        // [SerializeField] private Operation.Tetri tetriPrefab;
        [SerializeField] private Operation.TetriPiece tetriPiecePrefab;
        [SerializeField] private Operation.TetriCharacter tetriCharacterPrefab;


        public Operation.Tetri CreateTetri(Model.Tetri.Tetri modelTetri)
        {
            Operation.Tetri prefab =
                modelTetri.Type == Model.Tetri.Tetri.TetriType.Character
                ? (Operation.Tetri)tetriCharacterPrefab
                : (Operation.Tetri)tetriPiecePrefab;

            var comp = Object.Instantiate(prefab);
            comp.Initialize(modelTetri);
            return comp;
        }
    }
}
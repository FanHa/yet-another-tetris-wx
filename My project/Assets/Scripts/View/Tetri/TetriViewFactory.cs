using System;
using Model.Tetri;
using UnityEngine;

namespace View.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Bootstrap/Factories/Operation Tetri Factory")]
    public class TetriViewFactory : ScriptableObject
    {
        [SerializeField] private View.Tetri.CellBasedTetriView tetriPiecePrefab;
        [SerializeField] private View.Tetri.TetriCharacterView tetriCharacterPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tetriPiecePrefab == null)
            {
                Debug.LogError($"[{nameof(TetriViewFactory)}] Tetri piece prefab is missing.", this);
            }

            if (tetriCharacterPrefab == null)
            {
                Debug.LogError($"[{nameof(TetriViewFactory)}] Tetri character prefab is missing.", this);
            }
        }
#endif

        public View.Tetri.TetriView CreateTetri(Model.Tetri.Tetri modelTetri)
        {
            switch (modelTetri)
            {
                case CellBasedTetri normal:
                    var pieceView = UnityEngine.Object.Instantiate(tetriPiecePrefab);
                    pieceView.Initialize(normal);
                    return pieceView;
                case CharacterTetri character:
                    var characterView = UnityEngine.Object.Instantiate(tetriCharacterPrefab);
                    characterView.Initialize(character);
                    return characterView;
                default:
                    throw new ArgumentException("Unsupported Tetri type.", nameof(modelTetri));
            }
        }
    }
}
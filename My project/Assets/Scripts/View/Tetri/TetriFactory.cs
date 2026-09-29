using UnityEngine;

namespace View.Tetri
{
    [CreateAssetMenu(menuName = "Game Data/Bootstrap/Factories/Operation Tetri Factory")]
    public class TetriFactory : ScriptableObject
    {
        [SerializeField] private View.Tetri.TetriPiece tetriPiecePrefab;
        [SerializeField] private View.Tetri.TetriCharacter tetriCharacterPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (tetriPiecePrefab == null)
            {
                Debug.LogError($"[{nameof(TetriFactory)}] Tetri piece prefab is missing.", this);
            }

            if (tetriCharacterPrefab == null)
            {
                Debug.LogError($"[{nameof(TetriFactory)}] Tetri character prefab is missing.", this);
            }
        }
#endif

        public View.Tetri.TetriView CreateTetri(Model.Tetri.Tetri modelTetri)
        {
            View.Tetri.TetriView prefab =
                modelTetri.Type == Model.Tetri.Tetri.TetriType.Character
                ? (View.Tetri.TetriView)tetriCharacterPrefab
                : (View.Tetri.TetriView)tetriPiecePrefab;

            var comp = Object.Instantiate(prefab);
            comp.Initialize(modelTetri);
            return comp;
        }
    }
}
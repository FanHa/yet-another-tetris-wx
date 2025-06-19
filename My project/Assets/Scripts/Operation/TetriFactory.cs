using UnityEngine;

namespace Operation
{
    [CreateAssetMenu(menuName = "Config/TetriFactory")]
    public class TetriFactory : ScriptableObject
    {
        [SerializeField] private Operation.Tetri tetriPrefab;

        public Operation.Tetri CreateTetri(Model.Tetri.Tetri modelTetri)
        {
            // 实例化 Tetri 对象
            Operation.Tetri tetriComponent = Object.Instantiate(tetriPrefab);

            // 初始化 Tetri
            tetriComponent.Initialize(modelTetri);

            return tetriComponent;
        }
    }
}
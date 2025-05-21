using UnityEngine;

namespace Operation
{
    public class TetriFactory
    {
        private readonly GameObject tetriPrefab;

        public TetriFactory(GameObject tetriPrefab)
        {
            this.tetriPrefab = tetriPrefab;
        }

        public Operation.Tetri CreateTetri(Model.Tetri.Tetri modelTetri)
        {
            // 实例化 Tetri 对象
            GameObject itemObj = Object.Instantiate(tetriPrefab);
            var tetriComponent = itemObj.GetComponent<Operation.Tetri>();

            // 初始化 Tetri
            tetriComponent.Initialize(modelTetri);

            return tetriComponent;
        }
    }
}
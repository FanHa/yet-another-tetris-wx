using System;
using System.Collections.Generic;
using Model;
using UnityEngine;

namespace Controller
{

    public class OperationTableController : MonoBehaviour
    {
        public event Action<View.Tetri.TetriView> OnTetriBeginDrag;
        public event Action<View.Tetri.TetriView> OnTetriClick;

        [Header("每个格子的间距")]
        [SerializeField] private float cellSize;

        [SerializeField] private int width = 16;
        [SerializeField] private int height = 16;
        [SerializeField] private View.OperationTable.OperationTableView view;
        private View.OperationTable.OperationTableGridLayout gridLayout;
        private OperationTableModel model;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (view == null)
            {
                Debug.LogError($"[{nameof(OperationTableController)}] View is not assigned.", this);
            }

            if (width < 1)
            {
                Debug.LogError($"[{nameof(OperationTableController)}] Width must be at least 1.", this);
            }

            if (height < 1)
            {
                Debug.LogError($"[{nameof(OperationTableController)}] Height must be at least 1.", this);
            }

            if (cellSize <= 0f || float.IsNaN(cellSize) || float.IsInfinity(cellSize))
            {
                Debug.LogError($"[{nameof(OperationTableController)}] Cell size must be finite and greater than zero.", this);
            }
        }
#endif

        private void Awake()
        {
            model = new OperationTableModel(width, height);
            gridLayout = new View.OperationTable.OperationTableGridLayout(width, height, cellSize);
        }

        private void Start()
        {
            view.Initialize(gridLayout);
            view.OnItemCreated += HandleTetriCreated;
            model.OnTableChanged += HandleTableChanged;
            model.Clear();
        }

        private void HandleTetriCreated(View.Tetri.TetriView tetri)
        {
            tetri.OnBeginDragEvent += HandleTetriBeginDrag;
            tetri.OnClickEvent += HandleTetriClick;
        }

        private void HandleTetriBeginDrag(View.Tetri.TetriView tetri)
        {
            OnTetriBeginDrag?.Invoke(tetri);
        }

        private void HandleTetriClick(View.Tetri.TetriView tetri)
        {
            OnTetriClick?.Invoke(tetri);
        }

        private void OnDestroy()
        {
            model.OnTableChanged -= HandleTableChanged;
            view.OnItemCreated -= HandleTetriCreated;
        }

        private void HandleTableChanged()
        {
            view.Refresh(model);

        }

        /// <summary>
        /// Gets spawn data for a character Tetri that is currently placed on this operation table.
        /// </summary>
        internal UnitSpawnPlacement GetUnitSpawnPlacementForTetri(Model.Tetri.CharacterTetri tetri)
        {
            CharacterBuild build = model.GetPlacedCharacterBuild(tetri);
            Vector3 spawnOffset = view.GetTetriWorldPosition(tetri) - view.transform.position;
            return new UnitSpawnPlacement(build, spawnOffset);

        }

        public IReadOnlyList<UnitSpawnPlacement> GetUnitSpawnPlacements()
        {
            List<CharacterBuild> builds = model.GetCharacterBuilds();

            var placements = new List<UnitSpawnPlacement>(builds.Count);

            foreach (var build in builds)
            {
                Model.Tetri.Tetri ownerTetri = build.OwnerTetri;
                Vector3 spawnOffset = view.GetTetriWorldPosition(ownerTetri) - view.transform.position;
                var placement = new UnitSpawnPlacement(build, spawnOffset);

                placements.Add(placement);
            }

            return placements;
        }


        public bool TryPlaceTetri(Model.Tetri.Tetri tetri, Vector3 tetriCenterWorldPosition)
        {
            Vector3 tetriCenterInTableSpace = view.transform.InverseTransformPoint(tetriCenterWorldPosition);
            Vector2Int tetriTopLeftGridPosition = gridLayout.GetNearestTetriTopLeftGridPositionFromCenterInTableSpace(tetriCenterInTableSpace);
            return model.TryPlaceTetri(tetri, tetriTopLeftGridPosition);
        }

        public Vector3 SnapTetriCenterToGrid(Vector3 tetriCenterWorldPosition)
        {
            Vector3 tetriCenterInTableSpace = view.transform.InverseTransformPoint(tetriCenterWorldPosition);
            Vector2Int nearestTetriTopLeftGridPosition = gridLayout.GetNearestTetriTopLeftGridPositionFromCenterInTableSpace(tetriCenterInTableSpace);
            Vector3 snappedTetriCenterInTableSpace = gridLayout.GetTetriCenterInTableSpace(nearestTetriTopLeftGridPosition);
            return view.transform.TransformPoint(snappedTetriCenterInTableSpace);
        }

        internal void RemoveTetri(Model.Tetri.Tetri modelTetri)
        {
            model.RemoveTetri(modelTetri);
        }
    }

}
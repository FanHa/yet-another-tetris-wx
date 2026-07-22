using System;
using Units.Skills;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Config/Cells/Cell Definition")]
    public sealed class CellDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string runtimeTypeName;
        [SerializeField] private Sprite icon;

        public string Id => id;
        public string RuntimeTypeName => runtimeTypeName;
        public Sprite Icon => icon;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (TryGetExpectedId(out string expectedId) && !string.IsNullOrWhiteSpace(expectedId))
            {
                id = expectedId;
            }
        }
#endif


        public bool TryResolveCellType(out Type cellType, out string error)
        {
            cellType = null;
            error = null;

            if (string.IsNullOrWhiteSpace(runtimeTypeName))
            {
                error = $"RuntimeTypeName is empty. Definition={name}";
                return false;
            }

            Type resolvedType = Type.GetType(runtimeTypeName, false);
            if (resolvedType == null)
            {
                error = $"Cannot resolve RuntimeTypeName '{runtimeTypeName}'. Definition={name}";
                return false;
            }

            if (!typeof(Cell).IsAssignableFrom(resolvedType))
            {
                error = $"Resolved type '{resolvedType.FullName}' is not assignable to {nameof(Cell)}. Definition={name}";
                return false;
            }

            cellType = resolvedType;
            return true;
        }

        public bool TryGetExpectedId(out string expectedId)
        {
            expectedId = null;

            if (!TryResolveCellType(out Type cellType, out _))
            {
                return false;
            }

            try
            {
                Cell cell = Activator.CreateInstance(cellType) as Cell;
                if (cell == null)
                {
                    return false;
                }

                expectedId = cell.CellTypeId.ToString();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

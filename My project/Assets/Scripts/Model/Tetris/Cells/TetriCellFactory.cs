using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Units.Skills;
using Units.VisualEffects;
using UnityEditor;
using UnityEngine;

namespace Model.Tetri
{
    [CreateAssetMenu(menuName = "Factory/TetriCellModelFactory")]
    public class TetriCellFactory : ScriptableObject
    {

        public List<Type> AvailableCellTypes = new()
        {
            typeof(FrostZone),
            typeof(IceShield),
            typeof(IcyCage),
            typeof(Snowball),
            typeof(Fireball),
            typeof(FlameInject),
            typeof(BlazingField),
            typeof(FlamingRing)
        };

        public List<Type> AvailableCharacterTypes = new()
        {
            typeof(Square),
            typeof(Triangle),
            typeof(Circle),
            typeof(Aim)
        };

        [SerializeField] private CellLevelConfigManager cellLevelConfigManager;
        private readonly Dictionary<Type, SkillConfigGroup> cellConfigMap = new();

        public void OnEnable()
        {
            RegisterAll();
        }

        public void Register<TCell>(SkillConfigGroup configGroup)
            where TCell : Cell, new()
        {
            cellConfigMap[typeof(TCell)] = configGroup;
        }

        public void RegisterAll()
        {
            Register<FrostZone>(cellLevelConfigManager.FrostZoneConfigGroup);
            Register<IceShield>(cellLevelConfigManager.IceShieldConfigGroup);
            Register<IcyCage>(cellLevelConfigManager.IcyCageConfigGroup);
            Register<Snowball>(cellLevelConfigManager.SnowballConfigGroup);
            Register<Fireball>(cellLevelConfigManager.FireballConfigGroup);
            Register<FlameInject>(cellLevelConfigManager.FlameInjectConfigGroup);
            Register<BlazingField>(cellLevelConfigManager.BlazingFieldConfigGroup);
            Register<Model.Tetri.FlameRing>(cellLevelConfigManager.FlameRingConfigGroup);
        }

        public Cell CreatePadding()
        {
            return new Padding();
        }



        // public Cell CreateCell(Type type)
        // {
        //     if (type == null || !typeof(Cell).IsAssignableFrom(type))
        //         throw new ArgumentException("Invalid cell type provided.");

        //     // 1. 创建Cell实例（无参构造）
        //     var cell = (Cell)Activator.CreateInstance(type);

        //     // 2. 查找并注入配置
        //     if (cellLevelConfigMap != null && cellLevelConfigMap.TryGetValue(type, out var config) && config != null)
        //     {
        //         cell.SetLevelConfig(config);
        //     }

        //     return cell;
        // }

        public Cell CreateCell(Type type) 
        {
            if (type == null || !typeof(Cell).IsAssignableFrom(type))
                throw new ArgumentException("Invalid cell type provided.");

            // 1. 创建Cell实例（无参构造）
            var cell = (Cell)Activator.CreateInstance(type);

            // 2. 查找并注入配置
            if (cellConfigMap != null && cellConfigMap.TryGetValue(type, out var config) && config != null)
            {
                cell.SetLevelConfig(config);
            }
            else
            {
                Debug.LogWarning($"No config registered for cell type: {type.Name}");
            }
            return cell;
        }
    }

}

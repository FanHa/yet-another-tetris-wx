using System.Text;
using UnityEngine;

namespace UI.TetriInfo
{
    public class CharacterTetriDetailPanel : MonoBehaviour, ITetriDetailPanel
    {
        [SerializeField] private TMPro.TextMeshProUGUI characterNameText;
        [SerializeField] private TMPro.TextMeshProUGUI characterDescriptionText;
        [SerializeField] private TMPro.TextMeshProUGUI characterAttributeText;

        public void BindData(Operation.Tetri tetriComponent)
        {
            Model.Tetri.Tetri tetri = tetriComponent.ModelTetri;
            Model.Tetri.Character mainCell = tetri.GetMainCell() as Model.Tetri.Character;
            
            characterNameText.text = mainCell.CharacterName;
            characterDescriptionText.text = mainCell.Description();
            
            var character = tetriComponent as Operation.TetriCharacter;
            characterAttributeText.text = BuildAttributesText(character.PreviewUnit.Attributes);
        }


        private string BuildAttributesText(Units.Attributes a)
        {
            // 按行拼接：生命/护盾/攻速/攻距/移速/能量等
            var sb = new StringBuilder(128);

            // 生命
            sb.AppendLine($"生命: {a.CurrentHealth:0}/{a.MaxHealth.finalValue:0}");

            // 攻击力与攻速（攻速：每10秒与每秒）
            sb.AppendLine($"攻击力: {a.AttackPower.finalValue:0.##}");
            sb.AppendLine($"攻速: {a.AttacksPerTenSeconds.finalValue:0.##}/10s ({a.AttacksPerTenSeconds.finalValue / 10f:0.##}/s)");

            // 攻击范围
            sb.AppendLine($"攻击范围: {a.AttackRange.finalValue:0.##}");

            // 移速与能量回复
            sb.AppendLine($"移速: {a.MoveSpeed.finalValue:0.##}");
            sb.AppendLine($"能量回复: {a.EnergyPerSecond.finalValue:0.##}/s");

            return sb.ToString();
        }
    }
}
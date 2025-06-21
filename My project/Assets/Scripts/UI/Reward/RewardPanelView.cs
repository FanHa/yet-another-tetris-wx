using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using Model.Tetri;
using WeChatWASM;
using Operation;

namespace UI.Reward
{
    public class RewardPanelView : MonoBehaviour
    {
        
        public ItemSlot itemPrefab;
        public Transform itemParent;
        public event Action<Model.Rewards.Reward> OnItemSelected;
        [SerializeField] private Controller.Tetris tetris;
        [SerializeField] private TetriFactory tetriFactory;

        [Header("运行时引用")]
        [SerializeField] private Camera rewardPreviewCamera;
        
        public void SetRewards(List<Model.Rewards.Reward> rewards)
        {
            foreach (Transform child in itemParent)
            {
                Destroy(child.gameObject);
            }
            foreach (Model.Rewards.Reward reward in rewards)
            {
                ItemSlot item = Instantiate(itemPrefab, itemParent);
                item.SetReward(reward);
                var preview = GeneratePreview(reward);
                if (preview != null)
                {
                    item.SetPreviewCamera(rewardPreviewCamera);
                    item.SetPreview(preview);
                }

                // GameObject preview = null;
                // switch (reward)
                // {
                //     case Model.Rewards.AddTetri tetriReward:
                //         preview = tetris.GenerateTetriPreview(tetriReward.GetTetri());
                //         break;
                //     case Model.Rewards.NewCharacter characterReward:
                //         preview = tetris.GenerateCharacterPreview(characterReward.GetCharacter());
                //         break;
                //     case Model.Rewards.UpgradeTetri upgradeTetriReward:
                //         preview = tetris.GenerateUpgradeTetriPreview(
                //             upgradeTetriReward.GetTargetTetri(),
                //             upgradeTetriReward.GetTargetPosition(),
                //             upgradeTetriReward.GetNewCell());
                //         break;
                //     case Model.Rewards.UpgradeCharacter upgradeCharacterReward:
                //         preview = tetris.GenerateCharacterUpgradePreview(upgradeCharacterReward.GetTargetCharacter());
                //         break;
                //     default:
                //         // 可选：设置一个默认预览
                //         break;
                // }

                // if (preview != null)
                //     item.SetPreview(preview);

                // item.OnItemClicked += HandleItemClicked;
            }
        }

        public void ShowItems()
        {
            itemParent.gameObject.SetActive(true);
        }

        public void HideItems()
        {
            itemParent.gameObject.SetActive(false);
        }


        private GameObject GeneratePreview(Model.Rewards.Reward reward)
        {
            if (reward is Model.Rewards.AddTetri addTetriReward)
            {
                // 用 tetris.TetriFactory 生成一个 Tetri 物体作为预览
                var tetriObj = tetriFactory.CreateTetri(addTetriReward.GetTetri()).gameObject;
                SetLayerRecursively(tetriObj, LayerMask.NameToLayer("RewardPreview"));

                tetriObj.transform.SetParent(null, false); // 由外部决定父物体
                return tetriObj;
            }
            // 其他类型暂未实现
            return null;
        }

        private void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform)
            {
                SetLayerRecursively(child.gameObject, layer);
            }
        }

        public void HandleItemClicked(ItemSlot ItemSlot)
        {
            Debug.Log("Item clicked: " + ItemSlot.GetReward());
            OnItemSelected?.Invoke(ItemSlot.GetReward());
        }
    }
}


using System;
using Model.Tetri;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.Reward
{
    public class ItemSlot : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI detailText;
        [SerializeField] private Transform previewParent; // Parent for the Tetri preview grid

        private Camera previewCamera; // Camera for the Tetri preview

        private Model.Rewards.Reward item;        
        public event Action<ItemSlot> OnItemClicked;

        public void SetReward(Model.Rewards.Reward reward)
        {
            item = reward;
            nameText.text = reward.Name();
            detailText.text = reward.Description();
        }

        public void SetPreview(GameObject preview)
        {
            Vector3 pos = preview.transform.position;
            pos.x = previewCamera.transform.position.x;
            pos.y = previewCamera.transform.position.y;
            preview.transform.position = pos;
        }

        public void SetPreviewCamera(Camera camera)
        {
            previewCamera = camera;
        }

        public void SetPreviewTexture(RenderTexture texture)
        {
            // Assuming the previewParent has a RawImage component to display the texture
            RawImage rawImage = previewParent.GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = texture;
            }
        }

        public Model.Rewards.Reward GetReward()
        {
            return item;
        }

        public void OnPointerClick(PointerEventData eventData)
        {

            Debug.Log("Item clicked: " + item.Name());
            OnItemClicked?.Invoke(this);

        }


    }
}

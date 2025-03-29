using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventories.Description
{
    public class Item : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI numberText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        public void SetDescription(Sprite sprite, int number, string description)
        {
            image.sprite = sprite;
            if (number > 1)
            {
                numberText.gameObject.SetActive(true);
            }
            else
            {
                numberText.gameObject.SetActive(false);
            }
            this.numberText.text = "x" + number.ToString();
            descriptionText.text = description;
            
            
        }
    }
}


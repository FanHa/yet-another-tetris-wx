using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class DescriptionItem : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TMPro.TMP_Text title;
        public void SetDescription(Sprite sprite, string titleText)
        {
            image.sprite = sprite;
            title.text = titleText;
        }
    }
}


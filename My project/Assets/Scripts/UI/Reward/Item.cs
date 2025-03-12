using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    [SerializeField] private TMP_Text rewardText;
    private string reward;
    private Panel panel;

    void Start()
    {
        panel = GetComponentInParent<Panel>();
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void SetReward(string reward)
    {
        this.reward = reward;
        rewardText.text = reward;
    }

    public string GetReward()
    {
        return reward;
    }

    private void OnClick()
    {
        panel.OnItemClicked(this);
    }
}

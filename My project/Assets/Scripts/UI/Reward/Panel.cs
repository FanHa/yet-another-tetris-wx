using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class Panel : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform itemParent;

    public void SetRewards(List<string> rewards)
    {
        // 清空panel下的所有item
        foreach (Transform child in itemParent)
        {
            Destroy(child.gameObject);
        }

        // 创建新item
        foreach (string reward in rewards)
        {
            GameObject itemObject = Instantiate(itemPrefab, itemParent);
            Item item = itemObject.GetComponent<Item>();
            item.SetReward(reward);
            item.GetComponent<Button>().onClick.AddListener(() => OnItemClicked(item));
        }
    }

    public void OnItemClicked(Item item)
    {
        // 将点击事件传导到外面的controller
        Debug.Log("Item clicked: " + item.GetReward());
        // ...这里可以添加更多逻辑，例如通知外部controller...
    }
}

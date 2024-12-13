using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountdownTimer : MonoBehaviour
{
    public float countdownTime = 30f; // 倒计时时间（秒）
    private float currentTime;
    public TextMeshProUGUI countdownText;

    void Start()
    {
        currentTime = countdownTime;
    }

    void Update()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            countdownText.text = currentTime.ToString("F2"); // 显示两位小数
        }
        else
        {
            countdownText.text = "0.00";
            Time.timeScale = 0; // 暂停游戏
        }
    }
}

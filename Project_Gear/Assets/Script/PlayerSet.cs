using UnityEngine;

public class PlayerSet : MonoBehaviour
{
    [Header("Player Stats")]
    [SerializeField] private int maxHealth = 10; // 最大生命上限
    [SerializeField] private int currentHealth = 10; // 當前生命
    [SerializeField] private int chips = 10; // 初始持有籌碼

    public int MaxHealth
    {
        get => maxHealth;
        set
        {
            maxHealth = Mathf.Max(1, value); // 確保最大生命至少為 1
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth; // 當前生命不可超過最大生命上限
            }
        }
    }

    public int CurrentHealth
    {
        get => currentHealth;
        set
        {
            currentHealth = Mathf.Clamp(value, 0, maxHealth); // 限制當前生命在 0 和最大生命之間
        }
    }

    public int Chips
    {
        get => chips;
        set
        {
            chips = Mathf.Max(0, value); // 確保籌碼不為負數
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 初始化玩家數值
        Debug.Log($"Player Initialized: MaxHealth = {maxHealth}, CurrentHealth = {currentHealth}, Chips = {chips}");
    }

    // Update is called once per frame
    void Update()
    {
        // 測試用：按下 H 鍵增加最大生命，按下 C 鍵增加籌碼
        if (Input.GetKeyDown(KeyCode.H))
        {
            MaxHealth += 1;
            Debug.Log($"MaxHealth increased to {maxHealth}");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            Chips += 1;
            Debug.Log($"Chips increased to {chips}");
        }
    }
}

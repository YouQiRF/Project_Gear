using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class PointerController : MonoBehaviour
{
    [Header("Pointer Settings")]
    public float maxValue = 360f;   // total range
    public float speed = 40f;        // increase per second
    public float currentValue = 0f;
    private bool isRunning = false;
    private bool hasStopped = false;

    [Header("UI Display")]
    public TextMeshProUGUI displayText; // display number and effect

    [Header("Prefab Settings")]
    public float summonOffsetY = -200f; // spawn position offset on Y axis

    // Effect types (expandable in the future)
    public enum EffectType
    {
        ATK,   // 攻擊
        Heal,  // 回復
        Call   // 招換
    }

    [System.Serializable]
    public class EffectData
    {
        public EffectType effectType;   // effect type (menu)
        public int count;               // how many slots
        public float value;             // effect value (negative = damage, positive = heal)
        public float summonSpeed;       // if Call, new prefab's speed
        public GameObject summonPrefab; // specific PointerController prefab for this Call
    }

    public List<EffectData> effects = new List<EffectData>();
    private List<(float start, float end, EffectData effect)> effectRanges;

    private bool shouldSummonOnReset = false; // 新增變數

    void Start()
    {
        CalculateRanges();
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        while (true)
        {
            // 1. 等待 0.5 秒開始計時
            UpdateDisplay("Get Ready...");
            yield return new WaitForSeconds(0.5f);

            // 開始計時
            currentValue = 0f;
            isRunning = true;
            hasStopped = false;

            // 2. 計時過程
            while (isRunning)
            {
                currentValue += speed * Time.deltaTime;
                UpdateDisplay(Mathf.RoundToInt(currentValue).ToString());

                if (currentValue >= maxValue)
                {
                    currentValue = maxValue;
                    isRunning = false;
                    hasStopped = true;
                    UpdateDisplay("Miss!");
                }

                if (Input.GetKeyDown(KeyCode.Space) && !hasStopped)
                {
                    // 使用者按下空白鍵停下
                    isRunning = false;
                    hasStopped = true;
                    ShowResult();

                    // 等待 0.5 秒後繼續
                    yield return new WaitForSeconds(0.5f);

                    // 從停下的數字開始，花費 1 秒將數字跑完
                    float startValue = currentValue;
                    float elapsedTime = 0f;
                    while (elapsedTime < 1f)
                    {
                        elapsedTime += Time.deltaTime;
                        currentValue = Mathf.Lerp(startValue, maxValue, elapsedTime / 1f);
                        UpdateDisplay(Mathf.RoundToInt(currentValue).ToString());
                        yield return null;
                    }

                    currentValue = maxValue;
                    UpdateDisplay("Finished!");
                }

                yield return null;
            }

            // 3. 若 Miss 判定，執行相同邏輯
            if (currentValue >= maxValue)
            {
                yield return new WaitForSeconds(0.5f);

                float startValue = currentValue;
                float elapsedTime = 0f;
                while (elapsedTime < 1f)
                {
                    elapsedTime += Time.deltaTime;
                    currentValue = Mathf.Lerp(startValue, maxValue, elapsedTime / 1f);
                    UpdateDisplay(Mathf.RoundToInt(currentValue).ToString());
                    yield return null;
                }

                currentValue = maxValue;
                UpdateDisplay("Finished!");
            }

            // 4. 回到第 1 點執行
            ResetGame();
        }
    }

    private void ResetGame()
    {
        if (shouldSummonOnReset)
        {
            SummonNewPointer(); // 在重置時生成新的 prefab
            shouldSummonOnReset = false; // 重置標記
        }

        currentValue = 0f;
        isRunning = false;
        hasStopped = false;
        UpdateDisplay("Press Space to Start");
    }

    private void UpdateDisplay(string text)
    {
        if (displayText != null)
        {
            displayText.text = text;
        }
    }

    private void CalculateRanges()
    {
        effectRanges = new List<(float, float, EffectData)>();
        float currentStart = 1f;

        foreach (var eff in effects)
        {
            float length = eff.count;
            float start = currentStart;
            float end = currentStart + length - 1;

            effectRanges.Add((start, end, eff));
            currentStart = end + 1;
        }
    }

    private void ShowResult()
    {
        int stopValue = Mathf.RoundToInt(currentValue);

        foreach (var range in effectRanges)
        {
            if (stopValue >= range.start && stopValue <= range.end)
            {
                EffectData eff = range.effect;

                switch (eff.effectType)
                {
                    case EffectType.ATK:
                        UpdateDisplay($"Stopped at {stopValue} → ATK {Mathf.Abs(eff.value)}");
                        break;

                    case EffectType.Heal:
                        UpdateDisplay($"Stopped at {stopValue} → Heal {eff.value}");
                        break;

                    case EffectType.Call:
                        UpdateDisplay($"Stopped at {stopValue} → Call new pointer!");
                        shouldSummonOnReset = true; // 標記在下一次重置時生成新的 prefab
                        break;

                    default:
                        UpdateDisplay($"Stopped at {stopValue} → {eff.effectType} (not implemented)");
                        break;
                }

                return;
            }
        }

        UpdateDisplay($"Stopped at {stopValue} → No Effect");
    }

    private void SummonNewPointer()
    {
        foreach (var eff in effects)
        {
            if (eff.effectType == EffectType.Call && eff.summonPrefab != null)
            {
                // spawn position below current
                Vector3 newPos = transform.position + new Vector3(0, summonOffsetY, 0);

                GameObject newPointer = Instantiate(eff.summonPrefab, newPos, Quaternion.identity, transform.parent);

                // apply custom speed
                PointerController pc = newPointer.GetComponent<PointerController>();
                if (pc != null)
                {
                    pc.speed = eff.summonSpeed;
                }
            }
            else
            {
                Debug.LogWarning("Call effect triggered but summonPrefab is not assigned!");
            }
        }
    }
}

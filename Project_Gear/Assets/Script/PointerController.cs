using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PointerController : MonoBehaviour
{
    [Header("Pointer Settings")]
    public float maxValue = 360f;   // total range
    public float speed = 3f;        // increase per second
    private float currentValue = 0f;
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
        // future: Defense, Poison, Stun, etc.
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
        UpdateDisplay("Press Space to Start");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isRunning && !hasStopped)
            {
                // First press -> start
                currentValue = 0f;
                isRunning = true;
                hasStopped = false;
                UpdateDisplay("Start!");
            }
            else if (isRunning && !hasStopped)
            {
                // Second press -> stop
                isRunning = false;
                hasStopped = true;
                ShowResult();
            }
            else if (!isRunning && hasStopped)
            {
                // Third press -> reset
                ResetGame();
            }
        }

        if (isRunning)
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
            float length = eff.count * 3f;
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

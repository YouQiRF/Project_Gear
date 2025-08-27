using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreatRoulette : MonoBehaviour
{
    [Header("Roulette Settings")]
    public RectTransform pointer; // 指針的 RectTransform
    public PointerController pointerController; // 指向 PointerController 腳本
    public GameObject pieSegmentPrefab; // 圓餅圖片段的預製件
    public Transform pieParent; // 圓餅圖片段的父物件

    private List<(Image segmentImage, float startAngle, float endAngle)> pieSegments = new List<(Image, float, float)>();

    void Start()
    {
        if (pointerController != null)
        {
            GeneratePieChart();
        }
    }

    void Update()
    {
        if (pointerController != null)
        {
            // 更新指針的旋轉角度
            float angle = (pointerController.currentValue / pointerController.maxValue) * 360f;
            pointer.localRotation = Quaternion.Euler(0, 0, -angle);
        }
    }

    private void GeneratePieChart()
    {
        float totalRange = pointerController.maxValue;
        float currentStartAngle = 0f;

        // 確保 pieSegments 的數量足夠
        EnsurePieSegmentPool(pointerController.effects.Count);

        for (int i = 0; i < pointerController.effects.Count; i++)
        {
            var effect = pointerController.effects[i];

            // 計算每個片段的範圍
            float segmentRange = effect.count; // 每個 count 對應的範圍
            float fillAmount = segmentRange / totalRange;

            // 獲取或重用片段
            var (segmentImage, _, _) = pieSegments[i];

            // 設置片段的填充比例和顏色
            segmentImage.fillAmount = fillAmount;
            segmentImage.color = GetEffectColor(effect.effectType);

            // 設置片段的旋轉角度
            segmentImage.transform.localRotation = Quaternion.Euler(0, 0, -currentStartAngle);

            // 更新片段的起始和結束角度
            float startAngle = currentStartAngle; // 調整起始角度使其從頂部開始
            float endAngle = currentStartAngle + fillAmount * 360f;
            pieSegments[i] = (segmentImage, startAngle, endAngle);

            // 更新起始角度
            currentStartAngle += fillAmount * 360f;
        }

        // 隱藏多餘的片段
        for (int i = pointerController.effects.Count; i < pieSegments.Count; i++)
        {
            pieSegments[i].segmentImage.gameObject.SetActive(false);
        }
    }

    private void EnsurePieSegmentPool(int requiredCount)
    {
        // 如果現有片段不足，創建新的片段
        while (pieSegments.Count < requiredCount)
        {
            GameObject segment = Instantiate(pieSegmentPrefab, pieParent);
            Image segmentImage = segment.GetComponent<Image>();
            if (segmentImage != null)
            {
                pieSegments.Add((segmentImage, 0f, 0f));
            }
        }
    }

    public void UpdateVisibleRange(float startAngle, float endAngle)
    {
        foreach (var (segmentImage, segmentStartAngle, segmentEndAngle) in pieSegments)
        {
            // 檢查片段是否在指定範圍內
            if (IsAngleInRange(segmentStartAngle, segmentEndAngle, startAngle, endAngle))
            {
                segmentImage.gameObject.SetActive(true); // 顯示片段
            }
            else
            {
                segmentImage.gameObject.SetActive(false); // 隱藏片段
            }
        }
    }

    private bool IsAngleInRange(float segmentStart, float segmentEnd, float rangeStart, float rangeEnd)
    {
        // 將角度轉換為正值（0-360）
        segmentStart = NormalizeAngle(segmentStart);
        segmentEnd = NormalizeAngle(segmentEnd);
        rangeStart = NormalizeAngle(rangeStart);
        rangeEnd = NormalizeAngle(rangeEnd);

        // 檢查範圍是否重疊
        if (rangeStart < rangeEnd)
        {
            return (segmentStart >= rangeStart && segmentStart <= rangeEnd) ||
                   (segmentEnd >= rangeStart && segmentEnd <= rangeEnd);
        }
        else
        {
            // 跨越 0 度的範圍
            return (segmentStart >= rangeStart || segmentStart <= rangeEnd) ||
                   (segmentEnd >= rangeStart || segmentEnd <= rangeEnd);
        }
    }

    private float NormalizeAngle(float angle)
    {
        return (angle % 360f + 360f) % 360f;
    }

    private Color GetEffectColor(PointerController.EffectType effectType)
    {
        // 根據效果類型返回對應的顏色
        switch (effectType)
        {
            case PointerController.EffectType.ATK:
                return Color.red; // 攻擊
            case PointerController.EffectType.Heal:
                return Color.green; // 恢復
            case PointerController.EffectType.Call:
                return Color.blue; // 招喚
            default:
                return Color.white; // 預設顏色
        }
    }
}

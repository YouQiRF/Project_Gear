using UnityEngine;
using System.Collections;
public class StartBattle : MonoBehaviour
{
    [Header("Player Team")]
    public GameObject[] playerTeam = new GameObject[3]; // 玩家隊伍，最大 3 人

    [Header("Enemy Team")]
    public GameObject[] enemyTeam = new GameObject[3]; // 敵人隊伍，最大 3 人
    void Start()
    {
        // 初始化戰鬥
        SetupBattle();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SetupBattle()
    {
        // 檢查玩家隊伍
        Debug.Log("Player Team:");
        for (int i = 0; i < playerTeam.Length; i++)
        {
            if (playerTeam[i] != null)
            {
                Debug.Log($"Player {i + 1}: {playerTeam[i].name}");
            }
            else
            {
                Debug.Log($"Player {i + 1}: Empty Slot");
            }
        }

        // 檢查敵人隊伍
        Debug.Log("Enemy Team:");
        for (int i = 0; i < enemyTeam.Length; i++)
        {
            if (enemyTeam[i] != null)
            {
                Debug.Log($"Enemy {i + 1}: {enemyTeam[i].name}");
            }
            else
            {
                Debug.Log($"Enemy {i + 1}: Empty Slot");
            }
        }
    }
}

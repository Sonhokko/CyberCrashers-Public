using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using LootType = LootParams.LootType;

public class DailyRewards : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI status = null;
    [SerializeField] private Button claimButton = null;
    [SerializeField] private Image selectorEffect = null;
    [SerializeField] private GameObject dailyRewards = null;
    [SerializeField] private GameController gameController = null;
    [SerializeField] private MenuScript menuScript = null;
    [SerializeField] private Buff buffController = null;


    private Vector3[] dayPositions = new Vector3[]
    {
        new Vector3(-137.5f, 90f, 0),
        new Vector3(-52f, 90f, 0),
        new Vector3(45f, 90f, 0),
        new Vector3(129, 90f, 0),
        new Vector3(-120f, 0f, 0),
        new Vector3(-2f, 0f, 0),
        new Vector3(122f, 0f, 0)
    };

    // private int currentStreak
    // {
    //     get => PlayerPrefs.GetInt("currentStreak", 0);
    //     set => PlayerPrefs.SetInt("currentStreak", value);
    // }

    private System.DateTime? lastClaimTime
    {
        get
        {
            string data = PlayerPrefs.GetString("lastClaimedTime", null);
            if (!string.IsNullOrEmpty(data)) return System.DateTime.Parse(data);
            return null;
        }
        set
        {
            if (value != null) PlayerPrefs.SetString("lastClaimedTime", value.ToString());
            else PlayerPrefs.DeleteKey("lastClaimedTime");
        }
    }

    private bool canClaimReward = false;
    private int maxStreakCount = 7;
    private float claimCooldown = 24f;
    private float claimDeadLine = 48f;

    private void Awake()
    {
        if (SaveGame.sv.canOpenDaily)
        {
            UpdateRewardState();
            StartCoroutine(RewardStateUpdater());
        }
    }

    private void Start()
    {
        if (SaveGame.sv.canOpenDaily)
        {
            if (canClaimReward)
            {
                gameController.MenuGame();
                dailyRewards.SetActive(true);
                menuScript.HideHomeButton();
            }
            else
            {
                selectorEffect.gameObject.SetActive(false);
                gameController.PauseGame();
            }
        }
        SaveGame.sv.canOpenDaily = true;
    }

    private IEnumerator RewardStateUpdater()
    {
        while (true)
        {
            UpdateRewardState();
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    private void UpdateRewardState()
    {
        canClaimReward = true;

        if (lastClaimTime.HasValue)
        {
            var timeSpan = System.DateTime.UtcNow - lastClaimTime.Value;

            if (timeSpan.TotalHours > claimDeadLine)
            {
                lastClaimTime = null;
                selectorEffect.transform.localPosition = dayPositions[0];
                SaveGame.sv.currentStreak = 0;
            }
            else if (timeSpan.TotalHours < claimCooldown)
            canClaimReward = false;
        }
        UpdateRewardsUI();
    }

    private void UpdateRewardsUI()
    {
        claimButton.interactable = canClaimReward;

        if (canClaimReward)
        {
            status.text = Localization.item.dictionary["ClaimRewardTextFirst"];
            selectorEffect.gameObject.SetActive(true);
        }
        else
        {
            var nextClaimTime = lastClaimTime.Value.AddHours(claimCooldown);
            var currentClaimCooldown = nextClaimTime - System.DateTime.UtcNow;

            string cd = $"{currentClaimCooldown.Hours:D2}:{currentClaimCooldown.Minutes:D2}:{currentClaimCooldown.Seconds:D2}";
            status.text = Localization.item.dictionary["ClaimRewardTextTwo"] + cd + Localization.item.dictionary["ClaimRewardTextThree"];
        }
    }

    public void CanClaim()
    {
        if (!canClaimReward) return;

        selectorEffect.gameObject.SetActive(false);
 
        if (SaveGame.sv.currentStreak == 0) gameController.CoinSnatchMulty(250);
        else if (SaveGame.sv.currentStreak == 1) buffController.Set(LootType.EXPLOSION, true);
        else if (SaveGame.sv.currentStreak == 2) gameController.CoinSnatchMulty(500);
        else if (SaveGame.sv.currentStreak == 3) buffController.Set(LootType.FREEZE, true);
        else if (SaveGame.sv.currentStreak == 4) gameController.CoinSnatchMulty(1000);
        else if (SaveGame.sv.currentStreak == 5) buffController.Set(LootType.ASSISTANT, true);
        else gameController.CrystalCurrent(1);

        lastClaimTime = System.DateTime.UtcNow;
        SaveGame.sv.currentStreak = (SaveGame.sv.currentStreak + 1) % maxStreakCount;
        selectorEffect.transform.localPosition = dayPositions[SaveGame.sv.currentStreak];

        UpdateRewardState();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using GoogleMobileAds.Api;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject coins = null;
    public static Transform coinPar = null;

    [SerializeField] private Player player = null;
    [SerializeField] private Text coinText = null;
    [SerializeField] private Text shadowCoin = null;
    [SerializeField] private Text coinTextMenu = null;
    [SerializeField] private Text crystalText = null;
    [SerializeField] private Text crystalTextMenu = null;
    [SerializeField] private Buff buff = null;
    [SerializeField] public Image coinImg = null;
    [SerializeField] public Text healPoints = null;
    [SerializeField] public Text healPointsMenu = null;
    [SerializeField] private ItemShop crystalFunc = null;
    [SerializeField] private MenuScript menuScript = null;
    [SerializeField] private Difficult difficult = null;
    [SerializeField] private SoundController sound = null;
    [SerializeField] private Button advBut = null;

    [Header("GameObjects [Active/Deactive]")]
    [SerializeField] private CanvasGroup sliderParent = null;

    private IEnumerator sliderCoroutine = null;

    public AdMob ads = null;
    public bool stopShake = false;

    public ProtectInt coinValue = new ProtectInt();
    public ProtectInt crystalValue = new ProtectInt();

    public enum GameState
    {
        PLAY,
        PAUSE,
        MENU
    }

    public GameState state = GameState.PAUSE;

    [SerializeField] private Sprite[] pauseSprite = null;
    [SerializeField] private Image pauseImage = null;

    private void Start()
    {
        if (SaveGame.sv == null) Debug.Log("HELLO");
        coinPar = coinImg.transform;
        UpdateText();
        PauseGame();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && state == GameState.PLAY) PauseGame();
    }

    public void MenuGame()
    {
        PauseGame();
        SetSliderVisible(false);
        difficult.SetWarningVisible(false);
        difficult.SetScoreVisible(false);
        state = GameState.MENU;
    }

    public void PauseGame(bool rew = true)
    {
        Time.timeScale = 0;

        SetSliderVisible(true);
        if (rew) difficult.SetScoreVisible(false);
        ObstacleSpawner.thisScript.FreezeObstacles(false);
        state = GameState.PAUSE;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;

        ObstacleSpawner.thisScript.UnFreezeObstacles();
        state = GameState.PLAY;
    }

    public void ContinueGame()
    {
        SetSliderVisible(false);
        difficult.SetScoreVisible(true);
        menuScript.HideShop();
        menuScript.HideBuffs();
        UnpauseGame();
        pauseImage.sprite = pauseSprite[1];

    }

    public void ExitButton()
    {
        sound.PlayExitButton();
        RestartGame();
        MenuGame();
        menuScript.UnHideBuffs();
    }

    public void DeathGame()
    {
        PauseGame();
        SetSliderVisible(false);
        state = GameState.MENU;
        menuScript.HideShop();
        BonusAppearance.bonusAppearance.DestroyLoot();
        player.Restart();
        difficult.Death();
    }

    public void RestartGame()
    {
        ObstacleSpawner.thisScript.Explosion();
        BonusAppearance.bonusAppearance.DestroyLoot();
        buff.Restart();
        menuScript.UnhideShop();
        player.Restart();
        difficult.Reset();
        pauseImage.sprite = pauseSprite[0];
    }

    public void Destroyer(GameObject child) => Destroy(child);

    public IEnumerator CoinSnatch(Loot collideLoot)
    {
        SaveGame.sv.coinsNum = coinValue += 1;
        yield return new WaitUntil(() => collideLoot == null);
        StartCoroutine(crystalFunc.CoinGet());
        shadowCoin.text = coinTextMenu.text = coinText.text = (Int32.Parse(shadowCoin.text) + 1).ToString();
        if (Int32.Parse(shadowCoin.text) + 1 >= 2500)
        {
            double coines = (float)(Int32.Parse(shadowCoin.text) + 1)/(float)1000f - ((float)(Int32.Parse(shadowCoin.text) + 1)/(float)1000f) % 0.1;
            coinTextMenu.text = coinText.text = coines.ToString() + "K";
        }
    }

    public void CoinSnatchMulty(int coinCount = 1)
    {
        SaveGame.sv.coinsNum = coinValue += coinCount;
        UpdateText();
    }

    public void CrystalCurrent(int value = 1)
    {
        SaveGame.sv.crystalNum = crystalValue += value;
        UpdateText();
    }

    private void UpdateText()
    {
        crystalValue = SaveGame.sv.crystalNum;
        coinValue = SaveGame.sv.coinsNum;
        shadowCoin.text = coinTextMenu.text = coinText.text = coinValue.ToString();
        crystalTextMenu.text = crystalText.text = crystalValue.ToString();
        if (coinValue >= 1001)
        {
            double coines = (float)coinValue/(float)1000f - ((float)coinValue/(float)1000f) % 0.1;
            coinTextMenu.text = coinText.text = coines.ToString() + "K";
        }
        if (crystalValue >= 1001)
        {
            double crystalls = (float)crystalValue/(float)1000f - ((float)crystalValue/(float)1000f) % 0.1;
            crystalTextMenu.text = crystalText.text = crystalls.ToString() + "K";
        }
    }

    public void SetSliderVisible(bool isVisible)
    {
        if (sliderCoroutine != null) StopCoroutine(sliderCoroutine);
        sliderCoroutine = ChangeSliderTransparent(isVisible);
        StartCoroutine(sliderCoroutine);
    }

    public bool GetSliderVisible()
    {
        return sliderParent.alpha >= 0.5f ? true : false;
    }

    private IEnumerator ChangeSliderTransparent(bool isVisible)
    {
        float step = isVisible ? 0.1f : -0.1f;
        float targetAlpha = isVisible ? 1f : 0f;

        while (sliderParent.alpha != targetAlpha)
        {
            if (sliderParent.alpha > 1f) break;
            if (sliderParent.alpha < 0f) break;
            sliderParent.alpha += step;
            yield return new WaitForSecondsRealtime(0.05f);
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

using LootType = LootParams.LootType;

public class MenuScript : MonoBehaviour
{
    [SerializeField] private GameController gameController = null;
    [SerializeField] private GameObject panel = null;
    [SerializeField] private GameObject mainMenu = null;
    [SerializeField] private GameObject skinsMenu = null;
    [SerializeField] private GameObject coinsMenu = null;
    [SerializeField] private GameObject settingsMenu = null;
    [SerializeField] private GameObject backButton = null;
    [SerializeField] private GameObject groundsMenu = null;
    [SerializeField] private GameObject characterMenu = null;
    [SerializeField] private GameObject dailyRewards = null;
    [SerializeField] private GameObject buffsPanel = null;
    [SerializeField] private GameObject currencyPanel = null;
    [SerializeField] private GameObject homeButton = null;
    [SerializeField] private GameObject skinMenuButton = null;
    [SerializeField] private GameObject groundMenuButton = null;
    [SerializeField] private GameObject coinBonusesButton = null;
    [SerializeField] private GameObject shop = null;
    [SerializeField] private GameObject highScore = null;
    [SerializeField] private Image charTab = null;
    [SerializeField] private Image gunTab = null;
    [SerializeField] private GameObject exitButton = null;
    [SerializeField] private SoundController sound = null;
    [SerializeField] private Animator plusAnim = null;
    [SerializeField] public GameObject[] buyBuffs = null;
    private const int BuffsButton = 9;

    [SerializeField] private TextMeshProUGUI countDown = null;
    [SerializeField] private TextMeshProUGUI newHighScoreText = null;
    [SerializeField] private TextMeshProUGUI scoreText = null;

    [Header("Restart Menu")]
    [SerializeField] private Buff buffController = null;
    [SerializeField] private GameObject restartMenu = null;
    [SerializeField] private Button continueCoinButton = null;
    [SerializeField] private Button continueCrystallButton = null;
    [SerializeField] private Button continueAvertisingButton = null;
    [SerializeField] private TextMeshProUGUI continueCrystallValue = null;

    [Header("TimerForContinue")]
    [SerializeField] private Image timerImage;
    private bool disableAvertisingButton = false;
    private bool disableCoinButton = false;
    private int crystallMultiply = 1;

    private IEnumerator coroutine;

    private void Update()
    {
        if ((SaveGame.sv.coinsNum >= 50 && !plusAnim.enabled)
        || (SaveGame.sv.coinsNum < 50 && plusAnim.enabled)) plusAnim.enabled = !plusAnim.enabled;
        if (!plusAnim.enabled)
        {
            for (int i = 0; i <= 2; i++)
            {
                BuyScript.buyScript.buyBuff[i].transform.rotation = new Quaternion(0, 0, 0, 0);
                BuyScript.buyScript.buyBuff[i].transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public void StartButton()
    {
        gameController.PauseGame();
        panel.SetActive(false);
        currencyPanel.SetActive(true);
        dailyRewards.SetActive(false);
        homeButton.SetActive(true);
    }

    public void PanelButton()
    {
        sound.PlayClickOpen();
        currencyPanel.SetActive(false);
        panel.SetActive(true);
        highScore.SetActive(true);
        gameController.MenuGame();
        backButton.SetActive(false);
        homeButton.SetActive(false);
        UpadateBonusButton();

        // if (coroutine != null) StopCoroutine(coroutine);
        countDown.text = "";
    }

    public void DailyRewardButton()
    {
        panel.SetActive(false);
        dailyRewards.SetActive(true);
        backButton.SetActive(false);
    }

    public void HideBuffs()
    {
        for (int i = 0; i < BuffsButton; i++) {
            buyBuffs[i].SetActive(false);
        }
    }

    public void UnHideBuffs()
    {
        for (int i = 0; i < BuffsButton; i++) {
            buyBuffs[i].SetActive(true);
        }
    }

    public void HideHomeButton() => homeButton.SetActive(false);

    public void HideShop()
    {
        skinMenuButton.SetActive(false);
        groundMenuButton.SetActive(false);
        exitButton.SetActive(true);
        highScore.SetActive(true);
    }

    public void UnhideShop()
    {
        skinMenuButton.SetActive(true);
        groundMenuButton.SetActive(true);
        exitButton.SetActive(false);
        highScore.SetActive(true);
    }

    public void ShowRestartMenu()
    {
        coroutine = CountContinueTimer();
        StartCoroutine(coroutine);

        sound.PlayDeath();
        panel.SetActive(true);
        restartMenu.SetActive(true);
        backButton.SetActive(true);
        currencyPanel.SetActive(false);
        mainMenu.SetActive(false);
        skinsMenu.SetActive(false);
        coinsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        groundsMenu.SetActive(false);
        shop.SetActive(false);
        homeButton.SetActive(false);
        highScore.SetActive(false);

        scoreText.text = (Difficult.thisScript.score * 10).ToString();

        gameController.DeathGame();
        if (SaveGame.sv.coinsNum < 300 || disableCoinButton == true) continueCoinButton.interactable = false;
        else continueCoinButton.interactable = true;

        if (SaveGame.sv.crystalNum < 1) continueCrystallButton.interactable = false;
        else continueCrystallButton.interactable = true;

        if (disableAvertisingButton == false) continueAvertisingButton.interactable = true;
        else continueAvertisingButton.interactable = false;
    }

    public void DailyRewardCancelButton()
    {
        sound.PlayClickClose();
        dailyRewards.SetActive(false);
        highScore.SetActive(false);
        homeButton.SetActive(true);
        backButton.SetActive(false);
        gameController.PauseGame();
    }
    public void ChangeSkinsButton()
    {
        sound.PlayClickOpen();
        mainMenu.SetActive(false);
        characterMenu.SetActive(false);
        skinsMenu.SetActive(true);
        Color tmp = gunTab.color;
        tmp.a = 0f;
        gunTab.color = tmp;
        tmp.a = 0.8f;
        charTab.color = tmp;
        backButton.SetActive(true);
    }

    public void CharacterMenuButton()
    {
        sound.PlayClickOpen();
        shop.SetActive(true);
        mainMenu.SetActive(false);
        skinsMenu.SetActive(false);
        characterMenu.SetActive(true);
        Color tmp = charTab.color;
        tmp.a = 0f;
        charTab.color = tmp;
        tmp.a = 0.8f;
        gunTab.color = tmp;
        backButton.SetActive(true);
        buffsPanel.SetActive(true);
    }

    public void ChangeCoinsButton()
    {
        sound.PlayClickOpen();
        mainMenu.SetActive(false);
        coinsMenu.SetActive(true);
        backButton.SetActive(true);
    }

    public void GroundMenuButton()
    {
        sound.PlayClickOpen();
        mainMenu.SetActive(false);
        groundsMenu.SetActive(true);
        backButton.SetActive(true);
    }

    public void UpadateBonusButton()
    {
        if (SaveGame.sv.coinsNum <= 500) 
        {
            coinBonusesButton.SetActive(true);
        }
        else
        {
            coinBonusesButton.SetActive(false);
        }
    }

    public void CoinsBonusesButton()
    {
        sound.PlayClickOpen();
        gameController.ads.UserChoseToWatchAd();
        // SaveGame.sv.coinsNum += 500;
        gameController.CoinSnatchMulty(+500);
        UpadateBonusButton();
    }

    public void SettingButton()
    {
        sound.PlayClickOpen();
        mainMenu.SetActive(false);
        settingsMenu.SetActive(true);
        backButton.SetActive(true);
    }

    public void NewHighScore() => newHighScoreText.gameObject.SetActive(true);

    public void BackButton()
    {
        sound.PlayClickClose();
        // RESTART GAME
        if (restartMenu.activeInHierarchy)
        {
            gameController.RestartGame();
            StopCoroutine(coroutine);
            UnhideShop();
            UnHideBuffs();
            disableAvertisingButton = false;
            disableCoinButton = false;
            crystallMultiply = 1;
            continueCrystallValue.text = string.Empty;
            newHighScoreText.gameObject.SetActive(false);
        }

        backButton.SetActive(false);
        mainMenu.SetActive(true);
        skinsMenu.SetActive(false);
        coinsMenu.SetActive(false);
        settingsMenu.SetActive(false);
        groundsMenu.SetActive(false);
        shop.SetActive(false);
        restartMenu.SetActive(false);

        UpadateBonusButton();

        // if (!gameController.pauseAfterPlay) UnhideShop();
    }

    public void ContinueCoinButton()
    {
        if (SaveGame.sv.coinsNum >= 300)
        {
            disableCoinButton = true;
            sound.PlayClickOpen();
            gameController.CoinSnatchMulty(-300);
            restartMenu.SetActive(false);
            panel.SetActive(false);
            dailyRewards.SetActive(false);
            backButton.SetActive(false);
            highScore.SetActive(false);
            currencyPanel.SetActive(true);
            homeButton.SetActive(true);
            mainMenu.SetActive(true);
            newHighScoreText.gameObject.SetActive(false);
            gameController.PauseGame();

            ObstacleSpawner.thisScript.reverse = true;

            // UnHideBuffs();
        }
    }

    public void ContinueCrystallButton()
    {
        if (SaveGame.sv.crystalNum >= crystallMultiply)
        {
            sound.PlayClickOpen();


            SaveGame.sv.crystalNum -= crystallMultiply;
            restartMenu.SetActive(false);
            panel.SetActive(false);
            dailyRewards.SetActive(false);
            backButton.SetActive(false);
            highScore.SetActive(false);
            currencyPanel.SetActive(true);
            homeButton.SetActive(true);
            mainMenu.SetActive(true);
            newHighScoreText.gameObject.SetActive(false);
            gameController.PauseGame();

            ObstacleSpawner.thisScript.reverse = true;
            crystallMultiply *= 2;
            continueCrystallValue.text = "x" + crystallMultiply;
            // UnHideBuffs();
        }
    }

    public void ContinueAvertisingButton()
    {
        disableAvertisingButton = true;
        sound.PlayClickOpen();
        gameController.ads.UserChoseToWatchAd(false);
        restartMenu.SetActive(false);
        panel.SetActive(false);
        dailyRewards.SetActive(false);
        highScore.SetActive(false);
        backButton.SetActive(false);
        currencyPanel.SetActive(true);
        newHighScoreText.gameObject.SetActive(false);

        homeButton.SetActive(true);
        mainMenu.SetActive(true);
        // gameController.PauseGame();


        // UnHideBuffs();
    }

    public void SetEnglish()
    {
        sound.PlayClickOpen();
        Localization.Get().ChangeLanguage("en");
    }

    public void SetRussian()
    {
        sound.PlayClickOpen();
        Localization.Get().ChangeLanguage("ru");
    }


    private IEnumerator CountContinueTimer()
    {
        for (int countTimer = 20; countTimer >= 0; countTimer--)
        {
            timerImage.fillAmount = countTimer / 20f;
            yield return new WaitForSecondsRealtime(0.25f);
        }

        continueAvertisingButton.interactable = false;
        continueCoinButton.interactable = false;
        continueCrystallButton.interactable = false;
        yield return new WaitForSecondsRealtime(1f);

        BackButton();
    }
}

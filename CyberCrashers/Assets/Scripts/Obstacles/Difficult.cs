using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Difficult : MonoBehaviour
{
    [SerializeField] private MenuScript menuScript = null;
    [SerializeField] private Image lights = null;

    [SerializeField] private TextMeshProUGUI scoreText = null;
    [SerializeField] private TextMeshProUGUI hightScoreText = null;
    [SerializeField] private GameObject warningObject = null;
    private CanvasGroup warningGroup = null;
    private Animator scoreAnim = null;

    private float oldSpawnDelay;
    public ulong score { get; private set; } = 0;
    private ulong highScore = 0;
    public static Difficult thisScript { get; private set; } = null;

    [SerializeField] private bool showDifficultyDebug = false;

    private float displayedScore = 0;

    private float parabolaLenghtModifire = 7f;
    private float parabolaHeight = 0.05f;
    private float wavesPeriodicity = 20f;
    private float wavesScale = 0.15f;
    private float minimumStage = 1f;
    private float topValue = 4f;
    float spawnDelay = 0f;
    bool downSide = false;

    private float linearMax = 0.5f;
    private float linearDivider = 500f;
    public float linearPlus { get; private set; } = 0f;

    private uint linearObstacleMax = 100;
    private uint linearObstacleDivider = 40;
    public ulong linearObstacleStartPos { get; private set; } = 0;
    public ulong linearObstacleValue { get; private set; } = 0;

    Vector3 previousPoint = Vector3.zero;
    ulong currentPos = 0;

    private void Awake()
    {
        thisScript = this;
        warningGroup = warningObject.GetComponent<CanvasGroup>();
        scoreAnim = scoreText.GetComponent<Animator>();

        highScore = SaveGame.sv.hightScore;
        linearObstacleStartPos = highScore / linearObstacleDivider;

        spawnDelay = topValue;
    }

    private void Update()
    {
        if (displayedScore != score)
            displayedScore = Mathf.Lerp(displayedScore, score, 0.1f);
        if (score - displayedScore < 0.15f) displayedScore = score;

        currentPos = score % 22;

        if (showDifficultyDebug)
        {
            Debug.DrawLine(
                previousPoint - (ScreenParams.ScreenLeftSide * Vector3.left),
                new Vector3(currentPos * 0.25f, spawnDelay * 0.25f) - (ScreenParams.ScreenLeftSide * Vector3.left),
                Color.blue,
                20,
                true
            );
            Debug.DrawLine(new Vector3(ScreenParams.ScreenLeftSide, 0), new Vector3(ScreenParams.ScreenRightSide, 0), Color.red, 1);
            Debug.DrawLine(new Vector3(ScreenParams.ScreenLeftSide, topValue * 0.25f), new Vector3(ScreenParams.ScreenRightSide, topValue * 0.25f), Color.green, 1);
            previousPoint = new Vector3(currentPos * 0.25f, spawnDelay * 0.25f);
        }

        scoreText.text = ((ulong)displayedScore * 10) + "\0";
        hightScoreText.text = ((ulong)highScore * 10) + "\0";
    }

    private void ChangeMinimumThreshold(float threshold)
    {
        if (threshold < 0) threshold = 0.01f;
        minimumStage = threshold;
    }

    public void IncreaseScore()
    {
        scoreAnim.Play("Score Scale");
        score++;
        HighScore();

        if (linearPlus < linearMax)
            linearPlus = score / linearDivider;
        if (linearObstacleValue < linearObstacleMax)
            linearObstacleValue = score / linearObstacleDivider + linearObstacleStartPos;
        if (linearObstacleValue > linearObstacleMax)
            linearObstacleValue = linearObstacleMax;

        if (rainIE == null)
        {
            if (spawnDelay > minimumStage && !downSide)
                spawnDelay = topValue - Mathf.Sqrt((score / parabolaLenghtModifire) * parabolaHeight) + (Mathf.Sin(score * wavesPeriodicity) * wavesScale);
            else
            {
                downSide = true;
                spawnDelay = (Mathf.Sin(score * wavesPeriodicity) * wavesScale) + minimumStage;
            }
            if (spawnDelay < 0.75f)
                spawnDelay = 0.75f;
            ObstacleSpawner.thisScript.spawnDelay = spawnDelay;
        }
        if (score % 150 == 0 && rainIE == null)
        {
            rainIE = ObstacleRainEnumerator();
            StartCoroutine(rainIE);
        }
    }

    public void HighScore()
    {
        if (score > highScore)
        {
            highScore = score;
            menuScript.NewHighScore();
            linearObstacleStartPos = highScore / linearObstacleDivider;
        }

        SaveGame.sv.hightScore = highScore;
    }

    public void Death()
    {
        StopAllCoroutines();

        warningObject.SetActive(false);
        lights.gameObject.SetActive(false);
        oldSpawnDelay = ObstacleSpawner.thisScript.spawnDelay;
        ObstacleSpawner.thisScript.rain = false;
        rainIE = null;
        lighting = null;
    }

    public void Reset()
    {
        StopAllCoroutines();

        spawnDelay = topValue;
        displayedScore = 0f;
        score = 0;

        warningObject.SetActive(false);
        lights.gameObject.SetActive(false);
        ObstacleSpawner.thisScript.spawnDelay = 3;
        ObstacleSpawner.thisScript.rain = false;
        rainIE = null;
        lighting = null;
    }

    public void SetScoreVisible(bool isVisible)
    {
        scoreText.gameObject.SetActive(isVisible);
    }

    private IEnumerator rainIE = null;
    private IEnumerator ObstacleRainEnumerator()
    {
        SetWarningVisible(true);
        SetBlinking();
        yield return new WaitForSecondsRealtime(2.5f);

        int rnd = UnityEngine.Random.Range(0, 3);
        if (rnd == 0) BonusAppearance.bonusAppearance.type = LootParams.LootType.EXPLOSION;
        else if (rnd == 1) BonusAppearance.bonusAppearance.type = LootParams.LootType.FREEZE;
        else BonusAppearance.bonusAppearance.type = LootParams.LootType.ASSISTANT;

        yield return new WaitForSecondsRealtime(2.5f);
        SetWarningVisible(false);

        oldSpawnDelay = ObstacleSpawner.thisScript.spawnDelay;
        ObstacleSpawner.thisScript.spawnDelay = 0.15f;
        ObstacleSpawner.thisScript.rain = true;
        SetBlinking();
        yield return new WaitForSecondsRealtime(10f);

        ObstacleSpawner.thisScript.spawnDelay = oldSpawnDelay;
        ObstacleSpawner.thisScript.rain = false;

        rainIE = null;
    }

    public void SetWarningVisible(bool isVisible)
    {
        if (warningIE != null) StopCoroutine(warningIE);
        warningIE = ChangeWarningTransparent(isVisible);
        StartCoroutine(warningIE);
    }

    public bool GetWarningVisible()
    {
        return warningGroup.alpha >= 0.5f ? true : false;
    }

    private IEnumerator warningIE = null;
    private IEnumerator ChangeWarningTransparent(bool isVisible)
    {
        float step = isVisible ? 0.1f : -0.1f;
        float targetAlpha = isVisible ? 1f : 0f;

        warningObject.SetActive(true);
        while (warningGroup.alpha != targetAlpha)
        {
            if (warningGroup.alpha > 1f) break;
            if (warningGroup.alpha < 0f) break;
            warningGroup.alpha += step;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        warningObject.SetActive(isVisible);
    }


    public void SetBlinking()
    {
        if (lighting != null)
        {
            StopCoroutine(lighting);
            lights.gameObject.SetActive(false);
            lighting = null;
            return;
        }
        lighting = Blinking();
        StartCoroutine(lighting);
    }

    private IEnumerator lighting = null;
    private IEnumerator Blinking()
    {
        Color c = lights.color; 
        float alpha = 1.0f;
 
        lights.gameObject.SetActive(true);
        while (rainIE != null)
        {
            c.a = Mathf.MoveTowards(c.a, alpha, Time.deltaTime); 
            lights.color = c; 
            if (c.a == alpha)
            {
                if (alpha == 1.0f)
                {
                    alpha = 0.0f;
                }
                else
                    alpha = 1.0f;
            }
            yield return null;
        }
        lights.gameObject.SetActive(false);
    }
}

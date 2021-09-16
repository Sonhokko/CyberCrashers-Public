using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;

using GameState = GameController.GameState;

public class ScreenParams : MonoBehaviour
{
    [SerializeField] private Sprite menuButScale = null;
    [SerializeField] private Image menuBut = null;
    [SerializeField] private RectTransform currencyPan = null;
    [SerializeField] private RectTransform currencyPanMenuCoins = null;
    [SerializeField] private Player player = null;
    [SerializeField] private GameController gameController = null;
    [SerializeField] private GameObject topLeft = null;
    [SerializeField] private GameObject botRight = null;
    private static GameController gameControllerCopy = null;

    public static Camera Camera { get; private set; } = null;
    public static VHSPostProcessEffect Effect { get; private set; } = null;

    public static float ScreenLeftSide { get; private set; } = 0f;
    public static float ScreenRightSide { get; private set; } = 0f;
    public static float ScreenTopSide { get; private set; } = 0f;
    public static float ScreenBottomSide { get; private set; } = 0f;

    public static float ScreenTopSideOffseted { get; private set; } = 0f;
    public static float ScreenLeftSideOffseted { get; private set; } = 0f;
    public static float ScreenRightSideOffseted { get; private set; } = 0f;

    public static float LeftBuffPanelSide { get; private set; } = 0f;
    public static float RightBuffPanelSide { get; private set; } = 0f;
    public static float TopBuffPanelSide { get; private set; } = 0f;
    public static float BottomBuffPanelSide { get; private set; } = 0f;

    public static float width_left { get; private set; } = 0f;
    public static float width_right { get; private set; } = 0f;
    public static float height { get; private set; } = 0f;
    public static float obsHeight { get; private set; } = 0f;
    public static float floor { get; set; } = 0f;

    public static List<S_UnclickZone> unclickZones = new List<S_UnclickZone>();

    private void Awake()
    {
        ScreenParams.Camera = GetComponent<Camera>();
        ScreenParams.Effect = GetComponent<VHSPostProcessEffect>();

        float screenRatio = (float)Screen.width / (float)Screen.height;
        float targetRatio = player.spriteRenderer.bounds.size.x / player.spriteRenderer.bounds.size.y;

        if (screenRatio >= targetRatio) Camera.orthographicSize = (player.spriteRenderer.bounds.size.y / 2);
        else
        {
            float differenceInSize = targetRatio / screenRatio;
            Camera.orthographicSize = (player.spriteRenderer.bounds.size.y / 2 * differenceInSize);
        }

        ScreenLeftSide = Camera.ScreenToWorldPoint(Vector3.zero).x;
        ScreenRightSide = Camera.ScreenToWorldPoint(Vector3.right * Screen.width).x;
        ScreenBottomSide = Camera.ScreenToWorldPoint(Vector3.zero).y;
        ScreenTopSide = Camera.ScreenToWorldPoint(Vector3.up * Screen.height).y;

        ScreenTopSideOffseted = ScreenTopSide + 1.5f;
        ScreenLeftSideOffseted = ScreenLeftSide - 1.5f;
        ScreenRightSideOffseted = ScreenRightSide + 1.5f;
        gameControllerCopy = gameController;
    }

    private void SetBounds()
    {
        width_left = width_right = 1 / (ScreenParams.Camera.WorldToViewportPoint(new Vector3(1, 1, 0)).x - .5f) / 2;
        height = 1 / (ScreenParams.Camera.WorldToViewportPoint(new Vector3(1, 1, 0)).y - .5f) / 2;
    }

    private float CurrencyBot()
    {
        currencyPan.offsetMax = new Vector2(currencyPan.offsetMax.x, currencyPan.offsetMin.y + currencyPan.offsetMax.y);
        float botPosition = currencyPan.transform.position.y;
        currencyPan.offsetMax = new Vector2(currencyPan.offsetMax.x, 0);
        return botPosition;
    }

    public void Config(Rect dev)
    {
        StartCoroutine(player.PlayerAlignment());
        obsHeight = CurrencyBot();
        width_right = botRight.transform.position.x;
        width_left = topLeft.transform.position.x;
        height = topLeft.transform.position.y;
        floor = player.transform.position.y - player.boxCollider.size.y * .5f * player.transform.localScale.y;
    }

    private void Start()
    {
        Config(new Rect(0, 0, 0, 0));
    }

    public static bool CheckInZones()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

        if (results.Count != 0)
        {
            for (int i = 0; i < results.Count; i++)
            {
                if (results[i].gameObject.name == "Score" || results[i].gameObject.name == "FPS")
                {
                    results.Remove(results[i]);
                    break;
                }

                if (results[i].gameObject.name == "BufViewport" && gameControllerCopy.state == GameState.PLAY )
                {
                    results.Clear();
                    break;
                }

                if (results[i].gameObject.name == "BuffPanel" && gameControllerCopy.state == GameState.PLAY )
                {
                    results.Clear();
                    break;
                }

                if (results[i].gameObject.name == "buffVieverPrefab" && gameControllerCopy.state == GameState.PLAY )
                {
                    results.Clear();
                    break;
                }
            }
        }
        return results.Count > 0;
    }
}

public struct S_UnclickZone {
    public float left;
    public float right;
    public float top;
    public float bottom;

    public S_UnclickZone(float left, float right, float top, float bottom)
    {
        this.left = left;
        this.right = right;
        this.top = top;
        this.bottom = bottom;
    }
}

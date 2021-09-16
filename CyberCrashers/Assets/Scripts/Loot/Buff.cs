using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

using LootType = LootParams.LootType;

public class Buff : MonoBehaviour
{
    [SerializeField] private Player player = null;
    [SerializeField] private Transform buffContent = null;
    [SerializeField] private SoundController sound = null;
    [SerializeField] private GameController gameController = null;
    [SerializeField] private MenuScript menuController = null;
    [SerializeField] private GameObject blckHole = null;
    [SerializeField] private GameObject freezPhon = null;
    [SerializeField] private GameObject assist = null;
    public RectTransform buffPan = null;
    public BoxCollider2D panelCollider = null;

    public BuffItem[] buffList = new BuffItem[3];
    public float buffPanHeight = 0f;

    private float barStep = 0.05f;
    private float barPause = 0.5f;
    private Color transparent = new Color(1f, 1f, 1f, 0.4f);

    private void Awake()
    {
        buffList[0] = PreloadBuff(0, PressExplosion, LootType.EXPLOSION);
        buffList[1] = PreloadBuff(1, PressAssistant, LootType.ASSISTANT);
        buffList[2] = PreloadBuff(2, PressFreeze, LootType.FREEZE);
    }

    private void Update()
    {
        for (int i = 0; i < buffList.Length; i++)
        {
            buffList[i].image.fillAmount = buffList[i].fill;
        }

        if (buffPanHeight != 0) buffPan.position = new Vector3(0, (player.transform.position.y - player.boxCollider.size.y * 0.5f * player.transform.localScale.y) - buffPanHeight, buffPan.position.z);
    }

    public BuffItem Get(LootType type) { return buffList[GetIndex(type)]; }
    public void Set(LootType type, bool value)
    {
        buffList[GetIndex(type)].isInInventory = value;
        buffList[GetIndex(type)].image.color = Color.white;
    }

    private BuffItem PreloadBuff(int childId, UnityAction action, LootType type)
    {
        BuffItem item = new BuffItem(type);
        Transform original = buffContent.GetChild(childId);
        item.image = original.GetComponent<Image>();
        // if (item.image.color == Color.white) item.isInInventory = true;
        original.GetComponent<Button>().onClick.AddListener(action);
        return item;
    }

    public static int GetIndex(LootType type)
    {
        if (type == LootType.EXPLOSION) return 0;
        else if (type == LootType.ASSISTANT) return 1;
        else if (type == LootType.FREEZE) return 2;
        else return 0;
    }

    public void Restart()
    {
        StopAllCoroutines();
        for (int i = 0; i < 3; i++) buffContent.GetChild(i).GetComponent<Button>().interactable = true;

        buffList[0].isInInventory = false;
        buffList[0].cooldown = false;
        buffList[0].image.color = transparent;
        buffList[0].fill = 1f;
        buffList[0].image.fillAmount = 1f;
        buffList[1].isInInventory = false;
        buffList[1].cooldown = false;
        buffList[1].image.color = transparent;
        buffList[1].fill = 1f;
        buffList[1].image.fillAmount = 1f;
        buffList[2].isInInventory = false;
        buffList[2].cooldown = false;
        buffList[2].image.color = transparent;
        buffList[2].fill = 1f;
        buffList[2].image.fillAmount = 1f;
    }

    private void PressExplosion()
    {
        if (!buffList[0].cooldown && buffList[0].isInInventory && gameController.state == GameController.GameState.PLAY)
        {
            sound.PlayPremiumBuffs();
            StartCoroutine(StartCooldown(0));
            StartCoroutine(player.PressExplosion());
            buffList[0].isInInventory = false;
            buffList[0].image.color = transparent;
        }
    }

    private void PressAssistant()
    {
        if (!buffList[1].cooldown && buffList[1].isInInventory && gameController.state == GameController.GameState.PLAY)
        {
            sound.PlayPremiumBuffs();
            StartCoroutine(StartCooldown(1));
            player.PressAssistant();
            buffList[1].isInInventory = false;
            buffList[1].image.color = transparent;
        }
    }

    private void PressFreeze()
    {
        if (!buffList[2].cooldown && buffList[2].isInInventory && gameController.state == GameController.GameState.PLAY)
        {
            sound.PlayPremiumBuffs();
            StartCoroutine(StartCooldown(2));
            player.PressFreeze();
            buffList[2].isInInventory = false;
            buffList[2].image.color = transparent;
        }
    }

    private bool WaitIt(int id)
    {
        if (id == 0) return blckHole.activeInHierarchy;
        else if (id == 2) return freezPhon.activeInHierarchy;
        return assist.activeInHierarchy;
    }

    private IEnumerator StartCooldown(int id)
    {
        if (id != 1)
        {
            buffContent.GetChild(0).GetComponent<Button>().interactable = false;
            buffContent.GetChild(2).GetComponent<Button>().interactable = false;
        }
        else buffContent.GetChild(1).GetComponent<Button>().interactable = false;
        yield return new WaitUntil(() => WaitIt(id));
        yield return new WaitWhile(() => WaitIt(id));
        if (id != 1)
        {
            buffContent.GetChild(0).GetComponent<Button>().interactable = true;
            buffContent.GetChild(2).GetComponent<Button>().interactable = true;
        }
        else buffContent.GetChild(1).GetComponent<Button>().interactable = true;
        buffList[id].cooldown = true;
        for (float x = 0f; x <= 1f; x += barStep)
        {
            buffList[id].fill = x;
            yield return new WaitForSeconds(barPause);
        }
        buffList[id].fill = 1f;
        buffList[id].cooldown = false;
    }
}

public class BuffItem
{
    public bool isInInventory = false;
    public LootType type = 0;
    public Image image = null;

    public bool cooldown = false;
    public float fill = 1f;

    public BuffItem(LootType type)
    {
        this.type = type;
    }
}

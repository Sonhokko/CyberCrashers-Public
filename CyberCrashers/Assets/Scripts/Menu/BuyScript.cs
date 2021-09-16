using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using LootType = LootParams.LootType;

public class BuyScript : MonoBehaviour
{
    [SerializeField] private Buff buffs = null;
    [SerializeField] private ItemShopCrystal shop = null;
    [SerializeField] private GameController controller = null;
    [SerializeField] private Player playerController = null;
    [SerializeField] private SoundController sound = null;
    [SerializeField] public GameObject[] buyBuff = null;
    [SerializeField] private GameObject coinsPic = null;



    [Space]
    [Header("CHAR SKINS")]

    [SerializeField] private TextMeshProUGUI[] textHearts = null;
    [SerializeField] private TextMeshProUGUI[] textSpeed = null;
    [SerializeField] private TextMeshProUGUI[] textLuck = null;



    [SerializeField] private GameObject player = null;
    [SerializeField] private GameObject hand = null;
    [SerializeField] private GameObject gun = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private Sprite[] charHands = null;
    [SerializeField] private Sprite[] charSprites = null;
    [SerializeField] private SpriteRenderer handSprite = null;
    [SerializeField] private SpriteRenderer playerSpriteRender = null;
    [SerializeField] private BoxCollider2D playerBoxCollider = null;
    [SerializeField] private AnimationClip[] charStayAnim = null;

    public static BuyScript buyScript { get; private set; } = null;
    private const int charSkinCount = 8;
    public CharType currentPlayer = (CharType)8;
    private bool[] buyCharSkinButton = { true, false, false, false, false, false, false, false};

    [SerializeField] private Button[] charSkinsButton = null;
    private readonly Dictionary<CharType, Currency> charCurrency = new Dictionary<CharType, Currency>
    {
        {CharType.charOne, Currency.coins},
        {CharType.charTwo, Currency.coins},
        {CharType.charTree, Currency.coins},
        {CharType.charFour, Currency.coins},
        {CharType.charFive, Currency.coins},
        {CharType.charSix, Currency.coins},
        {CharType.charSeven, Currency.coins},
        {CharType.charEight, Currency.coins}
    };
    
    private readonly Dictionary<CharType, int> charPrice = new Dictionary<CharType, int>
    {
        {CharType.charOne, 300},
        {CharType.charTwo, 300},
        {CharType.charTree, 300},
        {CharType.charFour, 300},
        {CharType.charFive, 300},
        {CharType.charSix, 300},
        {CharType.charSeven, 300},
        {CharType.charEight, 300}
    };

    private readonly Dictionary<CharType, Vector3> charScale = new Dictionary<CharType, Vector3>
    {
        {CharType.charOne, new Vector3(0.1510426f, 0.1445967f, 1f)},
        {CharType.charTwo, new Vector3(0.1264947f, 0.1220564f, 1f)},
        {CharType.charTree, new Vector3(0.1428242f, 0.1322784f, 1f)},
        {CharType.charFour, new Vector3(0.1264947f, 0.1220564f, 1f)},
        {CharType.charFive, new Vector3(0.1462778f, 0.1543756f, 1f)},
        {CharType.charSix, new Vector3(0.1569633f, 0.1580692f, 1f)},
        {CharType.charSeven, new Vector3(0.1264947f, 0.1220564f, 1f)},
        {CharType.charEight, new Vector3(0.1264947f, 0.1220564f, 1f)}
    };

    private readonly Dictionary<CharType, Vector3> handsTransform = new Dictionary<CharType, Vector3>
    {
        {CharType.charOne, new Vector3(-0.8f, -0.18f, -0.02f)},
        {CharType.charTwo, new Vector3(-0.309f, -1.008f, -0.02f)},
        {CharType.charTree, new Vector3(-0.97f, -0.502f, -0.02f)},
        {CharType.charFour, new Vector3(-0.23f, -0.692f, -0.02f)},
        {CharType.charFive, new Vector3(-0.93f, -0.209f, -0.02f)},
        {CharType.charSix, new Vector3(-0.915f, 0.059f, -0.02f)},
        {CharType.charSeven, new Vector3(-0.247f, -0.475f, -0.02f)},
        {CharType.charEight, new Vector3(-0.36f, -0.291f, -0.02f)}
    };


    public static readonly Dictionary<CharType, float[]> charFeature = new Dictionary<CharType, float[]>
    {
        {CharType.charOne, new float[]{3,0.5f,15}},
        {CharType.charTwo, new float[]{3,1f,19}},
        {CharType.charTree, new float[]{3,1.5f,22}},
        {CharType.charFour, new float[]{4,1.8f, 25}},
        {CharType.charFive, new float[] {4,2f, 29}},
        {CharType.charSix, new float[]{4, 2.3f,32}},
        {CharType.charSeven, new float[]{5, 3f,38}},
        {CharType.charEight, new float[]{5, 3f,45}}
    };

    [Space]
    [Header("GUN SKINS")]

    [SerializeField] private TextMeshProUGUI[] textBulletSpeed = null;
    [SerializeField] private TextMeshProUGUI[] textGunSpeed = null;
    [SerializeField] private TextMeshProUGUI[] textCount = null;

    [SerializeField] private AudioSource shoot = null;
    [SerializeField] private AudioClip laser = null;
    [SerializeField] private AudioClip bulletShoot = null;



    [SerializeField] private Button[] gunSkinsButton = null;
    [SerializeField] private Sprite[] gunSprite = null;
    [SerializeField] private SpriteRenderer gunSpriteRender = null;

    private const int gunSkinCount = 10;
    private GunType currentGun = GunType.gunOne;
    private bool[] buyGunSkinButton = { true, false, false, false, false, false, false, false, false, false};

    private readonly Dictionary<GunType, Currency> gunCurrency = new Dictionary<GunType, Currency>
    {
        {GunType.gunOne, Currency.coins},
        {GunType.gunTwo, Currency.coins},
        {GunType.gunTree, Currency.coins},
        {GunType.gunFour, Currency.coins},
        {GunType.gunFive, Currency.coins},
        {GunType.gunSix, Currency.coins},
        {GunType.gunSeven, Currency.coins},
        {GunType.gunEight, Currency.coins},
        {GunType.gunNine, Currency.coins},
        {GunType.gunTen, Currency.coins}
    };
    private readonly Dictionary<GunType, int> gunPrice = new Dictionary<GunType, int>
    {
        {GunType.gunOne, 300},
        {GunType.gunTwo, 300},
        {GunType.gunTree, 300},
        {GunType.gunFour, 300},
        {GunType.gunFive, 300},
        {GunType.gunSix, 300},
        {GunType.gunSeven, 300},
        {GunType.gunEight, 300},
        {GunType.gunNine, 300},
        {GunType.gunTen, 300}
    };

    private readonly Dictionary<GunType, Vector3[]> gunTransformLocation = new Dictionary<GunType, Vector3[]>
    {
        {GunType.gunOne, new Vector3[] { new Vector3(-0.72f,0.11f,-0.01f), new Vector3(-0.74f,-0.79f,-0.01f), new Vector3(-0.88f,-0.32f,-0.01f), new Vector3(-0.59f,-0.48f,-0.01f), new Vector3(-0.97f,0.12f,-0.01f), new Vector3(-1.02f,0.43f,-0.01f), new Vector3(-0.81f,-0.24f,-0.01f), new Vector3(-0.77f,-0.06f,-0.01f)}},
        {GunType.gunTwo, new Vector3[] { new Vector3(-0.64f,0.86f,-0.01f), new Vector3(-0.37f,-0.07f,-0.01f), new Vector3(-0.77f,0.55f,-0.01f), new Vector3(-0.3f,0.11f,-0.01f), new Vector3(-0.89f,0.96f,-0.01f), new Vector3(-0.98f,1.24f,-0.01f), new Vector3(-0.76f,0.55f,-0.01f), new Vector3(-0.54f,0.75f,-0.01f) }},
        {GunType.gunTree, new Vector3[] { new Vector3(-0.55f,0.95f,-0.01f), new Vector3(-0.38f,0.01f,-0.01f), new Vector3(-0.64f,0.61f,-0.01f), new Vector3(-0.38f,0.41f,-0.01f), new Vector3(-0.74f,1.04f,-0.01f), new Vector3(-0.82f,1.3f,-0.01f), new Vector3(-0.48f,0.6f,-0.01f), new Vector3(-0.49f,0.91f,-0.01f) }},
        {GunType.gunFour, new Vector3[] { new Vector3(-0.41f,0.48f,-0.01f), new Vector3(-0.27f,-0.38f,-0.01f), new Vector3(-0.5f,0.15f,-0.01f), new Vector3(-0.26f,-0.04f,-0.01f), new Vector3(-0.62f,0.56f,-0.01f), new Vector3(-0.7f,0.86f,-0.01f), new Vector3(-0.35f,0.16f,-0.01f), new Vector3(-0.42f,0.33f,-0.01f) }},
        {GunType.gunFive, new Vector3[] { new Vector3(-0.5f,0.85f,-0.01f), new Vector3(-0.27f,-0.17f,-0.01f), new Vector3(-0.55f,0.41f,-0.01f), new Vector3(-0.09f,0.15f,-0.01f), new Vector3(-0.65f,0.74f,-0.01f), new Vector3(-0.7f,1.1f,-0.01f), new Vector3(-0.38f,0.34f,-0.01f), new Vector3(-0.35f,0.43f,-0.01f) }},
        {GunType.gunSix, new Vector3[] { new Vector3(-0.58f,0.91f,-0.01f), new Vector3(-0.3f,-0.18f,-0.01f), new Vector3(-0.63f,0.46f,-0.01f), new Vector3(-0.27f,0.14f,-0.01f), new Vector3(-0.67f,0.88f,-0.01f), new Vector3(-0.74f,1.16f,-0.01f), new Vector3(-0.35f,0.48f,-0.01f), new Vector3(-0.32f,0.54f,-0.01f) }},
        {GunType.gunSeven, new Vector3[] { new Vector3(-0.41f,0.83f,-0.01f), new Vector3(-0.24f,-0.26f,-0.01f), new Vector3(-0.51f,0.5f,-0.01f), new Vector3(-0.18f,0.08f,-0.01f), new Vector3(-0.62f,0.83f,-0.01f), new Vector3(-0.72f,1.11f,-0.01f), new Vector3(-0.32f,0.34f,-0.01f), new Vector3(-0.31f,0.37f,-0.01f) }},
        {GunType.gunEight, new Vector3[] { new Vector3(-0.48f,1.44f,-0.01f), new Vector3(-0.39f,0.67f,-0.01f), new Vector3(-0.56f,1.11f,-0.01f), new Vector3(-0.39f,0.93f,-0.01f), new Vector3(-0.73f,1.37f,-0.01f), new Vector3(-0.73f,1.44f,-0.01f), new Vector3(-0.41f,0.64f,-0.01f), new Vector3(-0.52f,0.81f,-0.01f) }},
        {GunType.gunNine, new Vector3[] { new Vector3(-0.41f,0.76f,-0.01f), new Vector3(-0.32f,-0.01f,-0.01f), new Vector3(-0.32f,0.5f,-0.01f), new Vector3(-0.31f,0.27f,-0.01f), new Vector3(-0.59f,0.79f,-0.01f), new Vector3(-0.56f,1f,-0.01f), new Vector3(-0.43f,0.54f,-0.01f), new Vector3(-0.38f,0.7f,-0.01f) }},
        {GunType.gunTen, new Vector3[] { new Vector3(-0.45f,0.73f,-0.01f), new Vector3(-0.3f,-0.2f,-0.01f), new Vector3(-0.55f,0.45f,-0.01f), new Vector3(-0.18f,0.08f,-0.01f), new Vector3(-0.62f,0.85f,-0.01f), new Vector3(-0.67f,1.14f,-0.01f), new Vector3(-0.37f,0.51f,-0.01f), new Vector3(-0.36f,0.58f,-0.01f) }}
    };

//fireDelay, bulletSpeed, bulletNumb
    private readonly Dictionary<GunType, float[]> gunFeature = new Dictionary<GunType, float[]>
    {
        {GunType.gunOne, new float[]{0.6f,7f,0f}},
        {GunType.gunTwo, new float[]{0.6f,8f,0f}},
        {GunType.gunTree, new float[]{0.7f,8f,1f}},
        {GunType.gunFour, new float[]{0.6f,9f,1f}},
        {GunType.gunFive, new float[]{0.5f,10f,0f}},
        {GunType.gunSix, new float[]{0.4f,9f,1f}},
        {GunType.gunSeven, new float[]{0.4f,10f,2f}},
        {GunType.gunEight, new float[]{0.4f,13f,1f}},
        {GunType.gunNine, new float[]{0.4f,14f,2f}},
        {GunType.gunTen, new float[]{0.5f,15f,3f}}
    };

    [Space]
    [Header("BACKGROUND SKINS")]


    [SerializeField] private Button[] backgroundButton = null;
    [SerializeField] private Sprite[] backgroundSprite = null;
    [SerializeField] private Image backgroundImage = null;

    private const int backgroundCount = 9;
    private Background currentBackground = Background.backgroundOne;
    private bool[] buyBackgroundButton = { true, false, false, false, false, false, false, false, false};

    private readonly Dictionary<Background, Currency> backgroundCurrency = new Dictionary<Background, Currency>
    {
        {Background.backgroundOne, Currency.coins},
        {Background.backgroundTwo, Currency.coins},
        {Background.backgroundThree, Currency.coins},
        {Background.backgroundFour, Currency.coins},
        {Background.backgroundFive, Currency.coins},
        {Background.backgroundSix, Currency.coins},
        {Background.backgroundSeven, Currency.coins},
        {Background.backgroundEight, Currency.coins},
        {Background.backgroundNine, Currency.coins}
    };
    private readonly Dictionary<Background, int> backgroundPrice = new Dictionary<Background, int>
    {
        {Background.backgroundOne, 300},
        {Background.backgroundTwo, 300},
        {Background.backgroundThree, 300},
        {Background.backgroundFour, 300},
        {Background.backgroundFive, 300},
        {Background.backgroundSix, 300},
        {Background.backgroundSeven, 300},
        {Background.backgroundEight, 300},
        {Background.backgroundNine, 300}
    };

    private Vector3 startpoint;
    private Vector3 endpoint;
    [SerializeField] private float coinsSpeed = 0.2f;
    private bool coinsOn = false;
    private float timeLeft = 0.4f;
    



    private void Awake()
    {
        buyScript = this;
        currentPlayer = CharType.charNone;
        UpdateSkins();
        UpdateGuns();
        UpdateBackground();
        AcceptPlayer((CharType)SaveGame.sv.currentPlayer, true);
        AcceptGun((GunType)SaveGame.sv.currentGun, true);
        AcceptBackground((Background)SaveGame.sv.currentBackground);
        UpdateButtons();
        UpdateText();
        UpdateTextGun();

        endpoint = Vector3.right;
    }

    private void Start()
    {
        startpoint = coinsPic.transform.position;
    }

    private void Update()
    {
        if(coinsOn)
        {
            timeLeft -= Time.fixedDeltaTime;

            coinsPic.transform.Translate(endpoint * 0.03f);
            if(coinsPic.transform.position.x >= (startpoint.x + 0.03)) {
                endpoint = Vector3.left;
            }
            else if (coinsPic.transform.position.x <= startpoint.x - 0.03) {
                endpoint = Vector3.right;
            }

            if(timeLeft < 0)
            {
                coinsOn = false;
                timeLeft = 0.4f;
                coinsPic.transform.position = startpoint;
            }
        }
    }

    private void UpdateText()
    {
        int heart = 0;
        float speed = 0;
        int luck = 0;

        for(int i = 0; i < charSkinCount; i++)
        {
            heart = (int)(charFeature[(CharType)i][0]);
            textHearts[i].text = heart.ToString();

            speed = (charFeature[(CharType)i][1] * 10);
            textSpeed[i].text = speed.ToString();

            luck = (int)(charFeature[(CharType)i][2]);
            textLuck[i].text = luck.ToString();
        }
    }


    private void UpdateSkins()
    {
        for(int i = 0; i < charSkinCount; i++) {
            buyCharSkinButton[i] = SaveGame.sv.buySkins[i];
        }
        for(int i = 0; i < charSkinCount; i++)
        {
            if(buyCharSkinButton[i])
            {
                charSkinsButton[i].transform.GetChild(0).gameObject.SetActive(true);
                charSkinsButton[i].transform.GetChild(1).gameObject.SetActive(false);
                charSkinsButton[i].transform.GetChild(2).gameObject.SetActive(false);
                charSkinsButton[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }

    }

    private void UpdateBackground()
    {
        for(int i = 0; i < backgroundCount; i++) {
            buyBackgroundButton[i] = SaveGame.sv.backgroundSkins[i];
        }
        for(int i = 0; i < backgroundCount; i++) {
            if(buyBackgroundButton[i])
            {
                backgroundButton[i].transform.GetChild(0).gameObject.SetActive(true);
                backgroundButton[i].transform.GetChild(1).gameObject.SetActive(false);
                backgroundButton[i].transform.GetChild(2).gameObject.SetActive(false);
                backgroundButton[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }

    }

    private void UpdateGuns()
    {
        for(int i = 0; i < gunSkinCount; i++) {
            buyGunSkinButton[i] = SaveGame.sv.gunSkins[i];
        }
        for(int i = 0; i < gunSkinCount; i++) {
            if(buyGunSkinButton[i])
            {
                gunSkinsButton[i].transform.GetChild(0).gameObject.SetActive(true);
                gunSkinsButton[i].transform.GetChild(1).gameObject.SetActive(false);
                gunSkinsButton[i].transform.GetChild(2).gameObject.SetActive(false);
                gunSkinsButton[i].transform.GetChild(3).gameObject.SetActive(false);
            }
        }

    }

    private void UpdateSound(GunType gun) 
    {
        if(gun == GunType.gunOne || gun == GunType.gunTwo || gun == GunType.gunFour ||
            gun == GunType.gunSix || gun == GunType.gunSix)
        {
            shoot.clip = bulletShoot;
        }
        else
        {
            shoot.clip = laser;
        }
    }

    private void UpdateTextGun()
    {
        int bulletSpeed = 0;
        int gunSeed = 0;
        int size = 0;

        for(int i = 0; i < gunSkinCount; i++)
        {
            bulletSpeed = (10 - (int)(gunFeature[(GunType)i][0] * 10));
            textBulletSpeed[i].text = bulletSpeed.ToString();

            gunSeed = (int)(gunFeature[(GunType)i][1]);
            textGunSpeed[i].text = gunSeed.ToString();

            size = (int)(gunFeature[(GunType)i][2]) + 1;
            textCount[i].text = size.ToString();
        }
    }
    private void UpdateButtons()
    {
        charSkinsButton[(int)currentPlayer].gameObject.SetActive(false);
        gunSkinsButton[(int)currentGun].gameObject.SetActive(false);
        backgroundButton[(int)currentBackground].gameObject.SetActive(false);
    }

    private void AcceptPlayer(CharType character, bool isAwake = false)
    {
        if(currentPlayer != character)
        {
            sound.PlayClickOpen();
            ChooseSprite(character);
            ChangeAnimation(character);
            ChangeHand(character);
            ChangeGunTransform(character, currentGun);
            ChangeScale(character);
            if (currentPlayer != CharType.charNone)
            {
                charSkinsButton[(int)currentPlayer].gameObject.SetActive(true);
                charSkinsButton[(int)character].gameObject.SetActive(false);
            }
            SaveGame.sv.currentPlayer = (int)character;
            currentPlayer = character;
            SetFeaters(character);
            if (!isAwake) StartCoroutine(playerController.PlayerAlignment());
        }
    }

        private void AcceptGun(GunType gun, bool isAwake = false)
    {
        if(currentGun != gun || isAwake)
        {
            sound.PlayClickOpen();
            // UpdateTextDeltaGun(currentGun, gun);
            gunSpriteRender.sprite = gunSprite[(int)gun];
            gunSkinsButton[(int)currentGun].gameObject.SetActive(true);
            gunSkinsButton[(int)gun].gameObject.SetActive(false);
            SaveGame.sv.currentGun = (int)gun;
            currentGun = gun;
            ChangeGunTransform(currentPlayer, gun);
            SetGunFeaters(gun);
            UpdateSound(gun);
        }
    }

    private void AcceptBackground(Background backgroundType)
    {
        if(currentBackground != backgroundType)
        {
            sound.PlayClickOpen();
            backgroundImage.sprite = backgroundSprite[(int)backgroundType];
            backgroundButton[(int)currentBackground].gameObject.SetActive(true);
            backgroundButton[(int)backgroundType].gameObject.SetActive(false);
            SaveGame.sv.currentBackground = (int)backgroundType;
            currentBackground = backgroundType;
        }
    }


    private void ChangeHand(CharType character)
    {
        handSprite.sprite = charHands[(int)character];
        hand.transform.localPosition = handsTransform[character];
    }

    private void ChangeGunTransform(CharType character, GunType guntype)
    {
        gun.transform.localPosition = gunTransformLocation[guntype][(int)character];
    }

    private void UpdateColider(CharType character)
    {
        playerBoxCollider.size = charSprites[(int)character].bounds.size;
    }


    private void ChangeScale(CharType character)
    {
        player.gameObject.transform.localScale = charScale[character];
        UpdateColider(character);
    }

    private void ChooseSprite(CharType character)
    {
        AnimatorOverrideController overrideController;
        if (animator.runtimeAnimatorController.GetType() == typeof(AnimatorOverrideController))
        {
            overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            overrideController["Stay"] = charStayAnim[(int)character];
            animator.runtimeAnimatorController = overrideController;
        }
        else
        {
            overrideController = new AnimatorOverrideController(); 
            overrideController.runtimeAnimatorController = animator.runtimeAnimatorController;
            overrideController["Stay"] = charStayAnim[(int)character];
            animator.runtimeAnimatorController = overrideController;
        }
    }


    private void BuySkin(CharType character)
    {
        if (charCurrency[character] == Currency.coins) 
        {
            if (SaveGame.sv.coinsNum >= charPrice[character]) 
            {
                sound.PlayClickOpen();
                controller.CoinSnatchMulty(charPrice[character] * -1);
                buyCharSkinButton[(int)character] = SaveGame.sv.buySkins[(int)character] = true;
                ChangeCharSkinButton((int)character);
                AcceptPlayer(character);
            }
        }
        else if (charCurrency[character] == Currency.crystal)
        {
            if (SaveGame.sv.crystalNum >= charPrice[character]) 
            {
                sound.PlayClickOpen();
                controller.CrystalCurrent(charPrice[character] * -1);
                buyCharSkinButton[(int)character] = SaveGame.sv.buySkins[(int)character] = true;
                ChangeCharSkinButton((int)character);
                AcceptPlayer(character);
            }
        }
    }

    private void BuyGun(GunType gun)
    {
        if (gunCurrency[gun] == Currency.coins) 
        {
            if (SaveGame.sv.coinsNum >= gunPrice[gun]) 
            {
                sound.PlayClickOpen();
                controller.CoinSnatchMulty(gunPrice[gun] * -1);
                buyGunSkinButton[(int)gun] = SaveGame.sv.gunSkins[(int)gun] = true;
                ChangeGunSkinButton((int)gun);
                AcceptGun(gun);
            }
        }
        else if (gunCurrency[gun] == Currency.crystal)
        {
            if (SaveGame.sv.crystalNum >= gunPrice[gun]) 
            {
                sound.PlayClickOpen();
                controller.CrystalCurrent(gunPrice[gun] * -1);
                buyGunSkinButton[(int)gun] = SaveGame.sv.gunSkins[(int)gun] = true;
                ChangeGunSkinButton((int)gun);
                AcceptGun(gun);
            }
        }
    }

    private void BuyBackground(Background background)
    {
        if (backgroundCurrency[background] == Currency.coins) 
        {
            if (SaveGame.sv.coinsNum >= backgroundPrice[background]) 
            {
                sound.PlayClickOpen();
                controller.CoinSnatchMulty(backgroundPrice[background] * -1);
                buyBackgroundButton[(int)background] = SaveGame.sv.backgroundSkins[(int)background] = true;
                ChangeBackgroundButton((int)background);
                AcceptBackground(background);
            }
        }
        else if (backgroundCurrency[background] == Currency.crystal)
        {
            if (SaveGame.sv.crystalNum >= backgroundPrice[background]) 
            {
                sound.PlayClickOpen();
                controller.CrystalCurrent(backgroundPrice[background] * -1);
                buyBackgroundButton[(int)background] = SaveGame.sv.backgroundSkins[(int)background] = true;
                ChangeBackgroundButton((int)background);
                AcceptBackground(background);
            }
        }
    }
    
    private void ChangeCharSkinButton(int skinNum)
    {
        charSkinsButton[skinNum].transform.GetChild(0).gameObject.SetActive(true);
        charSkinsButton[skinNum].transform.GetChild(1).gameObject.SetActive(false);
        charSkinsButton[skinNum].transform.GetChild(2).gameObject.SetActive(false);
        charSkinsButton[skinNum].transform.GetChild(3).gameObject.SetActive(false);
    }
    private void ChangeGunSkinButton(int skinNum)
    {
        gunSkinsButton[skinNum].transform.GetChild(0).gameObject.SetActive(true);
        gunSkinsButton[skinNum].transform.GetChild(1).gameObject.SetActive(false);
        gunSkinsButton[skinNum].transform.GetChild(2).gameObject.SetActive(false);
        gunSkinsButton[skinNum].transform.GetChild(3).gameObject.SetActive(false);
    }

    private void ChangeBackgroundButton(int skinNum)
    {
        backgroundButton[skinNum].transform.GetChild(0).gameObject.SetActive(true);
        backgroundButton[skinNum].transform.GetChild(1).gameObject.SetActive(false);
        backgroundButton[skinNum].transform.GetChild(2).gameObject.SetActive(false);
        backgroundButton[skinNum].transform.GetChild(3).gameObject.SetActive(false);
    }

    private void SetFeaters(CharType character) 
    {
        playerController.health = playerController.healthMax = (int)charFeature[character][0];
        controller.healPointsMenu.text = controller.healPoints.text = playerController.health.ToString();
        playerController.MoveSpeedval = charFeature[character][1];
    }

    private void SetGunFeaters(GunType gun) 
    {
        playerController.FireDelayVal = gunFeature[gun][0];
        playerController.BulletSpeed = gunFeature[gun][1];
        playerController.BulletCount = (int)gunFeature[gun][2];
        playerController.BulletType = (uint)gun;

    }


#region BuySkinButtons

    public void BuySkinButtonOne()
    {
        if(buyGunSkinButton[(int)GunType.gunOne]) 
        {
            AcceptGun(GunType.gunOne);
        }
        else
        {
            BuyGun(GunType.gunOne);
        }
    }
    public void BuySkinButtonTwo()
    {
        if(buyGunSkinButton[(int)GunType.gunTwo]) 
        {
            AcceptGun(GunType.gunTwo);
        }
        else
        {
            BuyGun(GunType.gunTwo);
        }
    }
    public void BuySkinButtonThree()
    {
        if(buyGunSkinButton[(int)GunType.gunTree]) 
        {
            AcceptGun(GunType.gunTree);
        }
        else
        {
            BuyGun(GunType.gunTree);
        }
    }
    public void BuySkinButtonFour()
    {
        if(buyGunSkinButton[(int)GunType.gunFour]) 
        {
            AcceptGun(GunType.gunFour);
        }
        else
        {
            BuyGun(GunType.gunFour);
        }
    }
    public void BuySkinButtonFive()
    {
        if(buyGunSkinButton[(int)GunType.gunFive]) 
        {
            AcceptGun(GunType.gunFive);
        }
        else
        {
            BuyGun(GunType.gunFive);
        }
    }
    public void BuySkinButtonSix()
    {
        if(buyGunSkinButton[(int)GunType.gunSix]) 
        {
            AcceptGun(GunType.gunSix);
        }
        else
        {
            BuyGun(GunType.gunSix);
        }
    }
    public void BuySkinButtonSeven()
    {
        if(buyGunSkinButton[(int)GunType.gunSeven]) 
        {
            AcceptGun(GunType.gunSeven);
        }
        else
        {
            BuyGun(GunType.gunSeven);
        }
    }
    public void BuySkinButtonEight()
    {
        if(buyGunSkinButton[(int)GunType.gunEight]) 
        {
            AcceptGun(GunType.gunEight);
        }
        else
        {
            BuyGun(GunType.gunEight);
        }
    }
    public void BuySkinButtonNine()
    {
        if(buyGunSkinButton[(int)GunType.gunNine]) 
        {
            AcceptGun(GunType.gunNine);
        }
        else
        {
            BuyGun(GunType.gunNine);
        }
    }
    public void BuySkinButtonTen()
    {
        if(buyGunSkinButton[(int)GunType.gunTen]) 
        {
            AcceptGun(GunType.gunTen);
        }
        else
        {
            BuyGun(GunType.gunTen);
        }
    }
#endregion BuySkinButtons

#region BuyGroundButtons

    public void BuyGroundButtonOne()
    {
        if(buyBackgroundButton[(int)Background.backgroundOne]) 
        {
            AcceptBackground(Background.backgroundOne);
        }
        else
        {
            BuyBackground(Background.backgroundOne);
        }
    }
    public void BuyGroundButtonTwo()
    {
        if(buyBackgroundButton[(int)Background.backgroundTwo]) 
        {
            AcceptBackground(Background.backgroundTwo);
        }
        else
        {
            BuyBackground(Background.backgroundTwo);
        }
    }
    public void BuyGroundButtonThree()
    {
        if(buyBackgroundButton[(int)Background.backgroundThree]) 
        {
            AcceptBackground(Background.backgroundThree);
        }
        else
        {
            BuyBackground(Background.backgroundThree);
        }
    }
    public void BuyGroundButtonFour()
    {
        if(buyBackgroundButton[(int)Background.backgroundFour]) 
        {
            AcceptBackground(Background.backgroundFour);
        }
        else
        {
            BuyBackground(Background.backgroundFour);
        }
    }
    public void BuyGroundButtonFive()
    {
        if(buyBackgroundButton[(int)Background.backgroundFive]) 
        {
            AcceptBackground(Background.backgroundFive);
        }
        else
        {
            BuyBackground(Background.backgroundFive);
        }
    }
    public void BuyGroundButtonSix()
    {
        if(buyBackgroundButton[(int)Background.backgroundSix]) 
        {
            AcceptBackground(Background.backgroundSix);
        }
        else
        {
            BuyBackground(Background.backgroundSix);
        }
    }
    public void BuyGroundButtonSeven()
    {
        if(buyBackgroundButton[(int)Background.backgroundSeven]) 
        {
            AcceptBackground(Background.backgroundSeven);
        }
        else
        {
            BuyBackground(Background.backgroundSeven);
        }
    }
    public void BuyGroundButtonEight()
    {
        if(buyBackgroundButton[(int)Background.backgroundEight]) 
        {
            AcceptBackground(Background.backgroundEight);
        }
        else
        {
            BuyBackground(Background.backgroundEight);
        }
    }
    public void BuyGroundButtonNine()
    {
        if(buyBackgroundButton[(int)Background.backgroundNine]) 
        {
            AcceptBackground(Background.backgroundNine);
        }
        else
        {
            BuyBackground(Background.backgroundNine);
        }
    }

#endregion BuyGroundButtons

    public void BuyCoinButton()
    {
        controller.ads.UserChoseToWatchAd();
    }

    public void BuyCrystalButton()
    {
        shop.quantity = 50;
        StartCoroutine(shop.CrystalBuy());
        sound.PlayPremiumBuffs();
    }

#region BuyCharacterButton

    public void BuyCharacterOne()
    {
        if(buyCharSkinButton[(int)CharType.charOne]) 
        {
            AcceptPlayer(CharType.charOne);
        }
        else
        {
            BuySkin(CharType.charOne);
        }
    }
    public void BuyCharacterTwo()
    {
        Debug.Log("Press button two");
        if(buyCharSkinButton[(int)CharType.charTwo]) 
        {
            AcceptPlayer(CharType.charTwo);
        }
        else
        {
            Debug.Log("Press false");
            BuySkin(CharType.charTwo);
        }
    }
    public void BuyCharacterThree()
    {
        if(buyCharSkinButton[(int)CharType.charTree]) 
        {
            AcceptPlayer(CharType.charTree);
        }
        else
        {
            BuySkin(CharType.charTree);
        }
    }
    public void BuyCharacterFour()
    {
        if(buyCharSkinButton[(int)CharType.charFour]) 
        {
            AcceptPlayer(CharType.charFour);
        }
        else
        {
            BuySkin(CharType.charFour);
        }
    }
    public void BuyCharacterFive()
    {
        if(buyCharSkinButton[(int)CharType.charFive]) 
        {
            AcceptPlayer(CharType.charFive);
        }
        else
        {
            BuySkin(CharType.charFive);
        }
    }
    public void BuyCharacterSix()
    {
        if(buyCharSkinButton[(int)CharType.charSix]) 
        {
            AcceptPlayer(CharType.charSix);
        }
        else
        {
            BuySkin(CharType.charSix);
        }
    }
    public void BuyCharacterSeven()
    {
       if(buyCharSkinButton[(int)CharType.charSeven]) 
        {
            AcceptPlayer(CharType.charSeven);
        }
        else
        {
            BuySkin(CharType.charSeven);
        } 
    }
    public void BuyCharacterEight()
    {
        if(buyCharSkinButton[(int)CharType.charEight]) 
        {
            AcceptPlayer(CharType.charEight);
        }
        else
        {
            BuySkin(CharType.charEight);
        }
    }

    private void MoveCoins()
    {
        coinsOn = true;
    }

    private void ChangeAnimation(CharType type) 
    {
        Debug.Log("CHANGE ANIM on: " + (int)type);
        player.GetComponent<Animator>().SetInteger("CharType", (int)type);
    }


    public void BuyExplosion()
    {
        if (SaveGame.sv.coinsNum >= 50) 
        {
            controller.CoinSnatchMulty(-50);
            buffs.Set(LootType.EXPLOSION, true);
            BonusAppearance.bonusAppearance.buffs.Add((int)LootParams.LootType.EXPLOSION);
            buyBuff[0].SetActive(false);
            buyBuff[3].SetActive(false);
            buyBuff[6].SetActive(false);
        }
        else
        {
            MoveCoins();
        }
    }

    public void BuyFreez()
    {
        if (SaveGame.sv.coinsNum >= 50) 
        {
            controller.CoinSnatchMulty(-50);
            buffs.Set(LootType.FREEZE, true);
            BonusAppearance.bonusAppearance.buffs.Add((int)LootParams.LootType.FREEZE);
            buyBuff[1].SetActive(false);
            buyBuff[4].SetActive(false);
            buyBuff[7].SetActive(false);
        }
        else
        {
            MoveCoins();
        }
    }

    public void BysAssist()
    {
        if (SaveGame.sv.coinsNum >= 50) 
        {
            controller.CoinSnatchMulty(-50);
            buffs.Set(LootType.ASSISTANT, true);
            BonusAppearance.bonusAppearance.buffs.Add((int)LootParams.LootType.ASSISTANT);
            buyBuff[2].SetActive(false);
            buyBuff[5].SetActive(false);
            buyBuff[8].SetActive(false);
        }
        else
        {
            MoveCoins();
        }
    }


public enum CharType {
    charOne,  //CyberRanger
    charTwo,
    charTree,
    charFour,
    charFive,
    charSix,
    charSeven,
    charEight,
    charNone
}

public enum GunType {
    gunOne,  //a99
    gunTwo,
    gunTree,
    gunFour,
    gunFive,
    gunSix,
    gunSeven,
    gunEight,
    gunNine,
    gunTen
}

public enum Currency {
    coins,
    crystal
}

public enum Background {
    backgroundOne,
    backgroundTwo,
    backgroundThree,
    backgroundFour,
    backgroundFive,
    backgroundSix,
    backgroundSeven,
    backgroundEight,
    backgroundNine,
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
#if UNITY_EDITOR
using UnityEditor.Animations;
#endif

using LootType = LootParams.LootType;
using LootState = Loot.LootState;
using GameState = GameController.GameState;

public class Player : MonoBehaviour
{
    [SerializeField] private Buff buffController = null;
    [SerializeField] private GameObject shieldObject = null;
    [SerializeField] private GameObject bulletsParent = null;
    [SerializeField] private GameObject[] bulletPrefabs = null;
    [SerializeField] private uint bulletType = 0;
    [SerializeField] public GameController gameController = null;
    [SerializeField] public Assistant assistant = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private MenuScript menuScript = null;

    //for BuffViewer
    [SerializeField] private GameObject buffViewerPrefab = null;
    [SerializeField] private Transform buffViewerContent = null;


    // Indication
    public SpriteRenderer spriteRenderer = null;
    [SerializeField] private SpriteRenderer armRenderer = null;
    [SerializeField] private SpriteRenderer gunRenderer = null;

    [SerializeField] private SoundController sound = null;


    private bool is_FacingRight = true;

    private Camera mainCamera = null;
    public BoxCollider2D boxCollider = null;

    // private bool lookToRight = true;

    // PLAYER HEALTH VARS
    [SerializeField] private bool godMode = false;
    [SerializeField] public int healthMax = 4;
    [SerializeField] public int health = 3;
    private float indicateScale = 0.15f;
    private bool damageCooldown = false;

    // PLAYER MOVING VARS
    public Vector3 startPos;
    private float screenX = 0f;
    private float screenY = 0f;
    private float realX = 0f;
    private float realXBuf = 0f;                                               // це треба?

    // PLAYER SHOOTING VARS
    [Header("MOVE SPEED")]
    [SerializeField] private float moveSpeedMax = 5f;
    [SerializeField] private float moveSpeed = 0.05f;
    [SerializeField] public float moveSpeedPlus = 0f;
    private Queue<IEnumerator> moveSpeedIE = new Queue<IEnumerator>();
    [Space]
    [Header("SPEED SHOOT")]
    [SerializeField] private float fireDelayMin = 0.05f;
    [SerializeField] private float fireDelay = 0.2f;
    [SerializeField] public float fireDelayMinus = 0f;
    private Queue<IEnumerator> speedShootIE = new Queue<IEnumerator>();
    [Space]
    [Header("BULLET SPEED")]
    [SerializeField] private float bulletSpeedMax = 15f;
    [SerializeField] private float bulletSpeed = 5f;
    [SerializeField] public float bulletSpeedPlus = 0f;
    private Queue<IEnumerator> bulletSpeedIE = new Queue<IEnumerator>();
    [Space]
    [Header("BULLET SIZE")]
    [SerializeField] private float bulletScaleMax = 0.875f;
    [SerializeField] private float bulletScale = 0.425f;
    [SerializeField] public float bulletScalePlus = 0f;
    [SerializeField] private bool autoFire = false;
    private Queue<IEnumerator> bulletSizeIE = new Queue<IEnumerator>();
    [Space]
    [Header("BULLET COUNT")]
    [SerializeField] private int count = 0;
    [SerializeField] public int countPlus = 0;
    private Queue<IEnumerator> bulletCountIE = new Queue<IEnumerator>();

    // BUFF VARS
    private UnityAction<GameObject>[] actionByType = null;
    public uint keepShield = 0;
    [SerializeField] public GameObject backGround;

    private readonly int[][] angles = new int[][]
    {
        new int[] {90},
        new int[] {120, 90, 60},
        new int[] {150, 120, 90, 60, 30},
        new int[] {180, 150, 120, 90, 60, 30, 0}
    };

    private readonly int anglesCount = 3;

    private Vector3 bulletOffset = Vector3.up * 1.25f;

    private List<Vector3> positionList = new List<Vector3>();

    private Color transparentColor = new Color(1f, 1f, 1f, 0f);

    private IEnumerator freezeDelayIE = null;
    private IEnumerator fireDelayIE = null;
    private IEnumerator damageIndicate = null;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        actionByType = new UnityAction<GameObject>[LootParams.lootCount + 1];
        actionByType[0] += TakeAssistant;
        actionByType[1] += TakeExplosion;
        actionByType[2] += TakeFreeze;
        actionByType[3] += TakeHeal;
        actionByType[4] += TakeShield;
        actionByType[5] += TakeBulletsCount;
        actionByType[6] += TakeBulletsSize;
        actionByType[7] += TakeBulletsSpeed;
        actionByType[8] += TakeSpeedShoot;
        actionByType[9] += TakeSpeedMove;
        actionByType[10] += TakeCoin;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        screenX = Screen.width / 2;
        health = healthMax;
        gameController.healPointsMenu.text = gameController.healPoints.text = health.ToString();
    }

    private void FixedUpdate()
    {
        if (!ObstacleSpawner.thisScript.reverse)
        {
            positionList.Add(transform.position);
            if (positionList.Count > ObstacleSpawner.thisScript.metSteps) positionList.RemoveAt(0);
        }
        else
        {
            if (positionList.Count > 0)
            {
                int lastPos = positionList.Count - 1;
                MoveTo(positionList[lastPos].x);
                positionList.RemoveAt(lastPos);
            }
        }
    }

    private void Update()
    {
        if (ObstacleSpawner.thisScript.reverse)
        {
            if (Time.timeScale < 8) Time.timeScale += 0.1f;
            backGround.transform.GetChild(0).gameObject.SetActive(true);
            ScreenParams.Effect.enabled = true;
        }

        if (health < 1) menuScript.ShowRestartMenu();

        // BULLETS BY SHOT BORDERS
        if (count < 0) count = 0;
        if (count > anglesCount) count = 3;

        if (Input.GetMouseButton(0) && gameController.state != GameState.MENU && !ObstacleSpawner.thisScript.reverse)
        {
            screenX = Input.mousePosition.x;
            screenY = Input.mousePosition.y;
            MoveTo();
        }

        if (autoFire) Fire();

        // PLAYER AND ASSISTENT SIDE CONTROL
        if (gameController.state == GameState.PLAY) 
        {
            float resIsLeftSide = realX - transform.position.x;
            bool isLeftSided = realX > transform.position.x ? true : false;
            if (resIsLeftSide >= 0.02f || resIsLeftSide <= -0.02f) ChangeDirection(isLeftSided);
            else resIsLeftSide = 0f;
            animator.SetFloat("Speed", Mathf.Abs(resIsLeftSide));
        }

        if (gameController.state != GameState.MENU)
        {
            transform.position = Vector3.Lerp(
                transform.position,
                new Vector3(realX, transform.position.y, transform.position.z),
                (moveSpeed + moveSpeedPlus) * (Time.deltaTime * 20f)
            );
        }

        if (transform.position.x + boxCollider.size.x * 0.5f * transform.localScale.x < ScreenParams.width_left)
        {
            transform.position = new Vector3(ScreenParams.width_left - boxCollider.size.x * 0.5f * transform.localScale.x, transform.position.y, transform.position.z);
            realX = transform.position.x;
        }
        if (transform.position.x + boxCollider.size.x * 0.5f * transform.localScale.x > ScreenParams.width_right)
        {
            transform.position = new Vector3(ScreenParams.width_right - boxCollider.size.x * 0.5f * transform.localScale.x, transform.position.y, transform.position.z);
            realX = transform.position.x;
        }

        foreach (Transform coin in LootParams.thisScript.transform)
        {
            Loot lootCoin = coin.GetComponent<Loot>();
            if (lootCoin.type == LootType.COIN && lootCoin.state == LootState.ACCOUNTED)
                coin.position = Vector3.MoveTowards(coin.position, lootCoin.dist, 500f * Time.deltaTime);
        }
    }

    public void ListAnig() => positionList.Clear();

    public IEnumerator PlayerAlignment()
    {
        buffController.buffPan.offsetMin = new Vector2(buffController.buffPan.offsetMin.x, buffController.buffPan.offsetMin.y + (buffController.panelCollider.size.y * 2));
        transform.position = new Vector3(0, buffController.buffPan.transform.position.y + boxCollider.size.y * .5f * transform.localScale.y, 0);
        buffController.buffPan.offsetMin = new Vector2(buffController.buffPan.offsetMin.x, buffController.buffPan.offsetMin.y - (buffController.panelCollider.size.y * 2));
        startPos = transform.position;
        yield return new WaitWhile(() => ScreenParams.floor == 0);
        buffController.buffPanHeight = ScreenParams.floor - buffController.buffPan.position.y;
    }

    public void Restart()
    {
        buffController.Restart();

        foreach (Transform child in bulletsParent.transform)
            GameObject.Destroy(child.gameObject);

        screenX = Screen.width / 2;
        realX = 0f;
        realXBuf = 0f;
        health = healthMax;
        gameController.healPointsMenu.text = gameController.healPoints.text = health.ToString();

        spriteRenderer.color = Color.white;
        armRenderer.color = Color.white;
        gunRenderer.color = Color.white;
        indicateScale = 0.15f;
        damageCooldown = false;

        // Clear Buffs IE
        while (moveSpeedIE.Count > 0)
            StopCoroutine(moveSpeedIE.Dequeue());
        while (speedShootIE.Count > 0)
            StopCoroutine(speedShootIE.Dequeue());
        while (bulletSpeedIE.Count > 0)
            StopCoroutine(bulletSpeedIE.Dequeue());
        while (bulletSizeIE.Count > 0)
            StopCoroutine(bulletSizeIE.Dequeue());
        while (bulletCountIE.Count > 0)
            StopCoroutine(bulletCountIE.Dequeue());

        if (freezeDelayIE != null)
        {
            StopCoroutine(freezeDelayIE);
            freezeDelayIE = null;
        }
        if (damageIndicate != null)
        {
            StopCoroutine(damageIndicate);
            damageIndicate = null;
        }

        keepShield = 0;
        moveSpeedPlus = 0f;
        countPlus = 0;
        bulletScalePlus = 0f;
        bulletSpeedPlus = 0f;
        fireDelayMinus = 0f;
        shieldObject.SetActive(false);
        assistant.gameObject.SetActive(false);

        assistant.Reset();
        ObstacleSpawner.thisScript.ResetBuffs();

        BonusAppearance.bonusAppearance.buffs.Clear();
        gameController.stopShake = true;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        is_FacingRight = true;
        animator.SetFloat("Speed", 0f);
        animator.Play("Stay");
    }

    private void ChangeDirection(bool isLeftSided)
    {
        if (is_FacingRight && !isLeftSided)
        {
            Flip();
        }
        if (!is_FacingRight && isLeftSided)
        {
            Flip();
        }
        assistant.isLeftSide = isLeftSided;
    }
    
    private void MoveTo(float x = 1000)
    {
        if (!ScreenParams.CheckInZones())
        {
            if (!autoFire && !ObstacleSpawner.thisScript.reverse) Fire();
            if (gameController.state == GameState.PAUSE)
            {
                gameController.ContinueGame();
                buffController.buffPan.pivot = new Vector2(.5f, .5f);
            }

            realXBuf = realX;
            if (x != 1000) realX = x;
            else realX = mainCamera.ScreenToWorldPoint(new Vector3(screenX, screenY, 0)).x;
        }
    }

    /*
        --------------
         BULLETS PART
        --------------
    */

    private void BulletSpawner()
    {
        int realCount = count + countPlus;
        realCount = realCount > anglesCount ? anglesCount : realCount;

        float realScale = bulletScale + bulletScalePlus;
        realScale = realScale > bulletScaleMax ? bulletScaleMax : realScale;

        float realSpeed = bulletSpeed + bulletSpeedPlus;
        realSpeed = realSpeed > bulletSpeedMax ? bulletSpeedMax : realSpeed;
        if (bulletType > 9) bulletType = 9; // Max Possible Bullet Type (HARD CODDED)
        for (int i = 0; i < angles[realCount].Length; i++)
        {
            GameObject newBullet = Instantiate(
                bulletPrefabs[bulletType],
                transform.GetChild(0).position,
                bulletPrefabs[bulletType].transform.rotation,
                bulletsParent.transform
            );

            newBullet.transform.localScale *= realScale;
            newBullet.transform.rotation = Quaternion.Euler(0, 0, angles[realCount][i]);
            newBullet.GetComponent<Bullet>().SetUpBullet(realSpeed);
            float part_1 = 3/newBullet.transform.localScale.x;
            float width = 1 / (ScreenParams.Camera.WorldToViewportPoint(new Vector3(1, 1, 0)).x - .5f) / 2;
            newBullet.transform.localScale = new Vector3(width/part_1, width/part_1, newBullet.transform.localScale.z);
        }
    }

    private void Fire()
    {
        if (fireDelayIE == null)
        {
            fireDelayIE = FireDelay();
            StartCoroutine(fireDelayIE);
            sound.PlayShoot();
        }
    }

    private IEnumerator FireDelay()
    {
        BulletSpawner();
        yield return new WaitForSeconds(fireDelay);
        fireDelayIE = null;
    }

    /*
        -------------
         HEALTH PART
        -------------
    */

    private void PushDamage()
    {
        if (keepShield < 1)
        {
            if (!damageCooldown )
            {
                if (!ObstacleSpawner.thisScript.reverse)
                {
                    if (health > 1) CameraShake.Shake(1.2f, 0.2f);
                    health -= 1;
                    gameController.healPointsMenu.text = gameController.healPoints.text = health.ToString();
                    StartCoroutine(DamageCooldown());
                    if (damageIndicate == null)
                    {
                        damageIndicate = DamageIndicate();
                        StartCoroutine(damageIndicate);
                    }
                }
            }
        }
        else if (keepShield > 0)
        {
            keepShield -= 1;
            if (keepShield < 1)
            {
                BonusAppearance.bonusAppearance.buffs.Remove((int)LootParams.LootType.SHIELD);
                shieldObject.SetActive(false);
            }
        }
    }

    private void PushHeal()
    {
        health = healthMax;
        gameController.healPointsMenu.text = gameController.healPoints.text = health.ToString();
    }

    private void TakeHeal(GameObject colideObj)
    {
        if (health < healthMax)
        {
            PushHeal();
            BonusAppearance.bonusAppearance.buffs.Remove((int)LootParams.LootType.HEAL);
            Destroy(colideObj);
        }
    }

    private IEnumerator DamageCooldown()
    {
        damageCooldown = true;
        yield return new WaitForSeconds(0.5f);
        damageCooldown = false;
    }

    private IEnumerator DamageIndicate()
    {
        float delay = health * indicateScale;
        for (int i = 0 ; i < 5; i++)
        {
            if (spriteRenderer.color.a == 1f)
            {
                spriteRenderer.color = transparentColor;
                gunRenderer.color = transparentColor;
                armRenderer.color = transparentColor;
            }
            else
            {
                spriteRenderer.color = Color.white;
                gunRenderer.color = Color.white;
                armRenderer.color = Color.white;
            }
            yield return new WaitForSeconds(delay > 1f ? 1f : delay);
        }
        spriteRenderer.color = Color.white;
        gunRenderer.color = Color.white;
        armRenderer.color = Color.white;
        damageIndicate = null;
    }

    /*
        -------------
         SHIELD PART
        -------------
    */

    private void TakeShield(GameObject colideObj)
    {
        if (keepShield < 1)
        {
            GameObject viewObj = Instantiate(buffViewerPrefab, Vector3.zero, Quaternion.identity, buffViewerContent);
            viewObj.GetComponent<BuffViewer>().StartTimer(5f, LootType.SHIELD, this);
            viewObj.name = "buffViewerPrefab";
            StartCoroutine(KeepShild());
            Destroy(colideObj);
        }
    }

    private IEnumerator KeepShild()
    {
        if (keepShield < 1)
        {
            keepShield = 3;
            shieldObject.SetActive(true);
        }
        yield break;
    }

    /*
        ----------------
         ASSISTANT PART
        ----------------
    */

    private void TakeAssistant(GameObject colideObj)
    {
        if (!buffController.Get(LootType.ASSISTANT).isInInventory)
        {
            buffController.Set(LootType.ASSISTANT, true);
            Destroy(colideObj);
        }
    }

    public void PressAssistant()
    {
        if (!assistant.gameObject.activeInHierarchy) assistant.gameObject.SetActive(true);
    }

    /*
        --------------------
         BULLETS COUNT PART
        --------------------
    */

    private void TakeBulletsCount(GameObject colideObj)
    {
        if (count + countPlus < anglesCount)
        {
            IEnumerator bulletCount = BulletsCount();
            bulletCountIE.Enqueue(bulletCount);
            StartCoroutine(bulletCount);
            Destroy(colideObj);
        }
    }

    private IEnumerator BulletsCount()
    {
        GameObject viewObj = Instantiate(buffViewerPrefab, Vector3.zero, Quaternion.identity, buffViewerContent);
        viewObj.GetComponent<BuffViewer>().StartTimer(5f, LootType.BULLETS_COUNT, null);
        viewObj.name = "buffViewerPrefab";
        countPlus += 1;
        yield return new WaitForSeconds(5f);
        countPlus -= 1;
        bulletCountIE.Dequeue();
    }

    /*
        -------------------
         BULLETS SIZE PART
        -------------------
    */

    private void TakeBulletsSize(GameObject colideObj)
    {
        if (bulletScale + bulletScalePlus < bulletScaleMax)
        {
            IEnumerator bulletSize = BulletsSize();
            bulletSizeIE.Enqueue(bulletSize);
            StartCoroutine(bulletSize);
            Destroy(colideObj);
        }
    }

    private IEnumerator BulletsSize()
    {
        GameObject viewObj = Instantiate(buffViewerPrefab, Vector3.zero, Quaternion.identity, buffViewerContent);
        viewObj.GetComponent<BuffViewer>().StartTimer(5f, LootType.BULLETS_SIZE, null);
        viewObj.name = "buffViewerPrefab";
        bulletScalePlus += 0.25f;
        yield return new WaitForSeconds(5f);
        bulletScalePlus -= 0.25f;
        bulletSizeIE.Dequeue();
    }

    /*
        --------------------
         BULLETS SPEED PART
        --------------------
    */

    private void TakeBulletsSpeed(GameObject colideObj)
    {
        if (bulletSpeed + bulletSpeedPlus < bulletSpeedMax)
        {
            IEnumerator bulletSpeed = BulletsSpeed();
            bulletSpeedIE.Enqueue(bulletSpeed);
            StartCoroutine(bulletSpeed);
            Destroy(colideObj);
        }
    }

    private IEnumerator BulletsSpeed()
    {
        GameObject viewObj = Instantiate(buffViewerPrefab, Vector3.zero, Quaternion.identity, buffViewerContent);
        viewObj.GetComponent<BuffViewer>().StartTimer(5f, LootType.BULLETS_SPEED, null);
        viewObj.name = "buffViewerPrefab";
        bulletSpeedPlus += 2.5f;
        yield return new WaitForSeconds(5f);
        bulletSpeedPlus -= 2.5f;
        bulletSpeedIE.Dequeue();
    }

    /*
        ------------------
         SPEED SHOOT PART
        ------------------
    */

    private void TakeSpeedShoot(GameObject colideObj)
    {
        if (fireDelay - fireDelayMinus > fireDelayMin)
        {
            IEnumerator speedShoot = SpeedShoot();
            speedShootIE.Enqueue(speedShoot);
            StartCoroutine(speedShoot);
            Destroy(colideObj);
        }
    }

    private IEnumerator SpeedShoot()
    {
        GameObject viewObj = Instantiate(buffViewerPrefab, Vector3.zero, Quaternion.identity, buffViewerContent);
        viewObj.GetComponent<BuffViewer>().StartTimer(5f, LootType.SPEED_SHOOT, null);
        viewObj.name = "buffViewerPrefab";
        fireDelayMinus += 0.05f;
        yield return new WaitForSeconds(5f);
        fireDelayMinus -= 0.05f;
        speedShootIE.Dequeue();
    }

    /*
        -----------------
         SPEED MOVE PART
        -----------------
    */

    IEnumerator moveSpeedIERef = null;
    private void TakeSpeedMove(GameObject colideObj)
    {
        if (moveSpeed + moveSpeedPlus < moveSpeedMax)
        {
            moveSpeedIERef = MoveSpeed();
            moveSpeedIE.Enqueue(moveSpeedIERef);
            StartCoroutine(moveSpeedIERef);
            Destroy(colideObj);
        }
    }

    private IEnumerator MoveSpeed()
    {
        GameObject viewObj = Instantiate(buffViewerPrefab, Vector3.zero, Quaternion.identity, buffViewerContent);
        viewObj.GetComponent<BuffViewer>().StartTimer(5f, LootType.SPEED_MOVE, null);
        viewObj.name = "buffViewerPrefab";
        moveSpeedPlus += 0.2f;
        yield return new WaitForSeconds(5f);
        moveSpeedPlus -= 0.2f;
        moveSpeedIE.Dequeue();
    }

    /*
        ----------------
         EXPLOSION PART
        ----------------
    */

    private void TakeExplosion(GameObject colideObj)
    {
        if (!buffController.Get(LootType.EXPLOSION).isInInventory)
        {
            buffController.Set(LootType.EXPLOSION, true);
            Destroy(colideObj);
        }
    }

    public IEnumerator PressExplosion()
    {
        ObstacleSpawner.thisScript.StartExplosion();
        yield return new WaitForSeconds(10f);
        StartCoroutine(ObstacleSpawner.thisScript.StopExplosion());
    }

    /*
        -------------
         FREEZE PART
        -------------
    */

    private void TakeFreeze(GameObject colideObj)
    {
        if (!buffController.Get(LootType.FREEZE).isInInventory)
        {
            buffController.Set(LootType.FREEZE, true);
            Destroy(colideObj);
        }
    }

    public void PressFreeze()
    {
        if (freezeDelayIE == null)
        {
            freezeDelayIE = Freeze();
            StartCoroutine(freezeDelayIE);
        }
    }

    public IEnumerator Freeze()
    {
        GameObject viewObj = Instantiate(buffViewerPrefab, Vector3.zero, Quaternion.identity, buffViewerContent);
        viewObj.GetComponent<BuffViewer>().StartTimer(8f, LootType.FREEZE, null);
        viewObj.name = "buffViewerPrefab";
        ObstacleSpawner.thisScript.FreezeObstacles();
        yield return new WaitForSeconds(8f);
        ObstacleSpawner.thisScript.UnFreezeObstacles();
        freezeDelayIE = null;
    }

    /*
        ----------------
         COLLISION PART
        ----------------
    */

    private void TakeCoin(GameObject colideObj)
    {
        Loot collideLoot = colideObj.GetComponent<Loot>();

        if (collideLoot.state == Loot.LootState.FREE)
        {
            collideLoot.rb.gravityScale = 0;
            collideLoot.dist = GameController.coinPar.position;
            collideLoot.state = Loot.LootState.ACCOUNTED;
            StartCoroutine(gameController.CoinSnatch(collideLoot));
        }
        return;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Loot")
        {
            sound.PlayBuffSimple();
            LootType type = other.gameObject.GetComponent<Loot>().type;
            actionByType[(int)type](other.gameObject);
        }
        if (other.gameObject.tag == "Obstacle")
        {
            sound.PlayCharCollision();
            if (!godMode) PushDamage();
            else CameraShake.Shake(1.2f, 0.2f);
        }
    }

    private void Flip()
    {
        is_FacingRight = !is_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }



    public float MoveSpeedval
    {
        get
        {
            return moveSpeed;
        }
        set
        {
            moveSpeed = value;
        }
    }

    public float FireDelayVal
    {
        get
        {
            return fireDelay;
        }
        set
        {
            fireDelay = value;
        }
    }

    public float BulletSpeed
    {
        get
        {
            return bulletSpeed;
        }
        set
        {
            bulletSpeed = value;
        }
    }

    public float BulletScale
    {
        get
        {
            return bulletScale;
        }
        set
        {
            bulletScale = value;
        }
    }

    public uint BulletType
    {
        get
        {
            return bulletType;
        }
        set
        {
            bulletType = value;
        }
    }

        public int BulletCount
    {
        get
        {
            return count;
        }
        set
        {
            count = value;
        }
    }
}

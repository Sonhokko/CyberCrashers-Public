using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] obstaclesPrefab;
    [SerializeField] public float spawnDelay;
    [SerializeField] public Player player;
    [SerializeField] private Sprite freezeBalls;
    [SerializeField] private Animator freezePhone = null;

    [SerializeField] public GameObject blackHole;

    private bool canSpawn = true;
    public bool freeze = false;
    public bool reverse = false;
    public bool explosion = false;
    private bool isFisrt = true;
    public bool rain = false;
    public int metSteps = 0;
    public static ObstacleSpawner thisScript { get; private set; } = null;

    private void Awake() => thisScript = this;

    private void Start()
    {
        blackHole.transform.GetChild(0).localScale = new Vector3(-1.1f, 1.3f, transform.localScale.z);
        blackHole.transform.GetChild(1).localScale = new Vector3(0.14f, 0.06f, transform.localScale.z);
    }

    private void OnEnable() => StartCoroutine(SpawnObstacles());

    private void Update()
    {
        if (explosion)
        {
            foreach (Transform i in transform)
            {
                if (i.gameObject.activeInHierarchy)
                {
                    float b = player.transform.position.y + player.boxCollider.size.y * .5f * player.transform.localScale.y;
                    float rad = i.GetComponent<CircleCollider2D>().radius;
                    if ((i.position.y + rad) <= ScreenParams.height
                    && (i.position.y - rad) >= b)
                    {
                        Rigidbody2D i_rb = i.GetComponent<Rigidbody2D>();
                        if (i_rb.gravityScale != 0)
                        {
                            i.GetComponent<Obstacle>().startedSucked = true;
                            i_rb.gravityScale = 0;
                            i_rb.velocity = new Vector2(0f, 0f);
                        }
                        i.position = Vector3.MoveTowards(i.position, blackHole.transform.GetChild(0).position, 3f * Time.deltaTime);
                        i.localScale = new Vector3(i.localScale.x/1.03f, i.localScale.y/1.03f, i.localScale.z);
                        if (Vector3.Distance(i.position, blackHole.transform.GetChild(0).position) < 0.05f) i.gameObject.GetComponent<Obstacle>().DestroyMeteor();
                    }
                }
            }
        }
        else
        {
            int check = 0;
            int checkActive = 0;
            metSteps = 0;
            foreach (Transform i in transform)
            {
                checkActive = 0;
                if (i.GetComponent<Obstacle>().startedSucked && i.gameObject.activeInHierarchy)
                {
                    i.position = Vector3.MoveTowards(i.position, blackHole.transform.GetChild(0).position, 23f * Time.deltaTime);
                    i.localScale = new Vector3(i.localScale.x/1.23f, i.localScale.y/1.23f, i.localScale.z);
                    if (Vector3.Distance(i.position, blackHole.transform.GetChild(0).position) < 0.05f) i.gameObject.GetComponent<Obstacle>().DestroyMeteor();
                }
                if (i.GetComponent<Obstacle>().positions > metSteps) metSteps = i.GetComponent<Obstacle>().positions;
                check++;
                if (!i.gameObject.activeInHierarchy)
                {

                    foreach (Transform met in transform)
                    {
                        if (met.GetComponent<Obstacle>().obsIdUpper == i.GetComponent<Obstacle>().obsId) checkActive++;
                    }
                    if (checkActive == 0) Destroy(i.gameObject);
                }
            }
            if (reverse)
            {
                StopAllCoroutines();
                if (check == 0)
                {
                    reverse = false;
                    Time.timeScale = 1;
                    StartCoroutine(StopRewind());
                    StartCoroutine(SpawnObstacles());
                }
            }
        }
    }

    public IEnumerator StopRewind()
    {
        player.backGround.transform.GetChild(0).gameObject.SetActive(false);
        player.backGround.transform.GetChild(1).gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        player.backGround.transform.GetChild(1).gameObject.SetActive(false);
        ScreenParams.Effect.enabled = false;
        player.ListAnig();
        player.gameController.PauseGame(false);
    }

    public IEnumerator SpawnObstacles(int obsNum = 12, int posCount = 0, List<Vector3> poses = null)
    {
        if (obsNum != 12)
        {
            GameObject nObject = Instantiate(obstaclesPrefab[obsNum], transform);
            nObject.GetComponent<Obstacle>().positions = posCount;
            nObject.GetComponent<Obstacle>().positionList = poses;
        }
        else
        {
            yield return new WaitWhile(() => ScreenParams.Effect.enabled);
            while (true)
            {
                if (canSpawn)
                {
                    if (!rain)
                    {
                        if (!IsRandomChoose())
                        {
                            GameObject nObject = Instantiate(obstaclesPrefab[Random.Range(0, obstaclesPrefab.Length)], transform);
                            Rigidbody2D nObject_rb = nObject.GetComponent<Rigidbody2D>();
                            if (freeze && !isFisrt) StartCoroutine(CreateFreezed(nObject.transform, nObject_rb));
                            isFisrt = false;
                        }
                    }
                    else
                        SpawnRainObstacle(transform.position);
                }
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }

    private bool IsRandomChoose()
    {
        int rand = Random.Range(0, 101);

        Debug.Log("Random Chance Is " + rand);
        Debug.Log("And Linear Obstacle Chance Value Is " + Difficult.thisScript.linearObstacleValue);
        if (rand <= (int)Difficult.thisScript.linearObstacleValue)
        {
            Debug.Log("RANDOM SECCUESSSSSSSSSS!!!!!!!!!!");
            int variation = Random.Range(0, 2);

            if (variation == 0)
            {
                int count = Random.Range(2, 4);

                for (int i = 0; i < count; i++)
                {
                    GameObject nObject = Instantiate(obstaclesPrefab[2], transform);
                    Rigidbody2D nObject_rb = nObject.GetComponent<Rigidbody2D>();
                    if (freeze && !isFisrt) StartCoroutine(CreateFreezed(nObject.transform, nObject_rb));
                    isFisrt = false;
                }
            }
            else if (variation == 1)
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject nObject = Instantiate(obstaclesPrefab[1], transform);
                    Rigidbody2D nObject_rb = nObject.GetComponent<Rigidbody2D>();
                    if (freeze && !isFisrt) StartCoroutine(CreateFreezed(nObject.transform, nObject_rb));
                    isFisrt = false;
                }
            }
        }
        return false;
    }

    public void SpawnRainObstacle(Vector2 position)
    {
        if (canSpawn)
        {
            GameObject nObject = Instantiate(obstaclesPrefab[2], position, Quaternion.identity, transform);
            Rigidbody2D nObject_rb = nObject.GetComponent<Rigidbody2D>();
            nObject_rb.velocity = new Vector2(Random.Range(-3f, 3f), Random.Range(4f, 5f));
            if (freeze) StartCoroutine(CreateFreezed(nObject.transform, nObject_rb));
        }
    }

    public void ResetBuffs()
    {
        freezePhone.gameObject.SetActive(false);
        freeze = false;
        blackHole.SetActive(false);
        explosion = false;
    }

    public void SpawnMiddleObstacle(int idi, Vector2 position, int posCountBig)
    {
        if (canSpawn)
        {
            GameObject nObject;
            Rigidbody2D nObject_rb;
            float direction = -2f;

            for (int i = 0; i < 2; i++)
            {
                nObject = Instantiate(obstaclesPrefab[1], position, Quaternion.identity, transform);
                Obstacle obs = nObject.GetComponent<Obstacle>();
                obs.obsIdUpper = idi;
                obs.positionsBig = posCountBig;
                nObject_rb = nObject.GetComponent<Rigidbody2D>();
                nObject_rb.velocity = new Vector2(direction, Random.Range(4f, 5f));
                direction = 2f;
                if (freeze) StartCoroutine(CreateFreezed(nObject.transform, nObject_rb));
            }
        }
    }

    public void SpawnSmallObstacle(int idi, Vector2 position, int posCountMiddle, int posCountBig)
    {
        if (canSpawn)
        {
            GameObject nObject;
            Rigidbody2D nObject_rb;
            float direction = -2f;

            for (int i = 0; i < 2; i++)
            {
                nObject = Instantiate(obstaclesPrefab[2], position, Quaternion.identity, transform);
                Obstacle obs = nObject.GetComponent<Obstacle>();
                obs.obsIdUpper = idi;
                obs.positionsMiddle = posCountMiddle;
                obs.positionsBig = posCountBig;
                nObject_rb = nObject.GetComponent<Rigidbody2D>();
                nObject_rb.velocity = new Vector2(direction, Random.Range(4f, 5f));
                direction = 2f;
                if (freeze) StartCoroutine(CreateFreezed(nObject.transform, nObject_rb));
            }
        }
    }

    private IEnumerator CreateFreezed(Transform meteor, Rigidbody2D meteor_rb, float b = -100f)
    {
        if (b == -100f)  yield return new WaitForSeconds(0.07f);
        yield return new WaitUntil(() => (meteor == null) || (meteor.transform.position.y + meteor.GetComponent<CircleCollider2D>().radius) <= ScreenParams.height);
        if (meteor == null) yield break;
        if (meteor != null)
        {
            yield return new WaitUntil(() => (meteor.transform.position.y - meteor.GetComponent<CircleCollider2D>().radius) >= b);
        }
        if (freeze && (meteor != null))
        {
            meteor_rb.constraints = RigidbodyConstraints2D.FreezeAll;
            meteor_rb.GetComponent<Animator>().enabled = false;
            meteor_rb.GetComponent<SpriteRenderer>().sprite = freezeBalls;
        }
    }

    public void FreezeObstacles(bool stop = true)
    {
        canSpawn = stop;
        freeze = true;
        if (canSpawn) freezePhone.gameObject.SetActive(true);
        int tmp_count = 0;
        int tmp_startCount = transform.childCount;
        Rigidbody2D[] obstacles = GetComponentsInChildren<Rigidbody2D>();
        foreach (var i in obstacles)
        {
            float a = i.transform.position.y - i.GetComponent<CircleCollider2D>().radius;
            float b = -100;
            if (player.boxCollider != null) b = player.transform.position.y + player.boxCollider.size.y * .5f * player.transform.localScale.y;
            if (a >= b && (a + (2f * i.GetComponent<CircleCollider2D>().radius)) <= ScreenParams.height)
            {
                i.constraints = RigidbodyConstraints2D.FreezeAll;
                if (canSpawn) {
                    i.GetComponent<Animator>().enabled = false;
                    i.GetComponent<SpriteRenderer>().sprite = freezeBalls;
                }
                tmp_count++;
            }
            else if (canSpawn) StartCoroutine(CreateFreezed(i.gameObject.transform, i, b));
            //  nObject = Instantiate(obstaclesPrefab[2], position, Quaternion.identity, i.GetComponent<Transform>());
        }
    }

    public void UnFreezeObstacles()
    {
        canSpawn = true;
        freeze = false;
        freezePhone.gameObject.SetActive(false);
        Rigidbody2D[] obstacles = GetComponentsInChildren<Rigidbody2D>();
        foreach (var i in obstacles)
        {
            i.constraints = RigidbodyConstraints2D.None;
            i.GetComponent<Animator>().enabled = true;
            i.AddForce(Vector2.left * 40f * (i.GetComponent<Transform>().position.x > 0 ? 1f : -1f));
            i.AddForce(Vector2.up * 60);
        }
    }

    public void Explosion(int ind = -12)
    {
        int tmp = 0;
        if (ind != -12)
        {
            foreach (Transform i in transform)
            {
                if (i.GetComponent<Obstacle>().obsIdUpper == ind) tmp++;
            }
        }
        foreach (Transform i in transform)
        {
            if (ind != -12 && tmp <= 1)
            {
                if (i.GetComponent<Obstacle>().obsId == ind && !i.gameObject.activeInHierarchy)
                {
                    Explosion(i.GetComponent<Obstacle>().obsIdUpper);
                    Destroy(i.gameObject);
                }
            }
            else if (ind == -12) Destroy(i.gameObject); 
        }
    }

    public void StartExplosion()
    {
        CameraShake.Shake(16, 0.08f);
        explosion = true;
        blackHole.SetActive(true);
    }

    public IEnumerator StopExplosion()
    {
        explosion = false;
        yield return new WaitForSeconds(1f);
        blackHole.transform.GetChild(1).gameObject.SetActive(true);
        blackHole.GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(1f);
        blackHole.SetActive(false);
        blackHole.GetComponent<Animator>().enabled = false;
        BonusAppearance.bonusAppearance.buffs.Remove((int)LootParams.LootType.EXPLOSION);
        // foreach (Transform i in transform)
        // {
        //     Destroy(i.gameObject);
        // }
            // transform.GetChild(i).gameObject.
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Assistant : MonoBehaviour
{
    [SerializeField] private Player player = null;
    [SerializeField] private GameObject bulletPrefab = null;
    [SerializeField] private GameObject bulletsParent = null;

    [SerializeField] private float speed = 15f;
    [SerializeField] private float fireDelay = 1f;
    [SerializeField] private float bulletScale = 1f;
    [SerializeField] private float bulletSpeed = 10f;

    private SpriteRenderer spriteRenderer = null;

    public bool isLeftSide = false;

    private Vector3 bulletOffset = Vector3.up * 1f;
    private Vector3 startPos;
    private float targetSide = 0.4f;
    private float animPart = 1;
    private float start_y;

    private IEnumerator fireDelayIE = null;
    private BoxCollider2D boxCol = null;
       
    private Vector3[] targetSides =
    {
        new Vector3(-0.8f, -0.33f, 0),
        new Vector3(0.8f, -0.33f, 0)
    };

    private void Start()
    {
        boxCol = GetComponent<BoxCollider2D>();
        transform.localScale = new Vector3(0.035f, 0.035f, 1);
        transform.position = new Vector3(0, ScreenParams.floor + (boxCol.size.y * transform.localScale.y / 2), 1);
        start_y = transform.position.y;
    }

    private void Awake() => spriteRenderer = GetComponentInChildren<SpriteRenderer>();

    private void Update()
    {
        if (transform.position.y - start_y >= 0.1f) animPart = -1;
        else if (transform.position.y - start_y <= 0f) animPart = 1;
        if (isLeftSide && Perms(-0.4f)) targetSide = -0.4f;
        else if (Perms(0.4f)) targetSide = 0.4f;
        Vector3 dist = new Vector3(player.transform.position.x + targetSide, transform.position.y, transform.position.z);
        dist.y += 0.0007f * animPart;
        transform.position = Vector3.MoveTowards(transform.position, dist, speed);
        if (gameObject.activeInHierarchy) Fire();
        spriteRenderer.flipX = isLeftSide;
    }

    private bool Perms(float side)
    {
        if (side < 0) return player.transform.position.x + side -  (boxCol.size.x * transform.localScale.x / 2) >= ScreenParams.width_left;
        return player.transform.position.x + side + (boxCol.size.x * transform.localScale.x / 2) <= ScreenParams.width_right;
    }

    public void Reset() => fireDelayIE = null;

    private void BulletSpawner()
    {
        GameObject newBullet = Instantiate(
            bulletPrefab,
            transform.position + bulletOffset,
            bulletPrefab.transform.rotation,
            bulletsParent.transform
        );

        newBullet.transform.localScale *= bulletScale;
        newBullet.GetComponent<Bullet>().SetUpBullet(bulletSpeed);
    }

    private void Fire()
    {
        if (fireDelayIE == null)
        {
            fireDelayIE = FireDelay();
            StartCoroutine(fireDelayIE);
        }
    }

    private IEnumerator FireDelay()
    {
        BulletSpawner();
        yield return new WaitForSeconds(fireDelay);
        fireDelayIE = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Obstacle")
        {
            if (fireDelayIE != null)
                StopCoroutine(fireDelayIE);

            fireDelayIE = null;
            gameObject.SetActive(false);
            BonusAppearance.bonusAppearance.buffs.Remove((int)LootParams.LootType.ASSISTANT);
        }
    }
}

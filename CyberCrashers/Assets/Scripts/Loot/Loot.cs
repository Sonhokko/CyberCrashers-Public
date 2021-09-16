using System.Collections;
using UnityEngine;

public class Loot : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollid;
    private CircleCollider2D circCollid;

    private float impulseStrength = 1.5f;
    private float impulseStrengthY = 2.5f;
    private bool groundColl = true;

    public Rigidbody2D rb;
    public Vector3 dist { get; private set; }
    public LootParams.LootType type { get; private set; }

    public enum LootState
    {
        FREE,
        ACCOUNTED
    }

    public LootState state = LootState.FREE;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circCollid = GetComponent<CircleCollider2D>();
    }

    private void Start()
    {
        var chance = UnityEngine.Random.Range(0, 101);

        if (chance >= 50)
        {
            impulseStrengthY *= -1;
            impulseStrength *= -1;
        }
        UpForce(impulseStrengthY, impulseStrength);
        StartCoroutine(CoinDestroyer());
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground" && groundColl)
        {
            groundColl = false;
            StartCoroutine(StopMoving());
        }
    }

    private IEnumerator CoinDestroyer()
    {
        yield return new WaitForSeconds(12f);
        Destroy(gameObject);
    }

    public void ForDel()
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        if ((transform.position.x + (circCollid.radius * transform.localScale.x) >= ScreenParams.width_right)
            || (transform.position.x - (circCollid.radius * transform.localScale.x) <= ScreenParams.width_left))
        {
            float y = transform.position.y;
            if ((y - (circCollid.radius * transform.localScale.y)) < ScreenParams.floor) y = ScreenParams.floor + (circCollid.radius * transform.localScale.y);
            if ((transform.position.x + (circCollid.radius * transform.localScale.x) >= ScreenParams.width_right))
                transform.position = new Vector3(ScreenParams.width_right - (circCollid.radius * transform.localScale.x), y, transform.position.z);
            else if ((transform.position.x - (circCollid.radius * transform.localScale.x) <= ScreenParams.width_left))
                transform.position = new Vector3(ScreenParams.width_left + (circCollid.radius * transform.localScale.x), y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x * -1, rb.velocity.y);
        }
        if (Time.timeScale != 0)
        {
            if (state == LootState.ACCOUNTED)
            {
                StopCoroutine(Destroying());
                if (Vector3.Distance(transform.position, dist) < 0.15f) ForDel();
            }
        }
    }

    private IEnumerator StopMoving()
    {
        yield return new WaitUntil(() => rb.velocity.x == 0f);

        if (state == LootState.FREE) StartCoroutine(Destroying());
        yield return new WaitForSeconds(4f);
        if (state == LootState.FREE)
        {
            if (type != LootParams.LootType.COIN) BonusAppearance.bonusAppearance.buffs.Remove((int)type);
            Destroy(gameObject);
        }
    }

    private void UpForce(float impulseStrY, float impulseStr)
    {
        rb.gravityScale = 0;
        rb.AddForce(new Vector3((impulseStr + impulseStrY) / impulseStr, impulseStrY, 0f) * impulseStr, ForceMode2D.Impulse);
        rb.gravityScale = 2.5f;
    }

    private IEnumerator Destroying()
    {
        yield return new WaitForSeconds(1.7f);
        Color lootColor = spriteRenderer.color;
        for (int x = 0; x <= 20; x++)
        {
            lootColor.a = 1f;
            if (x % 2 == 0) lootColor.a = 0f;
            spriteRenderer.color = lootColor;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void LootType(LootParams.LootType type, ref LootParams lootParams)
    {
        this.type = type;
        spriteRenderer.sprite = lootParams.lootSprites[(int)type];
        circCollid.radius = spriteRenderer.sprite.bounds.size.y / 2;
    }
}

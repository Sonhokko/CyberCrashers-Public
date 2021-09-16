using System;
using System.Collections.Generic;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected float maxHeight;
    protected CircleCollider2D cirCollider;

    private float randX;
    private int correctAngle = 90;
    private Vector2 whereToSpawn;
    private bool canBeDestroyed = true;
    private bool isForced = false;
    public bool startedSucked = false;
    public int positions = 0;
    public int positionsMiddle = 0;
    public int positionsBig = 0;
    public int obsId = -1;
    public int obsIdUpper = -1;
    private float speed = 0;
    private float ceiling;


    [SerializeField] protected SpriteRenderer sp;
    public List<Vector3> positionList = new List<Vector3>();
    protected int lucky;

    private void Start()
    {
        if (obsId == -1) IndexGen();
        if (obsIdUpper == -1) IndexGenUpper();
        ceiling = (ScreenParams.height*2 + (ScreenParams.height*-1 - ScreenParams.floor));
        transform.localScale = new Vector3(transform.localScale.x / 6.111111111111111f, transform.localScale.y / 6.111111111111111f, transform.localScale.z);
        lucky = (int)BuyScript.charFeature[BuyScript.buyScript.currentPlayer][2];
        cirCollider = GetComponent<CircleCollider2D>();
        rb.AddForce(Vector2.right * Random.Range(50, 150) * (Random.Range(0, 2) == 0 ? -1 : 1));
        rb.gravityScale += Difficult.thisScript.linearPlus;
    }

    private void FixedUpdate()
    {
        if (ObstacleSpawner.thisScript.reverse && gameObject.activeInHierarchy)
        {
            if (positionList.Count > 0)
            {
                int lastPos = positionList.Count - 1;
                transform.position = positionList[lastPos];
                positionList.RemoveAt(lastPos);
            }
            else UnHide(obsIdUpper);
        }
        else if (gameObject.activeInHierarchy)
        {
            positionList.Add(transform.position);
            positions = positionList.Count;
            positions += positionsBig + positionsMiddle;
        }
    }

    private void Update()
    {
        ceiling = (ScreenParams.height*2 + (ScreenParams.height*-1 - ScreenParams.floor));
        if (transform.position.y - (cirCollider.radius * transform.localScale.y) < ScreenParams.height && !canBeDestroyed) canBeDestroyed = true;
        float angle = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;
        if (ObstacleSpawner.thisScript.reverse)
        {
            correctAngle = 270;
            if ((transform.position.y - cirCollider.radius) >= ScreenParams.height) Destroy(gameObject);
        }
        transform.rotation = Quaternion.AngleAxis(angle - correctAngle, Vector3.forward);
        if (rb.velocity.x != 0) speed = rb.velocity.x;
        if (transform.position.x + (cirCollider.radius * transform.localScale.x) >= ScreenParams.width_right && !isForced)
        {
            isForced = true;
            rb.velocity = new Vector2(0, rb.velocity.y);
            StartCoroutine(WallBounce(-1));
        }
        else if (transform.position.x - (cirCollider.radius * transform.localScale.x) <= ScreenParams.width_left && !isForced)
        {
            isForced = true;
            rb.velocity = new Vector2(0, rb.velocity.y);
            StartCoroutine(WallBounce(1));
        }
        if (transform.position.y - (cirCollider.radius * transform.localScale.y) <= ScreenParams.floor)
        {
            transform.position = new Vector3(transform.position.x, ScreenParams.floor + (cirCollider.radius * transform.localScale.y), transform.position.z);
            float form = Mathf.Sqrt(2 * Physics2D.gravity.y * -1 * (ceiling - maxHeight) * rb.gravityScale);
            rb.velocity = new Vector2(rb.velocity.x, form);
        }
    }

    private IEnumerator WallBounce(int num)
    {
        rb.velocity = new Vector2(speed * -1 * rb.gravityScale, rb.velocity.y);
        yield return new WaitForSeconds(0.1f);
        isForced = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Bullet"))
        {
            if (canBeDestroyed && transform.position.y - (cirCollider.radius * transform.localScale.y) < ScreenParams.obsHeight)
            {
                Destroy(other.gameObject);
                DestroyObstacle();
                canBeDestroyed = false;
            }
        }
        else if (other.tag.Equals("Destroy"))
            Destroy(gameObject);
    }

    public void DestroyMeteor() => DestroyObstacle();

    private void IndexGen()
    {
        obsId = UnityEngine.Random.Range(0, 1000);
        foreach (Transform i in ObstacleSpawner.thisScript.transform)
        {
            if (i.GetComponent<Obstacle>().obsId == obsId && i.gameObject != gameObject) IndexGen();
        }
    }
    
    private void IndexGenUpper()
    {
        obsIdUpper = UnityEngine.Random.Range(0, 1000);
        foreach (Transform i in ObstacleSpawner.thisScript.transform)
        {
            if (i.GetComponent<Obstacle>().obsIdUpper == obsIdUpper && i.gameObject != gameObject) IndexGenUpper();
        }
    }

    virtual protected void UnHide(int ind) => Destroy(gameObject);

    virtual protected void DestroyObstacle() => Destroy(gameObject);

}

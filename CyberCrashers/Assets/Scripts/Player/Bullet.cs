using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 0f;
    public bool startedSucked = false;

    private void Start() => StartCoroutine(Killer());

    private void Update()
    {
        Debug.Log(ScreenParams.obsHeight + " ScreenParams.obsHeight");
        if (ObstacleSpawner.thisScript.explosion)
        {
            startedSucked = true;
            transform.position = Vector3.MoveTowards(transform.position, ObstacleSpawner.thisScript.blackHole.transform.GetChild(0).position, 3f * Time.deltaTime);
            transform.localScale = new Vector3(transform.localScale.x/1.01f, transform.localScale.y/1.01f, transform.localScale.z);
            if (Vector3.Distance(transform.position, ObstacleSpawner.thisScript.blackHole.transform.GetChild(0).position) < 0.05f) Destroy(gameObject);
        }
        else
        {
            if (startedSucked)
            {
                transform.position = Vector3.MoveTowards(transform.position, ObstacleSpawner.thisScript.blackHole.transform.GetChild(0).position, 10f * Time.deltaTime);
                transform.localScale = new Vector3(transform.localScale.x/1.07f, transform.localScale.y/1.07f, transform.localScale.z);
                if (Vector3.Distance(transform.position, ObstacleSpawner.thisScript.blackHole.transform.GetChild(0).position) < 0.05f) Destroy(gameObject);
            }
            else
            {
                transform.position += transform.right * speed * Time.deltaTime;

                if (transform.position.y + (GetComponent<BoxCollider2D>().size.y * 0.5f * transform.localScale.y) > ScreenParams.obsHeight) Destroy(gameObject);
                if (transform.position.x < ScreenParams.ScreenLeftSideOffseted
                    || transform.position.x > ScreenParams.ScreenRightSideOffseted)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    public void SetUpBullet(float speed) => this.speed = speed;

    private IEnumerator Killer()
    {
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
}

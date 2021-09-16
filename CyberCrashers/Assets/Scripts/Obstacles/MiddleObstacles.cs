using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiddleObstacles : Obstacle
{
    protected override void DestroyObstacle()
    {
        gameObject.SetActive(false);
        ObstacleSpawner.thisScript.SpawnSmallObstacle(obsId, transform.position, positions, positionsBig);
        Difficult.thisScript.IncreaseScore();
    }

    protected override void UnHide(int ind)
    {
        foreach (Transform i in ObstacleSpawner.thisScript.transform)
        {
            if (ind == i.GetComponent<Obstacle>().obsId && !i.gameObject.activeInHierarchy && i.name == "BigObstacleS(Clone)")
                i.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }
}

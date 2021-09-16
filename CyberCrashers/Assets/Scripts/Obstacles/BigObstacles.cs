using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigObstacles : Obstacle
{
    protected override void DestroyObstacle()
    {
        gameObject.SetActive(false);
        ObstacleSpawner.thisScript.SpawnMiddleObstacle(obsId, transform.position, positions);
        Difficult.thisScript.IncreaseScore();
    }
}

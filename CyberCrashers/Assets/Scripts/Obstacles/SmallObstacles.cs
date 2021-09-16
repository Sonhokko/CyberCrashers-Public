using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallObstacles : Obstacle
{
    protected override void DestroyObstacle()
    {
        ObstacleSpawner.thisScript.Explosion(obsIdUpper);
        Destroy(gameObject);
        // BonusAppearance.bonusAppearance.type = (LootParams.LootType)UnityEngine.Random.Range(0, LootParams.lootCount + 1);
        BonusAppearance.bonusAppearance.Appearance(transform.position, lucky, false);
        Difficult.thisScript.IncreaseScore();
    }

    protected override void UnHide(int ind)
    {
        foreach (Transform i in ObstacleSpawner.thisScript.transform)
        {
            if (ind == i.GetComponent<Obstacle>().obsId && !i.gameObject.activeInHierarchy && i.name == "MiddleObstacleS(Clone)")
                i.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }
}

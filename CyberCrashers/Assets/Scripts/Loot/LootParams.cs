using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootParams : MonoBehaviour
{
    public List<Sprite> lootSprites = null;

    public enum LootType
    {
        ASSISTANT,
        EXPLOSION,
        FREEZE,
        HEAL,
        SHIELD,
        BULLETS_COUNT,
        BULLETS_SIZE,
        BULLETS_SPEED,
        SPEED_SHOOT,
        SPEED_MOVE,
        COIN,
        NULL
    }

    public static int lootCount { get; private set; }

    public static LootParams thisScript = null;

    private void Awake()
    {
        thisScript = this;
        lootCount = lootSprites.Count - 1;
    }
}


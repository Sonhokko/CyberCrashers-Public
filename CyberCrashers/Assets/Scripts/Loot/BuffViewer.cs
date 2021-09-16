using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

using LootType = LootParams.LootType;

public class BuffViewer : MonoBehaviour
{
    private Image imageBuffs = null;
    private LootType lootType;
    private Player player = null;

    private void Awake()
    {
        imageBuffs = GetComponent<Image>();
    }

    private void Update()
    {
        if (lootType == LootType.SHIELD)
        {
            imageBuffs.fillAmount = player.keepShield / 3f;
            if (imageBuffs.fillAmount <= 0f) Destroy(gameObject);
        }
    }

    public void StartTimer(float time, LootType type, Player player)
    {
        this.player = player;
        lootType = type;

        imageBuffs.sprite = LootParams.thisScript.lootSprites[(int)type];
        if (lootType != LootType.SHIELD)
            StartCoroutine(Bufftimer(time, (int)type));
    }

    private IEnumerator Bufftimer(float time, int type)
    {
        imageBuffs.fillAmount = 1f;

        for (int i = 0; i < 10; i++)
        {
            imageBuffs.fillAmount -= 0.1f;
            yield return new WaitForSeconds(time / 10);
        }

        BonusAppearance.bonusAppearance.buffs.Remove(type);
        Destroy(gameObject);
    }
}

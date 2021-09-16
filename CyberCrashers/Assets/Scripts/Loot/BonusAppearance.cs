using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class BonusAppearance : MonoBehaviour
{
    [SerializeField] private LootParams lootParams;
    [SerializeField] private GameObject bonusPref;
    [SerializeField] private Player player;
    [SerializeField] private ObstacleSpawner obstacles;
    [SerializeField] private Buff buffPan;
    [SerializeField] private PhysicsMaterial2D forLoot;


    private const float coinSize = 0.015f;
    private const float nocoinSize = 0.025f;

    private bool isBuff = false;
    private int dopChance = 0;
    public List<int> buffs = new List<int>();
    public int friendlyChance = 15;

    public static BonusAppearance bonusAppearance { get; private set; } = null;
    public LootParams.LootType type = LootParams.LootType.NULL;

    private void Start()
    {
        bonusAppearance = this;
    }

    public void Appearance(Vector3 spawnPos, int lucky, bool cheat = false)
    {
        if (type != LootParams.LootType.NULL)
        {
            StartCoroutine(SpawnLoot(spawnPos, nocoinSize, (int)type));
            type = LootParams.LootType.NULL;
            return;
        }
        if (isBuff) return;
        if (!cheat)
        {
            int chance = UnityEngine.Random.Range(0, 101);

            if (((chance + dopChance) <= lucky) || (obstacles.transform.childCount >= 8 && chance <= friendlyChance + lucky))
            {
                int i = 0;
                if (player.health == player.healthMax) i = UnityEngine.Random.Range(4, LootParams.lootCount);
                else i = UnityEngine.Random.Range(3, LootParams.lootCount);

                if (buffs.Contains(i))
                {
                    StartCoroutine(SpawnLoot(spawnPos, coinSize, LootParams.lootCount));
                    dopChance = 100;
                    return;
                }
                if (dopChance != 0) dopChance = 0;
                if (friendlyChance != 0 && (obstacles.transform.childCount >= 8 && chance <= friendlyChance + lucky))
                {
                    friendlyChance = 0;
                    StartCoroutine(FriendlyShield());
                }

                StartCoroutine(SpawnLoot(spawnPos, nocoinSize, i));
            }
            else
            {
               StartCoroutine(SpawnLoot(spawnPos, coinSize, LootParams.lootCount));
            }
        }
        else
        {
            var chance = UnityEngine.Random.Range(0, LootParams.lootCount);
            StartCoroutine(SpawnLoot(spawnPos, chance == LootParams.lootCount ? coinSize : nocoinSize, chance, cheat));
        }
    }

    private void Update()
    {
        int num = 0;
        foreach (Transform coin in transform)
        {
            if (coin.GetComponent<SpriteRenderer>().sprite.name == "Coins") num++;
        }
    }

    private IEnumerator FriendlyShield()
    {
        yield return new WaitForSeconds(15f);
        friendlyChance = 15;
    }

    private IEnumerator SpawnLoot(Vector3 spawnPos, float scale, int type, bool cheat = false)
    {
        var loot = Instantiate(bonusPref, spawnPos, Quaternion.identity, gameObject.transform);
        loot.transform.position += Vector3.forward * 6;
        loot.transform.localScale *= scale;
        loot.GetComponent<Loot>().LootType((LootParams.LootType)type, ref lootParams);
        if (type != LootParams.lootCount)
        {
            buffs.Add(type);
            loot.GetComponent<CircleCollider2D>().sharedMaterial = forLoot;
        }
        yield break;
    }

    public void DestroyLoot(GameObject obj = null)
    {
        if (obj != null) GameObject.Destroy(obj);
        else foreach (Transform child in transform) GameObject.Destroy(child.gameObject);
    }
}

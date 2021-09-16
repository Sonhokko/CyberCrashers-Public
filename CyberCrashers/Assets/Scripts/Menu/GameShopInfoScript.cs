using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameShopInfoScript : MonoBehaviour
{
    [SerializeField] private GameObject GameShop = null;
    [SerializeField] private GameObject BackButton = null;
    [SerializeField] private GameObject BackButtonGameShop = null;

    [SerializeField] private ScrollRect ScrollChar = null;
    [SerializeField] private ScrollRect ScrollWeapon = null;

    [SerializeField] private Scrollbar ScrollbarChar = null;
    [SerializeField] private Scrollbar Scrollbarweapon = null;

    [Header("Characters")]
    [SerializeField] private GameObject BuyButtonCharOne = null;
    [SerializeField] private GameObject BuyButtonCharTwo = null;
    [SerializeField] private GameObject BuyButtonCharThree = null;
    [SerializeField] private GameObject ArrowSideChar = null;
    [SerializeField] private GameObject CharacterShop = null;

    [Header("Weapons")]
    [SerializeField] private GameObject BuyButtonWeaponOne = null;
    [SerializeField] private GameObject BuyButtonWeaponTwo = null;
    [SerializeField] private GameObject BuyButtonWeaponThree = null;
    [SerializeField] private GameObject ArrowSideWeapon = null;
    [SerializeField] private GameObject WeaponShop = null;




    public void BackButtonInfo()
    {
        BuyButtonCharOne.SetActive(true);
        ArrowSideChar.SetActive(true);
        ScrollChar.enabled = true;
        ScrollChar.horizontalNormalizedPosition = 0;
        CharacterShop.SetActive(false);
        GameShop.SetActive(true);
        BuyButtonWeaponOne.SetActive(true);
        ArrowSideWeapon.SetActive(true);
        ScrollWeapon.enabled = true;
        Scrollbarweapon.value = 0f;
        WeaponShop.SetActive(false);
        BackButton.SetActive(true);
        BackButtonGameShop.SetActive(false);

    }

    public void UnhideChar()
    {
        BuyButtonCharOne.SetActive(false);
        ArrowSideChar.SetActive(false);
        ScrollChar.horizontalNormalizedPosition = 0.2848239f;
        ScrollChar.enabled = false;
        CharacterShop.SetActive(true);
        GameShop.SetActive(false);
        BackButton.SetActive(false);
        BackButtonGameShop.SetActive(true);
    }
    public void UnhideWeapon()
    {
        BuyButtonWeaponOne.SetActive(false);
        ArrowSideWeapon.SetActive(false);
        ScrollWeapon.horizontalNormalizedPosition = 0.2201563f;
        ScrollWeapon.enabled = false;
        WeaponShop.SetActive(true);
        GameShop.SetActive(false);
        BackButton.SetActive(false);
        BackButtonGameShop.SetActive(true);
    }
    public void UnhideCharTwo()
    {
        BuyButtonCharTwo.SetActive(false);
        ArrowSideChar.SetActive(false);
        ScrollChar.horizontalNormalizedPosition = 0.5696167f;
        ScrollChar.enabled = false;
        CharacterShop.SetActive(true);
        GameShop.SetActive(false);
        BackButton.SetActive(false);
        BackButtonGameShop.SetActive(true);
    }
    public void UnhideWeaponTwo()
    {
        BuyButtonWeaponTwo.SetActive(false);
        ArrowSideWeapon.SetActive(false);
        ScrollWeapon.horizontalNormalizedPosition = 0.665455f;
        ScrollWeapon.enabled = false;
        WeaponShop.SetActive(true);
        GameShop.SetActive(false);
        BackButton.SetActive(false);
        BackButtonGameShop.SetActive(true);
    }
    public void UnhideCharThree()
    {
        BuyButtonCharThree.SetActive(false);
        ArrowSideChar.SetActive(false);
        ScrollChar.horizontalNormalizedPosition = 1.0f;
        ScrollChar.enabled = false;
        CharacterShop.SetActive(true);
        GameShop.SetActive(false);
        BackButton.SetActive(false);
        BackButtonGameShop.SetActive(true);
    }
    public void UnhideWeaponThree()
    {
        BuyButtonWeaponThree.SetActive(false);
        ArrowSideWeapon.SetActive(false);
        ScrollWeapon.horizontalNormalizedPosition = 1.0f;
        ScrollWeapon.enabled = false;
        WeaponShop.SetActive(true);
        GameShop.SetActive(false);
        BackButton.SetActive(false);
        BackButtonGameShop.SetActive(true);
    }
    

}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Localization : MonoBehaviour
{
    public static LanguageItem item = null;

    private static Localization rootObject = null;

    private static string currentLanguage = "en";

    [SerializeField] private List<TextMeshProUGUI> objectsToTranslate = null;

    private void Awake()
    {
        rootObject = this;
        LoadFile();
    }

    public static Localization Get()
    {
        return rootObject;
    }

    public void ChangeLanguage(string newLanguage)
    {
        currentLanguage = newLanguage;
        LoadFile();
    }

    public void LoadFile()
    {
        TextAsset jsonTextFile = Resources.Load<TextAsset>("TextAsset/" + currentLanguage);
        string tileFile = jsonTextFile.text;

        item = JsonUtility.FromJson<LanguageItem>(tileFile);
        item.dictionary = new Dictionary<string, string>(item.allWordsList.Count / 2);

        for (int i = 0; i < item.allWordsList.Count; i = i + 2)
            item.dictionary.Add(item.allWordsList[i], item.allWordsList[i + 1]);

        ChangeText();
    }

    public void ChangeText()
    {
        TextMeshProUGUI[] textComponents = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

        for (int a = 0; a < textComponents.Length; a++)
        {
            for (int i = 0; i < item.allWordsList.Count; i = i + 2)
            {
                if (textComponents[a].name == item.allWordsList[i]
                    || textComponents[a].transform.parent.name == item.allWordsList[i])
                {
                    textComponents[a].text = item.dictionary[item.allWordsList[i]];
                    break;
                }
            }
        }

        item.allWordsList.Clear();
    }
}

public class LanguageItem
{
    public List<string> allWordsList;
    public Dictionary<string, string> dictionary;
}

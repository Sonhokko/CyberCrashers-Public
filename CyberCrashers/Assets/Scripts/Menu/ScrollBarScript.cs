using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarScript : MonoBehaviour
{
    [SerializeField] private GameObject arrowLeft = null;
    [SerializeField] private GameObject arrowRight = null;
    [SerializeField] private Scrollbar scrollBar = null;

    private void Update()
    {
        if (arrowLeft != null)
        {
            if (scrollBar.value <= 0.05f) arrowLeft.SetActive(false);
            else arrowLeft.SetActive(true);
        }
        if (arrowRight != null)
        {
            if (scrollBar.value >= 0.95f) arrowRight.SetActive(false);
            else arrowRight.SetActive(true);
        }
    }

}

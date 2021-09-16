using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fpsCounterText;

    void Update()
    {
        int fps = (int)(1f / Time.unscaledDeltaTime);
        fpsCounterText.text = "FPS: " + fps;

        if (fps < 30) fpsCounterText.color = Color.red;
        else if (fps < 60) fpsCounterText.color = Color.yellow;
        else if (fps < 90) fpsCounterText.color = Color.blue;
        else fpsCounterText.color = Color.green;
    }
}

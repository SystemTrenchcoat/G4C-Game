using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightForcedAdjust : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas canvas;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        Invoke(nameof(AdjustHeight), 0.05f);
    }

    void AdjustHeight()
    {
        if (canvas != null)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                Vector2 referenceResolution = scaler.referenceResolution;

                float screenHeight = Screen.height;
                float referenceHeight = referenceResolution.y;

                float scaleFactor = screenHeight / referenceHeight;

                //change the float number to adjust for green area height scale
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, screenHeight * 0.15f / scaleFactor);
            }
        }
    }
}

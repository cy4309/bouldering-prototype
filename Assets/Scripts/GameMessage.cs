using UnityEngine;
using UnityEngine.UI;

public class GameMessage : MonoBehaviour
{
    static GameMessage instance;
    Text label;
    CanvasScaler scaler;
    float hideAt;

    public static void Show(string message)
    {
        if (instance == null)
        {
            GameObject host = new GameObject("GameMessage");
            instance = host.AddComponent<GameMessage>();
        }

        instance.label.text = message;
        instance.hideAt = Time.time + 2f;
    }

    void Awake()
    {
        instance = this;

        GameObject canvasObject = new GameObject("Canvas");
        canvasObject.transform.SetParent(transform, false);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        GameObject textObject = new GameObject("Message");
        textObject.transform.SetParent(canvasObject.transform, false);
        label = textObject.AddComponent<Text>();
        label.font = Resources.Load<Font>("GameMessage");
        label.fontSize = 28;
        label.alignment = TextAnchor.MiddleCenter;
        label.color = Color.white;
        label.horizontalOverflow = HorizontalWrapMode.Wrap;
        label.verticalOverflow = VerticalWrapMode.Overflow;

        RectTransform rect = label.rectTransform;
        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0f);
        rect.anchoredPosition = new Vector2(0f, 48f);
        rect.sizeDelta = new Vector2(1200f, 80f);

        Outline outline = textObject.AddComponent<Outline>();
        outline.effectColor = new Color(0f, 0f, 0f, 0.9f);
        outline.effectDistance = new Vector2(2f, -2f);
        FitText();
    }

    void Update()
    {
        FitText();
        if (label.text != "" && Time.time >= hideAt)
        {
            label.text = "";
        }
    }

    void FitText()
    {
        bool portrait = Screen.height > Screen.width;
        scaler.matchWidthOrHeight = portrait ? 1f : 0.5f;
        label.fontSize = portrait ? 36 : 28;
        label.rectTransform.sizeDelta = new Vector2(1200f, 80f);
    }
}

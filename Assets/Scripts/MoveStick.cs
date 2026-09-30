using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MoveStick : MonoBehaviour
{
    public static Vector2 Axis { get; private set; }
    public static bool PushUp => Axis.y > 0.45f;
    public static bool PushDown => Axis.y < -0.45f;

    const float Radius = 140f;

    bool tracking;
    Vector2 origin;
    RectTransform ring;
    RectTransform knob;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Create()
    {
        if (FindAnyObjectByType<MoveStick>() != null)
        {
            return;
        }

        new GameObject("MoveStick").AddComponent<MoveStick>();
    }

    void Awake()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 1f;

        ring = CreateCircle("Ring", 280f, false, new Color(1f, 1f, 1f, 0.28f));
        knob = CreateCircle("Knob", 110f, true, new Color(1f, 1f, 1f, 0.75f));
        SetVisible(false);
    }

    void Update()
    {
        if (!TryReadPointer(out Vector2 screen, out bool pressed))
        {
            EndDrag();
            return;
        }

        if (pressed && !tracking)
        {
            tracking = true;
            origin = screen;
            SetVisible(true);
            Place(ring, origin);
        }

        if (!pressed)
        {
            EndDrag();
            return;
        }

        Vector2 delta = screen - origin;
        if (delta.magnitude > Radius)
        {
            delta = delta.normalized * Radius;
        }

        Place(knob, origin + delta);
        Vector2 axis = delta / Radius;
        if (axis.magnitude < 0.15f)
        {
            axis = Vector2.zero;
        }

        Axis = axis;
    }

    void EndDrag()
    {
        tracking = false;
        Axis = Vector2.zero;
        SetVisible(false);
    }

    bool TryReadPointer(out Vector2 screen, out bool pressed)
    {
        Touchscreen touchscreen = Touchscreen.current;
        if (touchscreen != null && touchscreen.primaryTouch.press.isPressed)
        {
            screen = touchscreen.primaryTouch.position.ReadValue();
            pressed = true;
            return true;
        }

        Mouse mouse = Mouse.current;
        if (mouse != null)
        {
            screen = mouse.position.ReadValue();
            pressed = mouse.leftButton.isPressed;
            return true;
        }

        screen = Vector2.zero;
        pressed = false;
        return false;
    }

    RectTransform CreateCircle(string circleName, float size, bool filled, Color color)
    {
        GameObject circleObject = new GameObject(circleName);
        circleObject.transform.SetParent(transform, false);
        Image image = circleObject.AddComponent<Image>();
        image.sprite = CircleSprite(filled);
        image.color = color;
        image.raycastTarget = false;
        RectTransform rect = image.rectTransform;
        rect.sizeDelta = new Vector2(size, size);
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        return rect;
    }

    void Place(RectTransform rect, Vector2 screen)
    {
        RectTransform canvasRect = transform as RectTransform;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screen, null, out Vector2 local);
        rect.anchoredPosition = local;
    }

    void SetVisible(bool visible)
    {
        ring.gameObject.SetActive(visible);
        knob.gameObject.SetActive(visible);
    }

    static Sprite CircleSprite(bool filled)
    {
        const int size = 64;
        Texture2D texture = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float outer = size * 0.5f;
        float inner = filled ? 0f : outer * 0.72f;
        Vector2 center = new Vector2(outer, outer);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = Vector2.Distance(new Vector2(x + 0.5f, y + 0.5f), center);
                float alpha = distance <= outer && distance >= inner ? 1f : 0f;
                texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
            }
        }

        texture.Apply();
        return Sprite.Create(texture, new Rect(0f, 0f, size, size), new Vector2(0.5f, 0.5f));
    }
}

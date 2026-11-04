using UnityEngine;

public sealed class BuildInfoOverlay : MonoBehaviour
{
    public static bool Visible = true;

    private const float ReferenceHeight = 1080f;
    private const int ReferenceFontSize = 14;
    private const int MarginPx = 4;
    private const int OutlinePx = 1;
    private static readonly Color TextColor = Color.white;
    private static readonly Color OutlineColor = Color.black;

    private static readonly Vector2[] OutlineDirections =
    {
        new(-1, -1), new(0, -1), new(1, -1),
        new(-1, 0),              new(1, 0),
        new(-1, 1), new(0, 1), new(1, 1)
    };

    private GUIContent _content;
    private Font _font;
    private GUIStyle _style;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Create()
    {
        var info = Resources.Load<TextAsset>("BuildInfo");
        if (info == null) return;

        var go = new GameObject(nameof(BuildInfoOverlay));
        DontDestroyOnLoad(go);

        var overlay = go.AddComponent<BuildInfoOverlay>();
        overlay._content = new GUIContent(info.text.Trim());
        overlay._font = Font.CreateDynamicFontFromOSFont("Courier New", ReferenceFontSize);
    }

    private void OnGUI()
    {
        if (!Visible || Event.current.type != EventType.Repaint) return;

        _style ??= new GUIStyle(GUI.skin.label) { font = _font, wordWrap = false };
        _style.fontSize = Mathf.RoundToInt(ReferenceFontSize * Screen.height / ReferenceHeight);

        Vector2 size = _style.CalcSize(_content);
        var rect = new Rect(MarginPx, Screen.height - size.y - MarginPx, size.x, size.y);

        foreach (Vector2 direction in OutlineDirections)
        {
            DrawLabel(OffsetRect(rect, direction * OutlinePx), OutlineColor);
        }

        DrawLabel(rect, TextColor);
    }

    private void DrawLabel(Rect rect, Color color)
    {
        _style.normal.textColor = color;
        GUI.Label(rect, _content, _style);
    }

    private static Rect OffsetRect(Rect rect, Vector2 offset)
    {
        return new Rect(rect.position + offset, rect.size);
    }
}

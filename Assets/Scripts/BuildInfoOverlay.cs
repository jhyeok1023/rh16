using UnityEngine;

public sealed class BuildInfoOverlay : MonoBehaviour
{
    public static bool Visible = true;

    private const float ReferenceHeight = 1080f;
    private const int ReferenceFontSize = 14;
    private const int MarginPx = 4;
    private static readonly Color TextColor = new(1f, 1f, 1f, 0.6f);
    private static readonly Color ShadowColor = new(0f, 0f, 0f, 0.6f);

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
        overlay._font = Resources.Load<Font>("Art/Fonts/COUR");
    }

    private void OnGUI()
    {
        if (!Visible || Event.current.type != EventType.Repaint) return;

        _style ??= new GUIStyle(GUI.skin.label) { font = _font, wordWrap = false };
        _style.fontSize = Mathf.RoundToInt(ReferenceFontSize * Screen.height / ReferenceHeight);

        Vector2 size = _style.CalcSize(_content);
        var rect = new Rect(MarginPx, Screen.height - size.y - MarginPx, size.x, size.y);

        DrawLabel(new Rect(rect.x + 1, rect.y + 1, rect.width, rect.height), ShadowColor);
        DrawLabel(rect, TextColor);
    }

    private void DrawLabel(Rect rect, Color color)
    {
        _style.normal.textColor = color;
        GUI.Label(rect, _content, _style);
    }
}

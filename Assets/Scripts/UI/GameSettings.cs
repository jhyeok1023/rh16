using UnityEngine;

/// <summary>Player options, stored in PlayerPrefs.</summary>
public static class GameSettings
{
    public const int VolumeSteps = 10;

    private const int DefaultBgm = 8;
    private const int DefaultSe = 7;

    private const string BgmKey = "settings.bgm";
    private const string SeKey = "settings.se";
    private const string ScreenKey = "settings.screen";
    private const string ResolutionKey = "settings.resolution";
    private const string VSyncKey = "settings.vsync";

    public static readonly string[] ScreenLabels = { "전체 화면", "테두리 없는 창", "창 모드" };
    public static readonly string[] ResolutionLabels = { "1920 × 1080", "1600 × 900", "1280 × 720" };
    public static readonly string[] SwitchLabels = { "꺼짐", "켜짐" };

    private static readonly FullScreenMode[] ScreenModes =
    {
        FullScreenMode.ExclusiveFullScreen,
        FullScreenMode.FullScreenWindow,
        FullScreenMode.Windowed
    };

    private static readonly Vector2Int[] Resolutions =
    {
        new(1920, 1080),
        new(1600, 900),
        new(1280, 720)
    };

    public static int Bgm
    {
        get => PlayerPrefs.GetInt(BgmKey, DefaultBgm);
        set => PlayerPrefs.SetInt(BgmKey, Mathf.Clamp(value, 0, VolumeSteps));
    }

    public static int Se
    {
        get => PlayerPrefs.GetInt(SeKey, DefaultSe);
        set => PlayerPrefs.SetInt(SeKey, Mathf.Clamp(value, 0, VolumeSteps));
    }

    public static int ScreenIndex
    {
        get => PlayerPrefs.GetInt(ScreenKey, 0);
        set => PlayerPrefs.SetInt(ScreenKey, Wrap(value, ScreenModes.Length));
    }

    public static int ResolutionIndex
    {
        get => PlayerPrefs.GetInt(ResolutionKey, 0);
        set => PlayerPrefs.SetInt(ResolutionKey, Wrap(value, Resolutions.Length));
    }

    public static int VSync
    {
        get => PlayerPrefs.GetInt(VSyncKey, 1);
        set => PlayerPrefs.SetInt(VSyncKey, Wrap(value, SwitchLabels.Length));
    }

    public static float BgmVolume => Bgm / (float)VolumeSteps;
    public static float SeVolume => Se / (float)VolumeSteps;

    public static void ResetToDefaults()
    {
        foreach (string key in new[] { BgmKey, SeKey, ScreenKey, ResolutionKey, VSyncKey })
        {
            PlayerPrefs.DeleteKey(key);
        }

        Apply();
    }

    public static void Apply()
    {
        Vector2Int size = Resolutions[ResolutionIndex];
        Screen.SetResolution(size.x, size.y, ScreenModes[ScreenIndex]);
        QualitySettings.vSyncCount = VSync;
        PlayerPrefs.Save();
    }

    private static int Wrap(int value, int count)
    {
        return (value % count + count) % count;
    }
}

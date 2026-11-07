using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>Escape pauses the game and opens the menu over the play field.</summary>
[RequireComponent(typeof(PanelRenderer))]
public sealed class PauseMenu : MonoBehaviour
{
    [SerializeField] private string titleScene = "TitleScene";

    private PanelRenderer _renderer;
    private VisualElement _panel;
    private VisualElement _settingsLayer;
    private UiMenu _menu;
    private SettingsMenu _settings;
    private UiSpinners _spinners;
    private bool _paused;
    private bool _inSettings;
    private int _toggledFrame = -1;

    private void OnEnable()
    {
        _renderer = GetComponent<PanelRenderer>();
        _renderer.RegisterUIReloadCallback(Bind);
    }

    /// <summary>Runs once the panel is attached, and again whenever the UXML is reloaded.</summary>
    private void Bind(PanelRenderer panelRenderer, VisualElement root)
    {
        if (root.childCount == 0)
        {
            return;
        }

        _panel = root.Q("pause-panel");
        _settingsLayer = root.Q("pause-settings");
        _spinners = new UiSpinners(root);

        _menu = new UiMenu(_panel, root.Q("pause-cursor"));
        _menu.Submitted += OnSubmitted;
        _menu.Cancelled += Resume;

        _settings = new SettingsMenu(_settingsLayer);
        _settings.Closed += CloseSettings;

        _inSettings = false;
        SetVisible(_panel, _paused);
        SetVisible(_settingsLayer, false);
        if (_paused)
        {
            _menu.Activate();
        }
    }

    private void OnDisable()
    {
        _renderer.UnregisterUIReloadCallback(Bind);
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (_paused)
        {
            _spinners.Tick();
            if (_inSettings)
            {
                _settings.Tick();
            }
            else
            {
                _menu.Tick();
            }

            return;
        }

        Keyboard keyboard = Keyboard.current;
        if (_menu != null && keyboard != null && Time.frameCount != _toggledFrame && keyboard.escapeKey.wasPressedThisFrame)
        {
            Pause();
        }
    }

    private void Pause()
    {
        _paused = true;
        _toggledFrame = Time.frameCount;
        Time.timeScale = 0f;
        SetVisible(_panel, true);
        _menu.Activate();
    }

    private void Resume()
    {
        _paused = false;
        _toggledFrame = Time.frameCount;
        Time.timeScale = 1f;
        SetVisible(_panel, false);
    }

    private void OnSubmitted(string action)
    {
        switch (action)
        {
            case "resume":
                Resume();
                break;
            case "retry":
                Load(SceneManager.GetActiveScene().name);
                break;
            case "settings":
                OpenSettings();
                break;
            case "title":
                Load(titleScene);
                break;
        }
    }

    private void OpenSettings()
    {
        _inSettings = true;
        SetVisible(_panel, false);
        SetVisible(_settingsLayer, true);
        _settings.Activate();
    }

    private void CloseSettings()
    {
        _inSettings = false;
        SetVisible(_settingsLayer, false);
        SetVisible(_panel, true);
        _menu.Activate();
    }

    private static void SetVisible(VisualElement element, bool visible)
    {
        element.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private static void Load(string sceneName)
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(sceneName);
    }
}

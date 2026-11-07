using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>HUD of Play.uxml. Game logic pushes values in through the setters.</summary>
[RequireComponent(typeof(UIDocument))]
public sealed class GameHud : MonoBehaviour
{
    private const float MaxPower = 4f;
    private const float EmptyOpacity = 0.2f;

    public static GameHud Instance { get; private set; }

    private Label _hiScore;
    private Label _score;
    private Label _power;
    private Label _graze;
    private Label _bossTimer;
    private Label _spellName;
    private VisualElement _powerFill;
    private VisualElement _bossFill;
    private VisualElement _boss;
    private VisualElement _spellBanner;
    private List<VisualElement> _lives;
    private List<VisualElement> _spells;

    private void OnEnable()
    {
        Instance = this;
        VisualElement root = GetComponent<UIDocument>().rootVisualElement;

        _hiScore = root.Q<Label>("hi-score");
        _score = root.Q<Label>("score");
        _power = root.Q<Label>("power");
        _graze = root.Q<Label>("graze");
        _bossTimer = root.Q<Label>("boss-timer");
        _spellName = root.Q<Label>("spell-name");
        _powerFill = root.Q("power-fill");
        _bossFill = root.Q("boss-fill");
        _boss = root.Q("boss");
        _spellBanner = root.Q("spell-banner");
        _lives = root.Q("lives").Query(className: "flower").ToList();
        _spells = root.Q("spells").Query(className: "stock").ToList();

        SetHiScore(0);
        SetScore(0);
        SetLives(3);
        SetSpells(2);
        SetPower(0f);
        SetGraze(0);
        HideBoss();
        HideSpell();
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public void SetHiScore(long value) => _hiScore.text = FormatScore(value);

    public void SetScore(long value) => _score.text = FormatScore(value);

    public void SetGraze(int value) => _graze.text = value.ToString("N0");

    public void SetLives(int count) => SetStock(_lives, count);

    public void SetSpells(int count) => SetStock(_spells, count);

    public void SetPower(float value)
    {
        _power.text = value.ToString("0.00");
        SetFill(_powerFill, value / MaxPower);
    }

    public void ShowBoss(float health01, float secondsLeft)
    {
        _boss.style.display = DisplayStyle.Flex;
        SetFill(_bossFill, health01);
        _bossTimer.text = secondsLeft.ToString("00.00");
    }

    public void HideBoss() => _boss.style.display = DisplayStyle.None;

    public void ShowSpell(string spellName)
    {
        _spellName.text = spellName;
        _spellBanner.style.display = DisplayStyle.Flex;
    }

    public void HideSpell() => _spellBanner.style.display = DisplayStyle.None;

    private static void SetStock(List<VisualElement> icons, int count)
    {
        for (int i = 0; i < icons.Count; i++)
        {
            icons[i].style.opacity = i < count ? 1f : EmptyOpacity;
        }
    }

    private static void SetFill(VisualElement fill, float amount)
    {
        fill.style.width = Length.Percent(Mathf.Clamp01(amount) * 100f);
    }

    private static string FormatScore(long value)
    {
        return value.ToString("00,000,000");
    }
}

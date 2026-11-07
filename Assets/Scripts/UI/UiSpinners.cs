using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>Turns elements tagged <c>.spin-slow</c>, <c>.spin-mid</c> or <c>.spin-fast</c>, also while paused.</summary>
public sealed class UiSpinners
{
    private const float SlowDegreesPerSecond = 9f;
    private const float MidDegreesPerSecond = 18f;
    private const float FastDegreesPerSecond = 60f;

    private readonly List<VisualElement> _slow;
    private readonly List<VisualElement> _mid;
    private readonly List<VisualElement> _fast;

    public UiSpinners(VisualElement root)
    {
        _slow = root.Query(className: "spin-slow").ToList();
        _mid = root.Query(className: "spin-mid").ToList();
        _fast = root.Query(className: "spin-fast").ToList();
    }

    public void Tick()
    {
        Spin(_slow, SlowDegreesPerSecond);
        Spin(_mid, MidDegreesPerSecond);
        Spin(_fast, FastDegreesPerSecond);
    }

    private static void Spin(List<VisualElement> elements, float degreesPerSecond)
    {
        var rotation = new Rotate(new Angle(Time.unscaledTime * degreesPerSecond % 360f, AngleUnit.Degree));
        foreach (VisualElement element in elements)
        {
            element.style.rotate = rotation;
        }
    }
}

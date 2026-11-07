using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// Selection over the <c>.menu-item</c> elements of a container.
/// Up/Down move, Z/Enter confirm, X/Escape cancel, Left/Right adjust; disabled rows are skipped.
/// </summary>
public sealed class UiMenu
{
    private const string ItemClass = "menu-item";
    private const string SelectedClass = "menu-item--selected";
    private const string DisabledClass = "menu-item--disabled";

    private readonly List<VisualElement> _items;
    private readonly VisualElement _cursor;
    private int _index = -1;
    private int _activatedFrame;

    public event Action<string> Submitted;
    public event Action Cancelled;
    public event Action<int> Adjusted;

    public string CurrentName => _index >= 0 ? _items[_index].name : string.Empty;

    public UiMenu(VisualElement container, VisualElement cursor)
    {
        _items = container.Query(className: ItemClass).ToList();
        _cursor = cursor;

        foreach (VisualElement item in _items)
        {
            item.RegisterCallback<PointerEnterEvent>(_ => OnHover(item));
            item.RegisterCallback<ClickEvent>(_ => OnClick(item));
        }
    }

    /// <summary>Call when the menu appears. The key press that opened it is ignored.</summary>
    public void Activate()
    {
        _activatedFrame = Time.frameCount;
        Select(_index >= 0 ? _index : _items.FindIndex(IsEnabled));
    }

    public void Tick()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || Time.frameCount == _activatedFrame)
        {
            return;
        }

        if (keyboard.upArrowKey.wasPressedThisFrame)
        {
            Move(-1);
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            Move(1);
        }
        else if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            Adjusted?.Invoke(-1);
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            Adjusted?.Invoke(1);
        }
        else if (keyboard.zKey.wasPressedThisFrame || keyboard.enterKey.wasPressedThisFrame)
        {
            Submitted?.Invoke(CurrentName);
        }
        else if (keyboard.xKey.wasPressedThisFrame || keyboard.escapeKey.wasPressedThisFrame)
        {
            Cancelled?.Invoke();
        }
    }

    private void Select(int index)
    {
        if (index < 0)
        {
            return;
        }

        _index = index;
        for (int i = 0; i < _items.Count; i++)
        {
            _items[i].EnableInClassList(SelectedClass, i == index);
        }

        // The cursor lives inside the selected row, so its offset is plain USS.
        _items[index].Add(_cursor);
    }

    private void Move(int direction)
    {
        for (int step = 1; step <= _items.Count; step++)
        {
            int candidate = (_index + direction * step + _items.Count * step) % _items.Count;
            if (IsEnabled(_items[candidate]))
            {
                Select(candidate);
                return;
            }
        }
    }

    private void OnHover(VisualElement item)
    {
        if (IsEnabled(item))
        {
            Select(_items.IndexOf(item));
        }
    }

    private void OnClick(VisualElement item)
    {
        if (IsEnabled(item))
        {
            Select(_items.IndexOf(item));
            Submitted?.Invoke(item.name);
        }
    }

    private static bool IsEnabled(VisualElement item)
    {
        return !item.ClassListContains(DisabledClass);
    }
}

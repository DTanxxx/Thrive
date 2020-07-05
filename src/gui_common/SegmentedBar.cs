using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
///   A ProgressBar that is split up into IconProgressBars, data is stored in a dictionary
/// </summary>
public class SegmentedBar : Control
{
    public PackedScene IconProgressBarScene = GD.Load<PackedScene>("res://src/gui_common/IconProgressBar.tscn");
    public string Type;
    public float MaxValue;

    public Vector2 Size
    {
        get
        {
            return new Vector2(RectSize.x, RectSize.y);
        }
        set
        {
            RectSize = new Vector2(value[0], value[1]);
            RectMinSize = RectSize;
        }
    }

    public void UpdateAndMoveBars(Dictionary<string, float> data)
    {
        RemoveUnusedBars(this, data);

        int location = 0;
        foreach (var dataPair in data)
        {
            CreateAndUpdateBar(dataPair, this, location);
            location++;
        }

        foreach (var dataPair in data)
        {
            if (HasNode(dataPair.Key))
                UpdateDisabledBars(dataPair, this);
        }

        foreach (var dataPair in data)
        {
            if (HasNode(dataPair.Key))
                MoveBars(GetNode<IconProgressBar>(dataPair.Key));
        }
    }

    private void RemoveUnusedBars(SegmentedBar parent, Dictionary<string, float> data)
    {
        List<IconProgressBar> unusedBars = new List<IconProgressBar>();
        foreach (IconProgressBar progressBar in parent.GetChildren())
        {
            bool match = false;
            foreach (var dataPair in data)
            {
                if (progressBar.Name == dataPair.Key)
                    match = true;
            }

            if (!match)
                unusedBars.Add(progressBar);
        }

        foreach (IconProgressBar unusedBar in unusedBars)
            unusedBar.Free();
    }

    private void CreateAndUpdateBar(KeyValuePair<string, float> dataPair, SegmentedBar parent, int location = -1)
    {
        if (parent.HasNode(dataPair.Key))
        {
            IconProgressBar progressBar = (IconProgressBar)parent.GetNode(dataPair.Key);
            if (location >= 0)
            {
                progressBar.SetBarLocation(location);
                progressBar.SetBarActualLocation(location);
            }

            if (progressBar.Disabled)
                return;
            progressBar.SetBarLeftShift(GetPreviousBar(parent, progressBar).RectSize.x + GetPreviousBar(parent, progressBar).MarginLeft);
            progressBar.SetBarSize(new Vector2((float)Math.Floor(dataPair.Value / parent.MaxValue * Size[0]), Size[1]));
        }
        else
        {
            IconProgressBar progressBar = (IconProgressBar)IconProgressBarScene.Instance();
            progressBar.SetBarName(dataPair.Key);
            progressBar.SetBarColour(BarHelper.GetBarColour(Type, dataPair.Key, GetIndex() == 0));
            progressBar.SetBarLeftShift(GetPreviousBar(parent, progressBar).RectSize.x + GetPreviousBar(parent, progressBar).MarginLeft);
            progressBar.SetBarSize(new Vector2((float)Math.Floor(dataPair.Value / parent.MaxValue * Size[0]), Size[1]));
            progressBar.SetBarIconTexture(BarHelper.GetBarIcon(Type, dataPair.Key));
            if (location >= 0)
            {
                progressBar.SetBarLocation(location);
                progressBar.SetBarActualLocation(location);
            }

            parent.AddChild(progressBar);
            progressBar.Connect("gui_input", this, nameof(BarToggled), new Godot.Collections.Array() { progressBar });
        }
    }

    private void BarToggled(InputEvent @event, IconProgressBar bar)
    {
        if (@event is InputEventMouseButton eventMouse && @event.IsPressed())
        {
            bar.SetBarDisabledStatus(!bar.Disabled);
            HandleBarDisabling(bar);
        }
    }

    private IconProgressBar GetPreviousBar(SegmentedBar parent, IconProgressBar currentBar)
    {
        return currentBar.GetIndex() > 0 ?
            parent.GetChild<IconProgressBar>(currentBar.GetIndex() - 1) : new IconProgressBar();
    }

    private void UpdateDisabledBars(KeyValuePair<string, float> dataPair, SegmentedBar parent)
    {
        IconProgressBar progressBar = (IconProgressBar)parent.GetNode(dataPair.Key);
        if (!progressBar.Disabled)
            return;
        progressBar.SetBarLeftShift(GetPreviousBar(parent, progressBar).RectSize.x + GetPreviousBar(parent, progressBar).MarginLeft);
        progressBar.SetBarSize(new Vector2((float)Math.Floor(dataPair.Value / parent.MaxValue * Size[0]), Size[1]));
    }

    private void CalculateActualLocation(SegmentedBar parentBar)
    {
        List<IconProgressBar> children = new List<IconProgressBar>();
        foreach (IconProgressBar childBar in parentBar.GetChildren())
        {
            children.Add(childBar);
        }

        children = children.OrderBy(bar =>
        {
            return bar.Location + (bar.Disabled ? children.Count : 0);
        }).ToList();

        foreach (var childBar in children)
        {
            childBar.SetBarActualLocation(children.IndexOf(childBar));
        }
    }

    private void MoveByIndexBars(IconProgressBar bar)
    {
        bar.GetParent().MoveChild(bar, bar.ActualLocation);
    }

    private void HandleBarDisabling(IconProgressBar bar)
    {
        if (bar.Disabled)
        {
            bar.SetBarIconModulation(new Color(0, 0, 0));
            bar.SetBarColour(new Color(0.73f, 0.73f, 0.73f));
            MoveBars(bar);
        }
        else
        {
            bar.SetBarIconModulation(new Color(1, 1, 1));
            bar.SetBarColour(BarHelper.GetBarColour(Type, bar.Name, GetIndex() == 0));
            MoveBars(bar);
        }
    }

    private void MoveBars(IconProgressBar bar)
    {
        CalculateActualLocation(bar.GetParent<SegmentedBar>());
        foreach (IconProgressBar iconBar in bar.GetParent().GetChildren())
            MoveByIndexBars(iconBar);

        foreach (IconProgressBar iconBar in bar.GetParent().GetChildren())
        {
            float value = iconBar.RectSize.x / Size[0] * (float)((SegmentedBar)bar.GetParent()).MaxValue;
            CreateAndUpdateBar(new KeyValuePair<string, float>(iconBar.Name, value), (SegmentedBar)bar.GetParent());
        }

        foreach (IconProgressBar iconBar in bar.GetParent().GetChildren())
        {
            float value = iconBar.RectSize.x / Size[0] * (float)((SegmentedBar)bar.GetParent()).MaxValue;
            UpdateDisabledBars(new KeyValuePair<string, float>(iconBar.Name, value), (SegmentedBar)bar.GetParent());
        }
    }
}

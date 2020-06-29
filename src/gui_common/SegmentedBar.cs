using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
///   A ProgressBar that is split up into IconProgressBars, data is stored in a dictionary
/// </summary>
public class SegmentedBar : ProgressBar
{
    public PackedScene IconProgressBarScene = GD.Load<PackedScene>("res://src/gui_common/IconProgressBar.tscn");
    public string Type;

    public void BarToggled(InputEvent @event, IconProgressBar bar)
    {
        if (@event is InputEventMouseButton eventMouse && @event.IsPressed())
        {
            IconBarConfig config = new IconBarConfig(bar);
            config.Disabled = !config.Disabled;
            new SegmentedBarConfig(this).handleBarDisabling(bar);
        }
    }
}

/// <summary>
///   Configuration class for SegmentedBar class
/// </summary>
public class SegmentedBarConfig
{
    private SegmentedBar target;

    public SegmentedBarConfig(SegmentedBar target)
    {
        this.target = target;
    }

    public float[] Size
    {
        get { return new float[]{ target.RectSize.x, target.RectSize.y }; }
        set
        {
            target.RectSize = new Vector2(value[0], value[1]);
            target.RectMinSize = target.RectSize;
        }
    }

    public string Type
    {
        get { return target.Type; }
        set { target.Type = value; }
    }

    public void updateAndMoveBars(Dictionary<string, float> data)
    {
        removeUnusedBars(target, data);

        int location = 0;
        foreach (var dataPair in data)
        {
            createAndUpdateBar(dataPair, target, location);
            location++;
        }

        foreach (var dataPair in data)
        {
            if (target.HasNode(dataPair.Key))
                updateDisabledBars(dataPair, target);
        }

        foreach (var dataPair in data)
        {
            if (target.HasNode(dataPair.Key))
                moveBars(target.GetNode<IconProgressBar>(dataPair.Key));
        }
    }

    private void removeUnusedBars(ProgressBar parent, Dictionary<string, float> data)
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
        foreach(IconProgressBar unusedBar in unusedBars)
            unusedBar.Free();
    }

    private void createAndUpdateBar(KeyValuePair <string, float> dataPair, ProgressBar parent, int location = -1)
    {
        if (parent.HasNode(dataPair.Key))
        {
            IconProgressBar progressBar = (IconProgressBar)parent.GetNode(dataPair.Key);
            IconBarConfig config = new IconBarConfig(progressBar);
            if (config.Disabled) return;
            config.LeftShift = getPreviousBar(parent, progressBar).RectSize.x + getPreviousBar(parent, progressBar).MarginLeft;
            config.Size = new Vector2((float)Math.Floor(dataPair.Value / parent.MaxValue * Size[0]), Size[1]);
        }
        else
        {
            IconProgressBar progressBar = (IconProgressBar)new SegmentedBar().IconProgressBarScene.Instance();
            IconBarConfig config = new IconBarConfig(progressBar);
            parent.AddChild(progressBar);
            config.Name = dataPair.Key;
            config.Colour = BarHelper.GetBarColour(Type, dataPair.Key, target);
            config.LeftShift = getPreviousBar(parent, progressBar).RectSize.x + getPreviousBar(parent, progressBar).MarginLeft;
            config.Size = new Vector2((float)Math.Floor(dataPair.Value / parent.MaxValue * Size[0]), Size[1]);
            config.Texture = BarHelper.GetBarIcon(Type, dataPair.Key);
            progressBar.Connect("gui_input", target, nameof(target.BarToggled), new Godot.Collections.Array() { progressBar } );
            if (location >= 0)
            {
                config.Location = location;
                config.ActualLocation = location;
            }
        }
    }

    private IconProgressBar getPreviousBar(ProgressBar parent, IconProgressBar currentBar)
    {
        return currentBar.GetIndex() != 0 ?
            parent.GetChild<IconProgressBar>(currentBar.GetIndex() - 1) : new IconProgressBar();
    }

    private void updateDisabledBars(KeyValuePair <string, float> dataPair, ProgressBar parent)
    {
        IconProgressBar progressBar = (IconProgressBar)parent.GetNode(dataPair.Key);
        IconBarConfig config = new IconBarConfig(progressBar);
        if (!config.Disabled) return;
        config.LeftShift = getPreviousBar(parent, progressBar).RectSize.x + getPreviousBar(parent, progressBar).MarginLeft;
        config.Size = new Vector2((float)Math.Floor(dataPair.Value / parent.MaxValue * Size[0]), Size[1]);
    }

    private void calculateActualLocation(ProgressBar parentBar)
    {
        List<IconProgressBar> children = new List<IconProgressBar>();
        foreach (IconProgressBar childBar in parentBar.GetChildren())
        {
            children.Add(childBar);
        }

        children = children.OrderBy(bar => {
            IconBarConfig config = new IconBarConfig(bar);
            return config.Location + (config.Disabled ? children.Count : 0);
        }).ToList();

        foreach (var childBar in children)
        {
            IconBarConfig config = new IconBarConfig(childBar);
            config.ActualLocation = children.IndexOf(childBar);
        }
    }

    private void moveByIndexBars(IconProgressBar bar)
    {
        IconBarConfig config = new IconBarConfig(bar);
        bar.GetParent().MoveChild(bar, config.ActualLocation);
    }

    public void handleBarDisabling(IconProgressBar bar)
    {
        IconBarConfig config = new IconBarConfig(bar);
        if (bar.Disabled)
        {
            config.Modulate = new Color("#000000");
            config.Colour = new Color("#bbbbbb");
            moveBars(bar);
        }
        else
        {
            config.Modulate = new Color("#ffffff");
            config.Colour = BarHelper.GetBarColour(Type, bar.Name, target);
            moveBars(bar);
        }
    }
    
    private void moveBars(IconProgressBar bar)
    {
        calculateActualLocation(bar.GetParent<ProgressBar>());
        foreach (IconProgressBar iconBar in bar.GetParent().GetChildren())
            moveByIndexBars(iconBar);

        foreach (IconProgressBar iconBar in bar.GetParent().GetChildren())
        {
            float value = iconBar.RectSize.x / Size[0] * (float)((ProgressBar)bar.GetParent()).MaxValue;
            createAndUpdateBar(new KeyValuePair<string, float>(iconBar.Name, value), (ProgressBar)bar.GetParent());
        }

        foreach (IconProgressBar iconBar in bar.GetParent().GetChildren())
        {
            float value = iconBar.RectSize.x / Size[0] * (float)((ProgressBar)bar.GetParent()).MaxValue;
            updateDisabledBars(new KeyValuePair<string, float>(iconBar.Name, value), (ProgressBar)bar.GetParent());
        }
    }
}

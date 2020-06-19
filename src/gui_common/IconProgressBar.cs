using Godot;

public class IconProgressBar : ColorRect
{
    public bool disabled = false;
    public int location;
    public int actualLocation;
}

public class IconBarConfig
{
    private IconProgressBar target;
    private TextureRect icon;
    public IconBarConfig(IconProgressBar target)
    {
        this.target = target;
        this.icon = target.GetChild<TextureRect>(0);
    }

    public string Name {
        set { target.Name = value; }
    }
    public Color Colour {
        set { target.Color = value; }
    }
    public Vector2 Size {
        set {
            target.RectSize = value;
            icon.RectSize = new Vector2(value.y, value.y);
            icon.Visible = target.RectSize.x >= icon.RectSize.x;
        }
    }
    public float LeftShift {
        set { target.MarginLeft = value; }
    }
    public bool Disabled {
        get { return target.disabled; }
        set { target.disabled = value; }
    }
    public int Location {
        get { return target.location; }
        set { target.location = value; }
    }
    public int ActualLocation {
        get { return target.actualLocation; }
        set { target.actualLocation = value; }
    }
    public Texture Texture {
        set { icon.Texture = value; }
    }
    public Color Modulate {
        set { icon.Modulate = value; }
    }
}
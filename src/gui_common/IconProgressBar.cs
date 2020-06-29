using Godot;

public class IconProgressBar : ColorRect
{
    public bool Disabled = false;
    public int Location;
    public int ActualLocation;

}

public class IconBarConfig
{
    private IconProgressBar target;
    private TextureRect icon;
    public IconBarConfig(IconProgressBar target)
    {
        this.target = target;
        icon = target.GetChild<TextureRect>(0);
    }

    public string Name
    {
        get { return target.Name; }
        set { target.Name = value; }
    }

    public Color Colour
    {
        get { return target.Color; }
        set { target.Color = value; }
    }

    public Vector2 Size
    {
        get { return target.RectSize; }
        set
        {
            target.RectSize = value;
            icon.RectSize = new Vector2(value.y, value.y);
            icon.Visible = target.RectSize.x >= icon.RectSize.x;
        }
    }

    public float LeftShift
    {
        get { return target.MarginLeft; }
        set { target.MarginLeft = value; }
    }

    public bool Disabled
    {
        get { return target.Disabled; }
        set { target.Disabled = value; }
    }

    public int Location
    {
        get { return target.Location; }
        set { target.Location = value; }
    }

    public int ActualLocation
    {
        get { return target.ActualLocation; }
        set { target.ActualLocation = value; }
    }

    public Texture Texture
    {
        get { return icon.Texture; }
        set { icon.Texture = value; }
    }

    public Color Modulate
    {
        get { return icon.Modulate; }
        set { icon.Modulate = value; }
    }
}

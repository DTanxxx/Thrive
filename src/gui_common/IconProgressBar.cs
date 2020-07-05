using Godot;

public class IconProgressBar : ColorRect
{
    public bool Disabled { get; private set; }
    public int Location { get; private set; }
    public int ActualLocation { get; private set; }

    public void SetBarName(string name)
    {
        this.Name = name;
    }

    public void SetBarColour(Color colour)
    {
        this.Color = colour;
    }

    public void SetBarLeftShift(float leftShift)
    {
        this.MarginLeft = leftShift;
    }

    public void SetBarSize(Vector2 size)
    {
        this.RectSize = size;
        this.GetChild<TextureRect>(0).RectSize = new Vector2(size.y, size.y);
        this.GetChild<TextureRect>(0).Visible = this.RectSize.x >=  this.GetChild<TextureRect>(0).RectSize.x;
    }

    public void SetBarIconTexture(Texture texture)
    {
        this.GetChild<TextureRect>(0).Texture = texture;
    }

    public void SetBarIconModulation(Color colour)
    {
        this.GetChild<TextureRect>(0).Modulate = colour;
    }

    public void SetBarLocation(int location)
    {
        this.Location = location;
    }

    public void SetBarActualLocation(int actualLocation)
    {
        this.ActualLocation = actualLocation;
    }

    public void SetBarDisabledStatus(bool disabled)
    {
        this.Disabled = disabled;
    }
}
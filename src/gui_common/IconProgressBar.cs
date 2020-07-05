using Godot;

public class IconProgressBar : ColorRect
{
    public bool Disabled { get; private set; }
    public int Location { get; private set; }
    public int ActualLocation { get; private set; }

    public void SetBarName(string name)
    {
        Name = name;
    }

    public void SetBarColour(Color colour)
    {
        Color = colour;
    }

    public void SetBarLeftShift(float leftShift)
    {
        MarginLeft = leftShift;
    }

    public void SetBarSize(Vector2 size)
    {
        RectSize = size;
        GetChild<TextureRect>(0).RectSize = new Vector2(size.y, size.y);
        GetChild<TextureRect>(0).Visible = RectSize.x >= GetChild<TextureRect>(0).RectSize.x;
    }

    public void SetBarIconTexture(Texture texture)
    {
        GetChild<TextureRect>(0).Texture = texture;
    }

    public void SetBarIconModulation(Color colour)
    {
        GetChild<TextureRect>(0).Modulate = colour;
    }

    public void SetBarLocation(int location)
    {
        Location = location;
    }

    public void SetBarActualLocation(int actualLocation)
    {
        ActualLocation = actualLocation;
    }

    public void SetBarDisabledStatus(bool disabled)
    {
        Disabled = disabled;
    }
}

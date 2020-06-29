using Godot;

/// <summary>
///   Used to access the color and icon of a bar from a provided dictionary
/// </summary>
public static class BarHelper
{
    public static Color GetBarColour(string type, string name, ProgressBar segmentedBar)
    {
        switch (type)
        {
            case "ATP":
            {
                foreach (var organelle in SimulationParameters.Instance.GetAllOrganelles())
                {
                    if (organelle.Name == name)
                    {
                        if (segmentedBar.GetIndex() == 0)
                        {
                            return new Color(organelle.ProductionColour);
                        }
                        else if (segmentedBar.GetIndex() == 1)
                        {
                            return new Color(organelle.ConsumptionColour);
                        }
                        else
                        {
                            return new Color("#444444");
                        }
                    }
                }

                switch (name)
                {
                    case "baseMovement":
                        return new Color("#ff5524");
                    case "osmoregulation":
                        return new Color("#ffd63e");
                    default:
                        return new Color("#444444");
                }
            }
            default:
                return new Color("#444444");
        }
    }

    public static Texture GetBarIcon(string type, string name)
    {
        switch (type)
        {
            case "ATP":
            {
                foreach (var organelle in SimulationParameters.Instance.GetAllOrganelles())
                {
                    if (organelle.Name == name)
                        return GD.Load<Texture>(organelle.IconPath);
                }

                switch (name)
                {
                    case "baseMovement":
                        return GD.Load<Texture>("res://assets/textures/gui/bevel/baseMovementIcon.png");
                    case "osmoregulation":
                        return GD.Load<Texture>("res://assets/textures/gui/bevel/osmoIcon.png");
                    default:
                        return GD.Load<Texture>("res://assets/textures/gui/logo.png");
                }
            }
            default:
                return GD.Load<Texture>("res://assets/textures/gui/logo.png");
        }
    }
}

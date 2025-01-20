using Godot;

public partial class LandingPad : CsgBox3D
{
  [Export(PropertyHint.File, "*.tscn")]
  public string FilePath;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}
}

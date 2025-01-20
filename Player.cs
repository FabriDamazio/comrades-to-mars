using Godot;

public partial class Player : RigidBody3D
{
    public override void _Ready()
    {
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("boost"))
        {
            ApplyCentralForce(Basis.Y * (float)delta * 1000.0f);
        }
        if (Input.IsActionPressed("rotate_left"))
        {
            ApplyTorque(new Vector3(0, 0, 100.0f * (float)delta));
        }

        if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new Vector3(0, 0, -100.0f * (float)delta));
        }
    }
}

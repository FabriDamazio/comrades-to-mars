using Godot;
using System;

public partial class MovingHazard : AnimatableBody3D
{
    [Export]
    public Vector3 Destination;

    [Export]
    public float Duration = 1;

    public override void _Ready()
    {
        var tween = CreateTween();
        tween.SetLoops();
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.TweenProperty(this, "global_position", GlobalPosition + Destination, Duration);
        tween.TweenProperty(this, "global_position", GlobalPosition, Duration);
    }

    public override void _Process(double delta)
    {
    }
}

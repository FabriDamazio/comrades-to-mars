using Godot;

public partial class Player : RigidBody3D
{
    [Export(PropertyHint.Range, "750, 3000")]
    public float Thrust = 1000.0f;

    [Export(PropertyHint.Range, "0, 1000")]
    public float TorqueThrust = 100.0f;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("boost"))
        {
            ApplyCentralForce(Basis.Y * (float)delta * Thrust);
        }
        if (Input.IsActionPressed("rotate_left"))
        {
            ApplyTorque(new Vector3(0, 0, TorqueThrust * (float)delta));
        }

        if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new Vector3(0, 0, -TorqueThrust * (float)delta));
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("Goal"))
        {
            var landingPad = body as LandingPad;
            CompleteLevel(landingPad.FilePath);
        }

        if (body.IsInGroup("Hazard"))
        {
            CrashSequence();
        }
    }

    private void CrashSequence()
    {
        GD.Print("KABOOM!");
        CallDeferred("ReloadScene");
    }

    private void ReloadScene()
    {
        GetTree().ReloadCurrentScene();
    }

    private void CompleteLevel(string nextLevelFilePath)
    {
        GD.Print("Level Complete");
        CallDeferred("ChangeScene", nextLevelFilePath);
    }

    private void ChangeScene(string sceneFilePath)
    {
        GetTree().ChangeSceneToFile(sceneFilePath);
    }
}

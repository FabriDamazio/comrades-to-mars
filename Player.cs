using Godot;

public partial class Player : RigidBody3D
{
    [Export(PropertyHint.Range, "750, 3000")]
    public float Thrust = 1000.0f;

    [Export(PropertyHint.Range, "0, 1000")]
    public float TorqueThrust = 100.0f;

    private bool _isTrasitioning = false;
    private AudioStreamPlayer _explosionAudio;
    private AudioStreamPlayer _successAudio;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        _explosionAudio = GetNode<AudioStreamPlayer>("%ExplosionAudio");
        _successAudio = GetNode<AudioStreamPlayer>("%SuccessAudio");
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
        if (_isTrasitioning is false)
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
    }

    private void CrashSequence()
    {
        GD.Print("KABOOM!");
        _explosionAudio.Play();
        SetProcess(false);
        _isTrasitioning = true;
        var tween = CreateTween();
        tween.TweenInterval(2.5);
        tween.TweenCallback(Callable.From(ReloadScene));
    }

    private void ReloadScene()
    {
        GetTree().ReloadCurrentScene();
    }

    private void CompleteLevel(string nextLevelFilePath)
    {
        GD.Print("Level Complete");
        _successAudio.Play();
        SetProcess(false);
        _isTrasitioning = true;
        var tween = CreateTween();
        tween.TweenInterval(2.5);
        tween.TweenCallback(Callable.From(() => ChangeScene(nextLevelFilePath)));
    }

    private void ChangeScene(string sceneFilePath)
    {
        GetTree().ChangeSceneToFile(sceneFilePath);
    }
}

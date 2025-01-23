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
    private AudioStreamPlayer3D _rocketAudio;
    private GpuParticles3D _boosterParticles;
    private GpuParticles3D _leftBoosterParticles;
    private GpuParticles3D _rightBoosterParticles;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
        _explosionAudio = GetNode<AudioStreamPlayer>("%ExplosionAudio");
        _successAudio = GetNode<AudioStreamPlayer>("%SuccessAudio");
        _rocketAudio = GetNode<AudioStreamPlayer3D>("%RocketAudio");
        _boosterParticles = GetNode<GpuParticles3D>("%BoosterParticles");
        _rightBoosterParticles = GetNode<GpuParticles3D>("%RightBoosterParticles");
        _leftBoosterParticles = GetNode<GpuParticles3D>("%LeftBoosterParticles");
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("boost"))
        {
            ApplyCentralForce(Basis.Y * (float)delta * Thrust);
            _boosterParticles.Emitting = true;

            if (_rocketAudio.Playing is false)
            {
                _rocketAudio.Play();
            }
        }
        else
        {
            _rocketAudio.Stop();
            _boosterParticles.Emitting = false;
        }

        if (Input.IsActionPressed("rotate_left"))
        {
            ApplyTorque(new Vector3(0, 0, TorqueThrust * (float)delta));
            _rightBoosterParticles.Emitting = true;
        }
        else
        {
            _rightBoosterParticles.Emitting = false;
        }

        if (Input.IsActionPressed("rotate_right"))
        {
            ApplyTorque(new Vector3(0, 0, -TorqueThrust * (float)delta));
            _leftBoosterParticles.Emitting = true;
        }
        else
        {
            _leftBoosterParticles.Emitting = false;
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

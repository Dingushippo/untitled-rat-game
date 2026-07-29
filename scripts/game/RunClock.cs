using Godot;

public partial class RunClock : Node
{
    const float DAY_LENGTH = 420f;
    private float _timer = 0;
    private bool _timerActive = false;
    public float DayProgress => Mathf.Clamp(_timer / DAY_LENGTH, 0, 1f);
    public int Day {get; private set;} = 1;

    public void Pause() => _timerActive = false;
    public void Start() => _timerActive = true;
    public void ResetTimer() => _timer = 0;
    public void IncrementDay() => Day++;

    public override void _Process(double delta)
    {
        if (!_timerActive) return;
        
        _timer += (float)delta;

        if (_timer >= DAY_LENGTH)
        {
            EventBus.Publish(Event.Sundown, Day);
            Pause();
            ResetTimer();
        }
    }

}
using Godot;
using System;

public partial class RunClock : Node
{
    const float DAY_LENGTH = 5f; // 420 default, 7 minutes
    public static RunClock Instance;
    private float _timer = 0;
    private bool _timerActive = false;
    public float DayProgress => Mathf.Clamp(_timer / DAY_LENGTH, 0, 1f);
    public int Day { get; private set; } = 1;

    public void Pause() => _timerActive = false;
    public void Start() => _timerActive = true;
    public void ResetTimer()
    {
        _timer = 0;
        Pause();
    }

    public void ResetFull()
    {
        Day = 1;
        ResetTimer();
    }
    public void IncrementDay() => Day++;

    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            QueueFree();
        }
    }

    public override void _Process(double delta)
    {
        if (!_timerActive) return;

        _timer += (float)delta;

        if (_timer >= DAY_LENGTH)
        {
            EventBus.Publish(Event.Sundown, Day);
        }
    }

    public string TimeToSundown()
    {
        TimeSpan time = TimeSpan.FromSeconds(DAY_LENGTH - _timer);
        return time.ToString(@"mm\:ss");
    }

}
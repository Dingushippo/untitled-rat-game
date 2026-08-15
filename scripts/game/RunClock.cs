using Godot;
using System;

public partial class RunClock : Node
{
    const float DAY_LENGTH_DEFAULT = 420f;
    const int START_TIME_SECONDS = 7 * 60 * 60; // 7:00 AM
    const int END_TIME_SECONDS = 24 * 60 * 60; // 12:00 AM
    private static RunClock _instance;
    public static RunClock Instance => _instance;
    private float _timer = 0;
    private bool _timerActive = false;
    private float _dayLength;
    private bool _dayComplete;
    private string _currentTimeText = "";
    public float DayProgress => Mathf.Clamp(_timer / _dayLength, 0, 1f);
    public int Day { get; private set; } = 1;

    public void Pause() => _timerActive = false;
    public void Start() => _timerActive = true;
    public void ResetTimer()
    {
        _timer = 0;
        _dayComplete = false;
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
        if (!Singleton.ClaimOrFree(ref _instance, this)) return;
    }

    public override void _Ready()
    {
        _dayLength = GameManager.Instance.Tuning != null ? GameManager.Instance.Tuning.DayLength : DAY_LENGTH_DEFAULT;
        EventBus.Publish(new ClockTick(_currentTimeText, Day, DayProgress));
    }

    public override void _Process(double delta)
    {
        if (!_timerActive) return;

        _timer += (float)delta;

        if (_currentTimeText != GetClockText())
        {
            _currentTimeText = GetClockText();
            EventBus.Publish(new ClockTick(_currentTimeText, Day, DayProgress));
        }

        if (_timer >= _dayLength && !_dayComplete)
        {
            _dayComplete = true;
            EventBus.Publish(new Sundown(Day));
        }
    }

    public string GetClockText()
    {
        int newTime = (int)Mathf.Remap(_timer, 0, _dayLength, START_TIME_SECONDS, END_TIME_SECONDS);
        TimeSpan time = TimeSpan.FromSeconds(newTime);
        return time.ToString(@"hh\:mm");
    }

}
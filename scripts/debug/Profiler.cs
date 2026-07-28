using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

/// <summary>
/// Lightweight sampling profiler for C# hot paths, which Godot's own profiler cannot see because
/// it only instruments GDScript and engine internals.
///
/// Main thread only - no locking, deliberately.
/// </summary>
public static class Profiler
{
    /// <summary>Number of recent samples kept per section for the rolling figures.</summary>
    private const int WINDOW = 256;

    private static readonly double TICKS_TO_US = 1_000_000.0 / Stopwatch.Frequency;

    private static readonly Dictionary<string, Section> _sections = new();
    private static readonly Dictionary<string, Counter> _counters = new();

    public static bool Enabled { get; set; } = OS.IsDebugBuild();

    /// <summary>
    /// Times the enclosing block: <c>using (Profiler.Sample("throw.simulate")) { ... }</c>.
    /// Returns an inert scope when disabled so release builds pay almost nothing.
    /// </summary>
    public static Scope Sample(string name) => Enabled ? new Scope(name) : default;

    public static void Count(string name, long amount = 1)
    {
        if (!Enabled) return;

        if (!_counters.TryGetValue(name, out Counter counter))
        {
            counter = new Counter();
            _counters[name] = counter;
        }

        counter.Total += amount;
    }

    /// <summary>Call once per frame so counters can report a per-frame rate rather than a raw total.</summary>
    public static void EndFrame()
    {
        if (!Enabled) return;

        foreach (Counter counter in _counters.Values)
        {
            counter.Stats.Add(counter.Total - counter.PreviousTotal);
            counter.PreviousTotal = counter.Total;
        }
    }

    public static void Reset()
    {
        _sections.Clear();
        _counters.Clear();
    }

    public static bool HasSamples => _sections.Count > 0 || _counters.Count > 0;

    public static string Report()
    {
        if (!HasSamples)
            return "Profiler: no samples yet.";

        StringBuilder text = new();

        if (_sections.Count > 0)
        {
            text.AppendLine($"{"section",-24}{"calls",8}{"avg us",10}{"recent",10}{"p95",10}{"peak",10}");
            foreach ((string name, Section section) in _sections)
            {
                text.AppendLine(
                    $"{name,-24}{section.Stats.Count,8}{section.Stats.Average,10:F1}" +
                    $"{section.Stats.RecentAverage,10:F1}{section.Stats.Percentile95,10:F1}{section.Stats.Max,10:F1}"
                );
            }
        }

        if (_counters.Count > 0)
        {
            text.AppendLine();
            text.AppendLine($"{"counter",-24}{"total",12}{"per frame",12}{"peak frame",12}");
            foreach ((string name, Counter counter) in _counters)
            {
                text.AppendLine(
                    $"{name,-24}{counter.Total,12}{counter.Stats.RecentAverage,12:F1}{counter.Stats.Max,12:F0}"
                );
            }
        }

        return text.ToString();
    }

    private static void Stop(string name, long startTicks)
    {
        double elapsed = (Stopwatch.GetTimestamp() - startTicks) * TICKS_TO_US;

        if (!_sections.TryGetValue(name, out Section section))
        {
            section = new Section();
            _sections[name] = section;
        }

        section.Stats.Add(elapsed);
    }

    public readonly struct Scope : IDisposable
    {
        private readonly string _name;
        private readonly long _start;

        internal Scope(string name)
        {
            _name = name;
            _start = Stopwatch.GetTimestamp();
        }

        public void Dispose()
        {
            if (_name is not null)
                Stop(_name, _start);
        }
    }

    private sealed class Section
    {
        public readonly Stats Stats = new();
    }

    private sealed class Counter
    {
        public long Total;
        public long PreviousTotal;
        public readonly Stats Stats = new();
    }

    /// <summary>
    /// Running totals plus a fixed window of recent samples. The window is what you watch while
    /// tuning; the lifetime average hides the spikes that actually drop frames.
    /// </summary>
    private sealed class Stats
    {
        private readonly double[] _window = new double[WINDOW];
        private int _next;
        private int _filled;
        private double _total;

        public long Count { get; private set; }
        public double Max { get; private set; }
        public double Average => Count > 0 ? _total / Count : 0.0;

        public void Add(double value)
        {
            Count++;
            _total += value;
            if (value > Max) Max = value;

            _window[_next] = value;
            _next = (_next + 1) % WINDOW;
            if (_filled < WINDOW) _filled++;
        }

        public double RecentAverage
        {
            get
            {
                if (_filled == 0) return 0.0;

                double sum = 0.0;
                for (int i = 0; i < _filled; i++) sum += _window[i];
                return sum / _filled;
            }
        }

        public double Percentile95
        {
            get
            {
                if (_filled == 0) return 0.0;

                double[] sorted = new double[_filled];
                Array.Copy(_window, sorted, _filled);
                Array.Sort(sorted);
                return sorted[Mathf.Min(_filled - 1, (int)(_filled * 0.95f))];
            }
        }
    }
}

using System;

public interface IRitualTrigger
{
    public Action OnFulfilled { get; set; }
    public bool IsFulfilled { get; }
}
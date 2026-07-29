using Godot;

public enum Economy
{
    Tithes,
    Fervor
}

public partial class EconomyService : Node
{
    private const float GLOBAL_CYCLE_REDUCTION = 0.8f;
    private const float FERVOR_BOOST_THRESHOLD = 0.75f;
    public static EconomyService Instance {get; private set;}
    public float CycleBoost => Fervor / 100 >= FERVOR_BOOST_THRESHOLD ? GLOBAL_CYCLE_REDUCTION : 1f;
    public int Tithes
    {
        get => _tithes;
        set => _tithes = value;
    }

    private int _tithes;
    public int Fervor
    {
        get => _fervor;
        set => _fervor = Mathf.Clamp(value, 0, 100);
    }
    private int _fervor;
    private int _fervorDecayPerMinute = 1;
    private float _fervorDecayTimer = 0;

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
        HandleFervorDecay((float)delta);
    }

    private void HandleFervorDecay(float delta)
    {
        if (_fervorDecayTimer < 60f)
        {
            _fervorDecayTimer += delta;
        }
        Fervor -= _fervorDecayPerMinute;
        _fervorDecayTimer = 0;
    }

    public void AddTithes(int amount)
    {
        EventBus.Publish(Event.ResourceChanged, Economy.Tithes, Tithes, Tithes + amount);
        Tithes += amount;
    }

    public void AddFervor(int amount)
    {
        EventBus.Publish(Event.ResourceChanged, Economy.Fervor, Fervor, Fervor + amount);
        Fervor += amount;
    }
}
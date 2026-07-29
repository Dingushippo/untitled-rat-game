using Godot;

public enum Economy
{
    Tithes,
    Fervor
}

public partial class EconomyService : Node
{
    private const float FERVOR_CYCLE_BOOST_MULTIPLIER = 0.8f;
    private const int FERVOR_BOOST_THRESHOLD = 75;
    public static EconomyService Instance { get; private set; }
    public float CycleTimeScale => Fervor >= FERVOR_BOOST_THRESHOLD ? FERVOR_CYCLE_BOOST_MULTIPLIER : 1f;
    public int Tithes
    {
        get => _tithes;
        private set => _tithes = value;
    }

    // TODO implement
    private float _currentFervorMultiplier = 1f;
    private int _tithes;
    public int Fervor
    {
        get => _fervor;
        private set => _fervor = Mathf.Clamp(value, 0, 100);
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
        EventBus.Subscribe(Event.ItemSold, OnItemSold);
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
            return;
        }
        Fervor -= _fervorDecayPerMinute;
        _fervorDecayTimer = 0;
    }

    public void OnItemSold(params object[] args)
    {
        string itemId = (string)args[0];
        int amount = (int)args[1];
        ItemDef item = ItemDatabase.Get(itemId);
        // TODO Possibly add global value stuff here, either positive or negative
        AddTithes(item.BaseValue * amount);

        GD.Print($"Sold x{amount} {item.DisplayName}, current tithes: {Tithes}"); // Temp print TODO remove
    }

    public void AddTithes(int amount)
    {
        int oldTithes = Tithes;
        Tithes += amount;
        // May need to introduce a cap here sometime
        EventBus.Publish(Event.ResourceChanged, Economy.Tithes, oldTithes, Tithes);
    }

    public void AddFervor(int amount)
    {
        int oldFervor = Fervor;
        Fervor += amount;
        if (oldFervor != Fervor)
            EventBus.Publish(Event.ResourceChanged, Economy.Fervor, oldFervor, Fervor);
    }

    public void ResetForRun()
    {
        Fervor = 0;
        Tithes = 0;
        _fervorDecayTimer = 0;
    }
}
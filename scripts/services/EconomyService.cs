using Godot;

public enum Economy
{
    Tithes,
    Fervor
}

public partial class EconomyService : Node
{
    private const float FERVOR_CYCLE_BOOST_MULTIPLIER = 1.25f;
    private const int FERVOR_BOOST_OFF = 70;
    private const int FERVOR_BOOST_ON = 75;
    private static EconomyService _instance;
    public static EconomyService Instance => _instance;

    private bool _boosted;
    public float ProductionRateScale
    {
        get
        {
            if (_boosted && Fervor < FERVOR_BOOST_OFF) _boosted = false;
            else if (!_boosted && Fervor >= FERVOR_BOOST_ON) _boosted = true;
            return _boosted ? FERVOR_CYCLE_BOOST_MULTIPLIER : 1f;
        }
    }

    private int _tithes;
    public int Tithes
    {
        get => _tithes;
        private set
        {
            int oldTithes = _tithes;
            _tithes = value;
            EventBus.Publish(Event.ResourceChanged, Economy.Tithes, oldTithes, Tithes);
        }
    }


    private int _fervor;
    public int Fervor
    {
        get => _fervor;
        private set
        {
            int oldFervor = _fervor;
            _fervor = Mathf.Clamp(value, 0, 100);
            if (oldFervor != _fervor)
                EventBus.Publish(Event.ResourceChanged, Economy.Fervor, oldFervor, _fervor);
        }
    }

    private int _fervorDecayPerMinute = 1;
    private float _fervorDecayTimer = 0;

    public override void _EnterTree()
    {
        if (!Singleton.ClaimOrFree(ref _instance, this)) return;
        // EventBus.Subscribe(Event.ItemSold, OnItemSold);
        EventBus.Subscribe<ItemSold>(OnItemSold);
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

    public void OnItemSold(ItemSold item)
    {

        ItemDef itemDef = ItemDatabase.Get(item.ItemId);

        // TODO Possibly add global value stuff here, either positive or negative
        Tithes += itemDef.BaseValue * item.Amount;
        Fervor += 2 * item.Amount;
    }

    public void ResetForRun()
    {
        Fervor = 0;
        Tithes = 0;
        _fervorDecayTimer = 0;
    }
}
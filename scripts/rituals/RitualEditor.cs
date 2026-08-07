using Godot;
using System;

public partial class RitualEditor : Control
{
    [Export] public RitualEditorViewport Viewport;
    [Export] public RitualRenderer Renderer;

    [ExportGroup("UI buttons")]
    [Export] public Button NewButton; 
    [Export] public Button SaveButton; 
    [Export] public Button AddCircleButton; 
    [Export] public Button UndoButton; 
    [Export] public Button RedoButton;

    [ExportGroup("UI labels")]
    [Export] public LineEdit IDLineEdit;
    [Export] public LineEdit NameLineEdit;
    [Export] public TextEdit DescriptionTextEdit;

    [ExportGroup("Item lists")]
    [Export] public ItemList CircleList;

    [ExportGroup("Circle editor")]
    [Export] public HSlider RadiusSlider;
    [Export] public SpinBox ElementSpinbox;
    [Export] public HSlider ElementRadiusSlider;
    [Export] public HSlider AngleOffsetSlider;


    private RitualResource _currentRitual;
    private RitualCircleResource _currentCircle;

    public override void _Ready()
    {
        // Toolbar buttons
        NewButton.Pressed += OnNewButtonClick;
        AddCircleButton.Pressed += OnAddCircleClick;
        
        CircleList.ItemClicked += OnCircleItemClicked;
        CircleList.Clear();


        // Circle changes
        RadiusSlider.ValueChanged += UpdateCurrentCircle;
        ElementSpinbox.ValueChanged += UpdateCurrentCircle;
        ElementRadiusSlider.ValueChanged += UpdateCurrentCircle;
        AngleOffsetSlider.ValueChanged += UpdateCurrentCircle;

        AngleOffsetSlider.MaxValue = Mathf.Tau;
        AngleOffsetSlider.Step = Mathf.Tau / 100;
    }

    private void OnCircleItemClicked(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        RitualCircleResource clicked = (RitualCircleResource)CircleList.GetItemMetadata((int)index);
        _currentCircle = clicked;
        SetCurrentUIElements();
    }

    private void OnNewButtonClick()
    {
        // TODO add check to make sure nothing is overwritten
        _currentRitual = new();
        Renderer.RitualResource = _currentRitual;
        IDLineEdit.Text = _currentRitual.Id;
        NameLineEdit.Text = _currentRitual.DisplayName;
        DescriptionTextEdit.Text = _currentRitual.Description;
    }

    private void OnAddCircleClick()
    {
        _currentCircle = new();
        _currentRitual.RitualCircles.Add(_currentCircle);
        int index = CircleList.AddItem("circle");
        CircleList.SetItemMetadata(index, _currentCircle);

        SetCurrentUIElements();
        Renderer.QueueRedraw();
    }



    private void SetCurrentUIElements()
    {
        if (_currentCircle is null) return;

        RadiusSlider.SetValueNoSignal(_currentCircle.Radius);
        ElementSpinbox.SetValueNoSignal(_currentCircle.NumElements);
        ElementRadiusSlider.SetValueNoSignal(_currentCircle.ElementRadius);
        AngleOffsetSlider.SetValueNoSignal(_currentCircle.AngleOffset);
    }

    private void UpdateCurrentCircle(double _)
    {
        if (_currentCircle is null) return;
        _currentCircle.Radius = (float)RadiusSlider.Value;
        _currentCircle.NumElements = (int)ElementSpinbox.Value;
        _currentCircle.ElementRadius = (float)ElementRadiusSlider.Value;
        _currentCircle.AngleOffset = (float)AngleOffsetSlider.Value; 
        Renderer.QueueRedraw();
    }
}

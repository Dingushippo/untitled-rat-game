using Godot;
using System;
using System.Data.Common;
using System.IO;

public partial class RitualEditor : Control
{
    private const string RITUAL_RESOURCE_FOLDER = "res://resources/rituals/";
    [Export] public RitualEditorViewport Viewport;
    [Export] public RitualRenderer Renderer;
    [Export] public FileDialog Dialog;

    [ExportGroup("UI buttons")]
    [Export] public Button OpenButton;
    [Export] public Button SaveButton; 
    [Export] public Button AddCircleButton; 
    [Export] public Button UndoButton; 
    [Export] public Button RedoButton;

    [ExportGroup("Ritual metadata edit fields")]
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

    [ExportGroup("Circle editor value labels")]
    [Export] public Label RadiusValueLabel;
    [Export] public Label ElementRadiusValueLabel;
    [Export] public Label AngleOffsetValueLabel;


    private RitualResource _currentRitual;
    private string _currentRitualPath;
    private RitualCircleResource _currentCircle;

    public override void _Ready()
    {
        // Toolbar buttons
        OpenButton.Pressed += OnOpenButtonClick;
        AddCircleButton.Pressed += OnAddCircleClick;
        SaveButton.Pressed += OnSaveButtonClick;
        
        // Circle list
        CircleList.ItemClicked += OnCircleItemClicked;
        CircleList.Clear();

        // Ritual metadata
        IDLineEdit.TextChanged += x => _currentRitual.Id = x;
        NameLineEdit.TextChanged += x => _currentRitual.DisplayName = x;
        DescriptionTextEdit.TextChanged += () => _currentRitual.DisplayName = DescriptionTextEdit.Text;

        // Circle changes
        RadiusSlider.ValueChanged += UpdateCurrentCircle;
        ElementSpinbox.ValueChanged += UpdateCurrentCircle;
        ElementRadiusSlider.ValueChanged += UpdateCurrentCircle;
        AngleOffsetSlider.ValueChanged += UpdateCurrentCircle;

        // File dialog
        Dialog.FileSelected += OnDialogFileSelected;
    }

    private void OnCircleItemClicked(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        RitualCircleResource clicked = (RitualCircleResource)CircleList.GetItemMetadata((int)index);
        _currentCircle = clicked;
        SetCurrentUIElements();
    }

    private void OnOpenButtonClick()
    {
        Dialog.Popup();
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

    private void OnSaveButtonClick()
    {
        if (_currentRitual == null) {
            OS.Alert("No ritual resource active");
            return;
        };

        ResourceSaver.Save(_currentRitual, _currentRitualPath);  
    }

    private void OnDialogFileSelected(string filePath)
    {
        _currentRitualPath = filePath;

        if (ResourceLoader.Exists(filePath))
        {
            _currentRitual = ResourceLoader.Load<RitualResource>(filePath);
        } 
        else
        {
            string id = filePath.Split("/")[^1].TrimSuffix(".tres");
            _currentRitual = new()
            {
              Id = id  
            };
            ResourceSaver.Save(_currentRitual, filePath);  
        }

        Renderer.RitualResource = _currentRitual;

        IDLineEdit.Editable = true;
        NameLineEdit.Editable = true;
        DescriptionTextEdit.Editable = true;

        IDLineEdit.Text = _currentRitual.Id;
        NameLineEdit.Text = _currentRitual.DisplayName;
        DescriptionTextEdit.Text = _currentRitual.Description;

        Renderer.QueueRedraw();
    }


    private void SetCurrentUIElements()
    {
        if (_currentCircle is null) return;

        RadiusSlider.SetValueNoSignal(_currentCircle.Radius);
        ElementSpinbox.SetValueNoSignal(_currentCircle.NumElements);
        ElementRadiusSlider.SetValueNoSignal(_currentCircle.ElementRadius);
        AngleOffsetSlider.SetValueNoSignal(_currentCircle.AngleOffset);
        UpdateCircleValueLabels();
    }

    private void UpdateCircleValueLabels()
    {
        RadiusValueLabel.Text = RadiusSlider.Value.ToString();
        ElementRadiusValueLabel.Text = ElementRadiusSlider.Value.ToString();
        AngleOffsetValueLabel.Text = AngleOffsetSlider.Value.ToString();
    }

    private void UpdateCurrentCircle(double _)
    {
        if (_currentCircle is null) return;
        _currentCircle.Radius = (float)RadiusSlider.Value;
        _currentCircle.NumElements = (int)ElementSpinbox.Value;
        _currentCircle.ElementRadius = (float)ElementRadiusSlider.Value;
        _currentCircle.AngleOffset = (float)Mathf.DegToRad(AngleOffsetSlider.Value);
        UpdateCircleValueLabels();
        Renderer.QueueRedraw();
    }
}

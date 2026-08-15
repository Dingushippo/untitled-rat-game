using Godot;

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
    [Export] public HSlider SymbolScaleSlider;
    [Export] public HSlider SymbolRotationSlider;

    [ExportGroup("Circle editor value labels")]
    [Export] public Label RadiusValueLabel;
    [Export] public Label ElementRadiusValueLabel;
    [Export] public Label AngleOffsetValueLabel;
    [Export] public Label SymbolScaleValueLabel;
    [Export] public Label SymbolRotationValueLabel;


    private RitualResource _currentRitual;
    private string _currentRitualPath;
    private RitualCircleResource _currentCircle => _builder.Circle;
    private RitualCircleBuilder _builder;

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
        SymbolScaleSlider.ValueChanged += UpdateCurrentCircle;
        SymbolRotationSlider.ValueChanged += UpdateCurrentCircle;

        // File dialog
        Dialog.FileSelected += OnDialogFileSelected;

        // Create builder
        _builder = new();
    }

    private void OnCircleItemClicked(long index, Vector2 atPosition, long mouseButtonIndex)
    {
        RitualCircleResource clicked = (RitualCircleResource)CircleList.GetItemMetadata((int)index);
        _builder.SetCircle(clicked);
        SetCurrentUIElements();
    }

    private void OnOpenButtonClick()
    {
        Dialog.Popup();
    }

    private void OnAddCircleClick()
    {
        _builder.SetCircle(new());
        _currentRitual.RitualCircles.Add(_currentCircle);
        OnRitualChanged();
    }

    private void OnRitualChanged()
    {
        GD.Print($"Changing ritual: {_currentRitual.RitualCircles.Count}");
        if (_currentRitual.RitualCircles.Count == 0)
        {
            return;
        }
        CircleList.Clear();
        for (int i = 0; i < _currentRitual.RitualCircles.Count; i++)
        {
            int index = CircleList.AddItem($"Circle {i + 1}");
            CircleList.SetItemMetadata(index, _currentCircle);
        }
        SetCurrentUIElements();
        Renderer.QueueRedraw();
    }

    private void OnSaveButtonClick()
    {
        if (_currentRitual == null)
        {
            OS.Alert("No ritual resource active");
            return;
        }
        ;

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

        if (_currentRitual.RitualCircles.Count > 0)
        {
            _builder.SetCircle(_currentRitual.RitualCircles[0]);
        }

        OnRitualChanged();
    }


    private void SetCurrentUIElements()
    {
        if (_currentCircle is null) return;

        RadiusSlider.SetValueNoSignal(_currentCircle.Radius);
        ElementSpinbox.SetValueNoSignal(_currentCircle.NumElements);
        ElementRadiusSlider.SetValueNoSignal(_currentCircle.ElementRadius);
        AngleOffsetSlider.SetValueNoSignal(_currentCircle.AngleOffset);
        SymbolScaleSlider.SetValueNoSignal(_currentCircle.SymbolScale);
        SymbolRotationSlider.SetValueNoSignal(_currentCircle.SymbolRotation);
        UpdateCircleValueLabels();
    }

    private void UpdateCircleValueLabels()
    {
        RadiusValueLabel.Text = RadiusSlider.Value.ToString();
        ElementRadiusValueLabel.Text = ElementRadiusSlider.Value.ToString();
        AngleOffsetValueLabel.Text = AngleOffsetSlider.Value.ToString();
        SymbolScaleValueLabel.Text = SymbolScaleSlider.Value.ToString();
        SymbolRotationValueLabel.Text = SymbolRotationSlider.Value.ToString();
    }

    private void UpdateCurrentCircle(double _)
    {
        if (_currentCircle is null) return;
        _currentCircle.Radius = (float)RadiusSlider.Value;
        _currentCircle.ElementRadius = (float)ElementRadiusSlider.Value;
        _currentCircle.AngleOffset = (float)Mathf.DegToRad(AngleOffsetSlider.Value);
        _currentCircle.SymbolScale = (float)SymbolScaleSlider.Value;
        _currentCircle.SymbolRotation = (float)Mathf.DegToRad(SymbolRotationSlider.Value);
        _builder.RepositionElements();
        UpdateCircleValueLabels();
        Renderer.QueueRedraw();
    }
}
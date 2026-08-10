using Godot;
using System;

public partial class Result : Control
{
    [Export] public Label RunSuccess;
    [Export] public RichTextLabel RunStats;
    [Export] public Button Restart;
    [Export] public Button MainMenu;
}

using Godot;
using System;

[GlobalClass]
public partial class ReadyLogger : Godot.Node
{
    public override void _Ready()
    {
        GD.Print(this.Name);
    }
}

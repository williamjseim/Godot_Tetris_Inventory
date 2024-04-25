using Godot;
using System;

public partial class ContainerManager : Panel
{
    public override void _Ready()
    {
        base._Ready();
        this.GuiInput += Input;
    }
    public static event Action<InputEventMouseMotion> MouseMotion;

    public virtual void Input(InputEvent @event){
        if(@event is InputEventMouseMotion motion){
            MouseMotion?.Invoke(motion);
        }
    }
}

using Godot;
using System;

public partial class ContainerManager : Panel
{
    public override void _Ready()
    {
        base._Ready();
        this.GuiInput += ContainerInput;
    }
    public static event Action<InputEventMouseMotion> MouseMotion;

    public virtual void ContainerInput(InputEvent @event){
        if(@event is InputEventMouseMotion motion){
            this.MouseMotionInvoke(motion);
        }
    }

    protected virtual void MouseMotionInvoke(InputEventMouseMotion motion){
        MouseMotion?.Invoke(motion);
    }
}

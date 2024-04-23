using Godot;

public class ContainerItem : BaseItem{
    public Vector2I InternalStorageSize { get; protected set; } = Vector2I.One;
}
using Godot;

public class ContainerModifier : ItemModifier{
    public ContainerModifier(Vector2I containerSize) : base(){
        this.ContainerSize = containerSize;
        grid = new ItemSlot[containerSize.X, containerSize.Y];
    }

    public Vector2I ContainerSize { get; protected set; }
    public ItemSlot[,] grid { get; set; }

}
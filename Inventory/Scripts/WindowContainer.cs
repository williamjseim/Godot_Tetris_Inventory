using Godot;

public partial class WindowContainer : SlotContainer{
    [Export] protected BaseWindow window;
    public override bool ItemFits(Vector2I truePos, ItemSlot itemslot)
    {
        if(itemslot == window.Itemslot){
            return false;
        }
        return base.ItemFits(truePos, itemslot);
    }
}
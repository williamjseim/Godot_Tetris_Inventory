using Godot;

public partial class WindowContainer : SlotContainer{
    [Export] protected ContainerWindow window;
    public override bool ItemFits(Vector2I truePos, ItemSlot itemslot)
    {
        if(itemslot == window.Itemslot || window.BlackListed(itemslot.ItemHolder.Item.GetType().Name) || !window.whitelisted(itemslot.ItemHolder.Item.GetType().Name)){
            return false;
        }
        return base.ItemFits(truePos, itemslot);
    }
}
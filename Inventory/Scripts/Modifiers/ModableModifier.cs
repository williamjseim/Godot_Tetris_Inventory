using System;

public class ModableModifier : ItemModifier, ICloneable{
    public ModableModifier(){}
    public ModableModifier(ModableModifier modableModifier){
        this.AttachableItemsType = modableModifier.AttachableItemsType.Clone() as string[];
        this.AttachedItems = modableModifier.AttachedItems.Clone() as ItemData[];
    }

    public string[] AttachableItemsType {get; protected set;}
    public ItemData[] AttachedItems { get; set; }

    public object Clone()
    {
        return new ModableModifier(this);
    }
}
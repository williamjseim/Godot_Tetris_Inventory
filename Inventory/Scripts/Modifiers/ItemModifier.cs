public abstract class ItemModifier{
    public ItemModifier()
    {
        
    }

    public abstract class ModifierSaveData{
        public ModifierSaveData(ItemModifier modifier){
            
        }

        public abstract ItemModifier CreateInstance();
    }
}
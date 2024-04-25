using Godot;

public partial class Highlight : Panel{
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();
    public Highlight()
    {
        this.AddThemeStyleboxOverride("panel", styleBoxFlat);
        CustomMinimumSize = new Vector2(InventoryManager.SlotSize, InventoryManager.SlotSize);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    public void SetColor(Color? color, bool transperent = true){

        if(transperent){
            Color transparent = color != null ? (Color)color : Colors.Green;
            transparent.A = 0.3f;
            this.styleBoxFlat.BgColor = transparent;
        }
        else{
            this.styleBoxFlat.BgColor = color != null ? (Color)color : Colors.Green;
        }
    }
}
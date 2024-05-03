using Godot;

public partial class Highlight : Panel{
    StyleBoxFlat styleBoxFlat = new StyleBoxFlat();

    private bool _fits;
    public bool Fits
    {
        get { return _fits; }
        set { 
            if(value != _fits){
                _fits = value; 
                this.SetColor(_fits ? Colors.Green : Colors.Red);
            }
        }
    }
    
    public Highlight()
    {
        this.AddThemeStyleboxOverride("panel", styleBoxFlat);
        CustomMinimumSize = new Vector2(InventoryManager.slotSize, InventoryManager.slotSize);
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
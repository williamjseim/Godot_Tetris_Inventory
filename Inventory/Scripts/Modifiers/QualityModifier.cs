using Godot;

public enum Quality{
    Normal,
    Silver,
    Gold,
    Diamond,
}
public class QualityModifier : ItemModifier{
    public static Texture2D SilverQuality { get; protected set;}
    public static Texture2D GoldQuality { get; protected set;}
    public static Texture2D DiamondQuality { get; protected set;}

    public QualityModifier(Quality quality) : base()
    {
        this.Quality = quality;
    }
    private Quality _quality;
    public Quality Quality
    {
        get { return _quality; }
        protected set { _quality = value; }
    }

    public Texture QualityTexture { get{
        switch(Quality){
            case Quality.Silver:
            return SilverQuality;
            case Quality.Gold:
            return GoldQuality;
            case Quality.Diamond:
            return DiamondQuality;
            default:
            return null;
        }
    }}
}
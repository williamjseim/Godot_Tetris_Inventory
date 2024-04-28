public enum Quality{
    Normal,
    Silver,
    Gold,
    Diamond,
}
public class QualityModifier : ItemModifier{
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
}
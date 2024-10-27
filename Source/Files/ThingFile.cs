public class ItemFile
{
    public string DefName;

    public string MaterialDefName;

    public int Quantity;

    public int Quality;

    public int Hitpoints;

    public bool IsMinified;

    public float[] Color = new float[4];

    public TransformComponent TransformComponent = new TransformComponent();

    public PlantComponent PlantComponent = new PlantComponent();
}
public class AnimalFile
{
    public string DefName;

    public string Name;

    public string BiologicalAge;

    public string ChronologicalAge;

    public string Gender;

    public string FactionDef;

    public string KindDef;

    public HediffComponent[] Hediffs = new HediffComponent[0];

    public TrainableComponent[] Trainables = new TrainableComponent[0];

    public TransformComponent Transform = new TransformComponent();
}
public class Torch : Item
{
    public float drainPerSecond;

    public Torch(float drainPerSecond, ItemTemplate torchTemplate)
    {
        this.drainPerSecond = drainPerSecond;
        template = torchTemplate;
    }
}

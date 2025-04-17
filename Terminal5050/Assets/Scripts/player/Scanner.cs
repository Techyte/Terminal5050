public class Scanner : Item
{
    public float powerUsageCost;

    public Scanner(float powerUsageCost, ItemTemplate scannerTemplate)
    {
        this.powerUsageCost = powerUsageCost;
        template = scannerTemplate;
    }
}

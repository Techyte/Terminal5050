using UnityEngine;

[CreateAssetMenu]
public class ItemTemplate : ScriptableObject
{
    public string name;
    public Type type;
    public GameObject model;
    public Sprite sprite;
    public Mesh mesh;
    public bool canBeSold;
    public int sellPrice;
}

public enum Type
{
    Small,
    Large
}
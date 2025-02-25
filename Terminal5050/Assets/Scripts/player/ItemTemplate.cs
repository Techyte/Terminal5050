using UnityEngine;

[CreateAssetMenu]
public class ItemTemplate : ScriptableObject
{
    public string name;
    public Type type;
    public GameObject model;
    public Sprite sprite;
}

public enum Type
{
    Small,
    Large
}
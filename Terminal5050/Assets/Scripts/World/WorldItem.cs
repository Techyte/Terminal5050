using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldItem : Interactable
{
    public static List<WorldItem> worlditems = new List<WorldItem>();
    
    private static System.Random random = new System.Random();

    [SerializeField] private ItemTemplate defaultItem;
    [SerializeField] private MeshFilter filter;
    public MeshRenderer mRenderer;
    [SerializeField] private MeshCollider mCollider;
    [SerializeField] private Rigidbody rb;

    public string id;

    public Item Item => _item;
    
    private Item _item;

    private void Start()
    {
        if (_item == null)
        {
            Init(new Item(defaultItem));
        }

        hoverable = true;
    }

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(id))
        {
            id = GenerateId();
        }
    }

    public void Init(Item item)
    {
        _item = item;
        SetModel();

        if (string.IsNullOrEmpty(id))
        {
            id = GenerateId();
        }
        
        worlditems.Add(this);
    }

    private string GenerateId()
    {
        string generatedId = RandomString(5);

        while (IdExists(generatedId))
        {
            generatedId = RandomString(5);
        }

        return generatedId;
    }
    
    private static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    private bool IdExists(string id)
    {
        foreach (var worldItem in worlditems)
        {
            if (worldItem.id == id)
            {
                return true;
            }
        }

        return false;
    }

    private void SetModel()
    {
        filter.mesh = _item.template.mesh;
        mCollider.sharedMesh = _item.template.mesh;
        mCollider.convex = true;

        transform.localScale = _item.template.model.transform.localScale;

        mRenderer.sharedMaterial = _item.template.model.GetComponent<MeshRenderer>().sharedMaterial;

        mCollider.isTrigger = false;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
        
        mCollider.isTrigger = true;

        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    public static WorldItem GetWorldItemFromId(string id)
    {
        foreach (var worldItem in worlditems)
        {
            if (worldItem.id == id)
            {
                return worldItem;
            }
        }

        return null;
    }

    public void PickedUp()
    {
        worlditems.Remove(this);
        Destroy(gameObject);
    }

    public override void Interact(Player player)
    {
        Inventory inventory = player.inventory;

        inventory.ItemWantToBePickedUp(this);
    }

    public override string GetHoverText(Player player)
    {
        return _item.template.name;
    }

    public override Vector3 GetHoverBounds()
    {
        return mRenderer.bounds.center;
    }
}

using UnityEngine;

public class WorldItem : Interactable
{
    [SerializeField] private ItemTemplate defaultItem;
    [SerializeField] private LayerMask interactableLayer;
    
    private GameObject model;
    private Item _item;

    private void Start()
    {
        Init(new Item(defaultItem));
    }

    public void Init(Item item)
    {
        _item = item;
        SetModel();
    }

    private void SetModel()
    {
        model = Instantiate(_item.template.model, transform);
        
        model.transform.localPosition = Vector3.zero;
        model.layer = interactableLayer;
        
        MeshCollider modelCollider = model.AddComponent<MeshCollider>();
        modelCollider.sharedMesh = _item.template.mesh;
        modelCollider.convex = true;
        
        model.AddComponent<Rigidbody>();
        
        model.AddComponent<ItemModel>();
    }

    public void DestroyRigidbody()
    {
        Destroy(model.GetComponent<Rigidbody>());
    }

    public override void Interact(PersonalPowerManager pManager)
    {
        Inventory inventory = pManager.GetComponent<Inventory>();
        if (inventory.TryGainItem(_item))
        {
            ActionBar.Instance.NewOutput("Not enough space to pick up item", Color.red);
        }
    }
}

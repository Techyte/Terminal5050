using UnityEngine;

public class WorldItem : Interactable
{
    [SerializeField] private ItemTemplate defaultItem;
    [SerializeField] private MeshFilter filter;
    [SerializeField] private MeshRenderer mRenderer;
    [SerializeField] private MeshCollider mCollider;
    [SerializeField] private Rigidbody rb;
    
    private Item _item;

    private void Start()
    {
        if (_item == null)
        {
            Init(new Item(defaultItem));
        }
    }

    public void Init(Item item)
    {
        _item = item;
        SetModel();
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

    public override void Interact(PersonalPowerManager pManager)
    {
        Inventory inventory = pManager.GetComponent<Inventory>();

        bool local = pManager.GetComponent<Player>().local;
        
        if (!inventory.TryGainItem(_item))
        {
            if (local)
                ActionBar.Instance.NewOutput("Not enough space to pick up item", Color.red);
        }
        else
        {
            if (local)
                ActionBar.Instance.NewOutput($"+1 {_item.template.name}");
            Destroy(gameObject);
        }
    }
}

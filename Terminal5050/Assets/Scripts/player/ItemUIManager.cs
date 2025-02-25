using System;
using UnityEngine;
using UnityEngine.UI;

public class ItemUIManager : MonoBehaviour
{
    [SerializeField] private Image largeSprite;
    [SerializeField] private Image[] smallSprites;
    
    private Inventory _inventory;

    private void Awake()
    {
        _inventory = GetComponent<Inventory>();
    }
    
    private void Update()
    {
        for (int i = 0; i < _inventory.smallItems.Length; i++)
        {
            if (_inventory.smallItems[i] != null)
            {
                smallSprites[i].color = new Color(255, 255, 255, 1);
                smallSprites[i].sprite = _inventory.smallItems[i].template.sprite;
            }
            else
            {
                smallSprites[i].color = new Color(255, 255, 255, 0);
            }
        }
        
        if (_inventory.largeItem != null)
        {
            largeSprite.color = new Color(255, 255, 255, 1);
            largeSprite.sprite = _inventory.largeItem.template.sprite;
        }
        else
        {
            largeSprite.color = new Color(255, 255, 255, 0);
        }
    }
}

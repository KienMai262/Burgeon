using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIInventoryPage : MonoBehaviour
{
    [SerializeField] protected UIInventoryItem itemPrefab;
    [SerializeField] protected RectTransform content;

     List<UIInventoryItem> items = new List<UIInventoryItem>();

     public void InitializeInventoryUI(int inventorySize)
    {
        for (int i = 0; i < inventorySize; i++)
        {
            UIInventoryItem item = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(content, false);
            items.Add(item);
            // item.OnItemClicked += HandleItemSelection;
            // item.OnItemBeginDrag += HandleBeginDrag;
            // item.OnItemDroppedOn += HandleSwap;
            // item.OnItemEndDrag += HandleEndDrag;
            // item.OnRightMouseBtnClicked += HandleShowItemActions;
        }
        
    }
    public void Show()
    {
        // if (gameObject.tag == "Inventory")
        {
            gameObject.SetActive(true);
        }
        // if (itemDescription != null)
        // {
        //     itemDescription.ResetDescription();
        // }

        // ResetSelection();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        // ResetDraggtedItem();
        // itemActionPanel.Toggle(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] protected UIInventoryPage inventoryUI;

    public int inventorySize = 20;

    protected void Start()
    {
        inventoryUI.InitializeInventoryUI(inventorySize);
    }
    protected void HandleUpdate()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleInventory();
        }
    }
    public void ToggleInventory()
    {
        if (inventoryUI.isActiveAndEnabled)
        {
            inventoryUI.Hide();
        }
        else
        {
            inventoryUI.Show();
        }
    }
}

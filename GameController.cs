using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Idle,
    Inventory
};
public class GameController : MonoBehaviour
{
    PlayerState playerState = PlayerState.Idle;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject Inventory;
    [SerializeField] GameObject Seed;
    [SerializeField] GameObject Shop;
    private void Start() {
        //Show Inventory
        InventoryUI.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        InventoryUI.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

        //Show Seed
        SeedUI.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        SeedUI.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

        //Show Animal Food
        SelectUI.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        SelectUI.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };
        
        //Show Order
        OrderManager.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        OrderManager.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

        //Show Shop
        ShopUI.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        ShopUI.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

        //Show Menu
        MenuController.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        MenuController.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

        //Show Active
        ActiveUI.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        ActiveUI.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

        //Show State
        StateManager.Instance.OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        StateManager.Instance.OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

        //Show name to player
        Player.GetComponent<Player>().OnShowInventory += () => {
            playerState = PlayerState.Inventory;
        };
        Player.GetComponent<Player>().OnCloseInventory += () => {
            playerState = PlayerState.Idle;
        };

    }
    private void Update() {
        if(playerState == PlayerState.Idle)
        {
            Player.GetComponent<MoveController>().HandleUpdate();
            Inventory.GetComponent<InventoryUI>().HandleUpdate();
            Seed.GetComponent<SeedUI>().HandleUpdate();
            Shop.GetComponent<ShopUI>().HandleUpdate();
        }
    }
}

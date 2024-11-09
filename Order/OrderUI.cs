using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderUI : MonoBehaviour
{
    [SerializeField] protected GameObject orderPanel;
    private void Start() {
        orderPanel.SetActive(false);
    }
}

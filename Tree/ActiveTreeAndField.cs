using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTreeAndField : MonoBehaviour
{
    [SerializeField] protected GameObject player;
    [SerializeField] protected GameObject fieldAndTree;
    [SerializeField] protected GameObject note;
    [SerializeField] protected bool isActive = false;
    private void Start() {
        note.SetActive(false);
    }
    private void Update() {
        if(Vector2.Distance(player.transform.position, transform.position) < .5f) {
            note.SetActive(true);
            if(Input.GetKeyDown(KeyCode.F)) {
                isActive = true;
            }
        } else {
            note.SetActive(false);
        }

        if(isActive && fieldAndTree.activeSelf) {
            fieldAndTree.SetActive(true);
        }
    }
}

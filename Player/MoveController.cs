using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveController : MonoBehaviour
{
    [SerializeField] protected float speed = 1f;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected Vector2 input;
    public bool isMoving;
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void HandleUpdate()
    {
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        if (isMoving)
        {
            if (input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);
                Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
                if (input.x != 0 && input.y != 0)
                {
                    rb.velocity = new Vector2(move.x * speed / Mathf.Sqrt(2f), move.y * speed / Mathf.Sqrt(2f));
                }
                else
                {
                    rb.velocity = new Vector2(move.x * speed, move.y * speed);
                }
                isMoving = true;
                animator.SetBool("isMoving", true);
            }
            if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            {
                animator.SetBool("isMoving", false);
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIMove : MonoBehaviour
{
    [SerializeField] protected GameObject checkLocation;
    [SerializeField] protected float speed = 1;
    [SerializeField] protected float distance = 1;

    protected List<Vector2> directionList = new List<Vector2> { new Vector2(0, 1), new Vector2(1, 0), new Vector2(0, -1), new Vector2(-1, 0),
                                                            new Vector2(1, 1), new Vector2(1, -1), new Vector2(-1, -1), new Vector2(-1, 1) };

    protected Vector2 direction;
    protected Rigidbody2D rb;
    bool isMove = false;
    Animator anim;
    bool checkRandom = false;
    bool checkMove = true;


    private void Start()
    {
        direction = directionList[Random.Range(0, directionList.Count)];
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    protected void CanUseUpdate()
    {
        if (checkMove)
        {
            checkMove = false;
            Invoke("CheckMove", Random.Range(4, 10));
        }
        if (isMove)
        {
            rb.velocity = direction * Random.Range(500, speed * 1001) / 1000f;
            anim.SetBool("Eat", false);
            anim.SetBool("Happy", false);
            anim.SetBool("Run", true);
            anim.SetFloat("Move", direction.x / Mathf.Abs(direction.x));
            if (Vector2.Distance(checkLocation.transform.position, transform.position) < distance)
            {
                if (!checkRandom)
                {
                    Invoke("RandomDirection", Random.Range(3, 5));
                    checkRandom = true;
                }
            }
            else
            {
                direction = (checkLocation.transform.position - transform.position).normalized;
            }
        }
        else
        {
            anim.SetBool("Run", false);
            if (Random.Range(0, 2) == 0)
            {
                anim.SetBool("Eat", true);
            }
            else if(Random.Range(0, 2) == 1)
            {
                anim.SetBool("Happy", true);
            }
        }
    }
    protected void RandomDirection()
    {
        direction = directionList[Random.Range(0, directionList.Count)];
        checkRandom = false;
    }
    protected void CheckMove()
    {
        isMove = Random.Range(0, 2) == 0;
        checkMove = true;
    }
}

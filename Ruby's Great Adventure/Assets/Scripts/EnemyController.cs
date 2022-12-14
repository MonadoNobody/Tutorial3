using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 3.0f;
    public bool vertical;
    public float changeTime = 3.0f;
    Rigidbody2D rigidbody2d;
    float timer;
    int direction = 1;
    Animator animator;
    bool broken = true;
    public ParticleSystem smokeEffect;
    public AudioClip hitClip;
    public int damage = 1;
    private RubyController rubyController;
    public int robotHealth;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        timer = changeTime;
        animator = GetComponent<Animator>();   
        GameObject rubyControllerObject = GameObject.FindWithTag("RubyController");
         if (rubyControllerObject != null)

        {

            rubyController = rubyControllerObject.GetComponent<RubyController>(); //and this line of code finds the rubyController and then stores it in a variable
        }
    }
    
    void Update()
    {
        if(!broken)
        {
            return;
        }
        timer -= Time.deltaTime;

        if (timer < 0)
        {
            direction = -direction;
            timer = changeTime;
        }

    }
    
    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        if(!broken)
        {
            return;
        }
        if (vertical)
        {
        animator.SetFloat("Move X", 0);
        animator.SetFloat("Move Y", direction);
        position.y = position.y + Time.deltaTime * speed * direction;
        }
        else 
        {
            animator.SetFloat("Move X", direction);
            animator.SetFloat("Move Y", 0);
            position.x = position.x + Time.deltaTime * speed * direction;
        }
        rigidbody2d.MovePosition(position);

    }

    // Update is called once per frame
    
    void OnCollisionEnter2D(Collision2D other)
    {
        RubyController player = other.gameObject.GetComponent<RubyController>();
        if (player != null)
        {
            player.ChangeHealth(damage * -1);
            player.PlaySound(hitClip);

        }
    }
    public void Fix()
    {
        robotHealth = robotHealth - 1;
        if(robotHealth == 0){
        broken = false;
        rubyController.scoreUP();
        rigidbody2d.simulated = false;
        animator.SetTrigger("Fixed");
        
        smokeEffect.Stop();
        }
    }

}

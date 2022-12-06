using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class RubyController : MonoBehaviour
{

    public int maxHealth = 5;
    public int health { get { return currentHealth;}}
    int currentHealth;
    public float speed = 3.0f;
    bool isInvincible;
    float invincibleTimer;
    public float timeInvincible = 2.0f;
    Animator animator;
    Vector2 lookDirection = new Vector2(1,0);
    public GameObject projectilePrefab;
    AudioSource audioSource;
    public AudioClip throwClip;
    public GameObject healthEffect;
    public GameObject hitEffect;
    int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winLoseText;
    bool gameOver;
    public AudioClip victoryMusic;
    public AudioClip gameOverMusic;
    public GameObject winLoseObject;
    public AudioSource backgroundMusic;
    bool stageOneVictory;
    public int level;
    public int cogs = 4;
    public TextMeshProUGUI cogCounter;
    bool stageTwoVictory;
    bool stageThreeVictory;
    public bool isBoosted;
    public float boostTime = 5.0f;
    float boostedTimer;
    public int boostEffect = 2;
    bool boosted;
    public AudioClip ammoSFX;
    

    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rigidbody2d = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        winLoseObject.SetActive(false);
        
    }
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void scoreUP()
    {
        score += 1;
        scoreText.text = "Fixed Robots" + score.ToString() + "/ 4";
        if (score == 4 && level == 1)
        {
            stageOneVictory = true;
            winLoseText.text = "Talk to Jambi to progress to the next level";
            winLoseObject.SetActive(true);
        }
        if (score == 4 && level == 2)
        {
            winLoseText.text = "Talk to the dog to proceed to the boss level";
            winLoseObject.SetActive(true);
            stageTwoVictory = true;
        }
        if(score == 4 && level == 3)
        {
            backgroundMusic.Stop();
            PlaySound(victoryMusic);
            winLoseText.text = "YOU WIN!!! A Game by Alexander Robinson, Press R to Restart.";
            winLoseObject.SetActive(true);
            stageThreeVictory = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.R))
            {
                if (gameOver == true)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

                }
                if (stageThreeVictory == true)
                {
                    SceneManager.LoadScene("MainScene");
                    stageThreeVictory = false;
                }
            }

        Vector2 move = new Vector2(horizontal, vertical);

        if(!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            lookDirection.Set(move.x, move.y);
            lookDirection.Normalize();
        }
        animator.SetFloat("Look X", lookDirection.x);
        animator.SetFloat("Look Y", lookDirection.y);
        animator.SetFloat("Speed", move.magnitude);
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if(invincibleTimer < 0)
            {
                isInvincible = false;
            }
        }
        if (isBoosted)
        {
            boostedTimer -= Time.deltaTime;
            if(boostedTimer <0)
            {
                if (boosted == true)
                {
                    speed = speed / boostEffect;
                    boosted = false;
                }
                isBoosted = false;
            }
        }
        if(Input.GetKeyDown(KeyCode.C))
        {
            if (cogs > 0)
            {
            Launch();
            PlaySound(throwClip);
            cogs -= 1;
            cogCounter.text = "Cogs: " + cogs.ToString();
            }
            else 
            {
                return;
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f, lookDirection, 1.5f, LayerMask.GetMask("NPC"));
            if (hit.collider != null)
            {
                NonPlayerCharacter character = hit.collider.GetComponent<NonPlayerCharacter>();
                if (character != null)
                {
                    if (stageOneVictory == true)
                    {
                        //put Ruby at 46.36, -8.36
                        transform.position = new Vector2(46.36f, -8.36f);
                        SceneManager.LoadScene("Level2");
                        level = 2;
                        stageOneVictory = false;
                    }
                    if (stageTwoVictory == true)
                    {
                        SceneManager.LoadScene("BossLevel");
                        level = 3;
                        stageTwoVictory = false;
                    }
                    character.DisplayDialog();
                }
            }
        }
    }

    void FixedUpdate()
    {
        Vector2 position = rigidbody2d.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }

    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            if(isInvincible)
            {
                return;
            }
            isInvincible = true;
            invincibleTimer = timeInvincible;
            GameObject hitVFX = Instantiate(hitEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        if (amount > 0)
        {
            GameObject healVFX = Instantiate(healthEffect, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
        if (currentHealth <= 0)
        {
            backgroundMusic.Stop();
            winLoseObject.SetActive(true);
            gameOver = true;
            speed = 0;
            PlaySound(gameOverMusic);
        }
        
    }
    void Launch()
    {
        GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(lookDirection, 360);
        animator.SetTrigger("Launch");
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Ammo"))
        {
            cogs += 4;
            cogCounter.text = "Cogs: " + cogs.ToString();
            other.gameObject.SetActive(false);
            PlaySound(ammoSFX);
        }
    }
    public void SpeedPickup()
    {
                    if(isBoosted)
            {
                return;
            }
                    if(isBoosted == false)
                    {
                        speed = speed * boostEffect;
                        boosted =true;
                    }
            isBoosted = true;
            boostedTimer = boostTime;
    }
}

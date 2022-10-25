using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerScript : MonoBehaviour
{
    private Rigidbody2D rd2d;
    public Animator animator;
    private bool facingRight = true;
    private bool isOnGround;
    public Transform groundcheck;
    public float checkRadius;
    public LayerMask allGround;

    public float speed;
    public float jumpForce = 3;

    public Text score;
    public Text lives;
    public GameObject winText;
    public GameObject loseText;

    private int scoreValue = 0;
    private int livesTotal = 3;

    public class BoolEvent : UnityEvent<bool> {}

    // Start is called before the first frame update
    void Start()
    {
        rd2d = GetComponent<Rigidbody2D>();
        
        score.text = "Score: " + scoreValue.ToString();
        lives.text = "Lives: " + livesTotal.ToString();

        winText.SetActive(false);
        loseText.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float hozMovement = Input.GetAxis("Horizontal");
        float vertMovement = Input.GetAxis("Vertical");
        rd2d.AddForce(new Vector2(hozMovement * speed, vertMovement * speed));
        isOnGround = Physics2D.OverlapCircle(groundcheck.position, checkRadius, allGround);
    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
       if (collision.collider.tag == "Coin")
        {
            scoreValue += 1;
            score.text = "Score: " + scoreValue.ToString();
            Destroy(collision.collider.gameObject);
            FindObjectOfType<AudioManager>().Play("Pickup");
        }
        else if (collision.collider.tag == "Enemy")
        {
            livesTotal -= 1;
            lives.text = "Lives: " + livesTotal.ToString();
            Destroy(collision.collider.gameObject);
            FindObjectOfType<AudioManager>().Play("Damage");
        }

        if ((scoreValue == 4) && (collision.collider.tag == "Coin") && (livesTotal < 4))
        {
            transform.position = new Vector3(-31.5f,0f,0f);
            livesTotal = 3;
            lives.text = "Lives: " + livesTotal.ToString();
            FindObjectOfType<AudioManager>().Play("Move");
        }

        if ((scoreValue == 8) && (collision.collider.tag == "Coin"))
        {
            winText.SetActive(true);
            gameObject.SetActive(false);

            FindObjectOfType<AudioManager>().Stop("Theme");
            FindObjectOfType<AudioManager>().Play("Win");
        }

        if ((livesTotal == 0) && (collision.collider.tag == "Enemy"))
        {
            loseText.SetActive(true);
            gameObject.SetActive(false);

            FindObjectOfType<AudioManager>().Stop("Theme");
            FindObjectOfType<AudioManager>().Play("PlayerDeath");
        }

    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && isOnGround)
        {
            if (Input.GetKey(KeyCode.W))
            {
                rd2d.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
                animator.SetBool("IsJumping", true);

                FindObjectOfType<AudioManager>().Play("Jump");
            }
            else if (isOnGround)
            {
                animator.SetBool("IsJumping", false);
            }
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector2 Scaler = transform.localScale;
        Scaler.x = Scaler.x * -1;
        transform.localScale = Scaler;
    }

    void Update()
    {
        float hozMovement = Input.GetAxis("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(hozMovement));

        if (facingRight == false && hozMovement > 0)
        {
            Flip();
        }
        else if (facingRight == true && hozMovement < 0)
        {
            Flip();
        }

        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }
}
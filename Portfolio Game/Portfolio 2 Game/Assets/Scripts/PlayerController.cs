using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour, IDamage
{

    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [Range(1, 10)][SerializeField] int HP;
    [Range(2, 5)][SerializeField] int speed;
    [Range(2, 4)][SerializeField] int sprintMod;
    [Range(5, 20)][SerializeField] int jumpSpeed;
    [Range(1, 3)][SerializeField] int jumpMax;
    [Range(15, 45)][SerializeField] int gravity;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject arrow;
    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    int HPOrig;
    int jumpCount;

    float shootTimer;

    public float rotationSpeed;

    Vector3 moveDir;
    Vector3 playerVelocity;

    public Animator animator;
    public RuntimeAnimatorController playerArcher;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();

        sprint();

        animator.SetBool("isGrounded", controller.isGrounded);
        animator.SetFloat("velocityY", playerVelocity.y + 0.001f);
        
    }
    void movement()
    {
        shootTimer += Time.deltaTime;


        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        
        }



        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraForward.y = 0;
        cameraForward.Normalize();


        Vector3 moveDir = (cameraForward * verticalInput) + (cameraRight * horizontalInput);
        moveDir.Normalize();

        controller.Move(moveDir * speed * Time.deltaTime);
        if (moveDir != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        jump();
        controller.Move(playerVelocity * Time.deltaTime);
        playerVelocity.y -= gravity * Time.deltaTime;


        // Animation movement controls

        animator.SetFloat("Speed", moveDir.magnitude);
        animator.SetFloat("Horizontal", horizontalInput);
        animator.SetFloat("Vertical", verticalInput);
        animator.SetBool("IsSprinting", Input.GetButton("Sprint"));
        animator.SetFloat("Speed", Mathf.Round(moveDir.magnitude * 100f) / 100f);


    }

        void jump()
     {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;
            animator.SetBool("IsJumping", true);

        }
        if (controller.isGrounded && animator.GetBool("IsJumping") == true && playerVelocity.y + 0.001f <= 0.1f)
        {
            animator.SetBool("IsJumping", false);
        }
    }
    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    public void shoot()
    {
        
        shootTimer = 0;
        Instantiate(arrow, shootPos.position, transform.rotation);

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.TakeDamage(shootDamage);
            }

        }
        animator.SetTrigger("Shoot");
        animator.SetBool("atReady", false);
    }
    public void TakeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            gamemanager.instance.youLose();

        }
    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.playerDamageScreen.SetActive(false);

    }


    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

}
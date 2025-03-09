using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [Range(2, 5), SerializeField] int speed;
    [Range(2, 4), SerializeField] int sprintModifier;
    [Range(5, 20), SerializeField] int jumpSpeed; // Amount to force them up
    [Range(1, 3), SerializeField] int jumpMax; // How many times you can jump at once before landing
    [Range(15, 45), SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDistance;
    [SerializeField] float shootRate;

    int jumpCount;
    float shootTimer;

    Vector3 moveDirection;
    Vector3 playerVelocity; // External force applied

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDistance, Color.red);
        // Keep Update Clean
        movement();

        sprint();

    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVelocity = Vector3.zero;
        }
        //moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        // multiply by Delta time for fame independent movement
        //transform.position += moveDirection * speed * Time.deltaTime;

        moveDirection = (Input.GetAxis("Horizontal") * transform.right) +
                        (Input.GetAxis("Vertical") * transform.forward);

        controller.Move(moveDirection * speed * Time.deltaTime);

        jump();

        controller.Move(playerVelocity * Time.deltaTime);

        playerVelocity.y -= gravity * Time.deltaTime;

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
        {
            shoot();
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVelocity.y = jumpSpeed;

        }
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintModifier;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintModifier;
        }
    }

    void shoot()
    {
        shootTimer = 0;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDistance))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);

            }
        }
    }
}

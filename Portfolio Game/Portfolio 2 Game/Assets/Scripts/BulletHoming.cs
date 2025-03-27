using UnityEngine;

public class BulletHoming : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    [SerializeField] private float force;
    [SerializeField] private float rotationForce;
    
    private Rigidbody rb;
    Vector3 bulletVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 adjustedPos = new Vector3(gamemanager.instance.player.transform.position.x, gamemanager.instance.player.transform.position.y+1, gamemanager.instance.player.transform.position.z); 
        Vector3 Direction = adjustedPos - rb.position;
        Direction.Normalize();
        Vector3 rotationAmount = Vector3.Cross(transform.forward, Direction);
        rb.angularVelocity = rotationAmount * rotationForce;
        rb.linearVelocity = transform.forward * force;
        
    }
}

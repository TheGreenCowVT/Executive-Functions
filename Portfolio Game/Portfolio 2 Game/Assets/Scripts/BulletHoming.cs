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
        Vector3 Direction = gamemanager.instance.playerTargetPos.transform.position - rb.position;
        Direction.Normalize();
        Vector3 rotationAmount = Vector3.Cross(transform.forward, Direction);
        rb.angularVelocity = rotationAmount * rotationForce;
        rb.linearVelocity = transform.forward * force;
        
    }
}

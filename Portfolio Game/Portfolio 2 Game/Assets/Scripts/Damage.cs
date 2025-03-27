using UnityEngine;
using System.Collections;


public class damage : MonoBehaviour
{

    enum damageType { moving, stationary, overtime }
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [Range(1, 15)][SerializeField] int damageAmount;
    [Range(0.25f, 12)][SerializeField] float damageTime;
    [Range(10, 45)][SerializeField] int speed;
    [Range(1, 4)][SerializeField] int destroyTime;


    bool isDamaging;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        if (type == damageType.moving)
        {
            damageAmount = 1;
            rb.linearVelocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && (type == damageType.stationary || type == damageType.moving))
        {
            dmg.TakeDamage(damageAmount);
        }

        if (type == damageType.moving)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.overtime)
        {
            if (!isDamaging)
                StartCoroutine(damageOther(dmg));
        }

    }

    IEnumerator damageOther(IDamage d)
    {
        isDamaging = true;

        d.TakeDamage(damageAmount);
        yield return new WaitForSeconds(damageTime);

        isDamaging = false;

    }

    public void SetDamageAmount(int amount)
    {
        damageAmount = Mathf.Clamp(amount, 1, 100);
    }
}

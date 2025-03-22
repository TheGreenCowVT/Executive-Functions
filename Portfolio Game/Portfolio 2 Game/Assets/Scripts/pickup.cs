using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        weapon.ammoCur = weapon.ammoMax;
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickupable = other.GetComponent<IPickup>();

        if (pickupable != null)
        {
            pickupable.getWeaponStats(weapon);
            Destroy(gameObject);
        }
    }
}


using UnityEngine;

public class Weapon : MonoBehaviour

{

    [SerializeField] string weaponName;
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] float damage;
    [SerializeField] float range;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] Transform shootPos;
    [SerializeField] LayerMask ignoreLayer;

    public string WeaponName => weaponName;
    public GameObject WeaponPrefab => weaponPrefab;
    public float Damage => damage;
    public float Range => range;

    public Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void Attack()
    {
        
        if (projectilePrefab != null && shootPos != null)
        {

            Instantiate(projectilePrefab, shootPos.position, Camera.main.transform.rotation);
            ////TODO Needs to not be here
            //RaycastHit hit;
            //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range, ~ignoreLayer))
            //{
            //    Debug.Log(hit.collider.name);

            //    IDamage dmg = hit.collider.GetComponent<IDamage>();

            //    if (dmg != null)
            //    {
            //        dmg.TakeDamage((int)damage);
            //    }

            //}
            animator.SetTrigger("Shoot");
            animator.SetBool("atReady", false);
        }
    }

    public virtual void Equip(Transform handTransform)
    {
       
        
    }

    public virtual void Unequip()
    {
        Destroy(gameObject, 0.1f);
    }
}

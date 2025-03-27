using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu]

public class Weapon : ScriptableObject
{

    public GameObject model;
    public GameObject projectilePrefab;
    [Range(5, 50)] public float projectileSpeed;
    [Range(1, 100)] public int shootDamage;
    [Range(5, 1000)] public int shootDist;
    [Range(0.1f, 2)] public float shootRate;
    public int ammoCur;
    [Range(0, 50)] public int ammoMax;



    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootVol;
    public Image UIImage;
   
}

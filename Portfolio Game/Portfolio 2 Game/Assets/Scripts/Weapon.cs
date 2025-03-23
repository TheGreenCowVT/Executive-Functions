using UnityEngine;

[CreateAssetMenu]

public class Weapon : ScriptableObject
{

public GameObject model;
[Range(1, 100)] public int shootDamage;
[Range(5, 1000)] public int shootDist;
[Range(0.1f, 2)] public float shootRate;
[HideInInspector] public int ammoCur;
[Range(5, 50)] public int ammoMax;

    public GameObject projectilePrefab;
[Range(5, 50)] public float projectileSpeed;

    public ParticleSystem hitEffect;
public AudioClip[] shootSound;
[Range(0, 1)] public float shootVol;
   
}

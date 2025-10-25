using UnityEngine;
public class Bullet : MonoBehaviour
{
    //Siin me teeme oma muutujad
    public Transform bulletSpawn;
    public float bulletSpeed;

    //Siin me teeme kuuli sündmusteks valmis
    void Awake()
    {
        transform.rotation = bulletSpawn.localRotation;
        transform.position = bulletSpawn.localPosition;
    }

    //Siin toimuvad sündmused
    void Update()
    {
        transform.position = transform.forward * bulletSpeed * Time.deltaTime;
    }
}

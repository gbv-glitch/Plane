using System;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    //See on meie mängija
    public Transform target;

    // Seda koodi me jookseme alati
    void Update()
    {
        // Liigutame vastase edasi
        transform.position += transform.forward * Time.deltaTime * 15;

        //Siin me arvutame, kuhu me peame pöörama
        Quaternion targetRotation = Quaternion.LookRotation(target.position - transform.position);
        
        //Siin me pöörame vastast
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 60 * Time.deltaTime);
    }
}

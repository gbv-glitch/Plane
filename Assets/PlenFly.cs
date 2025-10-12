using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlenFly : MonoBehaviour
{
    //Kaamera positioon
    public Transform mainCameraPosition;

    //Lennuki positioon
    public Transform playerPosition;

    //Lennuki kiirus
    public float planeSpeed = 20;

    //Lennuki pööramine
    private float yaw;
    private float pitch;
    private float roll;
    private float horizontalInput;
    private float verticalInput;

    //Mängu sündmused toimuvad siin
    private void Update()
    {
        //Liigutame  lennuki edasi
        transform.position += transform.forward * planeSpeed * Time.deltaTime;//Viimane on seal, et kõigil oleks ükskõik mis arvutil sama kiirus

        //Me siin registreerime, mida mängija tahab teha
        horizontalInput = Input.GetAxis("Horizontal") * 3;
        verticalInput = -(Input.GetAxis("Vertical") * 2);

        //Siin paneme info oma muutujasse
        yaw += horizontalInput * Time.deltaTime * 25;
        pitch = Mathf.Lerp(0, 20, Mathf.Abs(verticalInput)) * MathF.Sign(verticalInput * Time.deltaTime);
        roll = Mathf.Lerp(0, 20, Mathf.Abs(horizontalInput)) * -MathF.Sign(horizontalInput * Time.deltaTime);

        //Siin me pöörame oma mängijat
        transform.rotation = Quaternion.Euler(Vector3.up * yaw + Vector3.right * pitch + Vector3.forward * roll);

        //Siin me toome kaamera mängija järel
        mainCameraPosition.position = playerPosition.position + new Vector3(0, 5f, -10f);

        //Siin me pöörame oma kaamerat mängija järel
        mainCameraPosition.rotation = playerPosition.rotation;
    }
    
}
using System;
using JetBrains.Annotations;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.UnityConsent;

public class PlaneControls : MonoBehaviour
{
    //Kaamera "giidi" positioon
    public Transform mainCameraGuidePosition;

    //Lennuki osade positioon
    public Transform leftCanard;
    public Transform rightCanard;
    public Transform rightElevon;
    public Transform leftElevon;

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

    //Püssikuul
    public GameObject bulletPrefab;

    //Pausile panemise võimalus
    public bool pause = false;

    //Mängu sündmused toimuvad siin
    private void Update()
    {
        //Siin me vaatame, kas mäng on pausile pandud
        if (pause == false)
        {    //Liigutame  lennuki edasi
            transform.position += transform.forward * planeSpeed * Time.deltaTime;//Viimane on seal, et kõigil oleks ükskõik mis arvutil sama kiirus

            //Me siin registreerime, mida mängija tahab teha
            horizontalInput = Input.GetAxis("Horizontal") * 4;
            verticalInput = -(Input.GetAxis("Vertical") * 5);

            //Siin paneme info oma muutujasse
            yaw += horizontalInput * Time.deltaTime * 25;
            pitch = Mathf.Abs(verticalInput) * MathF.Sign(verticalInput * Time.deltaTime) * 4;
            roll = Mathf.Abs(horizontalInput) *-MathF.Sign(horizontalInput * Time.deltaTime) * 7.5f;

            //Pöörame lennuki osad, et see näeb realistilisem välja
            rightElevon.localRotation = Quaternion.Euler(new Vector3(135 * MathF.Abs(roll / 90), 0, 0));
            leftElevon.localRotation = Quaternion.Euler(new Vector3(135 * MathF.Abs(roll / 90), 0, 0));
            rightCanard.localRotation = Quaternion.Euler(new Vector3(135 * MathF.Abs(roll / 90), 0, 0));
            leftCanard.localRotation = Quaternion.Euler(new Vector3(135 * MathF.Abs(roll / 90), 0, 0));

            //Siin me pöörame oma mängijat
            transform.rotation = Quaternion.Euler(Vector3.up * yaw + Vector3.right * pitch + Vector3.forward * roll * 3);

            //Siin me toome kaamera mängija järel
            mainCameraGuidePosition.position = playerPosition.position;

            //Siin me pöörame oma kaamerat mängija järel
            mainCameraGuidePosition.Rotate(0, horizontalInput * Time.deltaTime * 25, 0);

            //Siin me muudame mängija kiirust
            planeSpeed += Input.mouseScrollDelta.y;

            //Selleks et mängija ei saaks kohal seista, kontrollime, kas lennukiirus on väiksem kui 15. Kui on, siis me paneme selle 15ks.
            if (planeSpeed <= 15)
            {
                planeSpeed = 15;
            }

            //Siin me tulistame, kui mängija vajutab hiirele

            if (Input.GetMouseButton(0))
            {
                print("pewpew");
            }
        }

        //Siin me saame panna mängu pausile või selle jälle käima panna
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (pause == true)
            {
                pause = false;
            }
            else
            {
                pause = true;
            }
        }
    }
    
}
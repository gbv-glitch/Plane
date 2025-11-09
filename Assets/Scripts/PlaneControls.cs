using System;
using JetBrains.Annotations;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.UnityConsent;

public class PlaneControls : MonoBehaviour
{
    //Kuulide arv
    public float bullets = 600;

    //Kuulide arvu näitaja ekraanil
    public TextMeshProUGUI bulletCounter;

    //Kuulide arv kuni kahur ei tööta enam
    public TextMeshProUGUI gunOverheatCounter;

    //Kaamera "giidi" positioon
    public Transform mainCameraGuidePosition;

    //Me vaatame, mitu kuuli on mängija korraga tulistanud
    public float bulletsInOneBurst = 0;

    //Lennuki osade positioon
    public Transform leftCanard;
    public Transform rightCanard;
    public Transform rightElevon;
    public Transform leftElevon;

    //Lennuki positioon
    public Transform playerPosition;

    //Lennuki kiirus protsentides
    public float planeSpeedPercent = 20;

    //Lennuki maksimum kiirus
    public float planeMaxSpeed = 150;

    //Lennuki pööramine
    private float yaw;
    private float pitch;
    private float roll;
    private float horizontalInput;
    private float verticalInput;

    //Püssikuul
    public GameObject bulletPrefab;

    //Kuuli tekkimispositioon
    public Transform bulletSpawn;

    //Kuuli heli
    public AudioSource bulletSound;

    //Mootori hääl
    public AudioSource engineSound;

    //Pausile panemise võimalus
    public bool pause = false;

    //Kahuri laskmiskiiruse vähendamiseks
    public bool readyToShoot = true;

    //Kontrollimiseks, ega me pea ootama kuni paneme ennast jälle valmis tulistama
    public bool overheated = false;

    //Selle koodi me jookseme ühe korra mängu alguses
    void Start()
    {
        //Paneme heli staatuse selliseks, et see mängiks iga kord kui see ära lõppeb
        engineSound.loop = true;

        //Paneme hääle mängima
        engineSound.Play();
    }

    //Mängu sündmused toimuvad siin
    private void Update()
    {
        //Siin me vaatame, kas mäng on pausile pandud
        if (pause == false)
        {    //Liigutame  lennuki edasi
            transform.position += transform.forward * (planeMaxSpeed * planeSpeedPercent / 100) * Time.deltaTime;//Viimane on seal, et kõigil oleks ükskõik mis arvutil sama kiirus

            //Me siin registreerime, mida mängija tahab teha
            horizontalInput = Input.GetAxis("Horizontal") * 4;
            verticalInput = -(Input.GetAxis("Vertical") * 5);

            //Siin paneme info oma muutujasse
            yaw += horizontalInput * Time.deltaTime * 25;
            pitch = Mathf.Abs(verticalInput) * MathF.Sign(verticalInput * Time.deltaTime) * 4;
            roll = Mathf.Abs(horizontalInput) * -MathF.Sign(horizontalInput * Time.deltaTime) * 7.5f;

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
            planeSpeedPercent += Input.mouseScrollDelta.y * 10;

            //Selleks et mängija ei saaks kohal seista, kontrollime, kas lennukiiruse protsent on väiksem kui kümme. Kui on, siis me paneme selle kümneks.
            if (planeSpeedPercent <= 10)
            {
                planeSpeedPercent = 10;
            }


            //Selleks et mängija ei saaks liiga kiiresti lennata, kontrollime, kas lennukiiruse protsent on üle saja. Kui on, siis me paneme selle sajaks.
            if (planeSpeedPercent >= 100)
            {
                planeSpeedPercent = 100;
            }

            //Siin me tulistame, kui mängija vajutab hiirele, kui me oleme tulistamiseks valmis ja kui meil on veel kuuli alles

            if (Input.GetMouseButton(0) && readyToShoot && bullets > 0)
            {
                //Tekitame uue kuuli
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, transform.rotation);
                bullet.GetComponent<Bullet>().hostTag = "Player";

                //Võtame kuulide arvust ühe ära
                bullets -= 1f;

                //Paneme ühe juurde meie kuulide korraga tulistamise lugejale
                bulletsInOneBurst += 1;
                
                //Mängime püssiheli
                bulletSound.PlayOneShot(bulletSound.clip);

                //Selleks, et me ei saaks tulistada kohe pärast tulistamist
                readyToShoot = false;
            }

            //Kontrollime, kas mängija laseb laskmise nupust lahti
            if (Input.GetMouseButtonUp(0))
            {
                //Paneme korraga tulistatud kuulide lugeja nulli
                bulletsInOneBurst = 0;
            }

            //Kontrollime, kas mängija on liiga palju kuule tulistanud korraga
            if (bulletsInOneBurst >= 50)
            {
                //Selleks, et me järgmine kaader kohe ei saa tulistada
                overheated = true;

                //Tühistame olemasolevad invoke-kutsed, et vältida nende kogunemist
                CancelInvoke("ReadyToShoot");
                
                //Paneme ennast valmis tulistama peale 10 sekundi
                Invoke("ReadyToShoot", 10f);
            }
            //Muidu me oleme valmis jälle tulistama pärast 0,5 sekundit (normaalne tulistamine)
            else if (!overheated)
            {
                //Paneme ennast valmis tulistama peale 0,5 sekundi
                Invoke("ReadyToShoot", 0.5f);
            }

            //Siin me näitame, mitu kuuli on mängijal alles
            bulletCounter.text = "Bullets left: " + bullets;

            //Siin me näitame, mitu kuuli on kuni meie kahur saab liiga kuumaks
            gunOverheatCounter.text = "Bullets until overheating:" + MathF.Abs(50 - bulletsInOneBurst);
            
            //Siin me muudame teksti värvi, vastavalt sellele, kui lähedal me oleme ülekuumenemisele
            //Kontrollime, kas me peame muutma värvi kollaseks
            if (bulletsInOneBurst >= 15)
            {
                //Siin me muudame värvi
                gunOverheatCounter.color = new UnityEngine.Color(255, 255, 0);
            }
            //Kontrollime, kas me peame värvi oranžiks
            if (bulletsInOneBurst >= 30)
            {
                //Muudame värvi
                gunOverheatCounter.color = new UnityEngine.Color(255, 127.5f, 0);
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

    //Tulistamiseks valmis panemine
    public void ReadyToShoot()
    {
        //Paneme lugeja nulli ainult siis, kui kahur oli ülekuumenenud
        if (overheated)
        {
            bulletsInOneBurst = 0;
        }
        
        //Paneme enda muutuja falseks, et see saaks uuesti tööle hakata
        overheated = false;
        
        //Paneme ennast tulistamiseks valmis
        readyToShoot = true;
    }
}

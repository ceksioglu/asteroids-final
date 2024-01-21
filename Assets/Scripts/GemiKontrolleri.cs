using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GemiKontrolleri : MonoBehaviour
{

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Collider2D collider;
    public float thrust;
    public float turnThrust;
    private float thrustInput;
    private float turnInput;        //
    public float screenTop;         //Ekran limitleri
    public float screenBottom;
    public float screenLeft;
    public float screenRight;   
    public float bulletVelocity;    //Kurşun hızı
    public float deathForce;        //Çarpışma sertliği ayarı
    private bool hyperspace;        //true = su an zıplıyor
    private bool alive = true;      //true = hayatta
    public static bool pause = false;

    public int score;               //Skor
    public int lives;               //Can

    public Text scoreText;
    public Text livesText;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    public AudioSource audio;

    public GameObject explosion;
    public GameObject bullet;
    public GameObject bulletSpawn;
    public GameObject missile;
    public GameObject missileSpawn;
    public AlienScript alien;

    public Color inColor;
    public Color normalColor;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        hyperspace = false;
        scoreText.text = "Skor: " + score;
        livesText.text = "Kalan Can: " +lives;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if(pause)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        thrustInput = Input.GetAxis("Vertical");    //Yukarı ok tuşuyla ileri itme kuvveti uygular.
        turnInput = Input.GetAxis("Horizontal");    //Yan ok tuşlarıyla dönme kuvveti/tork uygular.
        
        if(Input.GetButtonDown("Fire1") && alive)   //Kurşun atma fonksiyonu.
        {
          GameObject newBullet = Instantiate(bullet, bulletSpawn.transform.position, bulletSpawn.transform.rotation);
          newBullet.GetComponent<Rigidbody2D> ().AddRelativeForce(Vector2.up * bulletVelocity);
          Destroy(newBullet, 5.0f);     //Beş saniye sonra kurşunu yok eder.
        }

        //Zıplama fonksiyonu 'Hyperspace'

        if(Input.GetButtonDown("Hyperspace") && hyperspace == false) 
        {
            hyperspace = true;
            spriteRenderer.enabled = false;
            collider.enabled = false;
            Invoke("Hyperspace", 1f);
        }

        //Gemiyi döndürme

        transform.Rotate(Vector3.forward * -turnInput * turnThrust * Time.deltaTime);

        //Ekran geçişleri

        Vector2 newPos =  transform.position;
        if(transform.position.y > screenTop)
        {
            newPos.y = screenBottom;
        }
        if(transform.position.y < screenBottom)
        {
            newPos.y = screenTop;
        }
        if(transform.position.x > screenRight)
        {
            newPos.x = screenLeft;
        }
        if(transform.position.x < screenLeft)
        {
            newPos.x = screenRight;
        }
        transform.position = newPos; //Üstteki şartlar sağlanmazsa pozisyonu olduğu halinde bırakır.
    }

    void FixedUpdate()
    {
        rb.AddRelativeForce (Vector2.up * thrustInput * thrust); //Gemiyi ileri götürme / momentum fonksiyonu.
        //rb.AddTorque(-turnInput); //Kontrolleri tersine çevrirmek için.
    }

    
    void ScorePoints(int pointsToAdd) //Puanlama sistemi
    {
        score += pointsToAdd;
        scoreText.text = "Skor: " + score; //Skor değiştiğinde text'i güncelleme fonksioynu
    }

    //Tekrar canlanma fonksiyonu
    void Respawn()
    {
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
        
        spriteRenderer.enabled = true;
        spriteRenderer.color = inColor;
        Invoke("Invulnerable",3f);
        alive = true;
            

    }
    //Dokunulmazlık fonksiyonu
    void Invulnerable()
    {
        collider.enabled = true;
        spriteRenderer.color = normalColor;
    }
    //Zıplama/Hyperspace

    void Hyperspace()
    //Rasgele yeni konuma geçme + Collider ve sprite renderer tekrardan aktive etme

    {
        Vector2 newPosition = new Vector2(Random.Range(-200f,200f), Random.Range(-100f,100f));
        transform.position = newPosition;

        spriteRenderer.enabled = true;
        collider.enabled = true;
        
        hyperspace = false;
    }

    void LoseLife()     //Çarpışan objelerin göreceli hızları/şiddeti belirlenen değeri geçerse gemiden bir can eksiltir.
    {
            lives--;
            //Patlama yaratma
            GameObject newExplosion = Instantiate(explosion,transform.position,transform.rotation);
            Destroy(newExplosion, 3f); //3 saniye sonra hazıfadan patlamayı kaldırır.
            livesText.text = "Kalan Can: " + lives;

            alive = false;
            spriteRenderer.enabled = false;
            collider.enabled = false;
            //Tekrar canlanma fonksiyonunu çağırma
            Invoke("Respawn",3f);

            if(lives <= 0){
                //Oyun Bitti
                GameOver();
            }
    }


    void OnCollisionEnter2D(Collision2D col)   //Trigger olmayan obje çarpışınca
    {     
        Debug.Log(col.relativeVelocity.magnitude);
        if (col.relativeVelocity.magnitude > deathForce)
        {
            LoseLife();
        }  
        else 
        {
            audio.Play(); //Çarpışma sesi
        }  
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("beam")) 
        {
            LoseLife();
            //alien.Disable();
        }
    }

    void GameOver()                        //Oyun bitince menüyü aktive et.
    {
        CancelInvoke();
        gameOverPanel.SetActive (true);
    }

    public void PlayAgain()               //Tekrar oyna buttonu
    {
        SceneManager.LoadScene ("Main");
    }

    public void MainMenu()               //Ana menü buttonu
    {
        SceneManager.LoadScene ("Menu");
    }
    public void Resume()                //Oyun devam ettirme
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        pause = false;
    }
    public void Pause()                 //Durdurma
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        pause = true;
    }
}

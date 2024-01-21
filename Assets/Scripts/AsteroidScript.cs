using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidScript : MonoBehaviour
{
    public Rigidbody2D rb;
    public float maxThrust;
    public float maxSpin;     //Asteroid ilk yüklendiğinde hız/tork sınırları.
    public float screenTop;     //Ekran Limitleri
    public float screenBottom;
    public float screenLeft;
    public float screenRight;
    public int asteroidSize;  //3=Büyük 2=Orta 1=Küçük boy
    public GameObject AsteroidMedium;
    public GameObject AsteroidSmall;
    public GameObject AsteroidMedium2;
    public GameObject AsteroidSmall2;
    public int points;
    public GameObject player;
    public GameObject explosion;

    public GameManager gm;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 thrust = new Vector2(Random.Range(-maxThrust,maxThrust), Random.Range(-maxThrust,maxThrust));   //Asteroid yüklendiğinde rasgele vektörel kuvvet ve tork ekler.
        float spin = Random.Range(-maxSpin,maxSpin);

        rb.AddForce(thrust); //Rigidbody 2D için vektörel kuvvet.
        rb.AddTorque(spin);

        // Oyuncuyu bulma fonksiyonu
        player = GameObject.FindWithTag("Player");
        //Oyun menajeri
        gm = GameObject.FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
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

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log ("Çarpışma:" + other.name);
        
        if(other.CompareTag("bullet")){     //Çarpanın kurşun olup olmadığını kontrol et
            Destroy(other.gameObject);      //Asteroid ile çarpışma olduğunda çarpıştığı objeyi siler.

            if(asteroidSize == 3)   {       //Asteroid boyutu büyük ise 2 adet orta boy yaratır.
                Instantiate(AsteroidMedium,transform.position,transform.rotation);
                Instantiate(AsteroidMedium2,transform.position,transform.rotation);
                gm.UpdateNumberOfAsteroids(1); //Game manager'a asteroid sayısını arttırmasını söyler.
              
            }   
            else if(asteroidSize == 2)   {       //Orta boy ise 2 adet küçük yaratır.
                Instantiate(AsteroidSmall,transform.position,transform.rotation);
                Instantiate(AsteroidSmall2,transform.position,transform.rotation);
                gm.UpdateNumberOfAsteroids(1);
            }
            else if(asteroidSize == 1)   {       //Küçük boy ise yok eder.
                gm.UpdateNumberOfAsteroids(-1);
            }

            player.SendMessage("ScorePoints",points); //Puanı günceller.

            GameObject newExplosion = Instantiate(explosion,transform.position,transform.rotation);     //Asteroid patlama efektini yaratır ve GameObject olarak saklar.
            Destroy(newExplosion, 3f);
                                            //Hafızayı boşaltmak için patlama efektini 3 saniye sonra kaldırır.
            Destroy(gameObject);            
                                            //Vurulan asteroidi oyundan siler.
        
        }
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienScript : MonoBehaviour
{

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Collider2D collider;
    public Vector2 direction;       //Uzaylının yönü için
    public float speed;
    public float bulletSpeed;

    public Transform player;
    public GameObject bullet;
    public GameObject explosion;
    public Transform startPosition;
    public float shootingDelay;     //Atış arası süre (sn)
    public float lastTimeShot = 0f;
    public bool disabled;           //1 = şu an deaktif
    public int points;
    public float timeBeforeSpawning; //(sn)
    public int currentLevel = 0;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform; //Oyuncuyu bulma
        timeBeforeSpawning = Random.Range(5f, 20f);          //Canlanmadan önce rasgele 5-20 saniye bekler.
        Invoke("Enable", timeBeforeSpawning);
        NewLevel();
    }

    // Update is called once per frame
    void Update()
    {
        if (disabled) 
        {
            return;
        }
        if (Time.time > lastTimeShot + shootingDelay) 
        {
            //Ateş etme.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion q = Quaternion.AngleAxis(angle,Vector3.forward);
            
            //Lazer yaratma.
            GameObject newBullet = Instantiate(bullet, transform.position, q);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f,bulletSpeed));
            Destroy(newBullet, 7.0f);
            //Ateş edildikten sonra senkronize etme.
            lastTimeShot = Time.time;
        }   
    }

    void FixedUpdate()
    {
        if (disabled) 
          {
            return;
          }
        //Hangi tarafa hareket edileceğini belirle. (oyuncuya doğru)
        direction = (player.position - transform.position).normalized;
        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    public void NewLevel()
    {
        Disable();                                      //Yeni seviye
        currentLevel++;
        timeBeforeSpawning = Random.Range(5f, 20f);
        Invoke("Enable", timeBeforeSpawning);
        bulletSpeed = 2000 * currentLevel;
        points = 500 * currentLevel;
    } 

    void Enable()
    {
        //Başlangıç posisyonuna geçer.
            transform.position = startPosition.position;
        //Collider ve sprite rendererları açar.
            collider.enabled = true;
            spriteRenderer.enabled = true;
            disabled = false;
    }

    public void Disable()
    {  
        //Uzaylı gemisinin collider ve sprite renderer'ı deaktive et.
            collider.enabled = false;
            spriteRenderer.enabled = false;
            disabled = true;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
       if  (other.CompareTag("bullet"))
       {
            //Puanı günceller.
                player.SendMessage("ScorePoints",points); 
            //Patlama yarat.
                GameObject newExplosion = Instantiate(explosion,transform.position,transform.rotation);     //Asteroid patlama efektini yaratır ve GameObject olarak saklar.
                Destroy(newExplosion, 3f);
            //Uzaylıyı deaktive et.
                Disable();
       }    
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.transform.CompareTag("Player"))
        {
        //Patlama yarat.
            GameObject newExplosion = Instantiate(explosion,transform.position,transform.rotation);     //Uzaylı patlama efektini yaratır ve GameObject olarak saklar.
            Destroy(newExplosion, 3f);
        //Uzaylıyı deaktive et.
            Disable();
        }
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    public int numberOfAsteroids;   //Sahnedeki asteroid sayısını takip için
    public int levelNumber = 1;     //Seviye 1'den başlat.
    public GameObject asteroid;
    public GameObject asteroid2;
    public AlienScript alien;
    public Text levelText;

    void Start()
    {
        levelText.text = "Seviye " + levelNumber;
    }

    public void UpdateNumberOfAsteroids(int change)  //Sahnedeki asteroid sayısını güncelle
    {
        numberOfAsteroids += change;

        //Sahnede asteroid kalıp kalmadığını kontrol etme
            if(numberOfAsteroids <= 0){
        //Yeni seviyeye geç
            Invoke("StartNewLevel", 3f);

        }
    }


    void StartNewLevel()
    {
        levelNumber++;

        //Yeni seviye için asteroid yarat
        for (int i=0; i < levelNumber; i++) {
            Vector2 spawnPosition = new Vector2(Random.Range(-230f,230f),117f);
            Vector2 spawnPosition2 = new Vector2(Random.Range(-230f,230f),-117f);
            Instantiate(asteroid, spawnPosition, Quaternion.identity);
            Instantiate(asteroid2, spawnPosition2, Quaternion.identity);
            numberOfAsteroids += 2;

      }
        //Uzaylıyı hazırla
        alien.NewLevel();
    }


}

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour {

    public GameObject[] maps;
    public bool isMainScene;
    public GameObject[] cars;
    public GameObject canvasLosePanel;
    public float timeToSpawnFrom = 2f, timeToSpawnTo = 4.5f;
    private int countCars;
    private Coroutine bottomCars, leftCars, rightCars, upCars;
    private bool isLoseOnce;
    public Text nowScore, topScore, coinsCount;
    public GameObject horn;
    public AudioSource turnSignal;
    [NonSerialized] public static int countLoses;
    private static bool isAdd;
    public GameObject adsManager;
    
    private void Start() {
        if (!isAdd && PlayerPrefs.GetString("NoAds") != "yes") {
            Instantiate(adsManager, Vector3.zero, Quaternion.identity);
            isAdd = true;
        }

        if (PlayerPrefs.GetInt("NowMap") == 2) {
            Destroy(maps[0]);
            maps[1].SetActive(true);
            Destroy(maps[2]);
        } else if (PlayerPrefs.GetInt("NowMap") == 3) {
            Destroy(maps[0]);
            Destroy(maps[1]);
            maps[2].SetActive(true);
        } else {
            maps[0].SetActive(true);
            Destroy(maps[1]);
            Destroy(maps[2]);
        }
        
        CarController.isLose = false;
        CarController.countCars = 0;
        
        if (isMainScene) {
            timeToSpawnFrom = 4f;
            timeToSpawnTo = 6f;
        }

        bottomCars = StartCoroutine(BottomCars());
        leftCars = StartCoroutine(LeftCars());
        rightCars = StartCoroutine(RightCars());
        upCars = StartCoroutine(UpCars());

        StartCoroutine(CreateHorn());
    }

    private void Update() {
        if (CarController.isLose && !isLoseOnce) {
            countLoses++;
            StopCoroutine(bottomCars);
            StopCoroutine(leftCars);
            StopCoroutine(rightCars);
            StopCoroutine(upCars);
            nowScore.text = "<color=#F65757>Score:</color> " + CarController.countCars.ToString();
            if (PlayerPrefs.GetInt("Score") < CarController.countCars)
                PlayerPrefs.SetInt("Score", CarController.countCars);

            topScore.text = "<color=#F65757>Top:</color> " + PlayerPrefs.GetInt("Score").ToString();
            
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins") + CarController.countCars);
            coinsCount.text = PlayerPrefs.GetInt("Coins").ToString();
            
            canvasLosePanel.SetActive(true);
            isLoseOnce = true;
        }
    }

    IEnumerator BottomCars() {
        while (true) {
            SpawnCar(new Vector3(-1.06f, -0.15f, -22.7f), 180f);
            float timeToSpawn = Random.Range(timeToSpawnFrom, timeToSpawnTo);
            yield return new WaitForSeconds(timeToSpawn);
        }
    }
    
    IEnumerator LeftCars() {
        while (true) {
            SpawnCar(new Vector3(-82.9f, -0.15f, 3.2f), 270f);
            float timeToSpawn = Random.Range(timeToSpawnFrom, timeToSpawnTo);
            yield return new WaitForSeconds(timeToSpawn);
        }
    }
    
    IEnumerator RightCars() {
        while (true) {
            SpawnCar(new Vector3(26.4f, -0.15f, 9.86f), 90f);
            float timeToSpawn = Random.Range(timeToSpawnFrom, timeToSpawnTo);
            yield return new WaitForSeconds(timeToSpawn);
        }
    }
    
    IEnumerator UpCars() {
        while (true) {
            SpawnCar(new Vector3(-8.11f, -0.15f, 58f), 0f, true);
            float timeToSpawn = Random.Range(timeToSpawnFrom, timeToSpawnTo);
            yield return new WaitForSeconds(timeToSpawn);
        }
    }

    void SpawnCar(Vector3 pos, float rotationY, bool isMoveFromUp = false) {
        GameObject newObj = Instantiate(cars[Random.Range(0, cars.Length)], pos, Quaternion.Euler(0,rotationY,0)) as GameObject;
        newObj.name = "Car - " + ++countCars;
        
        int random = isMainScene ? 1 : Random.Range(1, 4);
        if(isMainScene)
            newObj.GetComponent<CarController>().speed = 10f;
        
        switch (random) {
            case 1:
                // Move right
                newObj.GetComponent<CarController>().rightTurn = true;
                if (PlayerPrefs.GetString("music") != "No" && !turnSignal.isPlaying) {
                    turnSignal.Play();
                    Invoke("StopSound", 2f);
                }
                break;
            case 3:
                // Move left
                newObj.GetComponent<CarController>().leftTurn = true;
                if (PlayerPrefs.GetString("music") != "No" && !turnSignal.isPlaying) {
                    turnSignal.Play();
                    Invoke("StopSound", 2f);
                }
                
                if(isMoveFromUp)
                    newObj.GetComponent<CarController>().moveFromUp = true;
                break;
            case 5:
                // Move forward
                break;
            
        }
    }

    void StopSound() {
        turnSignal.Stop();
    }

    IEnumerator CreateHorn() {
        while (true) {
            yield return new WaitForSeconds(Random.Range(5, 9));
            if (PlayerPrefs.GetString("music") != "No")
                Instantiate(horn, Vector3.zero, Quaternion.identity);
        }
    }
    
}

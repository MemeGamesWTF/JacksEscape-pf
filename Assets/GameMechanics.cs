using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
public class GameMechanics : MonoBehaviour
{
    public GameObject startPanel; // Panel shown before the game starts
    public GameObject gameOverPanel; // Panel shown when the game is over
    public GameObject blast;
    public Text countdownText; // Text element to display the countdown
    public GameObject TapBtn;
    public Text ScoreTxt;
    private int score;
    public GameObject targetObject; // The object to rotate
    public float rotationSpeed = 100f; // Speed of rotation

    public bool isRotatingNegative = true; // Indicates the current rotation direction
    public bool isRotating = false; // Indicates whether the object is currently rotating
    public GameObject bombPrefab; // The bomb prefab to spawn
    public Transform spawnPoint; // The spawn point for bombs
    public float bombSpeed = 5f; // Speed of the bombs
    public float spawnInterval = 2f; // Time interval between bomb spawns

    private float spawnTimer = 0f;

    public int poolSize = 10; // Number of bombs in the pool
    private List<GameObject> bombPool; // Pool of bombs

    private bool isGameActive = false; // Tracks if the game is running

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip gameover;
    public AudioClip coin;
    public AudioClip tik;
    public AudioClip go;


    [DllImport("__Internal")]
  private static extern void SendScore(int score, int game);
    void Start()
    {
        ScoreTxt.text = "Score : " + score.ToString();
        // Initialize the bomb pool
        bombPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject bomb = Instantiate(bombPrefab);
            bomb.SetActive(false); // Disable the bomb initially
            bombPool.Add(bomb);
        }

        // Show the start panel and hide game over panel
        startPanel.SetActive(true);
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (!isGameActive) return;

        // Check for touch input
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                ToggleRotationDirection();
            }
        }

        // Check for mouse input (for testing in the editor)
        if (Input.GetMouseButtonDown(0))
        {
            ToggleRotationDirection();
        }

        // Rotate the object if it should be rotating
        if (isRotating)
        {
            float rotationDirection = isRotatingNegative ? -1f : 1f;
            targetObject.transform.Rotate(0f, 0f, rotationSpeed * rotationDirection * Time.deltaTime);
        }

        // Spawn bombs at intervals
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            SpawnBomb();
            spawnTimer = 0f;
        }
    }

    public void StartGame()
    {
        startPanel.SetActive(false); // Hide the start panel
        StartCoroutine(CountdownRoutine());
    }

    IEnumerator CountdownRoutine()
    {
        countdownText.gameObject.SetActive(true);

        for (int i = 3; i > 0; i--)
        {
            audioSource.PlayOneShot(tik);
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        audioSource.PlayOneShot(go);
        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        isGameActive = true;
        TapBtn.SetActive(true);
    }

    void ToggleRotationDirection()
    {
        // Start rotation if not already rotating
        if (!isRotating)
        {
            isRotating = true;
            return;
        }

        // Swap the rotation direction
        //isRotatingNegative = !isRotatingNegative;
    }

    public void ToggleDirectionFromButton()
    {
        isRotating = true;
        isRotatingNegative = !isRotatingNegative;
    }

    void SpawnBomb()
    {
        GameObject bomb = GetPooledBomb();
        if (bomb != null)
        {
            bomb.transform.position = spawnPoint.position;
            bomb.transform.rotation = Quaternion.identity;
            bomb.SetActive(true);

            Bomb bombMovement = bomb.GetComponent<Bomb>();
            bombMovement.speed = bombSpeed;
        }
    }

    GameObject GetPooledBomb()
    {
        foreach (GameObject bomb in bombPool)
        {
            if (!bomb.activeInHierarchy)
            {
                return bomb;
            }
        }
        return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            // collision.gameObject.SetActive(false);
            StartCoroutine(setActiveObject(collision.gameObject));
        }

        if (collision.gameObject.CompareTag("bomb"))
        {
            GameOver();
        }
    }
    IEnumerator setActiveObject(GameObject obj)
    {
        audioSource.PlayOneShot(coin);
        obj.SetActive(false);
        score++;
        ScoreTxt.text = "Score : " +   score.ToString();
        yield return new WaitForSeconds(2f);
        obj.SetActive(true);
    }
    void GameOver()
    {
        audioSource.PlayOneShot(gameover);
        blast.SetActive(true);
        StopAllCoroutines();
        isGameActive = false;
        gameOverPanel.SetActive(true); // Show the game over panel
        SendScore(score, 46);
        foreach (GameObject bomb in bombPool)
        {
            bomb.SetActive(false); // Disable all bombs
        }
    }

     public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

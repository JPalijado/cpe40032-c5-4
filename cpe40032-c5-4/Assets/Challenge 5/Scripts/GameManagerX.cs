using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManagerX : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI gameOverText;
    public GameObject titleScreen;
    public Button restartButton; 

    public List<GameObject> targetPrefabs;

    private int score;
    private float spawnRate = 1.5f;
    public bool isGameActive;

    private float timer = 60;
    private int lives;
    private bool isPlayed = false;

    public AudioClip spawnSound;
    public AudioClip gameOverSound;
    public AudioClip deductSound;
    public AudioClip wrongSound;
    public AudioClip munchSound;
    private AudioSource gameAudio;

    private float spaceBetweenSquares = 2.5f; 
    private float minValueX = -3.75f; //  x value of the center of the left-most square
    private float minValueY = -3.75f; //  y value of the center of the bottom-most square

    void Start()
    {
        // Gets the audio source
        gameAudio = GetComponent<AudioSource>();
    }

    // Problem 5 - The difficulty buttons don’t change the difficulty
    // Solution: Add an integer parameter difficulty to the StartGame method, then divide the spawnRate to it 
    // Start the game, remove title screen, reset score, and adjust spawnRate based on difficulty button clicked
    public void StartGame(int difficulty)
    {
        spawnRate /= difficulty;
        isGameActive = true;
        StartCoroutine(SpawnTarget());
        score = 0;
        UpdateScore(0);
        titleScreen.SetActive(false);
        UpdateLives(3);
    }

    void Update()
    {
        // If the game is not yet over, the countdown timer will run
        if (isGameActive == true)
        {
            CountdownTimer();
        }

        // If there is no time remaining, the game will end
        if (timer < 0)
        {
            if (isPlayed == false)
            {
                GameOver();
                isPlayed = true;
            }
        }
    }

    public void CountdownTimer()
    {
        timer -= Time.deltaTime;
        timerText.text = "Time Left: " + Mathf.Round(timer);
    }

    // While game is active spawn a random target
    IEnumerator SpawnTarget()
    {
        while (isGameActive)
        {
            yield return new WaitForSeconds(spawnRate);
            int index = Random.Range(0, targetPrefabs.Count);
            gameAudio.PlayOneShot(spawnSound, 0.5f);
            if (isGameActive)
            {
                Instantiate(targetPrefabs[index], RandomSpawnPosition(), targetPrefabs[index].transform.rotation);
            }
        }
    }

    // Generate a random spawn position based on a random index from 0 to 3
    Vector3 RandomSpawnPosition()
    {
        float spawnPosX = minValueX + (RandomSquareIndex() * spaceBetweenSquares);
        float spawnPosY = minValueY + (RandomSquareIndex() * spaceBetweenSquares);

        Vector3 spawnPosition = new Vector3(spawnPosX, spawnPosY, 0);
        return spawnPosition;

    }

    // Generates random square index from 0 to 3, which determines which square the target will appear in
    int RandomSquareIndex()
    {
        return Random.Range(0, 4);
    }

    // Problem 3 - The Score is being replaced by the word “score”
    // Solution: Concatinate the score variable to the "Score: " string
    // Update score with value from target clicked
    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.text = "Score: " + score;
        gameAudio.PlayOneShot(munchSound, 5.0f);
    }

    public void UpdateLives(int livesToChange)
    {
        // Increments the current lives
        lives += livesToChange;
        // Displays the current lives
        livesText.text = "Lives: " + lives;
        // Runs the gameover method if the current life is less than or equal to 0
        if (lives <= 0)
        {
            GameOver();
        }
    }

    public void ReduceLives()
    {
        // Reduce the remaining lives
        UpdateLives(-1);
        // Play explosion sound
        gameAudio.PlayOneShot(deductSound, 0.5f);
    }

    public void HitBadTarget()
    {
        // Reduce the remaining lives
        UpdateLives(-1);
        // Play explosion sound
        gameAudio.PlayOneShot(wrongSound, 2.0f);
    }

    // Problem 4 - When you lose, there’s no way to Restart
    // Solution: Set the restartButton.gameObject.SetActive(true), not false
    // Stop game, bring up game over text and restart button
    public void GameOver()
    {
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        isGameActive = false;
        gameAudio.PlayOneShot(gameOverSound, 2.0f);
    }

    // Restart game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

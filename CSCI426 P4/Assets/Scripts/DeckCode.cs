using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class DeckCode : MonoBehaviour
{
    // 0 - Bombs, 1 - point cards, 2 - point multiplers, 3 - bomb in next few cards, 4 - shuffle the whole deck
    // 5 - move the next two cards to bottom of deck, 6 - put nearest bomb at the bottom of the deck
    private List<int> deck = new List<int> { 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6 };
    private int points;
    public Button draw;
    public Button end;
    public TMP_Text pointsDisp;
    private int currCard;

    bool action = false;
    int highScore;
    bool firstShuffle;
    bool restart;


    // Richard adding shit
    public List<GameObject> CardModels;
    private GameObject CurrentModel;
    public Transform Spawnpoint;
    public Button Action;
    public TMP_Text BombChecker;
    public TMP_Text EyeText;
    public TMP_Text ShuffleText;
    public TMP_Text DefuseText;
    public TMP_Text ShiftText;
    public TMP_Text TotalPoints;
    public TMP_Text CardsRemaining;
    private int currentActionCase = -1;

    public AudioSource explode;
    public AudioSource drawing;
    public AudioSource cashOut;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = 0;
        firstShuffle = true;
        Shuffle();
        draw.onClick.AddListener(OnButtonClick);
        end.onClick.AddListener(End);
        Action.onClick.AddListener(OnAction);
        currCard = deck[0];
        Debug.Log(currCard);
        draw.gameObject.SetActive(true);
        end.gameObject.SetActive(true);
        Action.gameObject.SetActive(false);
        EyeText.gameObject.SetActive(false);
        ShuffleText.gameObject.SetActive(false);
        DefuseText.gameObject.SetActive(false);
        ShiftText.gameObject.SetActive(false);
        BombChecker.gameObject.SetActive(false);
        highScore = PlayerPrefs.GetInt("HighScoreKey", 0);
        pointsDisp.gameObject.SetActive(true);
        CardsRemaining.gameObject.SetActive(true);
        action = true;
        restart = false;
    }

    // Update is called once per frame
    void Update()
    {
        pointsDisp.text = "Points: " + points.ToString();
        CardsRemaining.text = "Cards Remaining: " + deck.Count.ToString();
        if (restart)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    void OnButtonClick()
    {
        if (!action)
        {
            points += 2;
        }
        if (deck.Count > 0)
        {
            drawing.Play();
            currCard = deck[0];
            deck.RemoveAt(0);
            Debug.Log(currCard);
            if (CurrentModel != null)
            {
                Destroy(CurrentModel);
            }
            CurrentModel = Instantiate(CardModels[currCard], Spawnpoint.position, Spawnpoint.rotation);
        }
        hidetext();
        switch (currCard)
        {
            case 0:
                Bombed();
                break;

            case 1:
                points += 5;
                break;

            case 2:
                points *= 2;
                break;

            case 3:
                // Enable action button and store case number
                EyeText.gameObject.SetActive(true);
                Action.gameObject.SetActive(true);
                currentActionCase = 3; // Store case number
                action = false;
                break;

            case 4:
                // Enable action button and store case number
                ShuffleText.gameObject.SetActive(true);
                Action.gameObject.SetActive(true);
                currentActionCase = 4; // Store case number
                action = false;
                break;

            case 5:
                // Enable action button and store case number
                ShiftText.gameObject.SetActive(true);
                Action.gameObject.SetActive(true);
                currentActionCase = 5; // Store case number
                action = false;
                break;

            case 6:
                // Enable action button and store case number
                DefuseText.gameObject.SetActive(true);
                Action.gameObject.SetActive(true);
                currentActionCase = 6; // Store case number
                action = false;
                break;
        }
    }

    void OnAction()
    {
        action = true;
        switch (currentActionCase)
        {
            case 3:
                if(points>=5){
                    points -= 5;
                    StartCoroutine(BombDisplay());
                    if(CheckBomb()){
                        BombChecker.text = "There is a Bomb!";
                    }else{
                        BombChecker.text = "There is no Bomb!";
                    }
                }
                break;
            case 4:
                if(points>=3){
                    points -= 3;
                    Shuffle();
                }
                break;
            case 5:
                if(points>=5){
                    points -= 5;
                    ToBottom();
                }
                break;

            case 6:
                if(points>=10){
                    points -= 10;
                    MoveClosest();
                }
                break;
        }

        // Reset the action
        Action.gameObject.SetActive(false);  // Disable the action button
        EyeText.gameObject.SetActive(false);
        ShuffleText.gameObject.SetActive(false);
        ShiftText.gameObject.SetActive(false);
        DefuseText.gameObject.SetActive(false);
        currentActionCase = -1;  // Reset the case
    }

    private bool CheckBomb()
    {
        for (int i = 0; i < 5; i++)
        {
            if (deck[i] == 0)
            {
                return true;
            }
        }
        return false;
    }

    private void Shuffle()
    {
        System.Random rand = new System.Random();
        for (int i = deck.Count - 1; i > 0; i--)
        {
            int j = rand.Next(0, i + 1);
            (deck[i], deck[j]) = (deck[j], deck[i]);
        }
        if (firstShuffle)
        {
            if (CheckBomb())
            {
                for (int i = 0; i < 5; i++)
                {
                    if (deck[i] == 0)
                    {
                        int ind = rand.Next(5, deck.Count);
                        while (deck[ind] == 0)
                        {
                            ind = rand.Next(5, deck.Count);
                        }
                        int temp = deck[ind];
                        deck[ind] = 0;
                        deck[i] = temp;
                    }
                }
            }
            if (deck[0] != 1)
            {
                for (int i = 1; i < deck.Count; i++)
                {
                    if (deck[i] == 1)
                    {
                        int temp = deck[0];
                        deck[0] = 1;
                        deck[i] = temp;
                        break;
                    }

                }
            }
            firstShuffle = false;
        }
    }

    private void ToBottom()
    {
        if (deck.Count >= 2)
        {
            int firstCard = deck[0];
            int secondCard = deck[1];
            deck.RemoveAt(0);
            deck.RemoveAt(0);
            deck.Add(firstCard);
            deck.Add(secondCard);
        }
    }

    private void MoveClosest()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            if (deck[i] == 0)
            {
                deck.RemoveAt(i);
                deck.Add(0);
                return;
            }
        }
    }
    IEnumerator BombDisplay()
    {
        BombChecker.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        BombChecker.gameObject.SetActive(false);
    }
    void hidetext(){
        EyeText.gameObject.SetActive(false);
        ShuffleText.gameObject.SetActive(false);
        DefuseText.gameObject.SetActive(false);
        ShiftText.gameObject.SetActive(false);
    }
    void Bombed()
    {
        // display that they lost
        explode.Play();
        deck.Clear();
        points = 0;
        //have some sort of code to reset deck and game and everything
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void End()
    {
        if (CurrentModel != null)
        {
            Destroy(CurrentModel);
            CurrentModel = null;
        }
        cashOut.Play();
        //display the total score
        if (points > highScore)
        {
            PlayerPrefs.SetInt("HighScoreKey", points);
        }
        draw.gameObject.SetActive(false);
        end.gameObject.SetActive(false);
        Action.gameObject.SetActive(false);
        EyeText.gameObject.SetActive(false);
        ShuffleText.gameObject.SetActive(false);
        DefuseText.gameObject.SetActive(false);
        ShiftText.gameObject.SetActive(false);
        pointsDisp.gameObject.SetActive(false);
        TotalPoints.gameObject.SetActive(true);
        CardsRemaining.gameObject.SetActive(false);
        TotalPoints.text = "Total Points: " + points.ToString() + "\nHigh Score: " + PlayerPrefs.GetInt("HighScoreKey", 0).ToString();
        restart = true;
    }
}

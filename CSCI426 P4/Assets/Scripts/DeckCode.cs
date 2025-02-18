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
    private int cardCost;
    private int pointsPerCard;
    bool action = false;


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
    private int currentActionCase = -1;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        points = 0;
        Shuffle();
        draw.onClick.AddListener(OnButtonClick);
        end.onClick.AddListener(End);
        Action.onClick.AddListener(OnAction);
        currCard = deck[0];
        cardCost = 2;
        pointsPerCard = 4;
        Debug.Log(currCard);
        Action.enabled = false;
        EyeText.enabled = false;
        ShuffleText.enabled = false;
        DefuseText.enabled = false;
        ShiftText.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (deck.Count % 4 == 0)
        {
            cardCost = (int)(cardCost * 1.25);
            pointsPerCard = (int)(pointsPerCard * 1.25);
        }
        pointsDisp.text = "Points: " + points.ToString();
    }

    void OnButtonClick()
    {
        if (deck.Count > 0)
        {
            deck.RemoveAt(0);
            currCard = deck[0];
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
                EyeText.enabled = true;
                Action.enabled = true;
                currentActionCase = 3; // Store case number
                break;

            case 4:
                // Enable action button and store case number
                ShuffleText.enabled = true;
                Action.enabled = true;
                currentActionCase = 4; // Store case number
                break;

            case 5:
                // Enable action button and store case number
                ShiftText.enabled = true;
                Action.enabled = true;
                currentActionCase = 5; // Store case number
                break;

            case 6:
                // Enable action button and store case number
                DefuseText.enabled = true;
                Action.enabled = true;
                currentActionCase = 6; // Store case number
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
        Action.enabled = false;  // Disable the action button
        EyeText.enabled = false;
        ShuffleText.enabled = false;
        ShiftText.enabled = false;
        DefuseText.enabled = false;
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
        BombChecker.enabled = true;
        yield return new WaitForSeconds(3f);
        BombChecker.enabled = false;
    }
    void hidetext(){
        EyeText.enabled = false;
        ShuffleText.enabled = false;
        DefuseText.enabled = false;
        ShiftText.enabled = false;
    }
    void Bombed()
    {
        // display that they lost
        deck.Clear();
        points = 0;
        //have some sort of code to reset deck and game and everything
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void End()
    {
        //display the total score

    }
}

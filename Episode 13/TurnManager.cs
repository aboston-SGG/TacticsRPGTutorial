using UnityEngine;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public Player[] players;
    public int activePlayerIndex = 0;
    public int turn = 1;

    public TextMeshProUGUI turnDisplay;
    public TextMeshProUGUI bannerText;
    public CanvasGroup turnBanner;

    public float fadeSpeed = 0.5f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        turnBanner.alpha = 1;
        bannerText.text = $"{players[activePlayerIndex].playerName}'s Turn";
        turnDisplay.text = $"Turn: {turn}";
    }

    // Update is called once per frame
    void Update()
    {
        if(turnBanner.alpha > 0)
        {
            turnBanner.alpha -= fadeSpeed * Time.deltaTime;
        }
    }

    // Ends the current player's turn, resets their units, and moves to the next player
    public void EndTurn()
    {
        players[activePlayerIndex].ResetUnits();
        activePlayerIndex = (activePlayerIndex + 1) % players.Length;

        if (activePlayerIndex == 0)
        {
            turn++;
            turnDisplay.text = $"Turn: {turn}";
        }

        bannerText.text = $"{players[activePlayerIndex].playerName}'s Turn";
        turnBanner.alpha = 1;
    }
}

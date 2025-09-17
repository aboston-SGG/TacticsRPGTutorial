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
        players[activePlayerIndex].isPlayerTurn = true;
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

        if (players[activePlayerIndex].readyToEndTurn)
        {
            EndTurn();
        }
    }

    // Ends the current player's turn, resets their units, and moves to the next player
    public void EndTurn()
    {
        Player currentPlayer = players[activePlayerIndex];

        currentPlayer.ResetUnits();
        currentPlayer.isPlayerTurn = false;
        activePlayerIndex = (activePlayerIndex + 1) % players.Length;
        currentPlayer = players[activePlayerIndex];
        currentPlayer.isPlayerTurn = true;
        currentPlayer.readyToEndTurn = false;

        if (currentPlayer.GetComponent<PlayerAI>())
        {
            PlayerAI ai = currentPlayer.GetComponent<PlayerAI>();
            ai.units.Clear();
            ai.units.AddRange(currentPlayer.playerUnits);
        }

        if (activePlayerIndex == 0)
        {
            turn++;
            turnDisplay.text = $"Turn: {turn}";
        }

        bannerText.text = $"{currentPlayer.playerName}'s Turn";
        turnBanner.alpha = 1;
    }
}

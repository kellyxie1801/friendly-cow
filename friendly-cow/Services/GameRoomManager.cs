namespace friendly_cow.Services;

public class GameRoomManager
{
    // Stores the current state of any active room
    private Dictionary<string, GameState> _rooms = new();

    // Event triggered whenever a room's state changes
    public event Action<string>? OnGameUpdated;

    public GameState GetRoom(string roomCode)
    {
        if (!_rooms.ContainsKey(roomCode))
        {
            _rooms[roomCode] = new GameState(); // Create room if it doesn't exist
        }
        return _rooms[roomCode];
    }

    public void MakeMove(string roomCode, int playerNumber, string move)
    {
        var room = GetRoom(roomCode);
        
        // Record the move
        if (playerNumber == 1) room.Player1Move = move;
        if (playerNumber == 2) room.Player2Move = move;

        // If both players have moved, determine the winner
        if (!string.IsNullOrEmpty(room.Player1Move) && !string.IsNullOrEmpty(room.Player2Move))
        {
            room.Result = DetermineWinner(room.Player1Move, room.Player2Move);
        }

        // Notify all connected browsers to update their UI
        OnGameUpdated?.Invoke(roomCode);
    }

    public async Task StartGameSession(string roomCode, int seconds)
    {
        var room = GetRoom(roomCode);
        room.IsStarted = true;
        room.Player1Score = 0;
        room.Player2Score = 0;
        room.Result = "";
        OnGameUpdated?.Invoke(roomCode);

        while (seconds > 0)
        {
            await Task.Delay(1000);
            seconds--;
        }

        room.IsStarted = false;
        
        if (room.Player1Score > room.Player2Score)
            room.Result = $"Player 1 Wins with {room.Player1Score} clicks!";
        else if (room.Player2Score > room.Player1Score)
            room.Result = $"Player 2 Wins with {room.Player2Score} clicks!";
        else
            room.Result = "It's a tie!";

        OnGameUpdated?.Invoke(roomCode);
    }

    public void AddClick(string roomCode, int playerNumber)
    {
        var room = GetRoom(roomCode);
        if (room.IsStarted)
        {
            if (playerNumber == 1) room.Player1Score++;
            if (playerNumber == 2) room.Player2Score++;

            OnGameUpdated?.Invoke(roomCode);
        }
    }

    public void ResetGame(string roomCode)
    {
        if (_rooms.ContainsKey(roomCode))
        {
            _rooms[roomCode] = new GameState();
            OnGameUpdated?.Invoke(roomCode);
        }
    }

    private string DetermineWinner(string p1, string p2)
    {
        if (p1 == p2) return "It's a tie!";
        
        if ((p1 == "Rock" && p2 == "Scissors") ||
            (p1 == "Paper" && p2 == "Rock") ||
            (p1 == "Scissors" && p2 == "Paper"))
        {
            return "Player 1 Wins!";
        }
        
        return "Player 2 Wins!";
    }

    public void SetPlayerIcon(string roomCode, int playerNumber, string icon)
    {
        var room = GetRoom(roomCode);
        if (playerNumber == 1) room.Player1Icon = icon;
        if (playerNumber == 2) room.Player2Icon = icon;

        OnGameUpdated?.Invoke(roomCode);
    }
}

// A simple class to hold the data for a single game room
public class GameState
{
    public string Player1Move { get; set; } = "";
    public string Player2Move { get; set; } = "";
    public string Result { get; set; } = "";

    // Für clicker
    public int Player1Score { get; set; } = 0;
    public int Player2Score { get; set; } = 0;
    public bool IsStarted { get; set; } = false;

    public string Player1Icon { get; set; } = "fa-solid fa-dog";
    public string Player2Icon { get; set; } = "fa-solid fa-cat";
}


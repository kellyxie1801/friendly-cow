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
}

// A simple class to hold the data for a single game room
public class GameState
{
    public string Player1Move { get; set; } = "";
    public string Player2Move { get; set; } = "";
    public string Result { get; set; } = "";
}
namespace Tick_tak_toe.Models;

public class Room
{
    public string RoomName { get; set; }
    public List<string> PlayersIdInRoom { get; set; } = new List<string>();
    public List<string> PlayersNameInRoom { get; set; } = new List<string>();
    public Dictionary<string, bool> PlayersReady { get; set; } = new Dictionary<string, bool>();
    public bool GameIsStarted { get; set; } = false;
    public string[,] Board = new string[3, 3];
    public string PlayersTurn { get; set; }
    public int MoveCounter = 0;
    public int PlayerCounter = 0;
}
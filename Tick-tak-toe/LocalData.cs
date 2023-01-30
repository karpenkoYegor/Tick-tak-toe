using Tick_tak_toe.Models;

namespace Tick_tak_toe;

public static class LocalData
{
    public static List<string> AuthUsers = new List<string>();
    public static Dictionary<string, Room> Rooms = new Dictionary<string, Room>();
    public static int RoomsInGame = 0;
}
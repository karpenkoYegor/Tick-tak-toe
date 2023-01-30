using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Tick_tak_toe.Hubs;

public class RoomHub : Hub
{
    private IHubContext<LobbyHub> _lobbyContext;
    public RoomHub(IHubContext<LobbyHub> lobbyContext)
    {
        _lobbyContext = lobbyContext;
    }
    public async Task JoinRoom(string nameRoom)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, nameRoom);
        var room = LocalData.Rooms[nameRoom];
        var name = Context.User.Identity.Name;
        if (!room.PlayersIdInRoom.Exists(p => p.Equals(name)))
        {
            room.PlayersIdInRoom.Add(Context.ConnectionId);
            room.PlayersNameInRoom.Add(name);
            room.PlayersReady.Add(name, false);
        }
        await Clients.
            Caller.
            SendAsync("join", $"{name}");
        await Clients.
            OthersInGroup(nameRoom).
            SendAsync("playerJoined", $"{name}");
        await Clients.Group(nameRoom).SendAsync("Notify", $"{name}");
        Context.Items.Add("nameRoom", nameRoom);
    }

    public async Task ReadyToStart(string nameRoom)
    {
        var playersReady = LocalData.Rooms[nameRoom].PlayersReady;
        playersReady[Context.User.Identity.Name] = !playersReady[Context.User.Identity.Name];
        if (playersReady.Values.Where(o => o).ToArray().Length == 2)
        {
            await Clients.
                Client(LocalData.Rooms[nameRoom].PlayersIdInRoom[0]).
                SendAsync("changeTurn");
            await Clients.
                Group(nameRoom).
                SendAsync("changeStatusTurn", LocalData.Rooms[nameRoom].PlayersNameInRoom[0]);
            await Clients.Group(nameRoom).SendAsync("startGame");
            LocalData.Rooms[nameRoom].PlayersTurn = "x";
            LocalData.Rooms[nameRoom].GameIsStarted = true;
        }
        await Clients.Group(nameRoom).SendAsync("readyToStart", $"{Context.User.Identity.Name}", playersReady[Context.User.Identity.Name]);
        await Clients.Caller.SendAsync("readyBtn", $"{Context.User.Identity.Name}", playersReady[Context.User.Identity.Name]);
    }

    public async void MovePlayer(string nameRoom, string indexCell)
    {
        int row = Convert.ToInt32(indexCell[0].ToString());
        int col = Convert.ToInt32(indexCell[1].ToString());
        var room = LocalData.Rooms[nameRoom];
        var data = new List<string>()
        {
            $"{Context.User.Identity.Name}", room.PlayersTurn, row.ToString(), col.ToString()
        };
        room.Board[row, col] = room.PlayersTurn;
        CheckWin();
        room.PlayersTurn = room.PlayersTurn == "o" ? "x" : "o";
        var currentNameTurn = 
            Context.User.Identity.Name == LocalData.Rooms[nameRoom].PlayersNameInRoom[0] ? 
                LocalData.Rooms[nameRoom].PlayersNameInRoom[1] : 
                LocalData.Rooms[nameRoom].PlayersNameInRoom[0];
        await Clients.OthersInGroup(nameRoom).SendAsync("changeTurn");
        await Clients.Group(nameRoom).SendAsync("changeStatusTurn", currentNameTurn);
        await Clients.Group(nameRoom).SendAsync("move", data.ToArray());
    }

    public async void EndGame()
    {
        var nameRoom = Context.Items["nameRoom"].ToString();
        var room = LocalData.Rooms[nameRoom];
        room.Board = new string[3, 3];
        room.MoveCounter = 0;
        room.PlayersTurn = "x";
        foreach (var key in room.PlayersReady.Keys)
        {
            room.PlayersReady[key] = false;
        }
    }
    public async void LeaveFromRoom()
    {
        var nameRoom = Context.Items["nameRoom"].ToString();
        LocalData.Rooms.Remove(nameRoom);
        await Clients.Group(nameRoom).SendAsync("playerLeave");
    }
    public async void RestartRoom()
    {
        var nameRoom = Context.Items["nameRoom"].ToString();
        var playersReady = LocalData.Rooms[nameRoom].PlayersReady;
        playersReady[Context.User.Identity.Name] = true;
        if (playersReady.Values.Where(o => o).ToArray().Length == 2)
        {
            await Clients.Group(nameRoom).SendAsync("restart");
        }
    }

    public async void SetReadyToRestart()
    {
        
    }

    public override Task OnConnectedAsync()
    {
        return base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var nameRoom = Context.Items["nameRoom"].ToString();
        var namePlayer = Context.User.Identity.Name;
        LocalData.Rooms[nameRoom].PlayerCounter--;
        foreach (var room in LocalData.Rooms)
        {
            _lobbyContext.Clients.All.SendAsync("joinRoom", room.Key, LocalData.Rooms[room.Key].PlayerCounter).GetAwaiter().GetResult();
        }
        try
        {
            LocalData.Rooms[nameRoom].PlayersReady[namePlayer] = false;
            Clients.Group(nameRoom).SendAsync("leave", namePlayer).GetAwaiter();
            Groups.RemoveFromGroupAsync(Context.ConnectionId, nameRoom).GetAwaiter();
            LocalData.Rooms[nameRoom].PlayersIdInRoom.Remove(namePlayer);
            LocalData.Rooms[nameRoom].PlayersReady.Remove(namePlayer);
        }
        catch (Exception e)
        {
            
        }
    }

    public void CheckWin()
    {
        
        var nameRoom = Context.Items["nameRoom"].ToString();
        var name = Context.User.Identity.Name;
        var board = LocalData.Rooms[nameRoom].Board;
        LocalData.Rooms[nameRoom].MoveCounter++;
        if (!String.IsNullOrEmpty(board[0,0]) && board[0,0] == board[0, 1] && board[0, 1] == board[0, 2])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if (!String.IsNullOrEmpty(board[1, 0]) && board[1, 0] == board[1, 1] && board[1, 1] == board[1, 2])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if (!String.IsNullOrEmpty(board[2, 0]) && board[2, 0] == board[2, 1] && board[2, 1] == board[2, 2])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if(!String.IsNullOrEmpty(board[0, 0]) && board[0, 0] == board[1, 0] && board[1, 0] == board[2, 0])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if(!String.IsNullOrEmpty(board[0, 1]) && board[0, 1] == board[1, 1] && board[1, 1] == board[2, 1])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if(!String.IsNullOrEmpty(board[0, 2]) && board[0, 2] == board[1, 2] && board[1, 2] == board[2, 2])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if (!String.IsNullOrEmpty(board[0, 0]) && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if (!String.IsNullOrEmpty(board[0, 2]) && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 2])
            Clients.Group(nameRoom).SendAsync("endMatch", "Победа " + name).GetAwaiter().GetResult();
        else if (LocalData.Rooms[nameRoom].MoveCounter == 9)
            Clients.Group(nameRoom).SendAsync("endMatch", "Ничья").GetAwaiter().GetResult();
    }
}


using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tick_tak_toe.Hubs;
using Tick_tak_toe.Models;

namespace Tick_tak_toe.Controllers;

public class RoomController : Controller
{
    private IHubContext<RoomHub> _roomContext;
    private IHubContext<LobbyHub> _lobbyContext;
    public RoomController(IHubContext<RoomHub> context, IHubContext<LobbyHub> lobbyContext)
    {
        _roomContext = context;
        _lobbyContext = lobbyContext;
    }
    public IActionResult Index(string roomName)
    {
        
        var model = new RoomViewModel()
        {
            NameRoom = roomName,
            NamePlayer = User.Identity.Name
        };
        LocalData.Rooms[roomName].PlayerCounter++;
        _lobbyContext.Clients.All.SendAsync("joinRoom", roomName, LocalData.Rooms[roomName].PlayerCounter).GetAwaiter().GetResult();
        return View(model);
    }

    public IActionResult Lobby()
    {
        return RedirectToAction("Lobby", "Lobby");
    }
}
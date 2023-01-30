using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Tick_tak_toe.Hubs;
using Tick_tak_toe.Models;

namespace Tick_tak_toe.Controllers;
[Authorize]
public class LobbyController : Controller
{
    private IHubContext<LobbyHub> _lobbyContext;
    public LobbyController(IHubContext<LobbyHub> context)
    {
        _lobbyContext = context;
    }
    public IActionResult Lobby()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Lobby(LobbyViewModel model)
    {
        if (ModelState.IsValid)
        {
            LocalData.Rooms.Add(model.RoomName, new Room());
            _lobbyContext.Clients.All.SendAsync("createRoom", model.RoomName).GetAwaiter().GetResult();
        }
        
        return RedirectToAction("Lobby");
    }

    public bool LobbyIsNotCreated(string RoomName)
    {
        return !LocalData.Rooms.ContainsKey(RoomName);
    }
}
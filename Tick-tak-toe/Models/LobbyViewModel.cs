using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Tick_tak_toe.Models;

public class LobbyViewModel
{
    [Required(ErrorMessage = "Введите имя комнаты")]
    [Remote(action: "LobbyIsNotCreated", controller: "Lobby", ErrorMessage = "Комната с таким именем уже существует")]
    public string RoomName { get; set; }
}
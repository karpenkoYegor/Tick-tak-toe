using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Tick_tak_toe.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите значение")]
    [Remote(action: "UserIsNotAuth", controller: "Account", ErrorMessage = "Пользователь уже авторизован, выберите другое имя")]
    public string Login { get; set; }
}
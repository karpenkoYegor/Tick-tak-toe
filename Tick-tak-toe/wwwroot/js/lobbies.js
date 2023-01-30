var connectionLobby = new signalR.HubConnectionBuilder().withUrl("/hubs/lobby").build();

var createBtn = document.addEventListener("onClick",
    () => connectionLobby.send("CreatedNewRoom"));

connectionLobby.on("createRoom", (value) => {
    var newCountSpan = document.getElementById("createdRooms");
    newCountSpan.innerHTML += `<div class="row mb-3">
                                   <div class="card" style="width: 18rem;">
                                       <ul class="list-group list-group-flush">
                                           <li class="list-group-item" id="${value}">${value}</li>
                                           <li class="list-group-item" id="roomName${value}">0/2</li>
                                           <li class="list-group-item">
                                               <a id="enterRoom${value}" class="btn btn-primary" href="/Room/Index?roomName=${value}">Войти</a>
                                           </li>
                                       </ul>
                                   </div>
                               </div>`
});

connectionLobby.on("joinRoom", (nameRoom, countPlayers) => {
    var playerCountStatus = document.getElementById("roomName" + nameRoom);
    var enterBtn = document.getElementById('enterRoom' + nameRoom);
    console.log(countPlayers, playerCountStatus);
    playerCountStatus.innerText = countPlayers + '/2';
    if (parseInt(countPlayers) === 2) {
        console.log(enterBtn);
        enterBtn.style.display = "none";
    } else {
        enterBtn.style.display = "inline";
    }
});

function fulfilled() {
    console.log("Success");
}

function rejected() {

}
connectionLobby.start().then(fulfilled, rejected);
var connectionRoom = new signalR.HubConnectionBuilder().withUrl("/hubs/room").build();
var containerDiv = document.getElementById("room");
var gameDiv = document.getElementById("game");
var roomName = document.getElementById("roomName").value;
var playerName = document.getElementById("playerName").value;
var playerTurnContainer = document.getElementById("turn");
var playerTurnText = document.getElementById("turnPlayer");
var modalText = document.getElementById("modal-body");
var formRoom = document.getElementById("formRoom");
var cellsArray = [document.querySelectorAll(".row-1"),
document.querySelectorAll(".row-2"),
    document.querySelectorAll(".row-3")];

var isPlayerTurn = false;

formRoom.hidden = false;

var readyFunction = () => {
    connectionRoom.invoke("ReadyToStart", roomName);
};

connectionRoom.on("Notify", (value) => {
    console.log(value, "connected to group");
    playerNickname = value;
});

connectionRoom.on("join", (value) => {
    createPlayerStatus(value);
});

connectionRoom.on("startGame", () => {
    console.log("start");
    gameDiv.hidden = false;
    formRoom.hidden = true;
});

connectionRoom.on("move",
    (values) => {
        var x = parseInt(values[2]);
        var y = parseInt(values[3]);
        cellsArray[x][y].innerHTML += `<p class="symbol">${values[1]}</p>`;
        $(cellsArray[x][y]).off("click");
    });

connectionRoom.on("endMatch",
    (value) => {
        $('#staticBackdrop').modal("show");
        modalText.innerText = value;
        connectionRoom.send("EndGame");
        $('#restartBtn').click(() => {
            for (var i = 0; i < cellsArray.length; i++) {
                for (var j = 0; j < cellsArray[i].length; j++) {
                    cellsArray[i][j].innerHTML = "";
                }
            }
            modalText.innerText = "Ожидайте ответа от соперника";
            connectionRoom.send("RestartRoom");
        });
        $('#exitBtn').click(() => {
            window.location.href = 'Lobby';
            connectionRoom.send("LeaveFromRoom");
        });
    });

connectionRoom.on("playerLeave",
    () => {
        modalText.innerText = "Ваш соперник вышел из комнаты, вы будете перенаправлены в лобби через 3 секунды";
        setTimeout(() => {
                window.location.href = 'Lobby';
            },
            3000);
    });

connectionRoom.on("restart",
    () => {
        $(".cell").click(e => {
            if (isPlayerTurn) {
                connectionRoom.invoke("MovePlayer",
                    roomName,
                    e.target.classList[1]);
                isPlayerTurn = false;
            }
        });
        $('#staticBackdrop').modal("hide");
    });

connectionRoom.on("playerJoined", (value) => {
    console.log(value, "joined");
    containerDiv.innerHTML += `<div class="row mb-3" id="${value}RowStatus">
                                    <div class="col-4">
                                        <p>${value}</p>
                                    </div>
                                    <div class="col-2" id="${value}status">
                                        Не готов
                                    </div>
                               </div>`;
});

connectionRoom.on("readyToStart", (nameRoom, ready) => {
    var status = document.getElementById(`${nameRoom}status`);
    if (ready) {
        status.innerHTML = "Готов";

    } else {
        status.innerHTML = "Не готов";
    }
        
});

connectionRoom.on("readyBtn", (nameRoom, ready) => {
    var readyBtn = document.getElementById('readyBtn');
    if (ready) {
        readyBtn.innerHTML = "Не готов";

    } else {
        readyBtn.innerHTML = "Готов";
    }

});

connectionRoom.on("leave", (value) => {
    var leaveDiv = document.getElementById(`${value}RowStatus`);
    leaveDiv.remove();
});

connectionRoom.on("changeTurn",
    () => {
        isPlayerTurn = true;
    });

connectionRoom.on("changeStatusTurn",
    (value) => {
        console.log(`Ход ${value}`);
        playerTurnText.innerText = `Ход ${value}`;
    });

function fulfilled() {
    console.log("Success");
    connectionRoom.invoke("JoinRoom", roomName);
    $(".cell").click(e => {
        if (isPlayerTurn) {
            connectionRoom.invoke("MovePlayer",
                roomName,
                e.target.classList[1]);
            isPlayerTurn = false;
        }
    });
    $("#readyBtn").click(readyFunction);
}

function rejected() {

}
connectionRoom.start().then(fulfilled, rejected);

function createPlayerStatus(name) {
    var rowStatus = document.getElementById(`${name}RowStatus`);
    if (rowStatus === null) {
        containerDiv.innerHTML += `<div class="row mb-3" id="${name}RowStatus">
                                    <div class="col-4">
                                        <p>${name}</p>
                                    </div>
                                    <div class="col-2" id="${name}status">
                                        Не готов
                                    </div>
                               </div>`
    }

}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/dummy/updateHub")
    .build();

connection.on("ReceiveUpdate", (prNo) => {
    const targetDiv = document.querySelector(`[data-prno="${prNo}"]`);

    if (targetDiv) {
        targetDiv.classList.add("active");
    }
});

connection.start()
    .then(() => console.log("SignalR Connected"))
    .catch((err) => console.error(err));
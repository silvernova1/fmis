const connection = new signalR.HubConnectionBuilder()
    .withUrl("/dummy/updatePrHub")
    .build();

connection.on("PrUpdateRmop", (prNo, htmlContent) => {
    const rmopDiv = document.querySelector(`.rmop-step[data-prno="${prNo}"]`);
    const rmopDate = document.querySelector(`.rmop-date[data-prno="${prNo}"]`);

    if (rmopDiv) {
        rmopDiv.classList.add("active");
    }

    if (rmopDate) {
        rmopDate.innerHTML = htmlContent;
        console.log("rmopDate found");
    } else {
        console.log("rmopDate not found");
    }
});

connection.on("PrUpdateCanvass", (prNo, htmlContent) => {
    const canvassDiv = document.querySelector(`.canvass-step[data-prno="${prNo}"]`);
    const canvassDate = document.querySelector(`.canvass-date[data-prno="${prNo}"]`);

    if (canvassDiv) {
        canvassDiv.classList.add("active");
    }

    if (canvassDate) {
        canvassDate.innerHTML = htmlContent;
        console.log("canvassDate found");
    } else {
        console.log("canvassDate not found");
    }
});

connection.on("PrUpdateAbstract", (prNo, htmlContent) => {
    const abstractDiv = document.querySelector(`.abstractNo-step[data-prno="${prNo}"]`);
    const abstractDate = document.querySelector(`.abstractNo-date[data-prno="${prNo}"]`);

    if (abstractDiv) {
        abstractDiv.classList.add("active");
    }

    if (abstractDate) {
        abstractDate.innerHTML = htmlContent;
        console.log("abstractDate found");
    } else {
        console.log("abstractDate not found");
    }
});

connection.on("PrUpdatePo", (prNo, htmlContent) => {
    const poDiv = document.querySelector(`.po-step[data-prno="${prNo}"]`);
    const poDate = document.querySelector(`.po-date[data-prno="${prNo}"]`);

    if (poDiv) {
        poDiv.classList.add("active");
    }

    if (poDate) {
        poDate.innerHTML = htmlContent;
        console.log("poDate found");
    } else {
        console.log("poDate not found");
    }
});

connection.on("PrUpdateTwg", (prNo, htmlContent) => {
    const twgDiv = document.querySelector(`.twg-step[data-prno="${prNo}"]`);
    const twgDate = document.querySelector(`.twg-date[data-prno="${prNo}"]`);

    if (twgDiv) {
        twgDiv.classList.add("active");
    }

    if (twgDate) {
        twgDate.innerHTML = htmlContent;
        console.log("twgDate found");
    } else {
        console.log("twgDate not found");
    }
});

connection.start()
    .then(() => console.log("SignalR Connected"))
    .catch((err) => console.error(err));
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/dummy/updatePrHub")
    .build();

connection.on("PrUpdateRmop", (prNo, prTrackingDate) => {
    const rmopDiv = document.querySelector(`.rmop-step[data-prno="${prNo}"]`);
    if (rmopDiv) {
        rmopDiv.classList.add("active");
        const stepDateDiv = rmopDiv.querySelector(".step-date");
        if (stepDateDiv) {
            if (prTrackingDate !== null) {
                const formattedDate = new Date(prTrackingDate).toLocaleDateString("en-US", {
                    month: "short",
                    day: "numeric",
                    year: "numeric"
                });
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${formattedDate}`;
            } else {
                const rmopDate = "";
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${rmopDate}`;
            }
        }
    }
});

connection.on("PrUpdateCanvass", (prNo, prTrackingDate) => {
    const canvassDiv = document.querySelector(`.canvass-step[data-prno="${prNo}"]`);
    if (canvassDiv) {
        canvassDiv.classList.add("active");
        const stepDateDiv = canvassDiv.querySelector(".step-date");
        if (stepDateDiv) {
            if (prTrackingDate !== null) {
                const formattedDate = new Date(prTrackingDate).toLocaleDateString("en-US", {
                    month: "short",
                    day: "numeric",
                    year: "numeric"
                });
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${formattedDate}`;
            } else {
                const canvassDate = "";
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${canvassDate}`;
            }
        }
    }

});

connection.on("PrUpdateAbstract", (prNo, prTrackingDate) => {
    const abstractDiv = document.querySelector(`.abstractNo-step[data-prno="${prNo}"]`);
    if (abstractDiv) {
        abstractDiv.classList.add("active");
        const stepDateDiv = abstractDiv.querySelector(".step-date");
        if (stepDateDiv) {
            if (prTrackingDate !== null) {
                const formattedDate = new Date(prTrackingDate).toLocaleDateString("en-US", {
                    month: "short",
                    day: "numeric",
                    year: "numeric"
                });
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${formattedDate}`;
            } else {
                const abstractNoDate = "";
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${abstractNoDate}`;
            }
        }
    }
});

connection.on("PrUpdatePo", (prNo, prTrackingDate) => {
    const poDiv = document.querySelector(`.po-step[data-prno="${prNo}"]`);
    if (poDiv) {
        poDiv.classList.add("active");
        const stepDateDiv = poDiv.querySelector(".step-date");
        if (stepDateDiv) {
            if (prTrackingDate !== null) {
                const formattedDate = new Date(prTrackingDate).toLocaleDateString("en-US", {
                    month: "short",
                    day: "numeric",
                    year: "numeric"
                });
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${formattedDate}`;
            } else {
                const poDate = "";
                stepDateDiv.innerHTML = `&nbsp; &nbsp; &nbsp; &nbsp; ${poDate}`;
            }
        }
    }
});

connection.start()
    .then(() => console.log("SignalR Connected"))
    .catch((err) => console.error(err));
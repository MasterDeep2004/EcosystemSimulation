const canvas = document.getElementById("ecosystemCanvas");
const ctx = canvas.getContext("2d");
const status = document.getElementById("status");

const evt = new EventSource("/api/simulation/stream");

evt.onopen = () => status.innerText = "Status: Running simulation...";

evt.onmessage = e => {
    const data = JSON.parse(e.data);

    // Clear canvas
    ctx.fillStyle = "#181818";
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    // Plants
    ctx.fillStyle = "green";
    ctx.fillRect(50, canvas.height - data.Plants * 0.2, 100, data.Plants * 0.2);

    // Herbivores
    ctx.fillStyle = "orange";
    ctx.fillRect(200, canvas.height - data.Herbivores * 0.5, 50, data.Herbivores * 0.5);

    // Carnivores
    ctx.fillStyle = "red";
    ctx.fillRect(300, canvas.height - data.Carnivores * 2, 30, data.Carnivores * 2);
};

evt.onerror = () => {
    status.innerText = "Status: Connection lost.";
    evt.close();
};

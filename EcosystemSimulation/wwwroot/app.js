const canvas = document.getElementById("ecosystemCanvas");
const ctx = canvas.getContext("2d");

const status = document.getElementById("status");
const generation = document.getElementById("generation");
const eventName = document.getElementById("event");

const source = new EventSource("/api/simulation/stream");

source.onopen = () => {
    status.innerText = "Simulation Running";
};

source.onmessage = event => {
    const data = JSON.parse(event.data);

    ctx.clearRect(
        0,
        0,
        canvas.width,
        canvas.height
    );

    generation.innerText =
        `Generation: ${data.Generation}`;

    eventName.innerText =
        data.EventName
            ? `Event: ${data.EventName}`
            : "Event: None";

    drawBar(
        120,
        data.Plants * 0.15,
        "green",
        "Plants",
        data.Plants
    );

    drawBar(
        400,
        data.Herbivores * 0.4,
        "orange",
        "Herbivores",
        data.Herbivores
    );

    drawBar(
        680,
        data.Carnivores * 2,
        "red",
        "Carnivores",
        data.Carnivores
    );
};


function drawBar(
    x,
    height,
    color,
    label,
    value
) {
    const baseY = 400;

    ctx.fillStyle = color;

    ctx.fillRect(
        x,
        baseY - height,
        120,
        height
    );

    ctx.fillStyle = "white";

    ctx.font = "18px Arial";

    ctx.fillText(
        label,
        x + 10,
        430
    );

    ctx.fillText(
        value,
        x + 20,
        Math.max(30, baseY - height - 10)
    );
}


source.onerror = () => {
    status.innerText =
        "Connection Lost";

    source.close();
};

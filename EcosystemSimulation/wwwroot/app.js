const canvas = document.getElementById("ecosystemCanvas");
const ctx = canvas.getContext("2d");

const status = document.getElementById("status");

const source = new EventSource("/api/simulation/stream");

source.onopen = () => {
    status.innerText = "Simulation Running";
};

source.onmessage = event => {

    const data = JSON.parse(event.data);

    ctx.clearRect(0,0,canvas.width,canvas.height);

    const baseY = 400;

    drawBar(120,data.Plants*0.15, "green","Plants",data.Plants);

    drawBar(400,data.Herbivores*0.4,"orange","Herbivores",data.Herbivores);

    drawBar(680,data.Carnivores*2,"red","Carnivores",data.Carnivores);
};

function drawBar(x,height,color,label,value){

    ctx.fillStyle=color;

    ctx.fillRect(x,400-height,120,height);

    ctx.fillStyle="white";

    ctx.font="18px Arial";

    ctx.fillText(label,x+10,430);

    ctx.fillText(value,x+20,380-height);
}

source.onerror = ()=>{

    status.innerText="Connection Lost";

    source.close();

};

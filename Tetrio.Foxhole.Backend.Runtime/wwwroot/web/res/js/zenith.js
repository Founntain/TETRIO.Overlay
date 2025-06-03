let usernameInfo = document.getElementById("usernameInfo").innerText;

//get all fields
let thisWeeksNormalScore = document.getElementById("thisWeeksNormalScore");
let pps = document.getElementById("pps");
let apm = document.getElementById("apm");
let vs = document.getElementById("vs");
let normalPersonalBest = document.getElementById("normalPersonalBest");
let mods = document.getElementById("mods");

let thisWeeksExpertScore = document.getElementById("thisWeeksExpertScore");
let ppsExpert = document.getElementById("ppsExpert");
let apmExpert = document.getElementById("apmExpert");
let vsExpert = document.getElementById("vsExpert");
let expertPersonalBest = document.getElementById("expertPersonalBest");
let modsExpert = document.getElementById("modsExpert");

let expertContainer = document.getElementById("expertContainer");

function updateStats() {
    let url = `${baseUrl}/zenith/${usernameInfo}/stats`

    console.log(url)

    fetch(url)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }

            return response.json();
        })
        .then(data => {
            console.log(data);

            // NORMAL


            let normalString = thisWeeksNormalScore.innerText;
            let cleanNormalString = normalString.replace(/[^0-9.]/g, '');
            cleanNormalString = cleanNormalString.replace(/,/g, '');

            animateValue(thisWeeksNormalScore, parseFloat(cleanNormalString), data.zenith.altitude, animationDuration, 0, "", " M");

            animateValue(pps, parseFloat(pps.innerText), data.zenith.pps, animationDuration, 0, "", " PPS");
            animateValue(apm, parseFloat(apm.innerText), data.zenith.apm, animationDuration, 0, "", " APM");
            animateValue(vs, parseFloat(vs.innerText), data.zenith.vs, animationDuration, 0, "", " VS");

            normalString = normalPersonalBest.innerText;
            cleanNormalString = normalString.replace(/[^0-9.]/g, '');
            cleanNormalString = cleanNormalString.replace(/,/g, '');

            animateValue(normalPersonalBest, parseFloat(cleanNormalString), data.zenith.best, animationDuration, 0, "", " M");

            mods.innerHTML = "";

            data.zenith.mods?.forEach(mod => {
                const img = document.createElement('img');
                img.classList.add("mod");
                img.src = `${imgUrl}${mod}.png`;
                mods.appendChild(img);
            });

            // EXPERT
            if(data.expertPlayed == false){
                expertContainer.style.display = "none";
            }else{
                expertContainer.style.display = "block";

                let expertString = thisWeeksExpertScore.innerText;
                let cleanExpertString = expertString.replace(/[^0-9.]/g, '');
                cleanExpertString = cleanExpertString.replace(/,/g, '');

                animateValue(thisWeeksExpertScore, parseFloat(cleanExpertString), data.zenithExpert.altitude, animationDuration, 0, "", " M");

                animateValue(ppsExpert, parseFloat(ppsExpert.innerText), data.zenithExpert.pps, animationDuration, 0, "", " PPS");
                animateValue(apmExpert, parseFloat(apmExpert.innerText), data.zenithExpert.apm, animationDuration, 0, "", " APM");
                animateValue(vsExpert, parseFloat(vsExpert.innerText), data.zenithExpert.vs, animationDuration, 0, "", " VS");

                expertString = expertPersonalBest.innerText;
                cleanExpertString = expertString.replace(/[^0-9.]/g, '');
                cleanExpertString = cleanExpertString.replace(/,/g, '');

                animateValue(expertPersonalBest, parseFloat(cleanExpertString), data.zenithExpert.best, animationDuration, 0, "", " M");

                modsExpert.innerHTML = "";

                data.zenithExpert.mods.forEach(mod => {
                    const img = document.createElement('img');
                    img.classList.add("mod");
                    img.src = `${imgUrl}${mod}.png`;
                    modsExpert.appendChild(img);
                });
            }
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

updateStats();

setInterval(updateStats, 30000);
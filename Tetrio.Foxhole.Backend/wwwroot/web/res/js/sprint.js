let usernameInfo = document.getElementById("usernameInfo").innerText;

let username = document.getElementById("username");
let finalTime = document.getElementById("finalTime");
let pps = document.getElementById("pps");
let kpp = document.getElementById("kpp");
let kps = document.getElementById("kps");
let finesse = document.getElementById("finesse");
let globalRank = document.getElementById("globalRank");
let localRank = document.getElementById("localRank");
let countryImage = document.getElementById("countryImage");

username.innerText = usernameInfo.toUpperCase();

function updateStats() {
    let url = `${baseUrl}/sprint/${usernameInfo}/stats`

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

            animateValue(finalTime, finalTime.innerText, data.timeString, animationDuration, 2, "", "")

            animateValue(pps, parseFloat(pps.innerText), data.pps, animationDuration, 0, "", "");
            animateValue(kpp, parseFloat(kpp.innerText), data.kpp, animationDuration, 0, "", "");
            animateValue(kps, parseFloat(kps.innerText), data.kps, animationDuration, 0, "", "");
            animateValue(finesse, parseFloat(finesse.innerText), data.finesse, animationDuration, 0, "", "");

            animateValue(globalRank, parseFloat(globalRank.innerText), data.globalRank, animationDuration, 1, "", "");
            animateValue(localRank, parseFloat(localRank.innerText), data.localRank, animationDuration, 1, "", "");

            if (data.country == null)
                countryImage.style.display = "none";
            else {
                countryImage.style.display = "block";
                countryImage.src = `https://tetr.io/res/flags/${data.country.toLowerCase()}.png`;
            }
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

updateStats();

setInterval(updateStats, 5000 * 60);
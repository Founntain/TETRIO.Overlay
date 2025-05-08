let usernameInfo = document.getElementById("usernameInfo").innerText;
let achievementInfo = document.getElementById("achievementInfo").innerText;

let username = document.getElementById("username");
let achievementName = document.getElementById("achievementName");
let placement = document.getElementById("placement");
let achievementValue = document.getElementById("achievementValue");
let achievementAdditionalValue = document.getElementById("achievementAdditionalValue");
let globalRank = document.getElementById("globalRank");
let countryImage = document.getElementById("countryImage");

username.innerText = usernameInfo.toUpperCase();

function updateStats() {
    let url = `${baseUrl}/achievement/${achievementInfo}/${usernameInfo}/data`

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

            achievementName.innerText = data.achievementName.toUpperCase();

            animateValue(achievementValue, parseFloat(achievementValue.innerText), data.user.value, animationDuration, 0, "", "")

            if((data.user.additionalData ?? 0) > 0){
                achievementAdditionalValue.style.display = "block";
                animateValue(achievementAdditionalValue, parseFloat(achievementAdditionalValue.innerText), data.user.additonalValue ?? 0, animationDuration, 0, "", "")
            }else{
                achievementAdditionalValue.style.display = "none";
            }

            animateValue(globalRank, parseFloat(globalRank.innerText), data.user.rank, animationDuration, 1, "", "");

            if (data.country == null)
                countryImage.style.display = "none";
            else {
                countryImage.style.display = "block";
                countryImage.src = `https://tetr.io/res/flags/${data.user.country.toLowerCase()}.png`;
            }
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

updateStats();

setInterval(updateStats, 5000 * 60);
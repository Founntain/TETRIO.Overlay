let usernameInfo = document.getElementById("usernameInfo").innerText;

let username = document.getElementById("username");
let profilePicture = document.getElementById("profilePicture");
let rankImage = document.getElementById("bigRankImage");
let lastSeasonRank = document.getElementById("lastSeasonRank");
let currentSeasonTr = document.getElementById("currentSeasonTr");
let lastSeaonTr = document.getElementById("lastSeasonTr");
let sprintPb = document.getElementById("sprintPb");
let blitzPb = document.getElementById("blitzPb");
let zenithPb = document.getElementById("zenithPb");

let prevRankImage = document.getElementById("prevRankImage");
let nextRankImage = document.getElementById("nextRankImage");
let progressBar = document.getElementById("progressBar");
let lastRank = document.getElementById("lastRank");

let tetraLeagueProgressContainer = document.getElementById("tetraLeagueProgressContainer");
let mods = document.getElementById("mods");

let firstLoad = true;

function fadeIn(element) {
    element.style.display = "block";

    setTimeout(() => {
        element.style.opacity = "1";
    }, 10);
}

function fadeOut(element) {
    element.style.display = "block";

    setTimeout(() => {
        element.style.opacity = "0";
    }, 10);
}


function updateTetreaLeagueProgressbar(data){

    if(firstLoad) fadeIn(tetraLeagueProgressContainer)

    let placements = data.league.gamesPlayed < 10;

    if (!placements) {
        if (data.league.rank === "z") {
            tetraLeagueProgressContainer.style.display = "none";
        } else {
            tetraLeagueProgressContainer.style.display = "";
        }
    } else {
        tetraLeagueProgressContainer.style.display = "none";
    }

    prevRankImage.src = `${imgUrl + data.league["prev_rank"]}.png`;

    if (data.league.rank === "x+")
        nextRankImage.src = `${imgUrl + "leaderboard1"}.png`;
    else {

        nextRankImage.src = `${imgUrl + data.league["next_rank"]}.png`;
    }

    if (data.league.rank === "d") {
        prevRankImage.src = null;
        prevRankImage.style.display = "none";
        animateValue(lastRank, parseInt(lastRank.innerText.replace(/[^0-9.]/g, '')), data.league.prev_at, animationDuration, 1, "#", "");
    } else {
        lastRank.style.display = "none";
        prevRankImage.style.display = "block";
    }

    let range = data.league["prev_at"] - data.league["Next_at"];
    let distance = data.league["prev_at"] - data.league.standing;
    let rankPercentage = (distance / range) * 100;

    progressBar.style.width = `${rankPercentage}%`;
}

function updateStats() {
    let url = `${baseUrl}/user/${usernameInfo}/stats`

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

            username.innerText = usernameInfo.toUpperCase();

            if(firstLoad) fadeIn(username)

            let lastSeasonRankImg = "z";
            let lastSeasonTr = 0;

            if(data.league.past["1"] !== undefined){
                lastSeasonRankImg = data.league.past["1"].rank;
                lastSeasonTr = data.league.past["1"].tr;

                lastSeasonRank.style.display = "block";
                lastSeaonTr.style.display = "block";
            }else{
                lastSeasonRank.style.display = "none";
                lastSeaonTr.style.display = "none";
            }

            if(data["40l"].record.user.avatar_revision !== null && data["40l"].record.user.avatar_revision !== 0){
                profilePicture.src = `https://tetr.io/user-content/avatars/${data["40l"].record.user.id}.jpg?rv=${data["40l"].record.user.avatar_revision}`
                if(firstLoad) fadeIn(profilePicture)
            }else{
                profilePicture.style.display = "none";
            }

            if(firstLoad) fadeIn(rankImage)

            rankImage.src = `${imgUrl + data.league.rank}.png`;
            lastSeasonRank.src = `${imgUrl + lastSeasonRankImg}.png`;

            if(firstLoad) fadeIn(profilePicture)
            if(firstLoad) fadeIn(lastSeasonRank)

            let trString = currentSeasonTr.innerText;
            let cleanTrString = trString.replace(/[^0-9.]/g, '');
            cleanTrString = cleanTrString.replace(/,/g, '');

            if(firstLoad) fadeIn(currentSeasonTr)
            animateValue(currentSeasonTr, parseFloat(cleanTrString), data.league.tr, animationDuration, 0, "", " TR");

            let lastSeasonTrString = lastSeaonTr.innerText;
            let lastSeasonTrCleanTrString = lastSeasonTrString.replace(/[^0-9.]/g, '');
            lastSeasonCleanTrString = lastSeasonTrCleanTrString.replace(/,/g, '');

            if(firstLoad) fadeIn(lastSeaonTr)
            animateValue(lastSeaonTr, parseFloat(lastSeasonCleanTrString), lastSeasonTr, animationDuration, 0, "", " TR (S1)");

            var sprintRecord = timeToMilliseconds(sprintPb.innerText);
            var sprintNewValue = parseFloat(data["40l"].record.results.stats.finaltime.toFixed(2))

            if(firstLoad){
                let elements = document.getElementsByClassName("gamemodeText");

                for(let i = 0; i < elements.length; i++) fadeIn(elements[i]);
            }

            if(firstLoad) fadeIn(sprintPb)
            if(firstLoad) fadeIn(blitzPb)
            if(firstLoad) fadeIn(zenithPb)

            animateValue(sprintPb, sprintRecord, sprintNewValue, animationDuration, 0, "", "", true);
            animateValue(blitzPb, parseFloat(blitzPb.innerText.replace(/[^0-9.]/g, '')), data["blitz"].record?.results?.stats?.score ?? 0, animationDuration);
            animateValue(zenithPb, parseFloat(zenithPb.innerText.replace(/[^0-9.]/g, '')), data["zenith"].best?.record?.results?.stats?.zenith.altitude ?? 0, animationDuration);

            updateTetreaLeagueProgressbar(data);

            mods.innerHTML = "";

            data["zenith"].best?.record.extras.zenith.mods.forEach(mod => {
                const img = document.createElement('img');
                img.classList.add("mod");
                img.src = `${imgUrl}${mod}.png`;
                mods.appendChild(img);
            });

            if(firstLoad) fadeIn(mods)

            firstLoad = false;
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

updateStats();

setInterval(updateStats, 15000);
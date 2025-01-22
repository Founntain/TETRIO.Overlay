let usernameInfo = document.getElementById("usernameInfo").innerText;
let expertInfo = document.getElementById("expertInfo").innerText;

let containers = document.getElementsByClassName("floorBoxWrapper")

function updateStats() {
    let url = `${baseUrl}/zenith/splits/${usernameInfo}/stats?expert=${expertInfo}`

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
            let notReached = false;
            let endReached = false;

            for (let i = 0; i < containers.length; i++) {

                let floorBox = containers[i];
                let floorData = data[i];


                if(endReached){
                    floorBox.parentElement.style.display = "none";
                }else{
                    floorBox.parentElement.style.display = "flex";
                }

                let floorTime = floorBox.getElementsByClassName("floorTime")[0];
                let floorDifference = floorBox.getElementsByClassName("floorDifference")[0];

                if(floorData.split === 0){
                    floorTime.innerText = "DNF";
                    endReached = true;
                }else{
                    floorTime.innerText = floorData.splitTime;
                }

                if(floorData.notReached === true){
                    if(notReached === false)
                        floorDifference.innerText = "NOT REACHED"
                    else{
                        floorDifference.innerText = " "
                    }
                    floorDifference.classList.add("notReached");
                    notReached = true;
                    continue;
                }else{
                    floorDifference.classList.remove("notReached");
                }

                var prefix = "";

                if(floorData.isSlower){
                    prefix = "+";

                    floorDifference.classList.remove("faster");
                    floorDifference.classList.add("slower");
                }else{
                    prefix = "-";

                    floorDifference.classList.remove("slower");
                    floorDifference.classList.add("faster");
                }

                if(floorData.differenceToGold === 0 && floorData.differenceToSecondGold > 0){
                    floorDifference.innerText = prefix + floorData.timeDifferenceToSecondGold;
                }else{
                    floorDifference.innerText = prefix + floorData.timeDifferenceToGold;
                }
            }
        })
        .catch(error => {
            console.error('There has been a problem with your fetch operation:', error);
        });
}

updateStats();

setInterval(updateStats, 10000);
const { protocol, hostname, port } = window.location;
const baseUrl = `${protocol}//${hostname}${port ? `:${port}` : ''}`;
const animationDuration = 500;
const imgUrl = "/web/res/img/";

function lerp(a, b, t) {
    return a + (b - a) * t;
}

function lerpInt(a, b, t) {
    return (a + (b - a) * t).toFixed(0);
}

function lerpText(start, end, t) {
    const length = Math.floor(start.length + (end.length - start.length) * t);
    let result = '';

    for (let i = 0; i < length; i++) {
        const startCharCode = i < start.length ? start.charCodeAt(i) : ' '.charCodeAt(0);
        const endCharCode = i < end.length ? end.charCodeAt(i) : ' '.charCodeAt(0);

        const charCode = Math.floor(startCharCode + (endCharCode - startCharCode) * t);
        result += String.fromCharCode(charCode);
    }

    return result;
}

function formatTime(milliseconds) {
    const minutes = Math.floor(milliseconds / 60000);
    const seconds = Math.floor((milliseconds % 60000) / 1000);
    const ms = Math.floor(milliseconds % 1000);

    const formattedMinutes = String(minutes).padStart(2, '0');
    const formattedSeconds = String(seconds).padStart(2, '0');
    const formattedMilliseconds = String(ms).padStart(3, '0');

    return `${formattedMinutes}:${formattedSeconds}.${formattedMilliseconds}`;
}

function timeToMilliseconds(timeString) {
    const [minutes, seconds] = timeString.split(':');
    const [sec, ms] = seconds.split('.');

    const minutesMs = parseInt(minutes, 10) * 60000;
    const secondsMs = parseInt(sec, 10) * 1000;
    const milliseconds = parseInt(ms, 10);

    return minutesMs + secondsMs + milliseconds;
}


function animateValue(element, start, end, duration, lerpType = 0, prefix = "", suffix = "", formatAsTime = false) {
    let currentValue;

    if(start === end){

        switch (lerpType) {
            case 0:
                if(formatAsTime)
                    currentValue = formatTime(start);
                else
                    currentValue = parseFloat(start.toFixed(2)).toLocaleString('en-US');

                break;
            case 1:
                if(formatAsTime)
                    currentValue = formatTime(start);
                else
                    currentValue = parseInt(start).toLocaleString('en-US');

                break;
            default:
                if(formatAsTime)
                    currentValue = formatTime(start);
                else
                    currentValue = start;

                break;
        }

        element.innerText = `${prefix}${currentValue}${suffix}`
        return;
    }

    let startTime = null;

    function animation(currentTime) {
        if (!startTime) startTime = currentTime;
        const elapsed = currentTime - startTime;

        const progress = Math.min(elapsed / duration, 1);

        switch (lerpType) {
            case 0:
                if(formatAsTime)
                    currentValue = formatTime(parseFloat(lerp(start, end, progress).toFixed(2)));
                else
                    currentValue = parseFloat(lerp(start, end, progress).toFixed(2)).toLocaleString('en-US');

                break;
            case 1:
                currentValue = parseInt(lerpInt(start, end, progress)).toLocaleString('en-US');

                break;
            case 2:
                currentValue = lerpText(start, end, progress);

                break;
        }

        element.innerText = `${prefix}${currentValue}${suffix}`

        if (progress < 1) {
            requestAnimationFrame(animation);
        }
    }

    requestAnimationFrame(animation);
}


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
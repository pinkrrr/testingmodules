// Set the date we're counting down to
var countDownDate = new Date("Aug 15, 2017 12:00:00").getTime();

// Update the count down every 1 second
var x = setInterval(function () {

    // Get todays date and time
    var now = new Date().getTime();

    // Find the distance between now an the count down date
    var distance = countDownDate - now;

    // Time calculations for days, hours, minutes and seconds
    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);
    var daysText = "days ";
    var hoursText = "hours ";
    var minutesText = "minutes ";
    var secondsText = "seconds ";

    if (days != 1) {
        daysText = " days : ";
    } else {
        daysText = " day : ";
    }
    if (hours != 1) {
        hoursText = " hours : ";
    } else {
        hoursText = " hour : ";
    }
    if (minutes != 1) {
        minutesText = " minutes : ";
    } else {
        minutesText = " minute : ";
    }
    if (seconds != 1) {
        secondsText = " seconds";
    } else {
        secondsText = " second";
    }
    // Output the result in an element with id="demo"
    document.getElementById("demo").innerHTML = days + daysText + hours + hoursText
        + minutes + minutesText + seconds + secondsText;

    // If the count down is over, write some text 
    if (distance < 0) {
        clearInterval(x);
        document.getElementById("demo").innerHTML = "NOW!";
    }
}, 1000);
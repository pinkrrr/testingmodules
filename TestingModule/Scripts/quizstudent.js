var qModel = $.parseJSON(document.getElementById("qModel").value);
var timerObject = document.getElementById("timer");


$(function () {
    if (timerObject != null) {
        timer(qModel.TimeLeft, timerObject, true);
    } else {
        $.connection.hub.start();
    }
});

function timer(distance, element, status) {
    var x = setInterval(function () {
        var minutes = zeroes(Math.floor((distance / (1000 * 60))));
        var seconds = zeroes(Math.floor((distance % (1000 * 60)) / 1000));
        element.innerHTML = minutes + ":" + seconds;
        distance = distance - 1000;
        if (distance < 0) {
            clearInterval(x);
            quizFinished();
            status = false;
        }
    }, 1000);
}

function zeroes(i) {
    if (i < 10) {
        i = "0" + i;
    }
    return i;
}
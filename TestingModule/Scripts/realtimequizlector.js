var statisticsModel = $.parseJSON(document.getElementById("statisticsModel").value);
var quiz = $.connection.quizHub;

$(function() {

    var totalCorrectAnswers = 1;
    var statisticsDynamicData = [];

    $.connection.hub.start().done(function() {
        retrieveResponses();
        sendInvintations();
        timer(statisticsModel.TimeLeft, "timer", true);
    });

    statisticsModel.Questions.forEach(function(item, i) {
        statisticsDynamicData[i] = {
            questionId: item.Id,
            totalAnswersCount: 0
        }
    });

    quiz.client.recieveStatistics = function(questionId, studentsCount, groupId) {
        statisticsDynamicData.forEach(function(item) {
            if (item.questionId === questionId) {
                item.totalAnswersCount++;
            }
            progress(item.questionId, item.totalAnswersCount, studentsCount);
        });
    };

    quiz.client.responseRecieved = function() {
        quiz.server.queryRealTimeStats(statisticsModel, false);
    };

    quiz.client.recieveRealTimeStatistics = function(realTimeStatistics) {
        realTimeStatistics.forEach(function(item) {
            progress(item.GroupId, item.QuestionId, item.CorrectAnswers, item.TotalAnswers);
        });
    }
});

function sendInvintations() {
    var students = statisticsModel.StudentIds;
    quiz.server.sendQVM(students, statisticsModel.ModuleHistory.Id);
    setInterval(function () {
        quiz.server.sendQVM(students, statisticsModel.ModuleHistory.Id)
    }, 5000);
}

function retrieveResponses() {
    var intervalTime = 10000;
    quiz.server.queryRealTimeStats(statisticsModel, true);
    setInterval(function () { quiz.server.queryRealTimeStats(statisticsModel, true) }, intervalTime);
}

function timer(distance, element, status) {
    var x = setInterval(function () {
        var minutes = zeroes(Math.floor((distance / (1000 * 60))));
        var seconds = zeroes(Math.floor((distance % (1000 * 60)) / 1000));
        document.getElementById(element).innerHTML = minutes + ":" + seconds;
        distance = distance - 1000;
        if (distance < 0) {
            clearInterval(x);
            document.getElementById(element).innerHTML = "00:00";
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
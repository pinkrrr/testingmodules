var quiz = $.connection.quizHub;
quiz.client.reciveModuleHistoryId = function (moduleHistoryId) {
    console.log(moduleHistoryId);
    window.location.href = '/quiz/' + moduleHistoryId;
};

$(function () {
    $.connection.hub.start();

    $.ajax({
        type: 'GET',
        url: '/quiz/checkforactiverealtimequiz',
        datatype: 'JSON',
        success: function (moduleHistoryId) {
            if (moduleHistoryId != 0) {
                window.location.href = '/quiz/' + moduleHistoryId;
            }
        },
        error: function (error) {
            console.log(error.responseText);
        }
    });

    $.ajax({
        type: 'GET',
        url: '/quiz/checkforactiveindividualquiz',
        datatype: 'JSON',
        success: function (individualQuizId) {
            if (individualQuizId != 0) {
                window.location.href = '/individualquiz/' + individualQuizId;
            }
        },
        error: function (error) {
            console.log(error.responseText);
        }
    });

    $.ajax({
        type: 'GET',
        url: '/quiz/checkforactivecumulativequiz',
        datatype: 'JSON',
        success: function (cumulativeQuizId) {
            if (cumulativeQuizId != 0) {
                window.location.href = '/cumulativequiz/' + cumulativeQuizId;
            }
        },
        error: function (error) {
            console.log(error.responseText);
        }
    });
});


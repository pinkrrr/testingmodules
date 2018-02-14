$(function () {

    
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
        url: '/quiz/checkforactivecumulativequizid',
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
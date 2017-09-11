$(document).ready(function () {

    function popup() {
        var _$editBtn = $('.table-edit-button');
        var _$removeBtn = $('.table-remove-button');
        var _$addBtn = $('.addNewItem-btn');
        var $inputTexts;
        var $popup = {
            edit: $('.popup.popup-edit'),
            add: $('.popup.popup-add'),
            remove: $('.popup.popup-remove')
        };
        var $popupRemoveName = $popup.remove.find('.popup-title span');
        var $tableQuestion = $('.table__question');
        var answers = [];

        function initShowEditPopup() {
            _$editBtn.on('click', function () {
                $inputTexts = $(this).closest('.table-row').find('[data-editable]');
                showPopupEdit($inputTexts);
                var $table = $(this).parent().prev();
                var nameId = $(this).closest('.table-row').find('.table-item_name_text').attr('data-id');
                $('#id').val(nameId);
                if ($tableQuestion.length) {
                    var $question = $(this).parent().prev().find('.table-header_name');
                    getAnswerList($table, showAnswersInEdit);
                    showQuestionInEdit($question);
                }
            });
        }

        function showPopupEdit($inputTexts) {
            $popup.edit.addClass('popup-active');

            $inputTexts.each(function (i) {
                $popup.edit.find('.input-text').eq(i).val($(this).text());
            });

        }

        function initSaveData() {
            var saveBtn = $('.popup').find('.popup-save-btn');
            var url = $popup.edit.find('form').attr('action');
            var method = $popup.edit.find('form').attr('method');

            $('form').on('submit', function (e) {
                //e.preventDefault();
                var inputText = $popup.edit.find('.input-text');

                var data = {
                    name: inputText.val(),
                    surname: inputText.val(),
                    login: inputText.val(),
                    password: inputText.val(),
                    minutesToPass: inputText.val(),
                    id: $('#id').val(),
                    accountId: $('#accountId').val(),
                    lectorId: $('#lectorId').val(),
                    disciplineId: $('#disciplineId').val(),
                    lectureId: $('#lectureId').val(),
                    moduleId: $('#moduleId').val(),
                    SpecialityId: $('#SpecialityId').val(),
                    GroupId: $('#GroupId').val(),
                    answer: inputText.val(),
                    answerId: $('#id').val(),
                    question: inputText.val(),
                    questionId: $('#id').val(),
                };

                sendData(data, url, method);

            });
        }
        function getDisciplineOption() {
            var ddlReport = document.getElementByXpath("<%=DropDownListReports.ClientID%>");

            var Text = ddlReport.options[ddlReport.selectedIndex].text;
            var Value = ddlReport.options[ddlReport.selectedIndex].value;
        }

        function closePopup(closeButton) {
            $('.closePopupBtn').on('click', function (e) {
                e.preventDefault();
                $(this).closest('.popup').removeClass('popup-active');
            })
        }

        function sendData(data, url, method) {
            $.ajax({
                url: url,
                method: method,
                data: data
            }).done(function (response) {
                // location.reload();
            });
        }

        function initShowRemovePopup() {
            _$removeBtn.on('click', function () {
                var removeLink = $(this).attr('data-remove');
                var removeName = $(this).closest('.table-row').find('.table-item_name').text();
                var removeSurname = $(this).closest('.table-row').find('.table-item_surname').text();
                showRemovePopup(removeName, removeSurname);
                initRemoveData(removeLink);
            });
        }

        function showRemovePopup(removeName, removeSurname) {
            $popup.remove.addClass('popup-active');
            if (removeSurname) {
                $popupRemoveName.text(removeName + ' ' + removeSurname);
            } else {
                $popupRemoveName.text(removeName);
            }
        }

        function initRemoveData(removeLink) {
            var btnRemove = $popup.remove.find('.popup-remove-btn');
            btnRemove.on('click', function (e) {
                e.preventDefault();
                window.location.href = removeLink;
            });
        }

        function showAddPopup() {
            $popup.add.addClass('popup-active');
        }

        function initShowAddPopup() {
            _$addBtn.on('click', function (e) {
                showAddPopup();
            });
        }

        function getAnswerList($table, callback) {
            answers = [];
            $table.find('.table-item_name_text').each(function (i, item) {
                answers.push({
                    text: $(this).text(),
                    id: $(this).attr('data-id')
                });
            });
            if (callback) {
                callback();
            }
        }

        function showAnswersInEdit() {
            var hmtlQuestionData = [];
            var answerHtml;
            answers.forEach(function (answer) {
                answerHTML = '<div class="answer_item" data-id="' + answer.id + '"><input id="' + answer.id + '" name="Question" type="text" class="input-text" value="' + answer.text + '"><div class="answer_remove"><i class="fa fa-trash" aria-hidden="true"></i></div></div>';
                hmtlQuestionData += answerHTML;
            });
            $popup.edit.find('.answer_item').remove();
            $(hmtlQuestionData).insertAfter($popup.edit.find('form .popup-title'));
        }

        function showQuestionInEdit($question) {
            var qText = $question.text();
            var qId = $question.attr('data-id');
            var questionHtml = '<input name="Answer" type="text" data-id="' + qId + '" value="' + qText + '" class="input-text question">';
            $popup.edit.find('.input-text.question').remove();
            $(questionHtml).insertAfter($popup.edit.find('form .popup-title'));
        }


        function getLecture() {
            var $dropdownDiscipline = $('#ddldiscipline');
            var $dropdownLection = $('#ddllecture');
            var disciplineId = null;

            $dropdownDiscipline.on('selectmenuselect', function (e, ui) {
                setLectionsListByDisciplines(ui.item.value);
            });

        }

        function setLectionsListByDisciplines(disciplineId) {
            var url = "/admin/GetLecturesByDiscipline/";
            var $lectureSelect = $("#ddllecture");

            disciplineId = parseInt(disciplineId);

            $.ajax({
                url: url,
                type: 'POST',
                data: { disciplineId: disciplineId },
                success: function (data) {
                    var optionsHTML = '';
                    for (var x = 0; x < data.length; x++) {
                        optionsHTML += "<option value=" + data[x].Value + ">" + data[x].Text + "</option>";
                    }
                    $lectureSelect.html(optionsHTML);
                    $lectureSelect.selectmenu("refresh");
                }
            })

        }

        var defaultLectureId = parseInt($('#ddldiscipline').find('option').first().attr('value'));


        initShowEditPopup();
        initShowRemovePopup();
        initShowAddPopup();
        closePopup();
        initSaveData();
        getLecture();
        if ($("#ddllecture").length) {
            setLectionsListByDisciplines(defaultLectureId);
        }


    }

    function selectmenuInit() {
        $('select').selectmenu();
    }

    function checkboxradioInit() {
        $('input:checkbox, input:radio').checkboxradio({
            icon: false
        });
    }

    function specialitiesStudentsAccordion() {

        $('.accordion').accordion({
            header: '.accordion_header',
            collapsible: true,
            heightStyle: "content",
            event: "click",
            beforeActivate: function (event, ui) {
                // The accordion believes a panel is being opened
                if (ui.newHeader[0]) {
                    var currHeader = ui.newHeader;
                    var currContent = currHeader.next('.ui-accordion-content');
                    // The accordion believes a panel is being closed
                } else {
                    var currHeader = ui.oldHeader;
                    var currContent = currHeader.next('.ui-accordion-content');
                }
                // Since we've changed the default behavior, this detects the actual status
                var isPanelSelected = currHeader.attr('aria-selected') == 'true';

                // Toggle the panel's header
                currHeader.toggleClass('ui-corner-all', isPanelSelected).toggleClass('accordion-header-active ui-state-active ui-corner-top', !isPanelSelected).attr('aria-selected', ((!isPanelSelected).toString()));

                // Toggle the panel's icon
                currHeader.children('.ui-icon').toggleClass('ui-icon-triangle-1-e', isPanelSelected).toggleClass('ui-icon-triangle-1-s', !isPanelSelected);

                // Toggle the panel's content
                currContent.toggleClass('accordion-content-active', !isPanelSelected)
                if (isPanelSelected) { currContent.slideUp(); } else { currContent.slideDown(); }

                return false; // Cancel the default action
            }
        });

    }

    function selectAllorNobody() {
        var _$btnSelectAll = $('.btnSelectAll');
        var _$btnSelectNobody = $('.btnSelectNobody');

        initSelectAll();
        initSelectNobody();

        function selectAll(table) {
            table.find('label').each(function () {
                if (!$(this).hasClass('ui-state-active')) {
                    $(this).click();
                }
            })
        }

        function initSelectAll() {
            _$btnSelectAll.on('click', function (e) {
                e.preventDefault();
                var table = $(this).closest('form').find('.table');
                selectAll(table);
            });
        }

        function selectNobody(table) {
            table.find('label').each(function () {
                if ($(this).hasClass('ui-state-active')) {
                    $(this).click();
                }
            })
        }

        function initSelectNobody() {
            _$btnSelectNobody.on('click', function (e) {
                e.preventDefault();
                var table = $(this).closest('form').find('.table');
                selectNobody(table);
            });
        }

    }

    function quiz() {
        var $nextQbtn = $('.nextQuestion');

        var $questionBlock = $('.questionBlock');
        var $question = $questionBlock.find('.question');

        var $answerList = $questionBlock.find('.answers');
        var $answers = $answerList.find('.answer');

        var _model = qModel;

        function selectAnswer($answer) {
            $answer.addClass('answer__active').siblings().removeClass('answer__active');
        }

        function getSelectedAnswerId() {
            return $('.answer').filter('.answer__active').attr('data-answerid');
        }

        function initSelectAnswer() {
            $answerList.on('click', '.answer', function () {
                selectAnswer($(this));
            })
        }

        function setQuestionData(model) {
            $question.attr('data-questionid', model.Question.Id);
            $question.html(model.Question.Text);
            $answerList.html('');
            model.Answers.forEach(function (item) {
                $answerList.append('<div class="answer" data-answerid="' + item.Id + '"><div class="answer_icon"><i class="fa fa-check-circle-o" aria-hidden="true"></i></div><div class="answer_text">' + item.Text + '</div></div>')
            })

        }

        function showNextQuestion() {
            var selectedAnswerId = getSelectedAnswerId();
            if (selectedAnswerId) {
                var quizHub = $.connection.quizHub;
                $.connection.hub.start().done(function () {
                    quizHub.server.saveResponse(_model, selectedAnswerId).done(function (model) {
                        _model = model;
                        console.log(_model);
                        if (_model !== null){
                            setQuestionData(_model);
                        } else {
                            quizFinished();
                        }
                    }).fail(function () {
                        quizFinished();
                    });
                });

            } else {
                return;
            }
        }

        function initNextQuestion() {
            $nextQbtn.click(function () {
                showNextQuestion();
            });
        }

        function quizFinished() {
            $questionBlock.remove();
            $('<div class="quizFinished"><h3>Тест закінчено.</h3><h4>Дякую за увагу!</h4></div>').prependTo('.studentBody');
        }


        initSelectAnswer();
        console.log(_model);
        if (_model !== null) {
            setQuestionData(_model);
        }
        initNextQuestion();


    }

    

    function statistics() {

        if ($('.body-content__statistics').length > 0) {
            initializeModuleStatisticsPage();
        }

        function initializeModuleStatisticsPage() {

            var $questionList = $('.body-content__statistics .questions');
            var questionList = [];

            console.log(statisticsModel);

            statisticsModel.forEach(function (item, i) {
                questionList.push('<div class="question" data-question-id="' + item.Id + '"><span class="question_title">' + item.Text + '</span><div class="question_progressbar"><div class="progress"></div></div></div>')
            });

            $questionList.html(questionList);

        }

    }

    function startLectureValidation() {

        var $startLecture = $('.popup-startLecture');
        var $groupCheckbox = $('.groupItem input[type="checkbox"]')
        var $startButton = $startLecture.find('.popup-start-btn');

        $groupCheckbox.on('change', function () {
            if ($groupCheckbox.is(':checked')) {
                $startButton.removeAttr('disabled');
            } else {
                $startButton.attr('disabled', '');
            }
        })

    }

    function quiestionsEditChecked() {
        var $checkButton = $('.table_questions .table-item_correct label');

        $checkButton.on('click', function () {
            $(this).closest('tbody').find('.table-item label input').each(function (i, item) {
                $(item).prop('checked', false)
            })
            $(this).prop('checked', true);
        })
    }

    popup();
    selectmenuInit();
    checkboxradioInit();
    specialitiesStudentsAccordion();
    selectAllorNobody();
    statistics();
    startLectureValidation();
    //quiestionsEditChecked();

    if ($('.questionBlock').length > 0) {
        quiz();
    }

});

function progress(qID, correctAnswersCount, studentsCount) {
    var progress = correctAnswersCount / studentsCount * 100;
    $('.body-content__statistics .question[data-question-id="' + qID + '"] .question_progressbar .progress').css('width', progress + '%');
}

function stopModule() {
    var $stopModuleBtn = $('#stopModuleButton');
    var quiz = $.connection.quizHub;
    $.connection.hub.start();
    $(document).on("click", "a#stopModuleButton", function () {
        quiz.server.stopModule();
    });
}

stopModule();

function historyStatisticsPage(model) {

    var chartDataObj = {
        id: null,
        answers: []
    };

    console.log(model);
    createHistoryStatisticsPage();

    function createHistoryStatisticsPage() {

        var moduleId = null;

        model.Modules.forEach(function (module) {

            $('<div/>', {
                class: 'module',
                id: 'module'+module.Id
            }).appendTo('.chartsWrapper');

            $('<div/>', {
                class: 'module_name',
                html: module.Name
            }).prependTo('#module' + module.Id);

            model.Questions.forEach(function (question) {

                if (question.ModuleId === module.Id) {

                    $('<div/>', {
                        class: 'question',
                        id: 'questionId' + question.Id
                    }).appendTo('#module' + module.Id);

                    $('<div/>', {
                        class: 'question_text',
                        html: question.Text
                    }).appendTo('#questionId' + question.Id);

                    chartDataObj.id = question.Id;

                    model.Groups.forEach(function (group) {

                        $('<div/>', {
                            class: 'group',
                            id: 'group' + group.Id
                        }).appendTo('#questionId' + question.Id);

                        $('<div/>', {
                            class: 'group_name',
                            html: group.Name
                        }).prependTo('#questionId' + question.Id + ' #group' + group.Id);

                        $('<div/>', {
                            class: 'pieChart',
                            id: 'chartContainer' + question.Id
                        }).appendTo('#questionId' + question.Id + ' #group' + group.Id);

                        model.AnswersCount.forEach(function (answer) {

                            if (question.Id === answer.QuestionId && answer.GroupId === group.Id) {

                                chartDataObj.answers.push({
                                    y: answer.Count,
                                    name: answer.Text
                                })

                            }

                        });

                        renderChart(chartDataObj);

                        chartDataObj.answers = [];

                    })

                }

            });

        });
    }

    function renderChart(chartData) {

        var chart = new CanvasJS.Chart("chartContainer" + chartData.id,
            {
                animationEnabled: true,
                legend: {
                    verticalAlign: "bottom",
                    horizontalAlign: "left",
                    fontSize: 16,
                    fontFamily: "arial"
                },
                theme: "theme3",
                backgroundColor: "transparent",
                data: [{
                    type: "pie",
                    indexLabelFontFamily: "Arial",
                    indexLabelFontSize: 20,
                    indexLabelFontWeight: "bold",
                    startAngle: 0,
                    indexLabelFontColor: "#ffffff",
                    indexLabelLineColor: "#ff0000",
                    indexLabelPlacement: "inside",
                    toolTipContent: "Відповідь: {name}",
                    showInLegend: true,
                    indexLabel: "{y}",
                    dataPoints: chartData.answers
                }]
            });
        chart.render();
    }

}

//function drawCharts(model) {
//    var moduleId = null;

//    getChartData();

//    model.Modules.forEach(function (module) {

//        //$('<div/>', {
//        //    class: 'module'
//        //}).appendTo('.chartsWrapper');

//        //$('<div/>', {
//        //    class: 'module_name',
//        //    html: module.Name
//        //}).appendTo('.chartsWrapper');

//        moduleId = module.Id;

//        model.Questions.forEach(function (question) {

//            if (question.ModuleId === moduleId){
//                //$('<div/>', {
//                //    class: 'question',
//                //    html: question.Text
//                //}).appendTo('.module');


//                chartModel = {

//                }

//            }

//        })

//        //$('<div class="pieChart" id="chartContainer' + item.Question.Id + '" style="height: 360px; width: 360px"></div>').appendTo('.chartsWrapper');
//        //createChart(item);
//    })
//}


//function getChartData(model) {

//    var answersArray = [];

//    model.Questions.forEach(function (question) {
        
//    })

//    model.AnswersCount.forEach(function (answer) {
//        answersArray.push({
//            y: answer.Count,
//            name: answer.Text
//        })
//    })

//    var chartData = {
//        id: model.Question.Id,
//        question: model.Question.Text,
//        answers: answersArray
//    }
//    return chartData;
//}
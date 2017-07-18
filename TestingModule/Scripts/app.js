$(document).ready(function () {

    function popup() {
        var _$editBtn = $('.table-edit-button');
        var _$removeButton = $('.table-remove-button');
        var inputText = '';

        var $popup = {
            popup: $('.popup'),
            editSpecialities: $('.popup-edit-specialities')
        }

        function initShowPopupEditSpecialities() {
            _$editBtn.on('click', function () {
                inputText = $(this).closest('.table-row').find('.specialityName').text();
                showPopupEditSpecialities(inputText);
            });
        }

        function showPopupEditSpecialities(speciality) {
            $popup.editSpecialities.addClass('popup-active');
            $popup.editSpecialities.find('.input-text').val(speciality);
        }

        function initSaveData(popupName) {
            var saveBtn = $('.' + popupName).find('.popup-save-btn');
            var url = $popup.editSpecialities.find('form').attr('action');
            var method = $popup.editSpecialities.find('form').attr('method');

            saveBtn.on('click', function (e) {
                e.preventDefault();
                var hiddenField = $popup.editSpecialities.find('#id');
                var id = hiddenField.val();

                hiddenField.val(id);
                var inputText = $popup.editSpecialities.find('.input-text').val();

                var data = {
                    text: inputText,
                    id: id
                };

                console.log(data);

                //sendData(data, url, method);

            });
        }

        function closePopup(closeButton) {
            $('.' + closeButton).on('click', function (e) {
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
                alert(response);
            });
        }

        initShowPopupEditSpecialities();
        closePopup('popup-cancel-btn');
        initSaveData('popup-edit-specialities');

    }

    popup();

})
(function () {

    function popup() {
        var _$editBtn = $('.table-edit-button');
        var _$removeButton = $('.table-remove-button');

        var $popup = {
            popup: $('.popup'),
            editSpecialities: $('.popup-edit-specialities')
        }

        function initShowPopupEditSpecialities() {
            _$editBtn.on('click', function () {
                var speciality = $(this).closest('.table-row').find('.specialityName').text();
                showPopupEditSpecialities(speciality);
            });
        }

        function showPopupEditSpecialities(speciality) {
            $popup.editSpecialities.addClass('popup-active');
            $popup.editSpecialities.find('.input-text').val(speciality);
            sendData(speciality);
        }

        function closePopup(closeButton) {
            $('.'+closeButton).on('click', function () {
                $(this).closest('.popup').removeClass('popup-active');
            })
        }

        function sendData(data) {
            $.ajax({
                url: 'Admin/EditSpeciality.html',
                data: data
            }).done(function (response) {
                alert(response);
            });
        }

        initShowPopupEditSpecialities();
        closePopup('popup-cancel-btn');

    }


    popup();


}());
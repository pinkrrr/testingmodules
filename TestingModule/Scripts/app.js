$(document).ready(function () {

    function popup() {
        var _$editBtn = $('.table-edit-button');
        var _$removeBtn = $('.table-remove-button');
        var _$addBtn = $('.addNewItem-btn');
        var inputText = '';
        var $popup = {
            edit: $('.popup.popup-edit'),
            add: $('.popup.popup-add'),
            remove: $('.popup.popup-remove')
        };
        var $popupRemoveName = $popup.remove.find('.popup-title span');

        function initShowEditPopup() {
            _$editBtn.on('click', function () {
                inputText = $(this).closest('.table-row').find('.table-item_name_text').text();
                showPopupEdit(inputText);
                var nameId = $(this).closest('.table-row').find('.table-item_name_text').attr('data-id');
                $('#id').val(nameId);
            });
        }

        function showPopupEdit(inputText) {
            $popup.edit.addClass('popup-active');
            $popup.edit.find('.input-text').val(inputText);
        }

        function initSaveData() {
            var saveBtn = $('.popup').find('.popup-save-btn');
            var url = $popup.edit.find('form').attr('action');
            var method = $popup.edit.find('form').attr('method');

            saveBtn.on('click', function (e) {
                //e.preventDefault();
                var inputText = $popup.edit.find('.input-text');

                var data = {
                    name: inputText.val(),
                    id: $('#id').val()
                };

                sendData(data, url, method);

            });
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
                var removeName = $(this).closest('.table-row').find('.table-item_name_text').text();
                showRemovePopup(removeName);
                initRemoveData(removeLink);
            });
        }

        function showRemovePopup(removeName) {
            $popup.remove.addClass('popup-active');
            $popupRemoveName.text(removeName);
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

        initShowEditPopup();
        initShowRemovePopup();
        initShowAddPopup();
        closePopup();
        initSaveData();

    }

    popup();

})
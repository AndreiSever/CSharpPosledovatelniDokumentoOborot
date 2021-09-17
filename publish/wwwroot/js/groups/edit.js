$(document).ready(function () {
    var removeUserOnClick = function () {
        $btn = $(this);
        $removedInput = $btn.siblings('.user-removed-input').first();
        if ($removedInput.val() === 'False') {
            $removedInput.val('True');
            $btn.text('Восстановить');
            $btn.closest('li').addClass('user-removed');
        } else if ($removedInput.val() === 'True') {
            $removedInput.val('False');
            $btn.text('Удалить');
            $btn.closest('li').removeClass('user-removed');
        }
    };
    $('#add-user-btn').click(function () {
        var $select = $('#user-select');
        var userId = $select.val();
        if (userId > 0) {
            var alreadyInList = false;
            $('#users-list .user-id-input').each(function (index, element) {
                var val = $(element).val();
                if (val === userId) {
                    alreadyInList = true;
                    return false;
                }
            });
            if (alreadyInList)
                return;
            var userName = $select.children(':selected').text();
            $list = $('#users-list');
            var count = $list.children('li').length;
            var $li = $('<li class="list-group-item d-flex justify-content-between align-items-center">'
                + '<span><a href="/Admin/Users/Details?id=' + userId +'">' + userName + '</a></span><a href="#" class="del-user-btn">Удалить</a>'
                + '<input type="hidden" value="' + userId + '" name="Input.Users[' + count + '].Id" class="user-id-input" />'
                + '<input type="hidden" value="' + userName + '" name="Input.Users[' + count + '].FullName" />'
                + '<input type="hidden" value="False" name="Input.Users[' + count + '].IsRemoved" class="user-removed-input" />'
                + '</li>');
            $li.appendTo($list);
            $li.find('a').click(removeUserOnClick);
        }
    });
    $('.del-user-btn').each(function (index, element) {
        $(element).click(removeUserOnClick);
    });
    $('#user-select').select2();
});
$(document).ready(function () {
    $rejectBtn = $('#reject-btn');
    $rejectBtn.click(function () {
        $isRejectedInput = $('#isrejected-input');
        $isRejectedInput.val('True');
        $form = $('#stage-submit-form');
        $form.submit();
    });
});
$("#AjaxForm").submit(function () {
    debugger
    $(this).find('input[data-validate],textarea[data-validate],select[data-validate]').trigger("blur");
    $(this).find('input[placeholder],textarea[placeholder]').each(function () { $hideplaceholder($(this)); });
    var numError = $(this).find('.check-error').length;
    if (numError) {
        $(this).find('.check-error').first().find('input[data-validate],textarea[data-validate],select[data-validate]').first().focus().select();
        return false;
    }
    $(this).ajaxSubmit({

        success: function (data) {
            if (data.Success) {
                debugger
                $.cookie('token', data.Token);
                window.location.href = "/Home/UploadPage";
            } else {
                alert(data.Message)
            }
        }
    });
    return false;
});
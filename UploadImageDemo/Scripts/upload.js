(function ($) {
    $.fn.UploadImage = function () {
        $(this).click(function () {

            var input = $("<input type='file' multiple='true' accept='image/bmp,image/x-png,image/jpeg' />").change(function () {

                if (this.files && this.files.length > 0) {

                    var form = $("<form method='post' action='/home/upload' enctype='multipart/form-data'><input name='imageFile' type='file' multiple='true' /></form>");

                    form.find('input')[0].files = this.files;

                    form.ajaxForm(function (response) {
                        console.log(response);
                    });

                    form.submit();
                }
            });
            input.click();
        });
    };
})(jQuery);
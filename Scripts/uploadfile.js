$dialog.dialog("option", "buttons", {
    "Save": function () {
        var dlg = $(this);
        var formData = new FormData($("#" + formName)[0]);
        $.ajax({
            url: /Controller/Home,
            type: 'POST',
            data: formData,
            processData: false,
            contentType: false,
            success: function (response, textStatus, xhr) {
            
                },
            
    error: function (xhr, status, error) {
                
            }
        });
    },
"Cancel": function () {
    $(this).dialog("close");
    $(this).empty();
}

});
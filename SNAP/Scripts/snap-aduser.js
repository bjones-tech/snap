/// <reference path="_references.js" />

$(document).ready(function () {
    $(".generate-password").click(function () {
        $.ajax({
            method: "GET",
            url: "/ADUsers/GeneratePassword",
            cache: false
        })
        .done(function (result) {
            $("#GeneratedPassword").val(result);
            $("#Password").val(result);
            $("#VerifiedPassword").val(result);
        });
    });
});
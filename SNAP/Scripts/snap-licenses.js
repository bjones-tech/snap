/// <reference path="_references.js" />

$(document).ready(function () {
    $("#O365License").change(function () {
        switch ($(this).val()) {
            case "E1/Windows":
            case "E3, EMS":
                $("#ComputerRequired").val(true);
                break;

            default:
                $("#ComputerRequired").val(false);
                break;
        }
    });
});
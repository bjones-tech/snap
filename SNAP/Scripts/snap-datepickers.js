/// <reference path="_references.js" />

$(document).ready(function () {
    $(".start-date-picker").datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: "-6M",
        maxDate: "+6M"
    });

    $(".end-date-picker").datepicker({
        changeMonth: true,
        changeYear: true,
        minDate: "0",
        maxDate: "+18M"
    });

    $(".start-date-picker").change(function () {
        var startDate = new Date($(this).val());
        var minEndDate = new Date(startDate.getFullYear(), startDate.getMonth(), startDate.getDate() + 1);

        $(".end-date-picker").val(null);
        $(".end-date-picker").datepicker("option", "minDate", minEndDate);
    });

    $(":checkbox[name=Immediate]").click(function () {

        var today = new Date();
        var formattedDate = today.getMonth() + 1 + "/" + today.getDate() + "/" + today.getFullYear();

        if ($(this).is(":checked")) {
            $(".end-date-picker").val(formattedDate);
            $(".end-date-picker").datepicker("option", "minDate", null);
            $(".end-date-picker").datepicker("option", "maxDate", "+1M");
        } else {
            $(".end-date-picker").val(null);
            $(".end-date-picker").datepicker("option", "minDate", "0");
            $(".end-date-picker").datepicker("option", "maxDate", "+12M");
        }
    });
});
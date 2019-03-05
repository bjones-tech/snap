/// <reference path="_references.js" />

function GetOfficeList(country) {
    var url = "/api/Locations/OfficeNames/" + country;

    if (country) {
        $("#Office").empty();
        
        $.getJSON(url, function (data) {
            $.each(data, function (index, value) {
                $("#Office").append("<option>" + value + "</option>");
                SetState($("#Office").val());
            });
        });
    }
}

function SetState(office) {
    var url = "/api/Locations/OfficeState/" + office;

    if (office) {
        $.getJSON(url, function (data) {
            $("#State").val(data);

            if (data == null) {
                $("#State").attr("readonly", false);
            } else {
                $("#State").attr("readonly", true);
            }
        });
    } else {
        $("#State").val(null);
        $("#State").attr("readonly", false);
    }
}

$(document).ready(function () {
    $("#Country").change(function () {
        GetOfficeList($("#Country").val());
    });

    if (!$("#State").val()) {
        SetState($("#Office").val());
    }

    $("#Office").change(function () {
        SetState($("#Office").val());
    });
});
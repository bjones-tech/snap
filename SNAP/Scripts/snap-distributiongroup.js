/// <reference path="_references.js" />

function SetAliasFormat(format) {
    var format = Number($("#Format").val());

    if (format != null) {
        $("#Alias1").hide();
        $("#Alias2").hide();

        switch (format) {
            case 1:
                $("#Alias1").attr("placeholder", "Business Unit");
                $("#Alias2").attr("placeholder", "Function");
                $("#Alias1").show();
                $("#Alias2").show();
                break;

            case 2:
                $("#Alias1").attr("placeholder", "Customer");
                $("#Alias2").attr("placeholder", "Function");
                $("#Alias1").show();
                $("#Alias2").show();
                break;

            case 3:
                $("#Alias1").attr("placeholder", "Function");
                $("#Alias1").show();
                break;

            default:
                break;
        }
    }
}

function SetAliasAndPreview() {
    $("#Alias").val(null);
    $("#Preview").val(null);

    var aliasPrefix = $("#AliasPrefix").val();

    var regex1 = new RegExp("[^a-zA-Z0-9]", "g");
    var alias1 = $("#Alias1").val();
    alias1 = alias1.replace(regex1, "");

    var regex2 = new RegExp("[^a-zA-Z0-9\.]", "g");
    var alias2 = $("#Alias2").val();
    alias2 = alias2.replace(regex2, "");

    if (alias1 && alias2) {
        var alias = aliasPrefix + "." + alias1 + "." + alias2;

        $("#Alias").val(alias);
        $("#Preview").val(alias);
    }

    else if (alias1) {
        var alias = aliasPrefix + "." + alias1;

        $("#Alias").val(alias);
        $("#Preview").val(alias);
    }
}

$(document).ready(function () {
    SetAliasFormat();

    $("#Format").change(function () {
        $("#Alias1").val(null);
        $("#Alias2").val(null);
        $("#Alias").val(null);
        $("#Preview").val(null);

        SetAliasFormat();
    });

    SetAliasAndPreview();

    $("#Alias1, #Alias2").bind("change keyup mousemove", function () {
        SetAliasAndPreview();
    });
});
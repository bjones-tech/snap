/// <reference path="_references.js" />

$(document).ready(function () {

    $("#Immediate").click(function () {
        if ($(this).is(":checked")) {
            $("#ImmediateWarning").html("WARNING: 'Immediately Disable Account' has been selected.<br/>Account will be disabled upon submitting the request.");
        } else {
            $("#ImmediateWarning").html(null);
        }
    });
});
/// <reference path="_references.js" />

function formSubmit(currentRequest) {
    $("#ajaxPartial").empty();
    $(".ajax-heading").remove();
    $("#ajaxPartial").append("Searching...");

    currentRequest = $.ajax({
        method: "GET",
        url: $("form").attr('action'),
        data: $("form").serialize(),
        cache: false,
        beforeSend: function () {
            if (currentRequest != null) {
                currentRequest.abort();
            }
        }
    })
    .done(function (html) {
        $("#ajaxPartial").empty();
        $("#ajaxPartial").append(html);
    });
}

$(document).ready(function () {
    $(".show-next-hidden").click(function () {
        $("div.hidden:first").removeClass("hidden");
    });

    var currentRequest = null;

    $(".keyup-submit").keyup(function () {
        formSubmit(currentRequest)
    });

    $(".click-submit").click(function () {
        formSubmit(currentRequest)
    });

    $(".change-submit").change(function () {
        formSubmit(currentRequest)
    });

    $('[data-toggle="tooltip"]').tooltip();

    $(".overlay-click").click(function () {
        $(".overlay").fadeIn();
        $(".overlay-message").fadeIn();
    });

    $("form").submit(function () {
        var managersEmail = $("#ManagersEmail").val();

        if (managersEmail) {
            if (managersEmail.search("@") == -1) {
                managersEmail = managersEmail + "@dimensiondata.com";
                $("#ManagersEmail").val(managersEmail);
            }
        }
    });

    $("select").change(function () {
        var selected = $(this).find("option:selected");

        var hasNullOption = false;

        $(this).find("option").each(function () {
            if (!$(this).val()) {
                hasNullOption = true;
            }
        });

        if (selected.val() && hasNullOption == true) {
            $(this).css("font-weight", "bold");
            $(this).find("option").css("font-weight", "normal");

            $(this).find("option").each(function () {
                if (!$(this).val()) {
                    $(this).css("font-weight", "bold");
                }
            });
        } else {
            $(this).css("font-weight", "normal");
        }
    });
});
/// <reference path="_references.js" />

$(document).ready(function () {
    $("form").submit(function () {
        var emailAddress = $("#EmailAddress").val();

        if (emailAddress) {
            if (emailAddress.search("@") == -1) {
                emailAddress = emailAddress + "@dimensiondata.com";
                $("#EmailAddress").val(emailAddress);
            }
        }

        var managersEmail = $("#ManagersEmail").val();

        if (managersEmail) {
            if (managersEmail.search("@") == -1) {
                managersEmail = managersEmail + "@dimensiondata.com";
                $("#ManagersEmail").val(managersEmail);
            }
        }

        var recipientsEmail = $("#RecipientsEmail").val();

        if (recipientsEmail) {
            if (recipientsEmail.search("@") == -1) {
                recipientsEmail = recipientsEmail + "@dimensiondata.com";
                $("#RecipientsEmail").val(recipientsEmail);
            }
        }

        $('[id*="InterviewersEmail"]').each(function () {

            var interviewersEmail = $(this).val();

            if (interviewersEmail) {
                if (interviewersEmail.search("@") == -1) {
                    var newEmail = interviewersEmail + "@dimensiondata.com";
                    $(this).val(newEmail);
                }
            }
        });
    });
});
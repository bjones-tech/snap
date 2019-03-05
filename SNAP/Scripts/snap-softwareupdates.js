/// <reference path="_references.js" />

$(document).ready(function () {

    $(".check-software-updates").click(function () {
        $(".update-status-style").html(null);

        $(this).addClass("disabled");

        $(".remove-compliant").removeClass("disabled");
        $(".remove-compliant").removeClass("hidden");
        $(".remove-compliant").show();

        $("tr.data-row").each(function () {
            var $this = $(this);
            var hostname = $.trim($(this).find(".hostname-data").html());
            var url = "/api/SoftwareUpdates/GetStatus";

            $this.find(".update-status-style").html("<div class='btn btn-dot btn-default btn-blink'></div>");

            (function blink() {
                $('.btn-blink').fadeOut(500).fadeIn(500, blink);
            })();

            $.ajax({
                method: "POST",
                url: url,
                data: { Hostname: hostname }
            }).done(function (data) {
                var updateStatus = data.UpdateStatus;

                var updateStatusStyle = data.UpdateStatusStyle;
                var updateStatusStyleDiv = "<div class='" + updateStatusStyle + "'></div>";

                var updateStatusAction = "Last Reboot: " + data.LastBootUpTime;
                var updateStatusActionDiv = "<div class='text-muted' style='font-size: 0.8em'>" + updateStatusAction + "</div>";

                $this.find(".update-status").html(updateStatus);
                $this.find(".update-status-style").html(updateStatusStyleDiv);
                $this.find(".update-status-action").html(updateStatusActionDiv);
            });
        });
    });

    $(".remove-compliant").click(function () {
        $(this).addClass("disabled");

        $("tr.data-row").each(function () {
            var $this = $(this);
            var hostname = $.trim($(this).find(".hostname-data").html());
            var url = "/api/SoftwareUpdates/GetStatus";

            $.ajax({
                method: "POST",
                url: url,
                data: { Hostname: hostname }
            }).done(function (data) {
                var updateStatus = data.UpdateStatus;

                var updateStatusStyle = data.UpdateStatusStyle;
                var updateStatusStyleDiv = "<div class='" + updateStatusStyle + "'></div>";

                var updateStatusAction = "Last Reboot: " + data.LastBootUpTime;
                var updateStatusActionDiv = "<div class='text-muted' style='font-size: 0.8em'>" + updateStatusAction + "</div>";

                $this.find(".update-status").html(updateStatus);
                $this.find(".update-status-style").html(updateStatusStyleDiv);
                $this.find(".update-status-action").html(updateStatusActionDiv);

                if (updateStatus == "Compliant") {
                    var itemCount = $("#itemCount");
                    var itemCountNewValue = itemCount.html() - 1;
                    itemCount.html(itemCountNewValue);
                    $this.remove();
                }
            });
        });
    });

    $(".click-submit").click(function () {
        $(".check-software-updates").removeClass("disabled");
        $(".check-software-updates").show();

        $(".remove-compliant").hide();
        $(".remove-compliant").addClass("disabled");
    });

    $(".update-auto-patch-group").change(function () {
        var row = $(this).parent().parent();
        var hostname = $.trim(row.find(".hostname-data").html());
        var autoPatch = $(this).is(":checked");
        var url = "/api/SoftwareUpdates/UpdateAutoPatchGroup";

        $.ajax({
            method: "POST",
            url: url,
            data: { Hostname: hostname, AutoPatch: autoPatch }
        });

        $(this).attr("disabled", true);
    });
});
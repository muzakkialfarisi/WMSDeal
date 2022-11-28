$('.btn-tracker').on('click', function () {
    $.ajax({
        url: "../../Tracking/Tracker",
        method: "GET",
        data: {
            OrderId: $('input[name="OrderId"]').val(),
        },
        success: function (data) {
            if (data != false) {
                $('.partial-detail').html(data);
            } else {
                toastr.error('Sales Order Notfound!', '', {
                    positionClass: 'toast-top-right',
                    closeButton: false,
                    progressBar: false,
                    newestOnTop: true,
                    rtl: $("body").attr("dir") === "rtl" || $("html").attr("dir") === "rtl",
                    timeOut: 3000
                });
            }
        },
        error: function (data) {
            console.log(data);
        }
    });
});
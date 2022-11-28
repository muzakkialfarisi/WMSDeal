$(function () {
    $('.table').DataTable({
    });
});

function Toaster(statuscode, value) {
    if (statuscode == "200")
    {
        toastr.success(value, '', {
            positionClass: 'toast-top-right',
            closeButton: false,
            progressBar: true,
            newestOnTop: true,
            rtl: $("body").attr("dir") === "rtl" || $("html").attr("dir") === "rtl",
            timeOut: 3000
        });
    }
    else
    {
        toastr.error(value, '', {
            positionClass: 'toast-top-right',
            closeButton: false,
            progressBar: true,
            newestOnTop: true,
            rtl: $("body").attr("dir") === "rtl" || $("html").attr("dir") === "rtl",
            timeOut: 3000
        });
    }
}

$('.btn-find').on('click', function () {
    var RetunedType = $('select[name="RetunedType"]').val();
    var KeyNumber = $('input[name="KeyNumber"]').val();
    if (RetunedType == "")
    {
        Toaster(400, "Invalid Modelstate!");
    }
    else
    {
        window.location.href = "/Returns/Detail?RetunedType=" + RetunedType + "&KeyNumber=" + KeyNumber;
    }
});
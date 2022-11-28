$(function () {
    $('.table').DataTable({
    });
});

$('.table tbody').on('click', 'tr', function () {
    window.location = window.location.href + "/Request?RequestId=" + $(this).data("id");
});
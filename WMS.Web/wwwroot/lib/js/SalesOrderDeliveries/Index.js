$(function () {
    $('#datetimepicker-from').datetimepicker({
        viewMode: 'years'
    });

    $('#datetimepicker-to').datetimepicker({
        viewMode: 'years'
    });

    $('.table').DataTable({
    });
});

$('.btn-edit-airwaybill').on('click', function () {
    $('.modal-title').html('OrderId - ' + $(this).data('id'));
    $('input[name="outsalesOrderDelivery.OrderId"]').val($(this).data('id'));
    $('input[name="outsalesOrderDelivery.AirwayBill"]').val($(this).data('airwaybill'));
});

$('.btn-offcanvas').on('click', function () {
    $('input[name="outsalesOrderDelivery.OrderId"]').val("");
    $('input[name="outsalesOrderDelivery.AirwayBill"]').val("");
});

$('.btn-submit-filter').on('click', function () {
    FunctTrackingFilter();
});

$('.btn-clear-filter').on('click', function () {
    $('input.tracking-filter').val("").change();
    $('select.tracking-filter').val("").change();
    FunctTrackingFilter();
});

function FunctTrackingFilter() {
    $.ajax({
        url: '../../Tracking/TrackingFilter',
        type: 'GET',
        data: $('form').serialize() + '&' + $.param({ dateTimeFrom: $('input[name="datetimepicker-from"').val() }, true) + '&' + $.param({ dateTimeTo: $('input[name="datetimepicker-to"').val() }, true),
        dataType: 'json',
        beforeSend: function () {
            $("#tbl-index").DataTable().clear().draw();
        },
        success: function (data) {
            var dataTable = $("#tbl-index").dataTable().api();
            var consignee = "";
            for (let i = 0; i < data.outsalesOrderDeliveries.length; i++) {
                for (let j = 0; j < data.outSalesOrderConsignees.length; j++) {
                    if (data.outSalesOrderConsignees[j].orderId == data.outsalesOrderDeliveries[i].orderId) {
                        consignee = data.outSalesOrderConsignees[j].cneeName;
                        break;
                    }
                }
                tr = document.createElement("tr");
                tr.innerHTML = '<tr>' +
                    '<td>' + data.outsalesOrderDeliveries[i].orderId + '</td>' +
                    '<td>' + new Date(data.outsalesOrderDeliveries[i].outSalesOrder.dateOrdered).toLocaleDateString(['ban', 'id']) + '</td>' +
                    '<td>' + data.outsalesOrderDeliveries[i].outSalesOrder.masDataTenant.name + '</td>' +
                    '<td>' + data.outsalesOrderDeliveries[i].outSalesOrder.masHouseCode.houseName + '</td>' +
                    '<td>' + data.outsalesOrderDeliveries[i].outSalesOrder.masPlatform.name + '</td>' +
                    '<td>' + consignee + '</td>' +
                    '<td>' + data.outsalesOrderDeliveries[i].airwayBill + '</td>' +
                    '<td> <button class="btn btn-sm text-primary btn-edit-airwaybill" onclick="return FunctDetails(\'' + data.outsalesOrderDeliveries[i].orderId + '\',\'' + data.outsalesOrderDeliveries[i].airwayBill + '\')" data-bs-toggle="modal" data-bs-target="#ModalEditAirwayBill"><i class="align-middle fas fa-fw fa-edit"></i></button> </td>' +
                    '</tr > ';
                dataTable.row.add(tr);
                tr.setAttribute('data-id', data.outsalesOrderDeliveries[i].orderId);
                dataTable.draw();
            }
            $('.offcanvas').offcanvas('hide');
        },
        error: function (data) {
            console.log(data.status + ':' + data.statusText, data.responseText);
        }
    });
}


function FunctDetails(OrderId, AirwayBill) {
    $('.modal-title').html('OrderId - ' + OrderId);
    $('input[name="outsalesOrderDelivery.OrderId"]').val(OrderId);
    $('input[name="outsalesOrderDelivery.AirwayBill"]').val(AirwayBill);
}
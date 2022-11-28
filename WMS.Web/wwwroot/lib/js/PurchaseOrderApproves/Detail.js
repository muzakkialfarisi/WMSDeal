$(function () {

    $('#table-purchaseorderproduct').DataTable({
    });

});


$('.btn-productapproval').on('click', function () {
    console.log($(this).data('id'));
    console.log($(this).data('status'));
    var RequestProductId = $(this).data('id');
    var Status = $(this).data('status');

    $.ajax({
        url: '../../PurchaseRequestProducts/GetPurchaseRequestProductById',
        type: 'GET',
        dataType: 'json',
        data: {
            RequestProductId: RequestProductId,
        },
        beforeSend: function () {
            $('.Quantity').show();
            $('input[name="IncRequestPurchaseProduct.RequestProductId"]').val("");
            $('input[name="IncRequestPurchaseProduct.Quantity"]').val("");
            $('input[name="IncRequestPurchaseProduct.ApprovedQuantity"]').val("");
            $('input[name="IncRequestPurchaseProduct.ApprovedQuantity"]').prop('min', 1);
            $('input[name="IncRequestPurchaseProduct.ApprovedQuantity"]').prop('readonly', false);
            $('input[name="IncRequestPurchaseProduct.Status"]').val("Approved");
        },
        success: function (data) {
            console.log(data);
            $('input[name="IncRequestPurchaseProduct.RequestProductId"]').val(data.requestProductId);
            $('input[name="IncRequestPurchaseProduct.Quantity"]').val(data.quantity);
            $('input[name="IncRequestPurchaseProduct.ApprovedQuantity"]').val(data.approvedQuantity);
            $('input[name="IncRequestPurchaseProduct.ApprovedQuantity"]').prop('max', data.quantity);
            $('textarea[name="IncRequestPurchaseProduct.ApprovedMemo"]').html("");
            if (Status == "Rejected") {
                $('.Quantity').hide();
                $('input[name="IncRequestPurchaseProduct.Status"]').val("Rejected");
            }
        },
        error: function () {
        }
    });

});

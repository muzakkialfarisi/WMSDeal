$(function () {
    $('.table').DataTable({
    });
});

var today = new Date();
var dd = today.getDate();
var mm = today.getMonth() + 1;
if (dd < 10) {
    dd = '0' + dd
}
if (mm < 10) {
    mm = '0' + mm
}
today = today.getFullYear() + '-' + mm + '-' + dd;

var index = 0;
$('.btn-add').on('click', function() {
    console.log($(this).data("id"));
    if ($("#RequestProductId" + $(this).data("id")).val() == null) {
        $.ajax({
            url: '../../PurchaseRequestProducts/GetPurchaseRequestProductById',
            type: 'GET',
            dataType: 'json',
            data: {
                RequestProductId: $(this).data("id"),
            },
            success: function (data) {
                console.log(data);
                var row = '' +
                    '<div class="rec-element mb-3">' +
                    '<div class="row mb-3">' +
                    '<div class="col-12 col-sm-3 text-center">' +
                    '<img src="../../../img/product/' + data.masProductData.beautyPicture + '" class="rounded-circle" height="60" width="60" asp-append-version="true"/>' +
                    '</div>' +
                    '<div class="col-11 col-sm-8">' +
                    '<div class="mb-2">' +
                    '<strong>' + data.masProductData.productName + '</strong> <br>' +
                    '<input type="hidden" class="form-control RequestProductId" id="RequestProductId' + data.requestProductId + '" name="IncRequestPurchase.IncRequestPurchaseProducts[' + index + '].RequestProductId" value="' + data.requestProductId + '">' +
                    '<input type="hidden" class="form-control ProductId" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].ProductId" value="' + data.productId + '">' +
                    '</div>' +
                    '<div class="row">' +
                    '<div class="col-4 text-primary">' +
                    'SKU' +
                    '</div>' +
                    '<div class="col-8">' +
                    data.masProductData.sku +
                    '</div>' +
                    '</div> ' +
                    '</div>' +
                    '<div class="col-1 col-sm-1">' +
                    '<a class="del-element btn link-primary"><i class="align-middle me-2 fas fa-fw fa-trash"></i></a>' +
                    '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<div class="col-12 col-sm-6">' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Quantity</label>' +
                    '<input type="number" class="form-control Quantity" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].Quantity" value="' + data.approvedQuantity + '" min="1" required onkeyup="update_price()">' +
                    '<span class="text-danger field-validation-valid" data-valmsg-for="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].Quantity" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '<div class="col-12 col-sm-6">' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Purchase Price</label>' +
                    '<input type="number" class="form-control UnitPrice" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].UnitPrice" value="' + data.finalPrice + '" min="0" required onkeyup="update_price()">' +
                    '<span class="text-danger field-validation-valid" data-valmsg-for="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].UnitPrice" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Expected Arrival Date</label>' +
                    '<input type="datetime-local" class="form-control ExpArrivalDate" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].ExpArrivalDate" value="' + data.expArrivalDate +'" min="'+ today +'" required >' +
                    '<span class="text-danger field-validation-valid" data-valmsg-for="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].ExpArrivalDate" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '<hr>' +
                    '</div>';
                $(row).insertBefore("#nextkolom");
                $('#jumlahkolom').val(index + 1);
                index++;

                $('#total-item').html(index);
                total_price = 0;
                $("#card-purchaseorderproducts .rec-element").each(function () {
                    total_price = parseInt(total_price) + parseInt($(this).find('.UnitPrice').val()) * parseInt($(this).find('.Quantity').val());
                })
                $('#total-price').html(total_price.toLocaleString("en"));
            },
            error: function () {
            }
        });
    }
});

$(document).on('click', '.del-element', function (e) {
    e.preventDefault()
    index--;
    $('#total-item').html(index);
    $(this).parents('.rec-element').remove();
    $('#jumlahkolom').val(index - 1);
    resetValues();
});

function resetValues() {
    counter = 0;
    total_price = 0;
    $("#card-purchaseorderproducts .rec-element").each(function () {
        $(this).find('.RequestProductId').attr("name", "IncRequestPurchase.IncRequestPurchaseProducts[" + counter + "].RequestProductId");
        $(this).find('.ProductId').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].ProductId");
        $(this).find('.Quantity').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].Quantity");
        $(this).find('.UnitPrice').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].UnitPrice");
        $(this).find('.ExpArrivalDate').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].ExpArrivalDate");
        total_price = parseInt(total_price) + parseInt($(this).find('.UnitPrice').val()) * parseInt($(this).find('.Quantity').val());
        counter++; //increment count
    });
    $('#total-price').html(total_price.toLocaleString("en"));
}

function update_price() {
    total_price = 0;
    $("#card-purchaseorderproducts .rec-element").each(function () {
        total_price = total_price + parseInt($(this).find('.UnitPrice').val()) * parseInt($(this).find('.Quantity').val());
    })
    $('#total-price').html(total_price.toLocaleString("en"));
}
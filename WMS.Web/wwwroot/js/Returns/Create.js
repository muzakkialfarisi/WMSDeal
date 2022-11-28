$(function () {
    $('#tbl-salesorderproducts').DataTable({
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
$('.btn-add').on('click', function () {
    console.log($(this).data("id"));
    if ($("#OrdProductId" + $(this).data("id")).val() == null) {
        $.ajax({
            url: '../SalesOrders/GetSalesORderProductByOrdProductId',
            type: 'GET',
            dataType: 'json',
            data: {
                OrdProductId: $(this).data("id"),
            },
            success: function (data) {
                console.log(data);
                var expireddate = '';
                if (data.masProductData.storageMethod == "FEFO") {
                    expireddate = '' +
                        '<div class="mb-3 form-group required">' +
                        '<label class="control-label">Expired Date</label>' +
                        '<input type="datetime-local" class="form-control DateOfExpired" name="invReturn.InvReturnProducts[' + index + '].DateOfExpired" min="' + today + '" required>' +
                        '<span class="text-danger field-validation-valid spanDateOfExpired" data-valmsg-for="invReturn.InvReturnProducts[' + index + '].DateOfExpired" data-valmsg-replace="true"></span>' +
                        '</div>';
                }
                var row = '' +
                    '<div class="rec-element mb-3">' +
                    '<div class="row mb-3">' +
                    '<div class="col-12 col-sm-3 text-center">' +
                    '<img src="../../../img/product/' + data.masProductData.beautyPicture + '" class="rounded-circle" height="60" width="60" asp-append-version="true"/>' +
                    '</div>' +
                    '<div class="col-12 col-sm-9">' +
                    '<div class="mb-2 row">' +
                    '<div class="col-10">' +
                    '<strong>' + data.masProductData.productName + '</strong> <br>' +
                    '<input type="hidden" class="form-control OrdProductId" id="OrdProductId' + data.ordProductId + '" name="invReturn.InvReturnProducts[' + index + '].OrdProductId" value="' + data.ordProductId + '">' +
                    '<input type="hidden" class="form-control ProductId" name="invReturn.InvReturnProducts[' + index + '].ProductId" value="' + data.productId + '">' +
                    '</div>' +
                    '<div class="col-2">' +
                    '<a class="del-element btn link-primary"><i class="align-middle me-2 fas fa-fw fa-trash"></i></a>' +
                    '</div>' +
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
                    '</div>' +
                    '<div class="row">' +
                    '<div class="col-12 col-sm-12">' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Quantity</label>' +
                    '<input type="number" class="form-control Quantity" name="invReturn.InvReturnProducts[' + index + '].Quantity" value="' + data.quantity + '" min="0" max="' + data.quantity + '" required >' +
                    '<span class="text-danger field-validation-valid spanQuantity" data-valmsg-for="invReturn.InvReturnProducts[' + index + '].Quantity" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    expireddate +
                    '<hr>' +
                    '<input type="number" class="form-control UnitPrice" name="invReturn.InvReturnProducts[' + index + '].UnitPrice" value="' + data.unitPrice + '" min="0" max="' + data.unitPrice + '" required hidden >' +
                    '</div>';
                $(row).insertBefore("#nextkolom");
                $('#jumlahkolom').val(index + 1);
                index++;

                $('#total-item').html(index);
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
    $("#card-products .rec-element").each(function () {
        $(this).find('.OrdProductId').attr("name", "invReturn.InvReturnProducts[" + counter + "].OrdProductId");
        $(this).find('.ProductId').attr("name", "invReturn.InvReturnProducts[" + counter + "].ProductId");
        $(this).find('.UNitPrice').attr("name", "invReturn.InvReturnProducts[" + counter + "].UNitPrice");
        $(this).find('.Quantity').attr("name", "invReturn.InvReturnProducts[" + counter + "].Quantity");
        $(this).find('.DateOfExpired').attr("name", "invReturn.InvReturnProducts[" + counter + "].DateOfExpired");

        $(this).find('.spanQuantity').attr("data-valmsg-for", "invReturn.InvReturnProducts[" + counter + "].Quantity");
        $(this).find('.spanDateOfExpired').attr("data-valmsg-for", "invReturn.InvReturnProducts[" + counter + "].DateOfExpired");
        counter++; //increment count
    });
}

$('#datetimepicker-datedelivered').datetimepicker({
    viewMode: 'months',
    format: 'DD/MM/YYYY'
});
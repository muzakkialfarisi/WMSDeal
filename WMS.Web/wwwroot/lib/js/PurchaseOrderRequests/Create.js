$(function () {

    $('.table').DataTable({
        pageLength: 5,
        lengthMenu: [[5, 10], [5, 10]]
    });

    if ($('select[name="TenantId"]').val() != "") {
        $.ajax({
            url: '../../Product/GetProductsByTenant',
            type: 'GET',
            dataType: 'json',
            data: {
                TenantId: $('select[name="TenantId"]').val(),
            },
            beforeSend: function () {
                $("#ListProductofTenant").DataTable().clear().draw();
                $('#ProductId').empty();
            },
            success: function (data) {
                $('#ProductId').append('<option selected disabled>Select...</option>');
                var dataTable = $("#ListProductofTenant").dataTable().api();
                for (let i = 0; i < data.length; i++) {
                    tr = document.createElement("tr");
                    tr.innerHTML = '' +
                        '<tr>' +
                        '<td class="text-center"> <img class="rounded-circle" height="45" width="45" src="../../../img/product/' + data[i].beautyPicture + '"></td>' +
                        '<td><strong>' + data[i].sku + '</strong><br>' + data[i].productName + '</td>' +
                        '<td class="text-center">' + data[i].invStorageZone.zoneName + '</td>' +
                        '<td class="text-center">' + data[i].invStorageSize.sizeName + '</td>' +
                        '<td class="text-center"><a class="btn link-primary" onClick="AddProduct(\'' + data[i].productId + '\')"><i class="fas fa-fw fa-angle-double-right"></i></a></td>' +
                        '</tr>';

                    dataTable.row.add(tr);
                    dataTable.draw();
                }
            },
            error: function (response) {
                console.log(response.responseText);
            }
        });
    }
    
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
function AddProduct(ProductId) {
    if ($('#ProductId' + ProductId + '').val() == null) {
        $.ajax({
            url: '../../Product/GetProductByProductId',
            type: 'GET',
            dataType: 'json',
            data: {
                ProductId: ProductId,
            },
            success: function (data) {
                var row = '' +
                    '<div class="rec-element mb-3">' +
                    '<div class="row mb-3">' +
                        '<div class="col-12 col-sm-3 text-center">' +
                            '<img src="../../../img/product/' + data.beautyPicture + '" class="rounded-circle" height="45" width="45" asp-append-version="true"/>' +
                        '</div>' +
                        '<div class="col-11 col-sm-8">' +
                            '<div class="mb-2">' +
                                '<strong>' + data.productName + '</strong> <br>' +
                    '<input type="hidden" class="form-control ProductId" id="ProductId' + data.productId + '" name="IncRequestPurchaseProducts[' + index + '].ProductId" value="' + data.productId + '">' +
                            '</div>' +
                            '<div class="row">' +
                                '<div class="col-4 text-primary">' +
                                    'SKU' +
                                '</div>' +
                                '<div class="col-8">' +
                                    data.sku +
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
                    '<label class="control-label">Bid Price</label>' +
                    '<input type="number" class="form-control BidPrice" name="IncRequestPurchaseProducts[' + index + '].BidPrice" value="' + data.purchasePrice + '" min="0" required>' +
                    '<span class="text-danger field-validation-valid spanBidPrice" data-valmsg-for="IncRequestPurchaseProducts[' + index + '].BidPrice" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '<div class="col-12 col-sm-6">' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Negotiate Price</label>' +
                    '<input type="number" class="form-control NegotiatedPrice" name="IncRequestPurchaseProducts[' + index + '].NegotiatedPrice" value="' + data.purchasePrice + '" min="0" required>' +
                    '<span class="text-danger field-validation-valid spanNegotiatedPrice" data-valmsg-for="IncRequestPurchaseProducts[' + index + '].NegotiatedPrice" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<div class="col-12 col-sm-6">' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Quantity</label>' +
                    '<input type="number" class="form-control Quantity" name="IncRequestPurchaseProducts[' + index + '].Quantity" value="1" min="1" required onkeyup="update_price()">' +
                    '<span class="text-danger field-validation-valid spanQuantity" data-valmsg-for="IncRequestPurchaseProducts[' + index + '].Quantity" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '<div class="col-12 col-sm-6">' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Purchase Price</label>' +
                    '<input type="number" class="form-control FinalPrice" name="IncRequestPurchaseProducts[' + index + '].FinalPrice" value="' + data.purchasePrice + '" min="0" required onkeyup="update_price()">' +
                    '<span class="text-danger field-validation-valid spanFinalPrice" data-valmsg-for="IncRequestPurchaseProducts[' + index + '].FinalPrice" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Expected Arrival Date</label>' +
                    '<input type="datetime-local" class="form-control ExpArrivalDate" name="IncRequestPurchaseProducts[' + index + '].ExpArrivalDate" min="'+ today +'" required>' +
                    '<span class="text-danger field-validation-valid spanExpArrivalDate" data-valmsg-for="IncRequestPurchaseProducts[' + index + '].ExpArrivalDate" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '<div class="mb-3 form-group">' +
                    '<label class="control-label">Note</label>' +
                    '<textarea class="form-control Memo" name="IncRequestPurchaseProducts[' + index + '].Memo"></textarea>' +
                    '<span class="text-danger field-validation-valid spanMemo" data-valmsg-for="IncRequestPurchaseProducts[' + index + '].Memo" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '<hr>' +
                    '</div>';
                $(row).insertBefore("#nextkolom");
                $('#jumlahkolom').val(index + 1);
                index++;

                $('#total-item').html(index);
                total_price = 0;
                $("#card-purchaseorderproducts .rec-element").each(function () {
                    total_price = parseInt(total_price) + parseInt($(this).find('.FinalPrice').val()) * parseInt($(this).find('.Quantity').val());
                })
                $('#total-price').html(total_price.toLocaleString("en"));
            },
            error: function () {
            }
        });
    }

};

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
        $(this).find('.ProductId').attr("name", "IncRequestPurchaseProducts[" + counter + "].ProductId");
        $(this).find('.BidPrice').attr("name", "IncRequestPurchaseProducts[" + counter + "].BidPrice");
        $(this).find('.spanBidPrice').attr("data-valmsg-for", "IncRequestPurchaseProducts[" + counter + "].BidPrice");
        $(this).find('.NegotiatedPrice').attr("name", "IncRequestPurchaseProducts[" + counter + "].NegotiatedPrice");
        $(this).find('.spanNegotiatedPrice').attr("data-valmsg-for", "IncRequestPurchaseProducts[" + counter + "].NegotiatedPrice");
        $(this).find('.Quantity').attr("name", "IncRequestPurchaseProducts[" + counter + "].Quantity");
        $(this).find('.sanQuantity').attr("data-valmsg-for", "IncRequestPurchaseProducts[" + counter + "].Quantity");
        $(this).find('.FinalPrice').attr("name", "IncRequestPurchaseProducts[" + counter + "].FinalPrice");
        $(this).find('.spanFinalPrice').attr("data-valmsg-for", "IncRequestPurchaseProducts[" + counter + "].FinalPrice");
        $(this).find('.ExpArrivalDate').attr("name", "IncRequestPurchaseProducts[" + counter + "].ExpArrivalDate");
        $(this).find('.spanExpArrivalDate').attr("data-valmsg-for", "IncRequestPurchaseProducts[" + counter + "].ExpArrivalDate");
        $(this).find('.Memo').attr("name", "IncRequestPurchaseProducts[" + counter + "].Memo");
        $(this).find('.spanMemo').attr("data-valmsg-for", "IncRequestPurchaseProducts[" + counter + "].Memo");
        total_price = parseInt(total_price) + parseInt($(this).find('.FinalPrice').val()) * parseInt($(this).find('.Quantity').val());
        counter++; //increment count
    })
    $('#total-price').html(total_price.toLocaleString("en"));
}

function update_price() {
    total_price = 0;
    $("#card-purchaseorderproducts .rec-element").each(function () {
        total_price = total_price + parseInt($(this).find('.FinalPrice').val()) * parseInt($(this).find('.Quantity').val());
    })
    $('#total-price').html(total_price.toLocaleString("en"));
}


$('select[name="TenantId"]').on('change', function () {
    index = 0;
    $('#total-item').html(index);
    $('#total-price').html(0);
    $('.rec-element').remove();
    $('#jumlahkolom').val(index - 1);

    $.ajax({
        url: '../../TenantDivision/GetTenantDivisionsByTenantId',
        type: 'GET',
        dataType: 'json',
        data: {
            TenantId: $(this).val(),
        },
        beforeSend: function () {
            $('select[name="TenantDivisionId"]').empty();
            $('select[name="TenantDivisionId"]').append('<option selected disabled>Select...</option>');
        },
        success: function (data) {
            for (let i = 0; i < data.length; i++) {
                $('select[name="TenantDivisionId"]').append('<option value="'+ data[i].id +'">'+ data[i].name +'</option>');
            }
        },
        error: function () {
        }
    });

    $.ajax({
        url: '../../TenantWarehouse/GetTenantWarehousesByTenantId',
        type: 'GET',
        dataType: 'json',
        data: {
            TenantId: $(this).val(),
        },
        beforeSend: function () {
            $('select[name="TenantHouseId"]').empty();
            $('select[name="TenantHouseId"]').append('<option selected disabled>Select...</option>');
        },
        success: function (data) {
            for (let i = 0; i < data.length; i++) {
                $('select[name="TenantHouseId"]').append('<option value="' + data[i].id + '">' + data[i].masHouseCode.houseName + '</option>');
            }
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });

    $.ajax({
        url: '../../Product/GetProductsByTenant',
        type: 'GET',
        dataType: 'json',
        data: {
            TenantId: $(this).val(),
        },
        beforeSend: function () {
            $("#ListProductofTenant").DataTable().clear().draw();
        },
        success: function (data) {
            var dataTable = $("#ListProductofTenant").dataTable().api();
            for (let i = 0; i < data.length; i++) {
                tr = document.createElement("tr");
                tr.innerHTML = '' +
                    '<tr>' +
                    '<td class="text-center"> <img class="rounded-circle" height="45" width="45" src="../../../img/product/' + data[i].beautyPicture + '"></td>' +
                    '<td><strong>' + data[i].sku + '</strong><br>' + data[i].productName + '</td>' +
                    '<td class="text-center">' + data[i].invStorageZone.zoneName + '</td>' +
                    '<td class="text-center">' + data[i].invStorageSize.sizeName + '</td>' +
                    '<td class="text-center"><a class="btn link-primary" onClick="AddProduct(\'' + data[i].productId + '\')"><i class="fas fa-fw fa-angle-double-right"></i></a></td>' +
                    '</tr>';

                dataTable.row.add(tr);
                tr.setAttribute('data-id', data[i].productId);
                dataTable.draw();
            }
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });
});

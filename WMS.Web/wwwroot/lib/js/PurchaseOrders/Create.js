$(function () {

    $('.table').DataTable({
        pageLength: 5,
        lengthMenu: [[5, 10], [5, 10]]
    });

    if ($('select[name="IncPurchaseOrder.TenantId"]').val() != "") {
        console.log("masku");
        $.ajax({
            url: '../../Product/GetProductsByTenant',
            type: 'GET',
            dataType: 'json',
            data: {
                TenantId: $('select[name="IncPurchaseOrder.TenantId"]').val(),
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
                        '<td class="text-center"> <img class="rounded-circle" height="60" width="60" src="../../../img/product/' + data[i].beautyPicture + '"></td>' +
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
                            '<img src="../../../img/product/' + data.beautyPicture + '" class="rounded-circle" height="60" width="60" asp-append-version="true"/>' +
                        '</div>' +
                        '<div class="col-11 col-sm-8">' +
                            '<div class="mb-2">' +
                                '<strong>' + data.productName + '</strong> <br>' +
                                '<input type="hidden" class="form-control ProductId" id="ProductId' + data.productId + '" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].ProductId" value="' + data.productId + '">' +
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
                    '<label class="control-label">Quantity</label>' +
                    '<input type="number" class="form-control Quantity" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].Quantity" value="1" min="1" required onkeyup="update_price()">' +
                    '<span class="text-danger field-validation-valid spanQuantity" data-valmsg-for="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].Quantity" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '<div class="col-12 col-sm-6">' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Purchase Price</label>' +
                    '<input type="number" class="form-control UnitPrice" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].UnitPrice" value="' + data.purchasePrice + '" min="0" required onkeyup="update_price()">' +
                    '<span class="text-danger field-validation-valid spanUnitPrice" data-valmsg-for="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].UnitPrice" data-valmsg-replace="true"></span>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="mb-3 form-group required">' +
                    '<label class="control-label">Expected Arrival Date</label>' +
                    '<input type="datetime-local" class="form-control ExpArrivalDate" name="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].ExpArrivalDate" min="'+ today +'" required>' +
                    '<span class="text-danger field-validation-valid spanExpArrivalDate" data-valmsg-for="IncPurchaseOrder.IncPurchaseOrderProducts[' + index + '].ExpArrivalDate" data-valmsg-replace="true"></span>' +
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
        $(this).find('.ProductId').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].ProductId");
        $(this).find('.Quantity').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].Quantity");
        $(this).find('.spanQuantity').attr("data-valmsg-for", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].Quantity");
        $(this).find('.UnitPrice').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].UnitPrice");
        $(this).find('.spanUnitPrice').attr("data-valmsg-for", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].UnitPrice");
        $(this).find('.ExpArrivalDate').attr("name", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].ExpArrivalDate");
        $(this).find('.spanExpArrivalDate').attr("data-valmsg-for", "IncPurchaseOrder.IncPurchaseOrderProducts[" + counter + "].ExpArrivalDate");
        total_price = parseInt(total_price) + parseInt($(this).find('.UnitPrice').val()) * parseInt($(this).find('.Quantity').val());
        counter++; //increment count
    })
    $('#total-price').html(total_price.toLocaleString("en"));
}

function update_price() {
    total_price = 0;
    $("#card-purchaseorderproducts .rec-element").each(function () {
        total_price = total_price + parseInt($(this).find('.UnitPrice').val()) * parseInt($(this).find('.Quantity').val());
    })
    $('#total-price').html(total_price.toLocaleString("en"));
}


$('select[name="IncPurchaseOrder.TenantId"]').on('change', function () {
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
            $('select[name="IncPurchaseOrder.TenantDivisionId"]').empty();
            $('select[name="IncPurchaseOrder.TenantDivisionId"]').append('<option selected disabled>Select...</option>');
        },
        success: function (data) {
            for (let i = 0; i < data.length; i++) {
                $('select[name="IncPurchaseOrder.TenantDivisionId"]').append('<option value="'+ data[i].id +'">'+ data[i].name +'</option>');
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
            $('select[name="IncPurchaseOrder.TenantHouseId"]').empty();
            $('select[name="IncPurchaseOrder.TenantHouseId"]').append('<option selected disabled>Select...</option>');
        },
        success: function (data) {
            for (let i = 0; i < data.length; i++) {
                $('select[name="IncPurchaseOrder.TenantHouseId"]').append('<option value="' + data[i].id + '">' + data[i].masHouseCode.houseName + '</option>');
            }
        },
        error: function (response) {
            console.log(response.responseText);
        }
    });

    $.ajax({
        url: '../../Supplier/GetSupplierByTenantId',
        type: 'GET',
        dataType: 'json',
        data: {
            TenantId: $(this).val(),
        },
        beforeSend: function () {
            $('select[name="IncPurchaseOrder.SupplierId"]').empty();
            $('select[name="IncPurchaseOrder.SupplierId"]').append('<option selected disabled>Select...</option>');
        },
        success: function (data) {
            for (let i = 0; i < data.length; i++) {
                $('select[name="IncPurchaseOrder.SupplierId"]').append('<option value="' + data[i].supplierId + '">' + data[i].name + '</option>');
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
            $('#ProductId').empty();
        },
        success: function (data) {
            $('#ProductId').append('<option selected disabled>Select...</option>');
            var dataTable = $("#ListProductofTenant").dataTable().api();
            for (let i = 0; i < data.length; i++) {
                tr = document.createElement("tr");
                tr.innerHTML = '' +
                    '<tr>' +
                    '<td class="text-center"> <img class="rounded-circle" height="60" width="60" src="../../../img/product/' + data[i].beautyPicture + '"></td>' +
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
const checkPasswordValidity = (value) => {
    const isNonWhiteSpace = /^\S*$/;
    if (!isNonWhiteSpace.test(value)) {
        return "Password must not contain Whitespaces.";
    }

    const isContainsUppercase = /^(?=.*[A-Z]).*$/;
    if (!isContainsUppercase.test(value)) {
        return "Password must have at least one Uppercase Character.";
    }

    const isContainsLowercase = /^(?=.*[a-z]).*$/;
    if (!isContainsLowercase.test(value)) {
        return "Password must have at least one Lowercase Character.";
    }

    const isContainsNumber = /^(?=.*[0-9]).*$/;
    if (!isContainsNumber.test(value)) {
        return "Password must contain at least one Digit.";
    }

    const isContainsSymbol =
        /^(?=.*[~`!@#$%^&*()--+={}\[\]|\\:;"'<>,.?/_₹]).*$/;
    if (!isContainsSymbol.test(value)) {
        return "Password must contain at least one Special Symbol.";
    }

    const isValidLength = /^.{6,16}$/;
    if (!isValidLength.test(value)) {
        return "Password must be 10-16 Characters Long.";
    }

    return true;
}

$('#btn-EditPassword').on('click', function () {
    const message = checkPasswordValidity($('input[name="NewPassword"]').val());
    if (message != true) {
        toastr.error(message, '', {
            positionClass: 'toast-top-right',
            closeButton: false,
            progressBar: true,
            newestOnTop: true,
            rtl: $("body").attr("dir") === "rtl" || $("html").attr("dir") === "rtl",
            timeOut: 3000
        });
    }
    else {
        if ($('input[name="NewPassword"]').val() != $('input[name="VerifyPassword"]').val()) {
            toastr.error("Password Does'nt Match", '', {
                positionClass: 'toast-top-right',
                closeButton: false,
                progressBar: true,
                newestOnTop: true,
                rtl: $("body").attr("dir") === "rtl" || $("html").attr("dir") === "rtl",
                timeOut: 3000
            });
        }
        else {
            $('#form-EditPassword').submit(function (e) {
                e.preventDefault();

                $.ajax({
                    type: 'POST',
                    url: '../../Home/EditPassword',
                    data: {
                        UserId: $('input[name="UserId"]').val(),
                        Password: $('input[name="Password"]').val(),
                        NewPassword: $('input[name="NewPassword"]').val(),
                        VerifyPassword: $('input[name="VerifyPassword"]').val(),
                    },
                    beforeSend: function () {
                        $('input[name="Password"]').val("");
                        $('input[name="NewPassword"]').val("");
                        $('input[name="VerifyPassword"]').val("");
                    },
                    success: function (data) {
                        console.log(data);
                        if (data.statusCode != 200) {
                            toastr.error('Password Was Wrong!', '', {
                                positionClass: 'toast-top-right',
                                closeButton: false,
                                progressBar: true,
                                newestOnTop: true,
                                rtl: $("body").attr("dir") === "rtl" || $("html").attr("dir") === "rtl",
                                timeOut: 3000
                            });
                        }
                        else {
                            toastr.success('Password Updated Successfully!', '', {
                                positionClass: 'toast-top-right',
                                closeButton: false,
                                progressBar: true,
                                newestOnTop: true,
                                rtl: $("body").attr("dir") === "rtl" || $("html").attr("dir") === "rtl",
                                timeOut: 3000
                            });
                        }
                    },
                    error: function (data) {
                        console.log(data.status + ':' + data.statusText, data.responseText);
                    }
                });
            })
        }
    }
});
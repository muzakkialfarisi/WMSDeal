$(function () {
    $('.table').DataTable({
    });

    $("#smartwizard-arrows-success").smartWizard({
        theme: "arrows",
        selected: $('input[name="step-wizard"]').val(),
        showStepURLhash: false,
        toolbarSettings: {
            showNextButton: false, // show/hide a Next button
            showPreviousButton: false, // show/hide a Previous button
        }
    });
});


$(document).ready(function () {
    updateVisibility($('#KeyType').val());

    $('#KeyType').change(function () {
        var selectedKeyType = $(this).val();
        updateVisibility(selectedKeyType);
    });

    function updateVisibility(selectedKeyType) {
        $('#linearCoefficients').hide();
        $('#nonlinearCoefficients').hide();
        $('#password').hide();

        if (selectedKeyType === 'LinearEquation') {
            $('#linearCoefficients').show();
        } else if (selectedKeyType === 'NonlinearEquation') {
            $('#nonlinearCoefficients').show();
        } else if (selectedKeyType === 'Password') {
            $('#password').show();
        }

        $('#SelectedKeyType').val(selectedKeyType);
    }
});

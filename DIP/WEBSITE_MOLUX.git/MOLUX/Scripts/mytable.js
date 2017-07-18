$(document).ready(function () {
    $('.nomenutable').DataTable(
    {
        "bLengthChange": true,
        "info": true,
        "aSort": true,
        "bSort": true,
        "pageLength": 10,
        "ordering": true,
        "lengthMenu": [
                [10, 20, 50, 100, -1],
                [10, 20, 50, 100, "All"]
        ]
    });
});
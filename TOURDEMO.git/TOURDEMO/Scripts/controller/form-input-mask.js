var FormInputMask = function () {
    
    var handlecurrency = function () {
        $(".currency").inputmask("decimal", {
            radixPoint: ".",
            autoGroup: true,
            groupSeparator: ",",
            groupSize: 3,
            autoUnmask: true,
            removeMaskOnSubmit: true
        });
    }
    var handleidentity = function ()
    {
        $(".indentity").inputmask("999 999 999 999", {
            placeholder: ""
            ,greedy: false
            , autoUnmask: true
            , removeMaskOnSubmit: true
        });
    }
    return {
        //main function to initiate the module
        init: function () {
            handlecurrency();
            handleidentity();
        },
        initiden: function() {
            handleidentity();
        },
        initcur: function() {
            handlecurrency();
        }
    };

}();
// init metronic core componets
$(document).ready(function () {
    FormInputMask.init();
});

var ODLeaveApplication = (function($, w, d) {
    var 
        main, _init, _initbutton, _initother, _setEmployeeid, _saveodleave, _processSaveODLeave, _validate;
    var 
        $Employee_id = $('#employee_id'),
        $save_form = $('#saveODLeaveForm'),
		$save_button = $('#saveODLeaveButton');
    var 
	    page_name = 'apply.aspx'
    employee_data_URL = page_name + '/ValidateEmployeeId';


    _setEmployeeid = function() {
        var user_details = sessvars.TAMS_ENV.user_details;
        if (user_details.user_name != "admin") {
            $('#employee_id').val(user_details.employee_id);
        }
    };

    _processEmployeeData = function(data) {
        var 
			status = data.status,
			message = data.return_data;


        if (status === 'success') {
            
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data });
            $save_form[0].reset();
            $employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
        }
    };

    _failure = function() {

        SAXAlert.show({ 'type': 'error', 'message': 'An error occurred while loading data for the page. Please try again. If the error persists, please contact Support.' });
        SAXLoader.close();
    };
    
    _onemployeeidchange = function() {
        var 
			ajax_options = {},
			data = SAXForms.get($save_form)
        employee_id = data['employee_id'];

        $.ajax({
            type: "POST",
            url: employee_data_URL,
            data: JSON.stringify({ employee_id: employee_id }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: successFunc,
            error: _failure
        });

        function successFunc(data) {
            _processEmployeeData(data.d);
        }


    };

    _initOther = function() {
        $('.datepicker').Zebra_DatePicker({
            format: 'd-M-Y'
        });

        $Employee_id.change(function() {
            _onemployeeidchange();
        });
    };

    _validate = function() {

        var 
			data = SAXForms.get($save_form),
			from_date = data['from_date'],
			to_date = data['to_date'],
			employee_id = data['employee_id']
        od_types = data['od_types'];

        if (from_date === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a From Date.' });
            return false;
        }
        if (to_date === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a To Date.' });
            return false;
        }

        if (moment(from_date) > moment(to_date)) {
            SAXAlert.show({ 'type': 'error', 'message': 'From Date cannot be greater than To Date.' });
            return false;
        }

        if (employee_id === '') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please enter an Employee ID.' });
            return false;
        }

        if (od_types === 'select') {
            SAXAlert.show({ 'type': 'error', 'message': 'Please select OD types.' });
            return false;
        }

        return true;
    };

    _processSaveLeave = function(data, additional) {

        var 
			status = data.status,
			message = data.return_data;

        if (status === 'success') {
            $save_form[0].reset();
            $Employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
        }

        SAXLoader.closeBlockingLoader();
        SAXAlert.show({ 'type': status, 'message': message });
        $save_button.button('reset');
    };

    _saveODLeave = function() {
        var 
			ajax_options = {},
			data = SAXForms.get($save_form);
        console.log(data);



        if (_validate()) {

            ajax_options = {
                'url': page_name + '/SubmitLeave_OD',
                'data': { current: JSON.stringify(data) },
                'callback': _processSaveLeave,
                'additional': {}
            }

            $save_button.button('loading');
            SAXLoader.showBlockingLoader();
            SAXHTTP.ajax(ajax_options);
        }

    };
    _initButtons = function() {
        var role = '',
			button_actions = {
			    'save-ODleave': function() {
			        _saveODLeave();
			    }
			};
        $('body').on('click', '[data-control="button"]', function(event) {
            role = $(event.target).data('role');
            button_actions[role].call(this, event);
        });
    };
    main = function() {

        _setEmployeeid();
        _initButtons();
        _initOther();
    };

    return {
        'main': main
    };
})(jQuery, window, document);
$(function() {
	ODLeaveApplication.main();
});
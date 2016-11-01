var OvertimeApplication = function($,w,d)
{
    var
        main,_setEmployeeid,_initButtons,_initOther,_validate,_getOtdata,_success,_failure,_processAvailableot,_renderData,_saveOT,_processSaveOT;
    var 
        $employee_id = $('#employee_id'),
        $date = $('#date'),
        $available_ot = $('#available_ot'),
        $save_form = $('#otApplicationForm'),
		$save_button = $('#save_OTbutton');
    var 
        page_name = 'apply.aspx',
        available_overtime_hrs_URL = page_name + '/CheckAvailableOT';
    
    _setEmployeeid =function()
    {
        var user_details = sessvars.TAMS_ENV.user_details;
	    if (user_details.user_name != "admin") 
	    {
		    $('#employee_id').val( user_details.employee_id);
	    }
    };
    
    _initOther = function()
    {
        $('.date-picker').Zebra_DatePicker({
		    format: 'd-M-Y',
		    onSelect: function () {
		        _getOtdata();
		    }
	    });     
		    
    };
    
    _renderData = function (data) {

		var data_length = data.length; 
		$available_ot.val(data);
            
	};
    _processAvailableot = function (data) {

        var status = data.status,
            results = {};

        if (status === 'success') { 

            results = JSON.parse(data.return_data);
            _renderData(results);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': data.return_data});
        }  
	};
	
    _failure = function () {

		SAXAlert.show({'type': 'error', 'message': 'An error occurred while loading OT data for the page. Please try again. If the error persists, please contact Support.'});
		SAXLoader.close();
	};

	_success = function (data1) {
	console.log(data1.d)

		_processAvailableot(data1.d);

		SAXLoader.close();
	}
    
    _getOtdata = function () {

		var 
			employee_id = $employee_id.val(), 
			date = $date.val(),
			promise1 = {}; 

			
		promise1 = $.ajax({url: available_overtime_hrs_URL, type: "POST", contentType: 'application/json; charset=utf-8', dataType: 'json', data: JSON.stringify({otdate:date , employee_id: employee_id}) });

        SAXLoader.show();
        $.when(promise1).then(_success, _failure);
	};


    _validate = function () {

		var
			data = SAXForms.get($save_form),
			otdate = data['date'],
			othrs = data['available_ot'],
			employee_id = data['employee_id'];

		if (otdate === '') {
			SAXAlert.show({'type': 'error', 'message': 'Please enter a Date.'});
			return false;
		}
		if (othrs === '') {
			SAXAlert.show({'type': 'error', 'message': 'OT is not available.'});
			return false;
		}

		if (employee_id === '') {
			SAXAlert.show({'type': 'error', 'message': 'Please enter an Employee ID.'});
			return false;
		}

		return true;
	};
	
	_processSaveOT = function(data)
	{
	    var
			status = data.status,
			message = data.return_data;
			console.log(status);

		if (status === 'success') {
			$save_form[0].reset();
			$employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
		}

		SAXLoader.closeBlockingLoader();
		SAXAlert.show({'type': status, 'message': message});
		$save_button.button('reset');
	};
	
	_saveOT = function()
	{
	var
			ajax_options = {},
			data = SAXForms.get($save_form);
			

		if (_validate()) {

			ajax_options = {
				'url': page_name + '/SubmitOT',
				'data': {jsonData: JSON.stringify(data)},
				'callback': _processSaveOT,
				'additional': {}
			}

			$save_button.button('loading');
			SAXLoader.showBlockingLoader();
			SAXHTTP.ajax(ajax_options);
			
		}
	};
	
	
    _initButtons = function()
    {
        var role = '';
        var button_actions = 
            {
                 'save_OT':function()
                 {
                    _saveOT();
                 }
                 
             };
         $('body').on('click', '[data-control="button"]', function (event) {
			role = $(event.target).data('role');
			button_actions[role].call(this, event);
		});
    };
    
    main = function () 
    {
		_setEmployeeid();
		
		_initButtons();
		_initOther();
	};
	

return {
		'main': main
	};
}(jQuery, window, document);

$(function() {
	OvertimeApplication.main();
});
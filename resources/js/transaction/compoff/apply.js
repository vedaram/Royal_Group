var  CompoffApplication  = (function($,w,d)
{
    var 
        main, _init, _initButtons, _initOther, 
        _setEmployeeid, 
        _saveCompoff, _processCompoff,_validate;
        
    var
        $Employee_id = $('#employee_id'),
        $save_form = $('#saveCompoffForm'),
        $from_date = $('#from_date');
		$save_button = $('#saveCompoffButton');
		
	var 
	    page_name = 'apply.aspx';
	    
	    
    _setEmployeeid = function()
    {
        var user_details = sessvars.TAMS_ENV.user_details;
	    if (user_details.user_name != "admin") 
	    {
		    $('#employee_id').val( user_details.employee_id);
	    }
    };

    _failure = function () {

		SAXAlert.show({'type': 'error', 'message': 'An error occurred while loading OT data for the page. Please try again. If the error persists, please contact Support.'});
		SAXLoader.close();
	};
	
	_validate = function () 
	{

		var
			data = SAXForms.get($save_form),
			from_date = data['from_date'],
			to_date = data['to_date'],
			employee_id = data['employee_id']
			 current_date = new Date();
			
        if (employee_id === '') {
			SAXAlert.show({'type': 'error', 'message': 'Please enter an Employee ID.'});
			return false;
		}
		if (from_date === '') {
			SAXAlert.show({'type': 'error', 'message': 'Please enter a From Date.'});
			return false;
		}
		if (to_date === '') {
			SAXAlert.show({'type': 'error', 'message': 'Please enter a To Date.'});
			return false;
		}

		if (moment(from_date) > moment(to_date)) {
			SAXAlert.show({'type': 'error', 'message': 'From Date cannot be greater than To Date.'});
			return false;
		}
		 current_date = current_date.toDateString();
		console.log(moment().subtract(90, 'days'));
		console.log(from_date);
		
		if(moment().subtract(90, 'days')>= moment(from_date))
		{
		    SAXAlert.show({'type': 'error', 'message': 'Compoff application dates should be within last 90 days from today.'});
			return false;
		}

		return true;
	};
	
	_processCompoff = function (data, additional) 
	{

		var
			status = data.status,
			message = data.return_data;

		if (status === 'success') {
			$save_form[0].reset();
			$Employee_id.val(sessvars.TAMS_ENV.user_details.user_name);
		}

		SAXLoader.closeBlockingLoader();
		SAXAlert.show({'type': status, 'message': message});
		$save_button.button('reset');
	};
	
	_saveCompoff = function()
	{
	    var
			ajax_options = {},
			data = SAXForms.get($save_form);
			console.log(data);
	
		    if (_validate()) {

			    ajax_options = {
				    'url': page_name + '/SubmitCompff',
				    'data': {current: JSON.stringify(data)},
				    'callback': _processCompoff,
				    'additional': {}
			    }

			    $save_button.button('loading');
			    SAXLoader.showBlockingLoader();
			    SAXHTTP.ajax(ajax_options);
		    }
	    
	};

    _initOther = function()
	{
	    $('.date-picker').Zebra_DatePicker({
		    format: 'd-M-Y',

	    });  
	};
	
	_initButtons = function()
	{
	    var role = '',
			button_actions = {
				'save-compoff': function () {
					_saveCompoff();
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

   
})(jQuery, window, document);

$(function() {
	CompoffApplication.main();
});
$(function () {
	
	var Company = (function ($, w, d) {

		function _renderDropdown(data) {
	        var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $('#filter_company'),
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['company_code'] + '">' + data[counter]['company_name'] + '</option>';
	        }

	        $element.append(select_HTML);
		}
		
		function getCompanyData() {
			return SAXHTTP.AJAX(
				"ot_eligibility.aspx/GetCompanyData", {}
			)
			.done(_renderDropdown)
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while loading Company data. Please try again."})
			});
		};

		return {
			get: getCompanyData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var OtherData = (function ($, w, d) {

		var
			$branch_code           = $('#filter_branch'),
	        $department_code       = $('#filter_department'),
	        $designation_code      = $('#filter_designation');

		function _render($element, data, key, value, default_data, no_data) {
			var 
				data_length = data.length,
	            select_HTML = '<option value="select">' + default_data + '</option>',
	            counter = 0;

	        if (data_length > 0) {
	            for (counter = 0; counter < data_length; counter += 1) {
	                select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
	            }
	        }
	        else {
	            select_HTML = '<option value="select">' + no_data + '</option>';
	        }
	        $element.empty();
	        $element.append(select_HTML);
		}

		function getOtherData(company_code) {

			return SAXHTTP.AJAX(
					"ot_eligibility.aspx/GetOtherData",
					{company_code: company_code}
				)
				.done(function (data) {

					var results = JSON.parse(data.d.return_data);
					_render($department_code, results.department, 'department_code', 'department_name', 'Select Department', 'No Departments found');
		            _render($branch_code, results.branch, 'branch_code', 'branch_name', 'Select Branch', 'No Branches found');
		            _render($designation_code, results.designation, 'designation_code', 'designation_name', 'Select Designation', 'No Designations found');
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Company data. Please try again."});
				});
		}

		return {
			get: getOtherData
		};

	}) (jQuery, window, document);	

	/************************************************************************************************************************************************/

	var OTEligibility = (function ($, w, d) {

		var 
			list_elements = {
				table: $('#dataTable'),
				table_body: $('#dataTable tbody')
			}
			buttons = {
				grant: $("#grantOTEligibility"),
				reject: $("#rejectOTEligibility")
			},
			dialogs = {
				grant: $("#grantDialog"),
				reject: $("#rejectDialog")
			};

		    _changeOTStatus = function (employees, operation) {

		        var counter = 0,
		            selected_rows = list_elements.table_body.find('input:checked'),
		            employees_length = employees.length,
		            status = '';

		        for (counter; counter < employees_length; counter += 1) {
		            status = operation == 1 ? '<span class="fa fa-circle text-green"></span> Yes' : '<span class="fa fa-circle text-red"></span> No';
		            $('tr#' + employees[counter]).find('td:nth-child(2)').html(status);
		        }

		        selected_rows.prop('checked', false);
		    };

		function reject() {
			var 
	            selected_employees = list_elements.table_body.find('input:checked'),
	            employees = [],
	            selected_employees_length = selected_employees.length;

            if (selected_employees_length == 0) {
            	SAXAlert.show({type: "error", message: "Please select one or more employees"});
            }

            buttons.reject.button("loading");

	        for (var i = 0; i < selected_employees_length; i++) { 
	            employees.push( $(selected_employees[i]).val());
	        };

	        SAXHTTP.AJAX(
	        		"ot_eligibility.aspx/SaveOTEligibility",
	        		{employees: JSON.stringify(employees), action: '0'}
	        	)
	        	.done(function () {
	        		_changeOTStatus(employees, 0);
	        		dialogs.reject.modal("hide");
	        	})
	        	.fail(function () {
	        		SAXAlert.show({type: "error", message: "An error occurred while saving changes. Please try again."});
	        	})
	        	.always(function () {
	        		buttons.reject.button("reset");
	        	});
		}

		function grant() {
			var 
	            selected_employees = list_elements.table_body.find('input:checked'),
	            employees = [],
	            selected_employees_length = selected_employees.length;

            if (selected_employees_length == 0) {
            	SAXAlert.show({type: "error", message: "Please select one or more employees"});
            	return false;
            }

            buttons.grant.button("loading");

	        for (var i = 0; i < selected_employees_length; i++) { 
	            employees.push( $(selected_employees[i]).val());
	        };

	        SAXHTTP.AJAX(
	        		"ot_eligibility.aspx/SaveOTEligibility",
	        		{employees: JSON.stringify(employees), action: '1'}
	        	)
	        	.done(function () {
	        		_changeOTStatus(employees, 1);
	        		dialogs.grant.modal("hide");
	        	})
	        	.fail(function () {
	        		SAXAlert.show({type: "error", message: "An error occurred while saving changes. Please try again."});
	        	})
	        	.always(function () {
	        		buttons.grant.button("reset");
	        	});
		}

		return {
			grant: grant,
			reject: reject
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var EmployeeListView = (function ($, w, d) {

		var
			page_number = 1,
			$filter_company = $("#filter_company"),
			$filters = $("#filters"),
			forms = {
				filter: $('#filterForm')
			},
			buttons = {
				pagination: $('#paginationButton'),
			},
			list_elements = {
				table: $('#dataTable'),
				message: $('#noData'),
				listview: $('#listView')
			};

		function resetFilters() {
			var company_code = $filter_company.find('option:selected').val();
	        page_number = 1;

	        forms.filter[0].reset();
	        $filter_company.val(company_code);

	        list_elements.table.find('tbody').empty();
	        getEmployeeData();
	        //$filters.slideToggle();
		}

		function loadMoreData() {
			page_number += 1;
			buttons.pagination.button("loading");
			getEmployeeData()
				.done(function () {
					buttons.pagination.button("reset");
				});
		}
			function _getHTML(data) {

		        var 
		        	data_length = data.length,
		            OT_status = '',
		            table_HTML = '',
		            counter = 0;

		        for ( counter = 0; counter < data_length; counter += 1) {

		            OT_status = data[counter]['ot_eligibility'] == 1 ? '<span class="fa fa-circle text-green"></span> Yes' : '<span class="fa fa-circle text-red"></span> No';

		            table_HTML += '<tr id="' + data[counter]['employee_code'] + '" >' +
		                            '<td><input type="checkbox" value="' + data[counter]['employee_code'] + '" ></td>' +
		                            '<td>' + OT_status + '</td>' +
		                            '<td>' + data[counter]['employee_code'] + '</td>' +
		                            '<td>' + data[counter]['employee_name'] + '</td>' +
		                        '</tr>' ;
		        }

		        return table_HTML;
			}
			function _render(results) { 

				var 
					table_body, 
					data = JSON.parse(results.d.return_data),
					data_length = data.length;

				list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

				if (data_length > 0) {
					// if table is hidden, show the table
					list_elements.listview.is(":hidden") ? list_elements.listview.show() : 0;

					table_body = list_elements.table.find("tbody");
					// get the HTML and append to the table.
					table_HTML = _getHTML(data);
					table_body.append(table_HTML);
					// hiding the pagination button
					table_body.children().length < page_number*30 ? buttons.pagination.hide() : buttons.pagination.show();
				}
				else { 
					list_elements.message.append("<h3>No Employee data found</h3>");
					// hide the table view.
					list_elements.listview.hide();
				}
			}
			    function _validateFilters() {

			        var data = SAXForms.get(forms.filter);

			        if(data.filter_company == "select") {
			            SAXAlert.show({'type': 'error', 'message': 'Please select a Company.'});
			            return false;
			        }

			        if(data.filter_keyword != "" && data.filter_by == 0) {
			            SAXAlert.show({'type': 'error', 'message': 'Please select a Filter By option.'});
			            return false;
			        }

			        if(data.filter_by != 0 && data.filter_keyword == "") {
			            SAXAlert.show({'type': 'error', 'message': 'Please enter a keyword.'});
			            return false;
			        }

			        return true;
			    }

		function getEmployeeData() {

			if (_validateFilters()) {

				SAXLoader.showBlockingLoader();
				
				$filters.slideToggle();

				return SAXHTTP.AJAX(
						"ot_eligibility.aspx/GetEmployeeData",
						{page_number: page_number, filters: JSON.stringify(SAXForms.get(forms.filter))}
					)
					.done(_render)
					.fail(function () {
						SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."});
					})
					.always(SAXLoader.closeBlockingLoader);
			}
		}

		function filterData() {
			list_elements.table.find("tbody").empty();
			getEmployeeData();
		}

		return {
			filter: filterData,
			get: getEmployeeData,
			more: loadMoreData,
			reset: resetFilters,
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var OTEligibilityMasterView = (function ($, w, d) {

		var 		
			$company = $('#filter_company'),
			$table = $('#dataTable'),
			$filters = $("#filters"),
			button_events = {
				"ot-eligibility/grant": OTEligibility.grant,
				"ot-eligibility/reject": OTEligibility.reject,
				"ot-eligibility/more": EmployeeListView.more,
				"filters/data": EmployeeListView.filter,
				"filters/reset": EmployeeListView.reset,
				"filters/toggle": function () {$filters.slideToggle();}
			},
			model_class, collection_class, collection;

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

		function _initOther() {
			$company.change(function () {
				var company_code = $(this).val();
				SAXLoader.showBlockingLoader();
				$.when(OtherData.get(company_code))
					.then(SAXLoader.closeBlockingLoader, null)
			});

			$('#checkall').change(function() {
	            var is_checked = $(this).is(':checked'),
	                checkboxes = $table.find('tbody input[type="checkbox"]');
	            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
	        });
		}

		function initialize() {
			_initButtons();
			_initOther();
		}

		return {
			init: initialize
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	// INITIAL PAGE LOAD
	SAXLoader.show();

	// initialize page components
	OTEligibilityMasterView.init();

	// Get company Data
	Company.get()
		.always(SAXLoader.close())
});
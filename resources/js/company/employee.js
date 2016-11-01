$(function () {

	var EmployeeExport = (function ($, w, d) {

		var
			buttons = {
				export_button: $('#exportButton')
			};

		function _processExport(data) {
			var status = data.d.status;
	        switch (status) {
	        case 'success':
	            SAXAlert.showAlertBox({'type': status, 'url': SAXUtils.getApplicationURL() + data.d.return_data});
	            break;
	        case 'info':
	            SAXAlert.show({'type': status, 'message': data.d.return_data});
	            break;
           };
		}

		function doExport() {

	  	buttons.export_button.button("loading");

			SAXHTTP.AJAX(
				"employee.aspx/DoExport", {}
			)
			.done(_processExport)
			.fail(function() {
				SAXAlert.show({type: "error", message: "An error occurred while exporting Employee data. Please try again."});
			})
			.always(function () { buttons.export_button.button("reset"); });
		}

		return {
			export: doExport
		};

	}) (jQuery, window, document);
	
	/************************************************Transaction Export***************************************************************************/
	
	var EmployeeTransactionExport = (function ($, w, d) {

		var
			buttons = {
				export_button1: $('#exportButtonTransaction')
			};

		function _processExport(data) {
			var status = data.d.status;
	        switch (status) {
	        case 'success':
	            SAXAlert.showAlertBox({'type': status, 'url': SAXUtils.getApplicationURL() + data.d.return_data});
	            break;
	        case 'info':
	            SAXAlert.show({'type': status, 'message': data.d.return_data});
	            break;
           };
		}

		function doExport() {

		 buttons.export_button1.button("loading");

			SAXHTTP.AJAX(
				"employee.aspx/DoExportTransaction", {}
			)
			.done(_processExport)
			.fail(function() {
				SAXAlert.show({type: "error", message: "An error occurred while exporting Employee data. Please try again."});
			})
			.always(function () { buttons.export_button1.button("reset"); });
		}

		return {
			exporttransaction: doExport
		};

	}) (jQuery, window, document);
	
	
	/**************************************************************************************************************************************************/

	/************************************************************************************************************************************************/

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
				"employee.aspx/GetCompanyData", {}
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

	OtherData = (function ($, w, d) {

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
					"employee.aspx/GetOtherData",
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

	var EmployeeView = (function ($, w, d) {

		var 
			buttons = {
				delete: $('#deleteButton'),
				reinstate_yes: $("#reinstateButtonYes"),
				reinstate_no: $("#reinstateButtonNo")
			},
			dialogs = {
				delete: $('#deleteDialog'),
				reinstate: $("#reinstateDialog")
			};

		function _request(url, data, success, error, additional) {

			var requestDeferred = $.Deferred();

			SAXHTTP.AJAX(url, data)
			.done(function (data) { 
				if (data.d.status == "success") {
					requestDeferred.resolve([additional.data], 1);
				}
				else {
					requestDeferred.reject();
				}
				SAXAlert.show({type: data.d.status, message: data.d.return_data});
			})
			.fail(function () {
				requestDeferred.reject();
				SAXAlert.show({type: "error", message: error});
			})

			return requestDeferred.promise();
		}

		function deleteEmployee(event) {
			var 
				employee_code = $(event.target).data("employee-id"),
				data = EmployeeMasterView.getCollection().get(employee_code).toJSON(),
				success = "Employee deleted successfully!",
				error = "An error occurred while deleting the Employee. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("employee.aspx/DeleteEmployee", {employee_id: employee_code}, success, error, {data: data}))
				.then(EmployeeListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		function reinstateEmployee(event) {
			var 
				employee_code = $(event.target).data("employee-id"),
				action = $(event.target).data("reinstate"),
				success = "Employee reinstated successfully!",
				error = "An error occurred while reinstated the Employee. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.reinstate_yes.button("loading");
			buttons.reinstate_no.button("loading");

			$.when(_request("employee.aspx/ReinstateEmployee", {employee_id: employee_code, action: action}, success, error, {}))
				.then(null, null)
				.done(function() { dialogs.reinstate.modal("hide"); })
				.always(function () { 
					buttons.reinstate_yes.button("reset"); 
					buttons.reinstate_no.button("reset"); 
				});
		}

		return {
			delete: deleteEmployee,
			reinstate: reinstateEmployee
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var EmployeeListView = (function ($, w, d) {

		var
			page_number = 1,
			is_filter = false,
			buttons = {
				pagination: $('#paginationButton'),
				filter: $("#filterButton")
			},
			dialogs = {
				filter: $('#filters')
			},
			forms = {
				filter: $('#filterForm')
			},
			list_elements = {
				table: $('#dataTable'),
				message: $('#noData'),
				listview: $('#listView')
			};

		function resetFilters() {
	        is_filter = false;
	        page_number = 1;

	        forms.filter[0].reset();
	        SAXLoader.show();

	        list_elements.table.find('tbody').empty();

	        getEmployeeData()
	        .done(render)
			.fail(function() {
				SAXLoader.show({type: "error", message: "An error occurred while loading data. Please try again."})
			})
			.always(function() {
				SAXLoader.close();
				dialogs.filter.slideToggle();
			});
		}

			function _validateFilters() {

		        var data = SAXForms.get(forms.filter);

		       /* if(data.filter_company == "select") {
		            SAXAlert.show({'type': 'error', 'message': 'Please select a Company.'});
		            return false;
		        } */

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

		function filterData() {
	        if (_validateFilters()) {
	            is_filter = true;
	            page_number = 1;

	            SAXLoader.show();

	            list_elements.table.find('tbody').empty();

	            filterEmployeeData()
	            .done(render)
				.fail(function() {
					SAXLoader.show({type: "error", message: "An error occurred while loading data. Please try again."})
				})
				.always(function() {
					SAXLoader.close();
					dialogs.filter.slideToggle();
				});
	        }
		}

		/* pagination functions */
		function loadMoreData() {

			var request;

			// disable pagination button to avoid multiple clicks
			buttons.pagination.button("loading"); 
			
			page_number += 1;

			if (is_filter) {
				request = filterEmployeeData();
			} else {
				request = getEmployeeData();
			}

			request
				.done(render)
				.fail(function () { 
						SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."}) 
				})
				.always(function () { 
					SAXLoader.close(); 
					buttons.pagination.button("reset"); 
				});
		}

		function removeRow(data, data_length) {

			// remove the row from the table
			list_elements.table.find("tr#" + data[0]["employee_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			EmployeeMasterView.getCollection().unset(data[0]["employee_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {

				var 
					branch_name = "",
					department_name = "",
					designation_name = "", 
					table_HTML = "",
                    OT_status = '',
                    ramadan_status= '',
					counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
				    OT_status = data[counter]['ot_eligibility'] == 1 ? '<span class="fa fa-circle text-green"></span> Yes' : '<span class="fa fa-circle text-red"></span> No';
				    ramadan_status = data[counter]['Ramadan_Eligibility'] == 1 ? '<span class="fa fa-circle text-green"></span> Yes' : '<span class="fa fa-circle text-red"></span> No';
					branch_name = data[counter]["branch_name"] == null ? "" : data[counter]["branch_name"];
					designation_name = data[counter]["designation_name"] == null ? "" : data[counter]["designation_name"];
					department_name = data[counter]["department_name"] == null ? "" : data[counter]["department_name"];

					if (data[counter]["employee_status"] == 2 || data[counter]["employee_status"] == 3 || data[counter]["employee_status"] == 4) {
						reinstate_icon = '<span class="fa fa-refresh text-blue action-icon" data-toggle="modal" data-target="#reinstateDialog" data-role="employee/reinstate" data-id="' + data[counter]["employee_code"] + '"></span>';
					}
					else {
						reinstate_icon = '<span class="fa fa-refresh text-grey action-icon"></span>';
					}

					table_HTML += '<tr id="' + data[counter]['employee_code'] + '" >' +
		                        '<td>' + data[counter]['employee_code'] + '</td>' +
		                        '<td>' + data[counter]['employee_name'] + '</td>' +
		                        '<td>' + data[counter]['company_name'] + '</td>' +
		                        '<td>' + branch_name + '</td>' +
		                        '<td>' + department_name + '</td>' +
		                        '<td>' + designation_name + '</td>' +
		                        '<td>' + data[counter]['enroll_id'] + '</td>' +
		                        '<td>' + OT_status + '</td>' +
                                '<td>' + ramadan_status + '</td>' +
		                        '<td>' + 
		                            '<a href="manage_employee.aspx#/edit/' + data[counter]["employee_code"] + '"><span class="fa fa-pencil action-icon"></span></a>' +
		                            '<span class="fa fa-user-times text-red action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="employee/delete" data-id="' + data[counter]["employee_code"] + '"></span>' +
		                            reinstate_icon +
		                        '</td>' +
		                    '</tr>' ;
				}

				return table_HTML;
			}

		function render(data, data_length) { 
			var table_body;

			list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

			if (data_length > 0) {
				// if table is hidden, show the table
				list_elements.listview.is(":hidden") ? list_elements.listview.show() : 0;

				table_body = list_elements.table.find("tbody");
				// get the HTML and append to the table.
				table_HTML = _getHTML(data, data_length);
				table_body.append(table_HTML);

				// hiding the pagination button
				table_body.children().length < page_number*30 ? buttons.pagination.hide() : buttons.pagination.show();

				// add the data to the collection also
				EmployeeMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No Employee data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function filterEmployeeData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"employee.aspx/FilterEmployeeData",
				{page_number: page_number, filters: JSON.stringify( SAXForms.get(forms.filter) )}
			)
			.done(function (data) {
				var data = JSON.parse(data.d.return_data);
				deferred.resolve(data, data.length);
			}).fail(function () {
				deferred.reject();
			});

			return deferred.promise();
		}

		function getEmployeeData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"employee.aspx/GetEmployeeData",
				{page_number: page_number}
			)
			.done(function (data) {
				var data = JSON.parse(data.d.return_data);
				deferred.resolve(data, data.length);
			}).fail(function () {
				deferred.reject();
			});

			return deferred.promise();
		}

		return {
			get: getEmployeeData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var EmployeeMasterView = (function ($, w, d) {

		var 		
			dialogs = {
				"delete": $("#deleteDialog"),
				"filters": $('#filters'),
				"reinstate": $("#reinstateDialog")
			},
			buttons = {
				delete: $('#deleteButton'),
				reinstate_yes: $("#reinstateButtonYes"),
				reinstate_no: $("#reinstateButtonNo")
			},
			button_events = {
				"employee/delete": EmployeeView.delete,
				"employee/reinstate": EmployeeView.reinstate,
				"employee/export": EmployeeExport.export,
				"employee/exporttransaction":EmployeeTransactionExport.exporttransaction,
				"employee/more": EmployeeListView.more,
				"filters/data": EmployeeListView.filter,
				"filters/reset": EmployeeListView.reset,
				"filters/toggle": function () { dialogs.filters.slideToggle() }
			},
			forms = {},
			$company = $('#filter_company'),
			model_class, collection_class, collection;

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "employee/delete": 
						buttons.delete.data("employee-id", $(event.relatedTarget).data("id"));
						break;
					case "employee/reinstate": 
						buttons.reinstate_yes.data("employee-id", $(event.relatedTarget).data("id"));
						buttons.reinstate_no.data("employee-id", $(event.relatedTarget).data("id"));
						break;
				}
			}

		function _initDialogs() {
			dialogs.delete.on("show.bs.modal", _setModalButton);
			dialogs.reinstate.on("show.bs.modal", _setModalButton);
		}

		/* models */
			getCollection = function () {
				return collection;
			}
		_initModels = function () {
			model_class = SAXModel.extend({ 'idAttribute': "employee_code" });

			// define the collection class
			collection_class = SAXCollection.extend({ 'baseModel': model_class });

			// create an instance of the collection_class
			// passing an empty array as the default data
			collection = new collection_class([]); 
		};

		function _initOther() {
			$company.change(function () {
				var company_code = $(this).val();
				SAXLoader.showBlockingLoader();
				$.when(OtherData.get(company_code))
					.then(SAXLoader.closeBlockingLoader, null)
			});
		}

		function initialize() {
			_initButtons();
			_initDialogs();
			_initModels();
			_initOther();
		}

		return {
			init: initialize,
			getCollection: getCollection
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/
	
	// INITIAL PAGE LOAD
	SAXLoader.show();

	// initialize page components
	EmployeeMasterView.init();

	EmployeeListView.get()
		.done(EmployeeListView.render)
		.done(Company.get)
		.fail(function () {
			SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."});
		})
		.always(SAXLoader.close);
});
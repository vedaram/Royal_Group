$(function () {
	
	var LeaveType = (function ($, w, d) {
	    
	    function _renderDropdown(data) {
	        var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $('#leave_type'),
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['leave_type'] + '">' + data[counter]['leave_type'] + '</option>';
	        }

	        $element.append(select_HTML);
		}
		
		function getLeaveTypeData() {
			return SAXHTTP.AJAX(
				"leave.aspx/GetLeaveTypeData", {}
			)
			.done(_renderDropdown)
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while loading Leave Type data. Please try again."})
			});
		};
		
		return {
		    get: getLeaveTypeData
		};
		
	}) (jQuery, window, document);
	
	var Company = (function ($, w, d) {

		function _renderDropdown(data) {
	        var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $('#company_code, #filter_company_code'),
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['company_code'] + '">' + data[counter]['company_name'] + '</option>';
	        }

	        $element.append(select_HTML);
		}
		
		function getCompanyData() {
			return SAXHTTP.AJAX(
				"leave.aspx/GetCompanyData", {}
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

	var EmployeeCategory = (function ($, w, d) {

		var 
			$employee_category_code = $("#employee_category_code");

		function _renderDropdown(data) {
	        var select_HTML = "<option value=\"select\">Select Employee Category</option>",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $employee_category_code,
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['employee_category_code'] + '">' + data[counter]['employee_category_name'] + '</option>';
	        }

	        $element.empty().append(select_HTML);
		}

		function getEmployeeCategoryData(company_code) {

			$employee_category_code.append("<option value=\"select\">Loading ...</option>");

			return SAXHTTP.AJAX(
					"leave.aspx/GetEmployeeCategoryData", {company_code: company_code}
				)
				.done(_renderDropdown)
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Employee Category data. Please try again."})
				});
		}

		return {
			get: getEmployeeCategoryData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var LeaveView = (function ($, w, d) {

		var buttons = {
			save: $('#saveButton'),
			delete: $('#deleteButton')
		},
		dialogs = {
			save: $('#saveDialog'),
			delete: $('#deleteDialog')
		},
		forms = {
			save: $('#saveForm')
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

		function _validate(data)  {

			if (data.company_code === "select") {
	            SAXAlert.show({ type: "error", message: "Please select a Company"});
	            return false;
	        }

	        if (data.employee_category_code === "select") {
	            SAXAlert.show({ type: "error", message: "Please select an Employee Category"});
	            return false;
	        }

	        if (data.leave_code === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Leave Code"});
	            return false;
	        }

	        if (data.leave_code != "" && !SAXValidation.code(data.leave_code)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Leave Code"});
	            return false;
	        }

	        if (data.leave_name === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Leave Name"});
	            return false;
	        }

	        if (data.leave_name != "" && !SAXValidation.name(data.leave_name)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Leave Name"});
	            return false;
	        }
	        
	        
	        if (data.leave_status === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Leave Status"});
	            return false;
	        }

//            if(data.max_leave === "") {
//	            SAXAlert.show({type: "error", message: "Please enter a value for Max Leave"});
//	            return false;
//	        }

//	        if(data.max_leave_carry_forward === "") {
//	            SAXAlert.show({type: "error", message: "Please enter a value for Max Leave Carry Forward."});
//	            return false;
//	        }

//	        if (!SAXValidation.isNumber(data.max_leave)) {
//	            SAXAlert.show({type: "error", message: "Please enter a numeric value for Max Leave"});
//	            return false;
//	        }

//	        if (!SAXValidation.isNumber(data.max_leave_carry_forward)) {
//	            SAXAlert.show({type: "error", message: "Please enter a numeric value for Max Leave Carry Forward"});
//	            return false;
//	        }

//	        if (+data.max_leave_carry_forward > +data.max_leave) {
//	            SAXAlert.show({type: "error", message: "Max Leave cannot be less than Max Leave Carry Forward"});
//	            return false;
//	        }

	        return true;
		}

		function deleteLeave(event) {
			var 
				leave_code = $(event.target).data("leave-id"),
				data = LeaveMasterView.getCollection().get(leave_code).toJSON(),
				success = "Leave deleted successfully!",
				error = "An error occurred while deleting the Leave. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("leave.aspx/DeleteLeave", {current: JSON.stringify(data)}, success, error, {data: data}))
				.then(LeaveListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		function editLeave(event) {
			var 
				form_data = SAXForms.get(forms.save),
				leave_code = $(event.target).data("leave-id"),
				previous = LeaveMasterView.getCollection().get(leave_code),
				success = "Leave details edited successfully!",
				error = "An error occurred while saving Leave details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();
				form_data["employee_category_name"] = $("#employee_category_code option:selected").text();

				_request("leave.aspx/EditLeave", {current: JSON.stringify(form_data), previous: JSON.stringify(previous)}, success, error, {data: form_data})
					.done(LeaveListView.delete)
					.done(LeaveListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function addLeave() {
			var 
			   	form_data = SAXForms.get(forms.save),				
	 
				success = "Leave added successfully!",
				error = "An error occurred while adding a new Leave . Please try again.";
				
			if (_validate(form_data)) {
				// disable the button to avoid multiple clicks
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();
				form_data["employee_category_name"] = $("#employee_category_code option:selected").text();

				$.when(_request("leave.aspx/AddLeave", {current: JSON.stringify(form_data)}, success, error, {data: form_data})) 
					.then(LeaveListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			add: addLeave,
			edit: editLeave,
			delete: deleteLeave
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var LeaveListView = (function ($, w, d) {

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
				listview: $("#listView")
			};

		function isFilter() {
			return is_filter;
		}

		function resetFilters() {
	        is_filter = false;
	        page_number = 1;

	        forms.filter[0].reset();
	        SAXLoader.show();

	        list_elements.table.find('tbody').empty();

	        getLeaveData()
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

		        if(data.filter_CompanyCode == "select") {
		            SAXAlert.show({'type': 'error', 'message': 'Please select a Company.'});
		            return false;
		        }

		        if(data.filter_by == 0 || (data.filter_keyword != "" && data.filter_by == 0)) {
		            SAXAlert.show({'type': 'error', 'message': 'Please select a Filter By option.'});
		            return false;
		        }

		        if(data.filter_keyword == "" || (data.filter_by != 0 && data.filter_keyword == "")) {
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

	            getLeaveData()
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
			// disable pagination button to avoid multiple clicks
			buttons.pagination.button("loading"); 
			
			page_number += 1;

			getLeaveData()
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
			list_elements.table.find("tr#" + data[0]["leave_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			LeaveMasterView.getCollection().unset(data[0]["leave_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['leave_code'] + '" >' +
		                        '<td>' + data[counter]['leave_code'] + '</td>' +
		                        '<td>' + data[counter]['leave_name'] + '</td>' +
		                        '<td>' + data[counter]['company_name'] + '</td>' +
		                        '<td>' + data[counter]['employee_category_code'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="leave/edit" data-id="' + data[counter]["leave_code"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="leave/delete" data-id="' + data[counter]["leave_code"] + '"></span>' +
		                        '</td>' +
		                    '</tr>' ;
				}

				return table_HTML;
			}

		function render(data, data_length) { 
			var table_body;

			list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

			if (data_length > 0) {

				list_elements.listview.is(":hidden") ? list_elements.listview.show() : 0;

				table_body = list_elements.table.find("tbody");
				// get the HTML and append to the table.
				table_HTML = _getHTML(data, data_length);
				table_body.append(table_HTML);
				// hiding the pagination button
				table_body.children().length < page_number*30 ? buttons.pagination.hide() : buttons.pagination.show();

				// add the data to the collection also
				LeaveMasterView.getCollection().set(data);
			}
			else {
				list_elements.message.append("<h3>No Leave data found</h3>");
				// hdie the table view
				list_elements.listview.hide();
			}
		}

		function getLeaveData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"leave.aspx/GetLeaveData",
				{page_number: page_number, is_filter: is_filter, filters: JSON.stringify( SAXForms.get(forms.filter) )}
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
			get: getLeaveData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters,
			isFilter: isFilter
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var LeaveMasterView = (function ($, w, d) {

		var 		
			dialogs = {
				"save": $("#saveDialog"),
				"delete": $("#deleteDialog"),
				"filters": $('#filters')
			},
			buttons = {
				save: $('#saveButton'),
				delete: $('#deleteButton')
			},
			button_events = {
				"leave/add": LeaveView.add,
				"leave/edit": LeaveView.edit,
				"leave/delete": LeaveView.delete,
				"leave/more": LeaveListView.more,
				"filters/data": LeaveListView.filter,
				"filters/reset": LeaveListView.reset,
				"filters/toggle": function () { dialogs.filters.slideToggle() }
			},
			forms = {
				save: $('#saveForm'),
			},
			$company_code = $('#company_code'),
			$employee_category_code = $("#employee_category_code"),
			fields = ["company_code", "employee_category_code", "leave_code"],
			model_class, collection_class, collection;

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

		/* dialogs */
			function _setEditButton(event) {
				
				var leave_code = $(event.relatedTarget).data("id");

				// set data for edit button
				buttons.save.data("role", "leave/edit");
				buttons.save.data("leave-id", leave_code); 

				// disable fields as required
				SAXForms.disable(fields);
			}
			function _setModalData(event) {
				var 
					$target = $(event.relatedTarget),
					role = $target.data("role"),
					leave_code = $target.data("id"),
					data = {};

				if (role == "leave/edit") {
					
					SAXLoader.show();

					data = LeaveMasterView.getCollection().get(leave_code).toJSON();

					EmployeeCategory.get(data.company_code)
						.done(function () {
							// fill save form data for the selected company
							SAXForms.set(forms.save, data);
							SAXLoader.close();
						});
				}
			}
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "leave/add":
						buttons.save.data("role", "leave/add"); 
						 var week_off_flag= $("#week_off_flag");
                        week_off_flag.prop("checked", true);
						break;
					case "leave/edit": 
						_setEditButton(event);
						break;
					case "leave/delete": 
						buttons.delete.data("leave-id", $(event.relatedTarget).data("id"));
						break;
				}
			}
			function _resetSaveModal(event) {
				$employee_category_code.empty();
				forms.save[0].reset();
				SAXForms.enable(fields);
			}

		function _initDialogs() {
			// before the modal is shown to the user,
			// change the function of the save button to add or edit.
			dialogs.save.on("show.bs.modal", _setModalButton);

			dialogs.save.on("shown.bs.modal", _setModalData);

			// reset the form on modal close.
			dialogs.save.on("hidden.bs.modal", _resetSaveModal);

			dialogs.delete.on("show.bs.modal", _setModalButton);
		}

				/* modals */
			getCollection = function () {
				return collection;
			}
		_initModels = function () {
			model_class = SAXModel.extend({ 'idAttribute': "leave_code" });

			// define the collection class
			collection_class = SAXCollection.extend({ 'baseModel': model_class });

			// create an instance of the collection_class
			// passing an empty array as the default data
			collection = new collection_class([]); 
		};

		function _initOther() {
			$company_code.change(function() {
				SAXLoader.show();
				EmployeeCategory.get($(this).val())
					.done(SAXLoader.close)
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
	LeaveMasterView.init();

	// get company data
	LeaveListView.get()
		.done(LeaveListView.render)
		.done(Company.get)
		.done(LeaveType.get)
		.fail(function() { 
			SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."})
		})
		.always(SAXLoader.close);
});
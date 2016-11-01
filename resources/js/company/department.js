$(function () {

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
				"department.aspx/getCompanyData", {}
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

	var DepartmentExport = (function ($, w, d) {

		var
			buttons = {
				export_button: $('#exportButton')
			},
			forms = {
				filter: $('#filterForm')
			}

		function _processExport(data) {
			var status = data.d.status;
	        switch (status) {
	        case 'success':
	            SAXAlert.showAlertBox({'type': status, 'url': SAXUtils.getApplicationURL() + data.d.return_data});
	            break;
	        case 'info':
	            SAXAlert.show({'type': status, 'message': data.return_data});
	            break;
           };
		}

		function doExport() {

			var is_filter, filters = {};

			buttons.export_button.button("loading");

			is_filter = DepartmentListView.isFilter();

			if (is_filter)
				filters = SAXForms.get(forms.filter)

			SAXHTTP.AJAX(
				"department.aspx/DoExport", {is_filter: is_filter, filters: JSON.stringify(filters)}
			)
			.done(_processExport)
			.fail(function() {
				SAXAlert.show({type: "error", message: "An error occurred while exporting Department data. Please try again."});
			})
			.always(function () { buttons.export_button.button("reset"); });
		}

		return {
			export: doExport
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DepartmentView = (function ($, w, d) {

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

	        if (data.department_name === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Department Name"});
	            return false;
	        }

	        if (data.department_name != "" && !SAXValidation.name(data.department_name)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Department Name"});
	            return false;   
	        }

	        if (data.department_code === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Department Code"});
	            return false;
	        }
	        
	        if (data.department_code != "" && !SAXValidation.code(data.department_code)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Department Code"});
	            return false;   
	        }

	        return true;
		}

		function deleteDepartment(event) {
			var 
				department_code = $(event.target).data("department-id"),
				data = DepartmentMasterView.getCollection().get(department_code).toJSON(),
				success = "Department deleted successfully!",
				error = "An error occurred while deleting the Department. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("department.aspx/DeleteDepartment", {current: JSON.stringify(data)}, success, error, {data: data}))
				.then(DepartmentListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		function editDepartment(event) {
			var 
				form_data = SAXForms.get(forms.save),
				department_code = $(event.target).data("department-id"),
				previous = DepartmentMasterView.getCollection().get(department_code),
				success = "Department details edited successfully!",
				error = "An error occurred while saving Department details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();

				_request("department.aspx/EditDepartment", {current: JSON.stringify(form_data), previous: JSON.stringify(previous)}, success, error, {data: form_data})
					.done(DepartmentListView.delete)
					.done(DepartmentListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function addDepartment() {
			var 
				form_data = SAXForms.get(forms.save),
				success = "Department added successfully!",
				error = "An error occurred while adding a new Department . Please try again.";

			if (_validate(form_data)) {
				// disable the button to avoid multiple clicks
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();

				$.when(_request("department.aspx/AddDepartment", {current: JSON.stringify(form_data)}, success, error, {data: form_data})) 
					.then(DepartmentListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			add: addDepartment,
			edit: editDepartment,
			delete: deleteDepartment
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	DepartmentListView = (function ($, w, d) {

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

		function isFilter() {
			return is_filter;
		}

		function resetFilters() {
	        is_filter = false;
	        page_number = 1;

	        forms.filter[0].reset();
	        SAXLoader.show();

	        list_elements.table.find('tbody').empty();

	        getDepartmentData()
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

		        if(data.filter_company_code == "select") {
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

	            getDepartmentData()
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

			getDepartmentData()
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
			list_elements.table.find("tr#" + data[0]["department_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			DepartmentMasterView.getCollection().unset(data[0]["department_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['department_code'] + '" >' +
		                        '<td>' + data[counter]['department_code'] + '</td>' +
		                        '<td>' + data[counter]['department_name'] + '</td>' +
		                        '<td>' + data[counter]['company_name'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="department/edit" data-id="' + data[counter]["department_code"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="department/delete" data-id="' + data[counter]["department_code"] + '"></span>' +
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
				DepartmentMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No Department data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function getDepartmentData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"department.aspx/getDepartmentData",
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
			get: getDepartmentData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters,
			isFilter: isFilter
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	DepartmentMasterView = (function ($, w, d) {

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
				"department/add": DepartmentView.add,
				"department/edit": DepartmentView.edit,
				"department/delete": DepartmentView.delete,
				"department/more": DepartmentListView.more,
				"department/export": DepartmentExport.export,
				"filters/data": DepartmentListView.filter,
				"filters/reset": DepartmentListView.reset,
				"filters/toggle": function () { dialogs.filters.slideToggle() }
			},
			forms = {
				save: $('#saveForm'),
			},
			fields = ["company_code", "department_code"],
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
				
				var department_code = $(event.relatedTarget).data("id"),
					data = DepartmentMasterView.getCollection().get(department_code).toJSON();

				// set data for edit button
				buttons.save.data("role", "department/edit");
				buttons.save.data("department-id", department_code); 

				// fill save form data for the selected department
				SAXForms.set(forms.save, data);

				// disable fields as required
				SAXForms.disable(fields);
			}
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "department/add":
						buttons.save.data("role", "department/add"); 
						break;
					case "department/edit": 
						_setEditButton(event);
						break;
					case "department/delete": 
						buttons.delete.data("department-id", $(event.relatedTarget).data("id"));
						break;
				}
			}
			function _resetSaveModal(event) {
				forms.save[0].reset();
				SAXForms.enable(fields);
			}

		function _initDialogs() {
			// before the modal is shown to the user,
			// change the function of the save button to add or edit.
			dialogs.save.on('show.bs.modal', _setModalButton);

			// reset the form on modal close.
			dialogs.save.on("hidden.bs.modal", _resetSaveModal);

			dialogs.delete.on("show.bs.modal", _setModalButton);
		}

		/* models */
			getCollection = function () {
				return collection;
			}
		_initModels = function () {
			model_class = SAXModel.extend({ 'idAttribute': "department_code" });

			// define the collection class
			collection_class = SAXCollection.extend({ 'baseModel': model_class });

			// create an instance of the collection_class
			// passing an empty array as the default data
			collection = new collection_class([]); 
		};

		function initialize() {
			_initButtons();
			_initDialogs();
			_initModels();
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
	DepartmentMasterView.init();

	// get company data
	DepartmentListView.get()
		.done(DepartmentListView.render)
		.done(Company.get)
		.fail(function() { 
			SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."})
		})
		.always(SAXLoader.close);
});
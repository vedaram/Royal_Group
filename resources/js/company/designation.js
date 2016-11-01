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
				"designation.aspx/getCompanyData", {}
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

	var DesignationExport = (function ($, w, d) {

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

			is_filter = DesignationListView.isFilter();

			if (is_filter)
				filters = SAXForms.get(forms.filter)

			SAXHTTP.AJAX(
				"designation.aspx/DoExport", {is_filter: is_filter, filters: JSON.stringify(filters)}
			)
			.done(_processExport)
			.fail(function() {
				SAXAlert.show({type: "error", message: "An error occurred while exporting Designation data. Please try again."});
			})
			.always(function () { buttons.export_button.button("reset"); });
		}

		return {
			export: doExport
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DesignationView = (function ($, w, d) {

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

	        if (data.designation_name === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Designation Name"});
	            return false;
	        }

	        if (data.designation_name != "" && !SAXValidation.name(data.designation_name)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Designation Name"});
	            return false;   
	        }

	        if (data.designation_code === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Designation Code"});
	            return false;
	        }
	        
	        if (data.designation_code != "" && !SAXValidation.code(data.designation_code)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Designation Code"});
	            return false;   
	        }

	        return true;
		}

		function deleteDesignation(event) {
			var 
				designation_code = $(event.target).data("designation-id"),
				data = DesignationMasterView.getCollection().get(designation_code).toJSON(),
				success = "Designation deleted successfully!",
				error = "An error occurred while deleting the Designation. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("designation.aspx/DeleteDesignation", {current: JSON.stringify(data)}, success, error, {data: data}))
				.then(DesignationListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		function editDesignation(event) {
			var 
				form_data = SAXForms.get(forms.save),
				designation_code = $(event.target).data("designation-id"),
				previous = DesignationMasterView.getCollection().get(designation_code),
				success = "Designation details edited successfully!",
				error = "An error occurred while saving Designation details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();

				_request("designation.aspx/EditDesignation", {current: JSON.stringify(form_data), previous: JSON.stringify(previous)}, success, error, {data: form_data})
					.done(DesignationListView.delete)
					.done(DesignationListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function addDesignation() {
			var 
				form_data = SAXForms.get(forms.save),
				success = "Designation added successfully!",
				error = "An error occurred while adding a new Designation . Please try again.";

			if (_validate(form_data)) {
				// disable the button to avoid multiple clicks
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();

				$.when(_request("designation.aspx/AddDesignation", {current: JSON.stringify(form_data)}, success, error, {data: form_data})) 
					.then(DesignationListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			add: addDesignation,
			edit: editDesignation,
			delete: deleteDesignation
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	DesignationListView = (function ($, w, d) {

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

	        getDesignationData()
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

	            getDesignationData()
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

			getDesignationData()
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
			list_elements.table.find("tr#" + data[0]["designation_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			DesignationMasterView.getCollection().unset(data[0]["designation_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['designation_code'] + '" >' +
		                        '<td>' + data[counter]['designation_code'] + '</td>' +
		                        '<td>' + data[counter]['designation_name'] + '</td>' +
		                        '<td>' + data[counter]['company_name'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="designation/edit" data-id="' + data[counter]["designation_code"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="designation/delete" data-id="' + data[counter]["designation_code"] + '"></span>' +
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
				DesignationMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No Designation data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function getDesignationData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"designation.aspx/getDesignationData",
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
			get: getDesignationData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters,
			isFilter: isFilter
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	DesignationMasterView = (function ($, w, d) {

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
				"designation/add": DesignationView.add,
				"designation/edit": DesignationView.edit,
				"designation/delete": DesignationView.delete,
				"designation/more": DesignationListView.more,
				"designation/export": DesignationExport.export,
				"filters/data": DesignationListView.filter,
				"filters/reset": DesignationListView.reset,
				"filters/toggle": function () { dialogs.filters.slideToggle() }
			},
			forms = {
				save: $('#saveForm'),
			},
			fields = ["company_code", "designation_code"],
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
				
				var designation_code = $(event.relatedTarget).data("id"),
					data = DesignationMasterView.getCollection().get(designation_code).toJSON();

				// set data for edit button
				buttons.save.data("role", "designation/edit");
				buttons.save.data("designation-id", designation_code); 

				// fill save form data for the selected designation
				SAXForms.set(forms.save, data);

				// disable fields as required
				SAXForms.disable(fields);
			}
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "designation/add":
						buttons.save.data("role", "designation/add"); 
						break;
					case "designation/edit": 
						_setEditButton(event);
						break;
					case "designation/delete": 
						buttons.delete.data("designation-id", $(event.relatedTarget).data("id"));
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
			model_class = SAXModel.extend({ 'idAttribute': "designation_code" });

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
	DesignationMasterView.init();

	// get company data
	DesignationListView.get()
		.done(DesignationListView.render)
		.done(Company.get)
		.fail(function() { 
			SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."})
		})
		.always(SAXLoader.close);
});
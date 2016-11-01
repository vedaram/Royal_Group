$(function () {
	var Company = (function ($, w, d) {
		var
			_renderDropdown = function (data) {
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
			},
			getCompanyData = function () {
				return SAXHTTP.AJAX(
					"holiday_group_master.aspx/getCompanyData", {}
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

	var HolidayGroupView = (function ($, w, d) {

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

			if (data.holiday_group_code === "") {
	            SAXAlert.show({ type: "error", message: "Please enter Holiday Group Code"});
	            return false;
	        }

	        if (data.holiday_group_code != "" && !SAXValidation.code(data.holiday_group_code)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Holiday Group Code"});
	            return false;   
	        }

	        if (data.holiday_group_name === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Holiday Group Name"});
	            return false;
	        }

	        if (data.holiday_group_name != "" && !SAXValidation.name(data.holiday_group_name)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Holiday Group Name"});
	            return false;   
	        }

	        if (data.company_code === "select") {
	            SAXAlert.show({ type: "error", message: "Please select a Company"});
	            return false;
	        }

	        if (data.max_days === "") {
	            SAXAlert.show({ type: "error", message: "Please enter the Maximum Restricted Days"});
	            return false;
	        }

	        if (data.max_days != "" && !SAXValidation.isNumber(data.max_days)) {
	            SAXAlert.show({ type: "error", message: "Please enter a numeric value for Maximum Restricted Days"});
	            return false;
	        }

			return true;
		}

		function deleteHolidayGroup(event) {
			var 
				holiday_group_code = $(event.target).data("holiday-group-id"),
				data = HolidayGroupMasterView.getCollection().get(holiday_group_code).toJSON(),
				success = "Holiday Group deleted successfully!",
				error = "An error occurred while deleting the Holiday Group. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("holiday_group_master.aspx/DeleteHolidayGroup", {current: JSON.stringify(data)}, success, error, {data: data}))
				.then(HolidayGroupListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		function editHolidayGroup(event) {
			var 
				form_data = SAXForms.get(forms.save),
				holiday_group_code = $(event.target).data("holiday-group-id"),
				previous = HolidayGroupMasterView.getCollection().get(holiday_group_code),
				success = "Holiday Group details edited successfully!",
				error = "An error occurred while saving Holiday Group details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();

				_request("holiday_group_master.aspx/EditHolidayGroup", {current: JSON.stringify(form_data), previous: JSON.stringify(previous)}, success, error, {data: form_data})
					.done(HolidayGroupListView.delete)
					.done(HolidayGroupListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function addHolidayGroup() {
			var 
				form_data = SAXForms.get(forms.save),
				success = "Holiday Group added successfully!",
				error = "An error occurred while adding a new Holiday Group. Please try again.";

			if (_validate(form_data)) {
				// disable the button to avoid multiple clicks
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();

				$.when(_request("holiday_group_master.aspx/AddHolidayGroup", {current: JSON.stringify(form_data)}, success, error, {data: form_data})) 
					.then(HolidayGroupListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			add: addHolidayGroup,
			edit: editHolidayGroup,
			delete: deleteHolidayGroup
		};

	}) (jQuery, window, document);

	var HolidayGroupListView = (function ($, w, d) {

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

	        getHolidayGroupData()
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

	            getHolidayGroupData()
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

			getHolidayGroupData()
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
			list_elements.table.find("tr#" + data[0]["holiday_group_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			HolidayGroupMasterView.getCollection().unset(data[0]["holiday_group_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['holiday_group_code'] + '" >' +
		                        '<td>' + data[counter]['holiday_group_code'] + '</td>' +
		                        '<td>' + data[counter]['holiday_group_name'] + '</td>' +
		                        '<td>' + data[counter]['company_code'] + '</td>' +
		                        '<td>' + data[counter]['company_name'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="holiday-group/edit" data-id="' + data[counter]["holiday_group_code"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="holiday-group/delete" data-id="' + data[counter]["holiday_group_code"] + '"></span>' +
		                        '</td>' +
		                    '</tr>' ;
				}

				return table_HTML;
			}

		function render(data, data_length) { 
			var table_body;

			list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

			if (data_length > 0) {
				// if table view is hidden, show the table view
				list_elements.listview.is(":hidden") ? list_elements.listview.show() : 0;

				table_body = list_elements.table.find("tbody");
				// get the HTML and append to the table.
				table_HTML = _getHTML(data, data_length);
				table_body.append(table_HTML);
				// hiding the pagination button
				table_body.children().length < page_number*30 ? buttons.pagination.hide() : buttons.pagination.show();

				// add the data to the collection also
				HolidayGroupMasterView.getCollection().set(data);
			}
			else {
				list_elements.message.append("<h3>No Holiday Groups data found</h3>");
				// hdie the table view
				list_elements.listview.hide();
			}
		}

		function getHolidayGroupData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"holiday_group_master.aspx/getHolidayGroupData",
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
			get: getHolidayGroupData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	var HolidayGroupMasterView = (function ($, w, d) {
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
				"holiday-group/add": HolidayGroupView.add,
				"holiday-group/edit": HolidayGroupView.edit,
				"holiday-group/delete": HolidayGroupView.delete,
				"holiday-group/more": HolidayGroupListView.more,
				"filters/data": HolidayGroupListView.filter,
				"filters/reset": HolidayGroupListView.reset,
				"filters/toggle": function () { dialogs.filters.slideToggle() }
			},
			forms = {
				save: $('#saveForm'),
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

		/* dialogs */
			function _setEditButton(event) {
				
				var holiday_group_code = $(event.relatedTarget).data("id");

				// set data for edit button
				buttons.save.data("role", "holiday-group/edit");
				buttons.save.data("holiday-group-id", holiday_group_code); 

				// fill save form data for the selected company
				SAXForms.set(forms.save, HolidayGroupMasterView.getCollection().get(holiday_group_code).toJSON());

				// disable fields as required
				SAXForms.disable(["company_code", "holiday_group_code"]);
			}
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "holiday-group/add":
						buttons.save.data("role", "holiday-group/add"); 
						break;
					case "holiday-group/edit": 
						_setEditButton(event);
						break;
					case "holiday-group/delete": 
						buttons.delete.data("holiday-group-id", $(event.relatedTarget).data("id"));
						break;
				}
			}
			function _resetSaveModal(event) {
				forms.save[0].reset();
				SAXForms.enable(["holiday_group_code", "company_code"]);
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
			model_class = SAXModel.extend({ 'idAttribute': "holiday_group_code" });

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

	// INITIAL PAGE LOAD
	SAXLoader.show();

	// init page components
	HolidayGroupMasterView.init();

	// get company data
	HolidayGroupListView.get()
		.done(HolidayGroupListView.render)
		.done(Company.get)
		.fail(function() {
			SAXLoader.show({type: "error", message: "An error occurred while loading data. Please try again."})
		})
		.always(SAXLoader.close);
});
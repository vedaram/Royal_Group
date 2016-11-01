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
				"holiday.aspx/GetCompanyData", {}
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

	var HolidayView = (function ($, w, d) {

		var 
			buttons = {
				save: $('#saveButton'),
				delete: $('#deleteButton')
			},
			dialogs = {
				save: $('#saveDialog'),
				delete: $('#deleteDialog')
			},
			forms = {
				save: $('#saveForm')
			}
			$holiday_code = $('#holiday_code');

		function generateHolidayCode(company_code) {
			
			SAXLoader.show();

			return SAXHTTP.AJAX(
						"holiday.aspx/GenerateHolidayCode",
						{company_code: company_code}
					)
					.done(function (data) {
						var holiday_code = data.d.return_data;
						$holiday_code.val(holiday_code);
					})
					.fail(function () {
						SAXAlert.show({type: "error", message: "An error occurred while generating the Holiday Code. Please try again."});
					})
					.always(SAXLoader.close);
		}

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

	        if (data.holiday_name === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Holiday Name"});
	            return false;
	        }

	        if (data.holiday_name != "" && !SAXValidation.name(data.holiday_name)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Holiday Name"});
	            return false;   
	        }

	        if (data.holiday_code === "") {
	            SAXAlert.show({ type: "error", message: "Please enter a Holiday Code"});
	            return false;
	        }
	        
	        if (data.holiday_code != "" && !SAXValidation.code(data.holiday_code)) {
	            SAXAlert.show({ type: "error", message: "Please enter a valid Holiday Code"});
	            return false;   
	        }

	        if (data.holiday_from === "") {
	            SAXAlert.show({ type: "error", message: "Please select a From Date"});
	            return false;
	        }

	        if (data.holiday_to === '') {
	            SAXAlert.show({ type: "error", message: "Please select a To Date"});
	            return false;   
	        }

	        if (moment(data.holiday_from).valueOf() > moment(data.holiday_to).valueOf()) {
	            SAXAlert.show({ type: "error", message: "From date cannot be greater than To date"});
	            return false;   
	        }

	        return true;
		}

			function _formatDates(data) {
				data["holiday_from"] = moment(data["holiday_from"], "DD-MMM-YYYY").format("DD/MM/YYYY");
				data["holiday_to"] = moment(data["holiday_to"], "DD-MMM-YYYY").format("DD/MM/YYYY");

				return data;
			}


		function deleteHoliday(event) {
			var 
				holiday_code = $(event.target).data("holiday-id"),
				data = HolidayMasterView.getCollection().get(holiday_code).toJSON(),
				success = "Holiday deleted successfully!",
				error = "An error occurred while deleting the Holiday. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			data = _formatDates(data);

			$.when(_request("holiday.aspx/DeleteHoliday", {current: JSON.stringify(data)}, success, error, {data: data}))
				.then(HolidayListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		function editHoliday(event) {
			var 
				form_data = SAXForms.get(forms.save),
				holiday_code = $(event.target).data("holiday-id"),
				previous = HolidayMasterView.getCollection().get(holiday_code),
				success = "Holiday details edited successfully!",
				error = "An error occurred while saving Holiday  details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();

				form_data = _formatDates(form_data);

				_request("holiday.aspx/EditHoliday", {current: JSON.stringify(form_data), previous: JSON.stringify(previous)}, success, error, {data: form_data})
					.done(HolidayListView.delete)
					.done(HolidayListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function addHoliday() {
			var 
				form_data = SAXForms.get(forms.save),
				success = "Holiday added successfully!",
				error = "An error occurred while adding a new Holiday . Please try again.";

			if (_validate(form_data)) {
				// disable the button to avoid multiple clicks
				buttons.save.button("loading");

				// adding company name to the form data
				form_data["company_name"] = $("#company_code option:selected").text();
				
				form_data = _formatDates(form_data);

				$.when(_request("holiday.aspx/AddHoliday", {current: JSON.stringify(form_data)}, success, error, {data: form_data})) 
					.then(HolidayListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			add: addHoliday,
			edit: editHoliday,
			delete: deleteHoliday,
			code: generateHolidayCode
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var HolidayListView = (function ($, w, d) {

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

	        getHolidayData()
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

	            getHolidayData()
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

			getHolidayData()
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
			list_elements.table.find("tr#" + data[0]["holiday_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			HolidayMasterView.getCollection().unset(data[0]["holiday_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['holiday_code'] + '" >' +
		                        '<td>' + data[counter]['holiday_code'] + '</td>' +
		                        '<td>' + data[counter]['holiday_name'] + '</td>' +
		                        '<td>' + moment(data[counter]['holiday_from'], "DD/MM/YYYY").format("DD-MMM-YYYY") + '</td>' +
		                        '<td>' + moment(data[counter]['holiday_to'], "DD/MM/YYYY").format("DD-MMM-YYYY") + '</td>' +
		                        '<td>' + data[counter]['holiday_type'] + '</td>' +
		                        '<td>' + data[counter]['company_name'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="holiday/edit" data-id="' + data[counter]["holiday_code"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="holiday/delete" data-id="' + data[counter]["holiday_code"] + '"></span>' +
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
				HolidayMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No Holiday data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function getHolidayData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"holiday.aspx/getHolidayData",
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
			get: getHolidayData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var HolidayMasterView = (function ($, w, d) {
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
				"holiday/add": HolidayView.add,
				"holiday/edit": HolidayView.edit,
				"holiday/delete": HolidayView.delete,
				"holiday/more": HolidayListView.more,
				"filters/data": HolidayListView.filter,
				"filters/reset": HolidayListView.reset,
				"filters/toggle": function () { dialogs.filters.slideToggle() }
			},
			forms = {
				save: $('#saveForm'),
			},
			$company_code = $('#company_code'),
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
				
				var holiday_code = $(event.relatedTarget).data("id"),
					data = HolidayMasterView.getCollection().get(holiday_code).toJSON();

				// set data for edit button
				buttons.save.data("role", "holiday/edit");
				buttons.save.data("holiday-id", holiday_code); 

				data["holiday_from"] = moment(data["holiday_from"], "DD/MM/YYYY").format("DD-MMM-YYYY");
				data["holiday_to"] = moment(data["holiday_to"], "DD/MM/YYYY").format("DD-MMM-YYYY");

				// fill save form data for the selected company
				SAXForms.set(forms.save, data);

				// disable fields as required
				SAXForms.disable(["company_code", "holiday_code"]);
			}
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "holiday/add":
						buttons.save.data("role", "holiday/add"); 
						break;
					case "holiday/edit": 
						_setEditButton(event);
						break;
					case "holiday/delete": 
						buttons.delete.data("holiday-id", $(event.relatedTarget).data("id"));
						break;
				}
			}
			function _resetSaveModal(event) {
				forms.save[0].reset();
				SAXForms.enable(["holiday_code", "company_code"]);
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
			model_class = SAXModel.extend({ 'idAttribute': "holiday_code" });

			// define the collection class
			collection_class = SAXCollection.extend({ 'baseModel': model_class });

			// create an instance of the collection_class
			// passing an empty array as the default data
			collection = new collection_class([]); 
		};

		function _initOther() {
			$(".datepicker").Zebra_DatePicker({
				format: "d-M-Y"
			});

			$company_code.change(function () {
				HolidayView.code($(this).val());
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

	// init page components
	HolidayMasterView.init();

	// get company data
	HolidayListView.get()
		.done(HolidayListView.render)
		.done(Company.get)
		.fail(function() {
			SAXLoader.show({type: "error", message: "An error occurred while loading data. Please try again."});
		})
		.always(SAXLoader.close);
});
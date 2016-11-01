$(function () {

	/************************************************************************************************************************************************/

	var DeviceView = (function ($, w, d) {

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

		function _validate(data) {

	        return true;
		}

		/* Generic request for Company View. */
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

		function updateDevice(event) {
			var 
				form_data = SAXForms.get(forms.save),
				device_id = $(event.target).data("device-id"),
				success = "Device details updated successfully!",
				error = "An error occurred while saving Device details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				_request("information.aspx/UpdateDevice", {current: JSON.stringify(form_data)}, success, error, {data: form_data})
					.done(DeviceListView.delete)
					.done(DeviceListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function deleteDevice(event) {
			var 
				device_id = $(event.target).data("device-id"),
				data = DeviceMasterView.getCollection().get(device_id).toJSON(),
				success = "Device deleted successfully!",
				error = "An error occurred while Device the Company. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("information.aspx/DeleteDevice", {current: JSON.stringify(data)}, success, error, {data: data}))
				.then(DeviceListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		return {
			update: updateDevice,
			delete: deleteDevice
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DeviceListView = (function ($, w, d) {

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

	        getDeviceData()
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

	            getDeviceData()
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

			getDeviceData()
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
			list_elements.table.find("tr#" + data[0]["device_id"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			DeviceMasterView.getCollection().unset(data[0]["device_id"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['device_id'] + '" >' +
		                        '<td>' + data[counter]['device_id'] + '</td>' +
		                        '<td>' + data[counter]['device_location'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="device/update" data-id="' + data[counter]["device_id"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="device/delete" data-id="' + data[counter]["device_id"] + '"></span>' +
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
				DeviceMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No Device data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function getDeviceData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"information.aspx/GetDeviceData",
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
			get: getDeviceData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DeviceMasterView = (function ($, w, d) {

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
				"device/update": DeviceView.update,
				"device/delete": DeviceView.delete,
				"device/more": DeviceListView.more,
				"filters/data": DeviceListView.filter,
				"filters/reset": DeviceListView.reset,
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
				
				var device_id = $(event.relatedTarget).data("id"),
					data = DeviceMasterView.getCollection().get(device_id).toJSON();

				// set data for edit button
				buttons.save.data("role", "device/update");
				buttons.save.data("device-id", device_id); 

				data["download_punch_time"] = moment(data["download_punch_time"]).format("DD-MMM-YYYY HH:mm:ss");

				// fill save form data for the selected company
				SAXForms.set(forms.save, data);

				// disable fields as required
				SAXForms.disable(["device_id"]);
			}
				function _setModalButton(event) {

					var role = $(event.relatedTarget).data('role');

					switch (role) {
						case "device/update": 
							_setEditButton(event);
							break;
						case "device/delete": 
							buttons.delete.data("device-id", $(event.relatedTarget).data("id"));
							break;
					}
				}
				function _resetSaveModal(event) {
					forms.save[0].reset();
					SAXForms.enable([]);
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
				function getCollection() {
					return collection;
				}
			function _initModels() {
				model_class = SAXModel.extend({ 'idAttribute': "device_id" });

				// define the collection class
				collection_class = SAXCollection.extend({ 'baseModel': model_class });

				// create an instance of the collection_class
				// passing an empty array as the default data
				collection = new collection_class([]); 
			}

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
	DeviceMasterView.init();

	DeviceListView.get()
		.done(DeviceListView.render)
		.fail(function () {
			SAXAlert.show({type: "error", message: "An error occurred while getting Device data. Please try again."});
		})
		.always(function () {
			SAXLoader.close();
		});

});
$(function() {

	var DeviceView = (function ($, w, d) {

		var buttons = {
			save: $('#saveButton'),
			delete: $('#deleteButton')
		},
		dialogs = {
			save: $('#saveDialog'),
			delete: $('#deleteDialog'),
			device: $("#deviceDialog")
		},
		forms = {
			save: $('#saveForm')
		},
		$device_dialog_content = $("#statusPlaceholder");

		// Creating a configuration object which will contain device type to device model mapping.
		// Adding it in this file to avoid another HTTP request to download a config file.
		var device_model_to_type = {
			'I100'       : 'Anviz',
			'VF30'       : 'Anviz',
			'VP30'       : 'Anviz',
			'T60'        : 'Anviz',
			'OA1000'     : 'Anviz',
			'BSFACE602'  : 'BioSecurity',
			'WS700'      : 'BioSecurity',
			'BioLite Net': 'Suprema',
			'HandPunch'  : 'HandPunch',
			'I50'        : 'SecurAx',
			'T5'         : 'Anviz',
			'CB01'       : 'Zk',
			'CB02'       : 'Zk'
		};

		function _validate(data) {

			data['device_type'] = device_model_to_type[data['device_model']];

			if (data.device_id === '') {
				SAXAlert.show({'type': 'error', 'message': 'Please enter a Device ID.'});
				return false;
			}

			if (!$.isNumeric(data.device_id)) {
				SAXAlert.show({type: "error", message: "Please enter a numeric value for Device ID."});
				return false;
			}

			if (data.device_name === '') {
				SAXAlert.show({'type': 'error', 'message': 'Please enter a Device Name.'});
				return false;
			}

			if (data.communication_type === 'select') {
				SAXAlert.show({'type': 'error', 'message': 'Please select Communication Type'});
				return false;
			}

			if (data.communication_type === 'LAN' && data.device_ip === '') {
				SAXAlert.show({'type': 'error', 'message': 'Please enter the Device IP.'});
				return false;
			}

			if (data.communication_type === 'DNS' && data.device_ip === '') {
				SAXAlert.show({'type': 'error', 'message': 'Please enter a Host.'});
				return false;
			}

			if (data.communication_type === 'LAN' && data.device_ip != "" && !SAXValidation.ipAddress(data.device_ip)) {
				SAXAlert.show({type: "error", message: "Please enter a valid IP Address."});
				return false;
			}

			if (data.device_model === 'select') {
				SAXAlert.show({'type': 'error', 'message': 'Please select a Device Model.'});
				return false;
			}

			if (data.device_type != 'Anviz' && data.communication_type === 'WAN') {
				SAXAlert.show({'type': 'error', 'message': 'WAN Communication Type can be selected for Anviz devices only.'});
				return false;
			}

			if (data.finger_print === 0 && data.card_number === 0 && data.pin_number === 0) {
				SAXAlert.show({'type': 'error', 'message': 'Please select atleast one option from Device Category.'});
				return false;
			}

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

		function getDeviceTime(event) {
			var
				device_id = $(event.target).data("id"),
				data = DeviceMasterView.getCollection().get(device_id).toJSON();

			SAXLoader.show();

			SAXHTTP.AJAX(
					"information.aspx/GetDeviceTime",
					{current: JSON.stringify(data)}
				)
			.done(function (data) {
				$device_dialog_content.text(data.d.return_data);
				dialogs.device.modal("show");
			})
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while establishing a connection with the Device. Please try again." });
			})
			.always(SAXLoader.close);
		}

		function setDeviceTime(event) {
			var
				device_id = $(event.target).data("id"),
				data = DeviceMasterView.getCollection().get(device_id).toJSON();

			SAXLoader.show();

			SAXHTTP.AJAX(
					"information.aspx/SetDeviceTime",
					{current: JSON.stringify(data)}
				)
			.done(function (data) {
				$device_dialog_content.text(data.d.return_data);
				dialogs.device.modal("show");
			})
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while establishing a connection with the Device. Please try again." });
			})
			.always(SAXLoader.close);
		}

		function testConnection(event) {
			var
				device_id = $(event.target).data("id"),
				data = DeviceMasterView.getCollection().get(device_id).toJSON();

			SAXLoader.show();

			SAXHTTP.AJAX(
					"information.aspx/TestDeviceConnection",
					{current: JSON.stringify(data)}
				)
			.done(function (data) {
				connection_status = $('tr#' + device_id).find('td:nth-child(6)');

				if (data.d.return_data === 'connected') {
					$(connection_status).html('<span class="text-green fa fa-circle"></span>');
				}
				else {
					$(connection_status).html('<span class="text-red fa fa-circle"></span>');
				}

			})
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while establishing a connection with the Device. Please try again." });
			})
			.always(SAXLoader.close);
		}

		function deleteDevice(event) {
			var 
				device_id = $(event.target).data("device-id"),
				form_data = DeviceMasterView.getCollection().get(device_id).toJSON(),
				success = "Device deleted successfully!",
				error = "An error occurred while deleting Device details. Please try again.";

				// disable save button to avoid multiple clicks.
				buttons.delete.button("loading");

			_request("information.aspx/DeleteDeviceInformation", {current: JSON.stringify(form_data)}, success, error, {data: form_data})
				.done(function() {DeviceListView.delete([form_data], 1)})
				.done(function () { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset") });
		}

		function editDevice(event) {
			var 
				form_data = SAXForms.get(forms.save),
				device_id = $(event.target).data("device-id"),
				previous = DeviceMasterView.getCollection().get(device_id);
				success = "Device details updated successfully!",
				error = "An error occurred while saving Device details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				form_data['device_type'] = device_model_to_type[form_data['device_model']];

				_request("information.aspx/EditDeviceInformation", {current: JSON.stringify(form_data), previous: JSON.stringify(previous)}, success, error, {data: form_data})
					.done(DeviceListView.delete)
					.done(DeviceListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function addDevice(event) {
			var 
				form_data = SAXForms.get(forms.save),
				device_id = $(event.target).data("device-id"),
				success = "Device details updated successfully!",
				error = "An error occurred while saving Device details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				form_data['device_type'] = device_model_to_type[form_data['device_model']];

				_request("information.aspx/AddDeviceInformation", {current: JSON.stringify(form_data)}, success, error, {data: form_data})
					.done(DeviceListView.delete)
					.done(DeviceListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			add: addDevice,
			edit: editDevice,
			delete: deleteDevice,
			test: testConnection,
			setTime: setDeviceTime,
			getTime: getDeviceTime
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	var DeviceListView = (function ($, w, d) {

		var 
			page_number = 1,
			buttons = {
				pagination: $('#paginationButton')
			},
			list_elements = {
				table: $('#dataTable'),
				message: $('#noData'),
				listview: $('#listView')
			};

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

				var  
					table_HTML = "", counter = 0, status;

				for (counter = 0; counter < data_length; counter += 1) {

					status = data[counter]["status"];

					if (status == "connected") {
						status = '<span class="text-green fa fa-circle"></span>';
					}
					else {
						status = '<span class="text-red fa fa-circle"></span>';
					}

					table_HTML += '<tr id="' + data[counter]['device_id'] + '" >' +
		                        '<td>' + data[counter]['device_id'] + '</td>' +
		                        '<td>' + data[counter]['device_name'] + '</td>' +
		                        '<td>' + data[counter]['communication_type'] + '</td>' +
		                        '<td>' + data[counter]['device_ip'] + '</td>' +
		                        '<td>' + data[counter]['device_model'] + '</td>' +
		                        '<td>' + status + '</td>' +
		                        '<td>' + 
		                            '<div class="dropdown">' +
			        					'<a id="actionsDropdown" data-target="#" href="javascript:void(0);" data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">' +
			        						'<span class="fa fa-ellipsis-h"></span>' +
			        					'</a>' +
			        					'<ul class="dropdown-menu dropdown-menu-right" aria-labelledby="actionsDropdown">' +
			        						'<li><a href="#" data-role="device/edit" data-toggle="modal" data-target="#saveDialog" data-id="' + data[counter]["device_id"] + '"><span class="fa fa-pencil text-orange action-icon"></span> Edit Device Info</a></li>' +
			        						'<li><a href="#" data-control="button" data-role="device/get-time" data-id="' + data[counter]["device_id"] + '"><span class="fa fa-clock-o  text-blue action-icon"></span> Get Device Time</a></li>' +
			        						'<li><a href="#" data-control="button" data-role="device/set-time" data-id="' + data[counter]["device_id"] + '"><span class="fa fa-clock-o text-blue action-icon"></span> Set Device Time</a></li>' +
			        						'<li><a href="#" data-control="button" data-role="device/test" data-id="' + data[counter]["device_id"] + '"><span class="fa fa-signal text-blue action-icon"></span> Test Connection</a></li>' +
			        						'<li><a href="#" data-role="device/delete" data-toggle="modal" data-target="#deleteDialog" data-id="' + data[counter]["device_id"] + '"><span class="fa fa-trash-o text-red action-icon"></span> Delete Device Info</a></li>' +
			        					'</ul>' +
			        				'</div>' +
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
			get: getDeviceData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	var DeviceMasterView = (function ($, w, d) {

		var 		
			dialogs = {
				"save": $("#saveDialog"),
				"delete": $("#deleteDialog")
			},
			buttons = {
				save: $('#saveButton'),
				delete: $('#deleteButton')
			},
			button_events = {
				"device/add": DeviceView.add,
				"device/edit": DeviceView.edit,
				"device/delete": DeviceView.delete,
				"device/test": DeviceView.test,
				"device/set-time": DeviceView.setTime,
				"device/get-time": DeviceView.getTime
			},
			forms = {
				save: $('#saveForm'),
			},
			$communication_type = $("#communication_type"),
			$device_ip = $("#device_ip"),
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

			function _setDeviceCategory(data) {

				switch (data["category"]) {

				case 'F':
					data["finger_print"] = 1;
					break;
				case 'C':
					data["card_number"] = 1;
					break;
				case 'P':
					data["pin_number"] = 1;
					break;
				case 'FC':
					data["finger_print"] = 1;
					data["card_number"] = 1;
					break;
				case 'FP':
					data["finger_print"] = 1;
					data["pin_number"] = 1;
					break;
				case 'PC':
					data["pin_number"] = 1;
					data["card_number"] = 1;
					break;
				case 'FCP':
					data["finger_print"] = 1;
					data["card_number"] = 1;
					data["pin_number"] = 1;
					break;
				}

				return data;
			}

			function _setEditButton(event) {
				
				var device_id = $(event.relatedTarget).data("id"),
					data = DeviceMasterView.getCollection().get(device_id).toJSON();

				// set data for edit button
				buttons.save.data("role", "device/edit");
				buttons.save.data("device-id", device_id); 

				// fill save form data for the selected device
				data = _setDeviceCategory(data);
				SAXForms.set(forms.save, data);
				$communication_type.trigger("change");
				$device_ip.val(data["device_ip"]);

				// disable fields as required
				SAXForms.disable(["device_id"]);
			}

				function _setModalButton(event) {

					var role = $(event.relatedTarget).data('role');

					switch (role) {
						case "device/add":
							buttons.save.data("role", "device/add"); 
							break;
						case "device/edit": 
							_setEditButton(event);
							break;
						case "device/delete": 
							buttons.delete.data("device-id", $(event.relatedTarget).data("id"));
							break;
					}
				}

				function _resetSaveModal(event) {
					forms.save[0].reset();
					SAXForms.enable(["device_id"]);
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

		function _initOther() {
			$communication_type.change(function () {
				var value = $(this).val();
				if (value == "WAN" || value == "USB") {
					$device_ip.parent().hide();
				}
				else {
					$device_ip.parent().show();
					$device_ip.val("");
					if (value == "DNS") {
						$device_ip.next().text("Host Name");
					}
					else {
						$device_ip.next().text("Device IP");	
					}
				}
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

	/******************************************************************************************************************/
	
	// INITIAL PAGE LOAD
	SAXLoader.show();

	DeviceMasterView.init();

	DeviceListView.get()
		.done(DeviceListView.render)
		.fail(function () {
			SAXAlert.show({type: "error", message: "An error occurred while loading device data. Please try again."});
		})
		.always(function () {
			SAXLoader.close();
		});
});
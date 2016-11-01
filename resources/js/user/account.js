$(function () {

	var AccountView = (function ($, w, d) {

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

			if (data.employee_code == "") {
				SAXAlert.show({type: "error", message: "Please enter an Employee Code"});
				return false;
			}

			if (data.username == "") {
				SAXAlert.show({type: "error", message: "Please enter an Username"});
				return false;	
			}

			if (data.password == "") {
				SAXAlert.show({type: "error", message: "Please enter a Password"});
				return false;
			}

			if (data.confirm_password == "") {
				SAXAlert.show({type: "error", message: "Please confirm the Password"});
				return false;
			}

			if (data.password != data.confirm_password) {
				SAXAlert.show({type: "error", message: "Password and confirm password should be the same"});
				return false;
			}

	        return true;
		}

		function deleteAccount(event) {
			var 
				employee_code = $(event.target).data("employee-code"),
				data = AccountMasterView.getCollection().get(employee_code).toJSON(),
				success = "User deleted successfully!",
				error = "An error occurred while deleting the user. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("account.aspx/DeleteUser", {employee_id: employee_code}, success, error, {data: data}))
				.then(AccountListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		function updateAccount(event) {
			var 
				form_data = SAXForms.get(forms.save),
				employee_code = $(event.target).data("employee-code"),
				previous_user_id = AccountMasterView.getCollection().get(employee_code).toJSON()["employee_code"],
				success = "User details edited successfully!",
				error = "An error occurred while saving User  details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				_request("account.aspx/UpdateUser", {current: JSON.stringify(form_data), previous_user_id: previous_user_id}, success, error, {data: form_data})
					.done(AccountListView.delete)
					.done(AccountListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			update: updateAccount,
			delete: deleteAccount
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	var AccountListView = (function ($, w, d) {

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

		        if (data.filter_keyword != "" && data.filter_by == 0) {
		            SAXAlert.show({'type': 'error', 'message': 'Please select a Filter By option.'});
		            return false;
		        }

		        if (data.filter_by != 0 && data.filter_keyword == "") {
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
		}

		/* pagination functions */
		function loadMoreData() {
			// disable pagination button to avoid multiple clicks
			buttons.pagination.button("loading"); 
			
			page_number += 1;

			getEmployeeData()
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
			AccountMasterView.getCollection().unset(data[0]["employee_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['employee_code'] + '" >' +
		                        '<td>' + data[counter]['employee_code'] + '</td>' +
		                        '<td>' + data[counter]['username'] + '</td>' +
		                        '<td>' + data[counter]['employee_name'] + '</td>' +
		                        '<td>' + data[counter]['password'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="account/update" data-id="' + data[counter]["employee_code"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="account/delete" data-id="' + data[counter]["employee_code"] + '"></span>' +
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
				AccountMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No User data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function getEmployeeData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"account.aspx/GetUserData",
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
			get: getEmployeeData,
			render: render,
			delete: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	var AccountMasterView = (function ($, w, d) {

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
				"account/update": AccountView.update,
				"account/delete": AccountView.delete,
				"account/more": AccountListView.more,
				"filters/data": AccountListView.filter,
				"filters/reset": AccountListView.reset,
				"filters/toggle": function () {
					dialogs.filters.slideToggle();
				}
			},
			forms = {
				save: $('#saveForm'),
			},
			$confirm_password = $("#confirm_password"),
			model_class, collection_class, collection;

		/* dialogs */
			function _setEditButton(event) {
				
				var employee_code = $(event.relatedTarget).data("id"),
					user_data = AccountMasterView.getCollection().get(employee_code).toJSON();

				// set data for edit button
				buttons.save.data("employee-code", employee_code); 

				// fill save form data for the selected company
				SAXForms.set(forms.save, user_data);

				// set the value for confirm password
				$confirm_password.val(user_data["password"]);

				// disable fields as required
				SAXForms.disable(["employee_code", "access_level"]);
			}
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "account/update": 
						_setEditButton(event);
						break;
					case "account/delete": 
						buttons.delete.data("employee-code", $(event.relatedTarget).data("id"));
						break;
				}
			}
			function _resetSaveModal(event) {
				forms.save[0].reset();
				SAXForms.enable(["employee_code", "access_level"]);
			}

		function _initDialogs() {
			// before the modal is shown to the user,
			// change the function of the save button to add or edit.
			dialogs.save.on('show.bs.modal', _setModalButton);

			// reset the form on modal close.
			dialogs.save.on("hidden.bs.modal", _resetSaveModal);

			dialogs.delete.on("show.bs.modal", _setModalButton);
		}

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
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
	AccountMasterView.init();

	// get employee data
	AccountListView.get()
		.done(AccountListView.render)
		.fail(function() {
			SAXLoader.show({type: "error", message: "An error occurred while loading data. Please try again."});
		})
		.always(SAXLoader.close);
});
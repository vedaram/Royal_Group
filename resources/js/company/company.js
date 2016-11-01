$(function () {

	var CompanyExport = (function ($, w, d) {

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
	            SAXAlert.show({'type': status, 'message': data.return_data});
	            break;
           };
		}

		function doExport() {

			buttons.export_button.button("loading");

			SAXHTTP.AJAX(
				"company.aspx/DoExport", {}
			)
			.done(_processExport)
			.fail(function() {
				SAXAlert.show({type: "error", message: "An error occurred while exporting Company data. Please try again."});
			})
			.always(function () { buttons.export_button.button("reset"); });
		}

		return {
			export: doExport
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var CompanyView  = (function ($, w, d) {

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
			var
	            company_code = data['company_code'],
	            company_name = data['company_name'],
	            email = data['email_address'],
	            url = data['website'];

	        if (company_code === "") {
	            SAXAlert.show({type: "error", message: "Please enter a Company Code"});
	            return false;
	        }

	        if (!SAXValidation.code( company_code )) {
	            SAXAlert.show({type: "error", message: "Please enter a valid Company Code"});
	            return false;
	        }

	        if (company_name === "") {
	            SAXAlert.show({type: "error", message: "Please enter a Company Name"});
	            return false;
	        }

	        if (data["phone_number"] != "" && !SAXValidation.isNumber(data["phone_number"])) {
	        	SAXAlert.show({type: "error", message: "Please enter a numeric value for Phone Number"});
	        	return false;
	        }

	        if (data["fax_number"] != "" && !SAXValidation.isNumber(data["fax_number"])) {
	        	SAXAlert.show({type: "error", message: "Please enter a numeric value for Fax Number"});
	        	return false;
	        }

	        if (email !== '' && !SAXValidation.email(email) ) {
	            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a valid Email Address'});
	            return false;
	        }

	        if (url !== '' && !SAXValidation.url(url) ) {
	            SAXAlert.show({ 'type': 'error', 'message': 'Please enter a valid URL'});
	            return false;
	        }

	        if (data["pin_code"] != "" && !SAXValidation.isNumber(data["pin_code"])) {
	        	SAXAlert.show({type: "error", message: "Please enter a numeric value for PIN code"});
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

		function addCompany() {
			var 
				form_data = SAXForms.get(forms.save),
				success = "Company added successfully!",
				error = "An error occurred while adding a new Company. Please try again.";

			if (_validate(form_data)) {
				// disable the button to avoid multiple clicks
				buttons.save.button("loading");

				$.when(_request("company.aspx/AddCompany", {current: JSON.stringify(form_data)}, success, error, {data: form_data})) 
					.then(CompanyListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function editCompany(event) {
			var 
				form_data = SAXForms.get(forms.save),
				company_code = $(event.target).data("company-id"),
				previous = CompanyMasterView.getCollection().get(company_code),
				success = "Company details edited successfully!",
				error = "An error occurred while saving Company details. Please try again.";

			if (_validate(form_data)) {
				// disable save button to avoid multiple clicks.
				buttons.save.button("loading");

				_request("company.aspx/EditCompany", {current: JSON.stringify(form_data), previous: JSON.stringify(previous)}, success, error, {data: form_data})
					.done(CompanyListView.delete)
					.done(CompanyListView.render)
					.done(function () { dialogs.save.modal("hide"); })
					.always(function () { buttons.save.button("reset") });
			}
		}

		function deleteCompany(event) {
			var 
				company_code = $(event.target).data("company-id"),
				data = CompanyMasterView.getCollection().get(company_code).toJSON(),
				success = "Company deleted successfully!",
				error = "An error occurred while deleting the Company. Please try again.";

			// disable the button to avoid multiple clicks
			buttons.delete.button("loading");

			$.when(_request("company.aspx/DeleteCompany", {current: JSON.stringify(data)}, success, error, {data: data}))
				.then(CompanyListView.delete)
				.done(function() { dialogs.delete.modal("hide"); })
				.always(function () { buttons.delete.button("reset"); });
		}

		return {
			add: addCompany,
			edit: editCompany,
			delete: deleteCompany
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var CompanyListView = (function ($, w, d) {

		var 
			page_number = 1,
			buttons = {
				pagination: $('#paginationButton'),
			},
			list_elements = {
				table: $('#dataTable'),
				listview: $('#listView'),
				message: $("#noData")
			};

		/* pagination functions */
		function loadMoreData() {
			// disable pagination button to avoid multiple clicks
			buttons.pagination.button("loading"); 
			
			page_number += 1;

			getCompanyData()
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
			list_elements.table.find("tr#" + data[0]["company_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			CompanyMasterView.getCollection().unset(data[0]["company_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['company_code'] + '" >' +
		                        '<td>' + data[counter]['company_code'] + '</td>' +
		                        '<td>' + data[counter]['company_name'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="company/edit" data-id="' + data[counter]["company_code"] + '"></span>' +
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="company/delete" data-id="' + data[counter]["company_code"] + '"></span>' +
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
				CompanyMasterView.getCollection().set(data);
			}
			else {
				list_elements.message.append("<h3>No Company data found</h3>");
				// hdie the table view
				list_elements.listview.hide();
			}
		}

		/* get company data function */
		function getCompanyData() {

			var companyDataDeferred = $.Deferred();

			SAXHTTP.AJAX(
				"company.aspx/getCompanyData",
				{page_number: page_number}
			)
			.done(function (data) {
				var data = JSON.parse(data.d.return_data);
				companyDataDeferred.resolve(data, data.length);
			}).fail(function () {
				companyDataDeferred.reject();
			});

			return companyDataDeferred.promise();
		};

		return {
			get: getCompanyData,
			render: render,
			delete: removeRow,
			more: loadMoreData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var CompanyMasterView = (function ($, w, d) {

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
				"company/add": CompanyView.add,
				"company/edit": CompanyView.edit,
				"company/delete": CompanyView.delete,
				"company/more": CompanyListView.more,
				"company/export": CompanyExport.export
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
				
				var company_code = $(event.relatedTarget).data("id");

				// set data for edit button
				buttons.save.data("role", "company/edit");
				buttons.save.data("company-id", company_code); 

				// fill save form data for the selected company
				SAXForms.set(forms.save, CompanyMasterView.getCollection().get(company_code).toJSON());

				// disable fields as required
				SAXForms.disable(["company_code"]);
			}
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "company/add":
						buttons.save.data("role", "company/add"); 
						break;
					case "company/edit": 
						_setEditButton(event);
						break;
					case "company/delete": 
						buttons.delete.data("company-id", $(event.relatedTarget).data("id"));
						break;
				}
			}
			function _resetSaveModal(event) {
				forms.save[0].reset();
				SAXForms.enable(["company_code"]);
			}

		function _initDialogs() {
			// before the modal is shown to the user,
			// change the function of the save button to add or edit.
			dialogs.save.on('show.bs.modal', _setModalButton);

			// reset the form on modal close.
			dialogs.save.on("hidden.bs.modal", _resetSaveModal);

			dialogs.delete.on("show.bs.modal", _setModalButton);
		}

		/* modals */
			getCollection = function () {
				return collection;
			}
		_initModels = function () {
			model_class = SAXModel.extend({ 'idAttribute': "company_code" });

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

	// initialize page components
	CompanyMasterView.init();

	// get company data
	CompanyListView.get()
		.done(CompanyListView.render)
		.fail(function() { 
			SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."})
		})
		.always(SAXLoader.close);
});
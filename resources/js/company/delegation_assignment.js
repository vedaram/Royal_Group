$(function () {
	
	var Company = (function ($, w, d) {

		function _renderDropdown(data) {
	        var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $('#company_code'),
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['company_code'] + '">' + data[counter]['company_name'] + '</option>';
	        }

	        $element.append(select_HTML);
		}
		
		function getCompanyData() {
			return SAXHTTP.AJAX(
				"delegation_assignment.aspx/GetCompanyData", {}
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

	var OtherData = (function ($, w, d) {

		var
			$branch_code = $('#branch_code'),
			$manager_id = $('#manager_id'),
			$delegation_manager_id = $('#delegation_manager_id');

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

		function getManagerData(company_code, branch_code) {

			
			$delegation_manager_id.empty().append("<option value=\"select\">Loading ...</option>");

			return SAXHTTP.AJAX(
					"delegation_assignment.aspx/GetManagerData",
					{company_code: company_code, branch_code: branch_code}
				)
				.done(function (data) {
					var results = JSON.parse(data.d.return_data);
		           // _render($manager_id, results, 'employee_code', 'employee_name', 'Select Manager', 'No Managers found');
		            _render($delegation_manager_id, results, 'employee_code', 'employee_name', 'Select Delegation Manager', 'No Managers found');
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Manager data. Please try again."});
				});
		}
		
		function getSourceManagerData(company_code, branch_code) {

			$manager_id.empty().append("<option value=\"select\">Loading ...</option>");
			

			return SAXHTTP.AJAX(
					"delegation_assignment.aspx/getSourceManagerData",
					{company_code: company_code, branch_code: branch_code}
				)
				.done(function (data) {
					var results = JSON.parse(data.d.return_data);
		            _render($manager_id, results, 'employee_code', 'employee_name', 'Select Manager', 'No Managers found');
		            
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Manager data. Please try again."});
				});
		}

		function getBranchData(company_code) {

			$branch_code.empty().append("<option value=\"select\">Loading ...</option>");

			return SAXHTTP.AJAX(
					"delegation_assignment.aspx/GetBranchData",
					{company_code: company_code}
				)
				.done(function (data) {
					var results = JSON.parse(data.d.return_data);
		            _render($branch_code, results, 'branch_code', 'branch_name', 'Select Branch', 'No Branches found');
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Branch data. Please try again."});
				});
		}

		return {
			branch: getBranchData,
			manager: getManagerData,
			manager1: getSourceManagerData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DelegationView = (function ($, w, d) {

		var 
			buttons = {
				save: $('#saveButton')
			},
			dialogs = {
				save: $('#saveDialog')
			},
			forms = {
				save: $('#saveForm')
			};

		function _validate(data)  {

			if (data.company_code === "select") {
	            SAXAlert.show({ type: "error", message: "Please select a Company"});
	            return false;
	        }

	        if (data.branch_code === "select") {
	            SAXAlert.show({ type: "error", message: "Please select a Branch"});
	            return false;
	        }

	        if (data.from_date === "") {
	            SAXAlert.show({ type: "error", message: "Please select a From Date"});
	            return false;
	        }

	        if (data.to_date === "") {
	            SAXAlert.show({ type: "error", message: "Please select a To Date"});
	            return false;   
	        }

	        if (moment(data.from_date).valueOf() > moment(data.to_date).valueOf()) {
	            SAXAlert.show({ type: "error", message: "From date cannot be greater than To date"});
	            return false;   
	        }

	        if (data.manager_id === "select") {
	            SAXAlert.show({ type: "error", message: "Please select a Manager"});
	            return false;
	        }

	        if (data.delegation_manager_id === "select") {
	            SAXAlert.show({ type: "error", message: "Please select a Delegation Manager"});
	            return false;
	        }

	        return true;
		}

			function _formatDates(data) {
				data["from_date"] = moment(data["from_date"], "DD-MMM-YYYY").format("DD-MM-YYYY");
				data["to_date"] = moment(data["to_date"], "DD-MMM-YYYY").format("DD-MM-YYYY");

				return data;
			}

		function saveDelegation() {
			var 
				form_data = SAXForms.get(forms.save);

			if (_validate(form_data)) {
				// disable the button to avoid multiple clicks
				buttons.save.button("loading");
				
				form_data = _formatDates(form_data);

				SAXHTTP.AJAX(
						"delegation_assignment.aspx/SaveDelegationAssignment", {current: JSON.stringify(form_data)}						
					)
					.done(function () {

						form_data["from_date"] = moment(form_data["from_date"], "DD-MM-YYYY").toJSON();
						form_data["to_date"] = moment(form_data["to_date"], "DD-MM-YYYY").toJSON();
						// remove the existing row from the table
						DelegationListView.remove([form_data], 1);
						// render the new row in the table
						DelegationListView.render([form_data], 1);
						// hide the save dialog once done.
						dialogs.save.modal("hide");
						SAXAlert.show({type: "success", message: "Delegation Assignment saved successfully!"});
					})
					.fail(function () {
						SAXAlert.show({type: "error", message: "An error occurred while saving Delegation Assignment. Please try again."})
					})
					.always(function () { buttons.save.button("reset") });
			}
		}

		return {
			save: saveDelegation
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DelegationListView = (function ($, w, d) {

		var 
			page_number = 1,
			buttons = {
				pagination: $('#paginationButton')
			}
			forms = {
				filter: $('#filterForm')
			},
			list_elements = {
				table: $('#dataTable'),
				message: $('#noData'),
				listview: $('#listView')
			};

		function loadMoreData() {
			// disable pagination button to avoid multiple clicks
			buttons.pagination.button("loading"); 
			
			page_number += 1;

			getDelegationData()
				.done(render)
				.fail(function () { 
						SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."}) 
				})
				.always(function () { 
					SAXLoader.close(); 
					buttons.pagination.button("reset"); 
				});
		}

		/* remove function */
		function removeRow(data, data_length) {

			// remove the row from the table
			list_elements.table.find("tr#" + data[0]["manager_id"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			DelegationMasterView.getCollection().unset(data[0]["manager_id"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['manager_id'] + '" >' +
		                        '<td>' + moment(data[counter]['from_date']).format("DD-MMM-YYYY") + '</td>' +
		                        '<td>' + moment(data[counter]['to_date']).format("DD-MMM-YYYY") + '</td>' +
		                        '<td>' + data[counter]['manager_id'] + '</td>' +
		                        '<td>' + data[counter]['delegation_manager_id'] + '</td>' +
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
					DelegationMasterView.getCollection().set(data);
				}
				else { 
					list_elements.message.append("<h3>No Delegation assignments found</h3>");
					// hide the table view.
					list_elements.listview.hide();
				}
			}
		function getDelegationData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"delegation_assignment.aspx/getDelegationData",
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
			get: getDelegationData,
			render: render,
			remove: removeRow,
			more: loadMoreData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var DelegationMasterView = (function ($, w, d) {

		var 		
			dialogs = {
				"save": $("#saveDialog")
			},
			buttons = {
				save: $('#saveButton')
			},
			button_events = {
				"delegation-assignment/save": DelegationView.save,
				"delegation-assignment/more": DelegationListView.more,
			},
			forms = {
				save: $('#saveForm'),
			},
			$company_code = $('#company_code'),
			$branch_code = $('#branch_code'),
			model_class, collection_class, collection;

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

			function _resetSaveModal(event) {
				forms.save[0].reset();
			}
		function _initDialogs() {
			// reset the form on modal close.
			dialogs.save.on("hidden.bs.modal", _resetSaveModal);
		}

		/* models */
			getCollection = function () {
				return collection;
			}
		_initModels = function () {
			model_class = SAXModel.extend({ 'idAttribute': "manager_id" });

			// define the collection class
			collection_class = SAXCollection.extend({ 'baseModel': model_class });

			// create an instance of the collection_class
			// passing an empty array as the default data
			collection = new collection_class([]); 
		};

		function _initOther() {
			$(".datepicker").Zebra_DatePicker({
				format: "d-M-Y",
				direction: true
			});

			$company_code.change(function () {
				OtherData.branch($(this).val());
			});

			$branch_code.change(function () {
				var company_code = $company_code.val();
				OtherData.manager(company_code, $(this).val());
				OtherData.manager1(company_code, $(this).val());
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

	// initialize components on the page
	DelegationMasterView.init();

	// get delegation assignment and company data
	DelegationListView.get()
		.done(DelegationListView.render)
		.done(Company.get)
		.fail(function() {
			SAXLoader.show({type: "error", message: "An error occurred while loading data. Please try again."})
		})
		.always(SAXLoader.close);
});
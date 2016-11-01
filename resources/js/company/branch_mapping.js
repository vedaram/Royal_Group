$(function () {

	var Company = (function ($, w, d) {

		function _renderDropdown(data) {
	        var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            data_length = data.length,
	            counter = 0,
	            $element = $('#filter_company'),
	            $parent = $element.parent();

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['company_code'] + '">' + data[counter]['company_name'] + '</option>';
	        }

	        $element.append(select_HTML);
		}
		
		function getCompanyData() {
			return SAXHTTP.AJAX(
				"branch_mapping.aspx/GetCompanyData", {}
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

	var Branch = (function ($, w, d) {

		var
			buttons = {
				branch: $("#branchButton")
			},
			page_elements = {
				branch: $("#branchData"),
				manager: $("#managerHRData")
			},
			list_elements = {
				branch_table: $('#branchTable'),
				message: $('#branchData #noData'),
				manager_table: $("#managerHRTable")
			};

			function _getHTML(data) {
				var 
					all_branches = data.all_branches,
					all_branches_length = all_branches.length,
					selected_branches = data.selected_branches,
					checked = "",
					table_HTML = "", 
					counter = 0;

				selected_branches = _groupByKey(selected_branches);

				for (counter = 0; counter < all_branches_length; counter += 1) { 
					checked = selected_branches[all_branches[counter]["branch_code"]] != undefined ? "checked" : ""; 
					table_HTML += '<tr id="' + all_branches[counter]['branch_code'] + '" >' +
		                        '<td><input type="checkbox" id="' + all_branches[counter]["branch_code"] + '" value="' + all_branches[counter]["branch_code"] + '" ' + checked + '/> ' + all_branches[counter]['branch_name'] + '</td>' +
		                    '</tr>' ;
				}

				return table_HTML;
			}

			function _groupByKey(data) {
				var 
					result = {},
					counter = 0,
					data_length = data.length;

				for (counter = 0; counter < data_length; counter+=1) {
					result[data[counter]["BranchCode"]] = data[counter]["BranchCode"];
				}

				return result;
			}

		function _render(data) {
			var 
				table_body,
				data_length = data.all_branches.length;

			if (data_length > 0) {
				table_body = list_elements.branch_table.find("tbody");
				// get the HTML and append to the table.
				table_HTML = _getHTML(data);
				table_body.empty().append(table_HTML);
			}
			else { 
				list_elements.message.append("<h3>No Branch data found</h3>");
			}
		}

		function getBranchData(manager_id, company_code) {

			return SAXHTTP.AJAX(
					"branch_mapping.aspx/GetBranchData",
					{manager_id: manager_id, company_code: company_code}
				)
				.done(function (data) {
					var results = JSON.parse(data.d.return_data);

					if (data.d.status == "success") { 
		            	_render(results);
		            	page_elements.manager.addClass("hide");
		            	page_elements.branch.removeClass("hide");
					}
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Branch data. Please try again."});
				});
		}

		return {
			get: getBranchData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var Manager = (function ($, w, d) {

		var	
			$filter = $("#filter"),
			list_elements = {
				table: $('#managerHRTable'),
				message: $('#managerHRData #noData')
			};

			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['employee_code'] + '" >' +
		                        '<td><input type="checkbox" id="' + data[counter]["employee_code"] + '" value="' + data[counter]["employee_code"] + '"/> ' + data[counter]['employee_name'] + '</td>' +
		                    '</tr>' ;
				}

				return table_HTML;
			}

		function _render(data) {
			var 
				table_body,
				data_length = data.length;

			if (data_length > 0) {
				table_body = list_elements.table.find("tbody");
				// get the HTML and append to the table.
				table_HTML = _getHTML(data, data_length);
				table_body.empty().append(table_HTML);
			}
			else { 
				list_elements.message.append("<h3>No Manager data found</h3>");
			}
		}

		function getManagerData(company_code) {

			var filter = $filter.val();

			return SAXHTTP.AJAX(
					"branch_mapping.aspx/GetManagerData",
					{filter: filter, company_code: company_code}
				)
				.done(function (data) {
					var results = JSON.parse(data.d.return_data);
		            _render(results);
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while loading Manager data. Please try again."});
				});
		}

		return {
			get: getManagerData
		};

	}) (jQuery, window, document);


	/************************************************************************************************************************************************/

	var BranchMapping = (function ($, w, d) {

		var
			$company = $("#filter_company"),
			$manager_table = $('#managerHRTable'),
			$branch_table = $('#branchTable'),
			$update_button = $('#updateButton');

			function _validate(managers, branches) {

				if (managers.length === 0) {
					SAXAlert.show({type: "error", message: "Please select at least one Manager"});
					return false;
				}

				if (branches.length === 0) {
					SAXAlert.show({type: "error", message: "Please select at least one Branch"});
					return false;
				}

				return true;
			}

		function _save(branches, managers) {

			SAXHTTP.AJAX(
					"branch_mapping.aspx/SaveBranchMapping",
					{branches: JSON.stringify(branches), managers: JSON.stringify(managers), company_code: $company.val()}
				)
				.done(function () {
					// Uncheck the selected input boxes.
					$manager_table.find('tbody input:checked').prop('checked', false);
					$branch_table.find('tbody input:checked').prop('checked', false);
					// Display the result of the action.
					SAXAlert.show({type: "success", message: "Branch Mapping updated successfully!"});
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while updating mapping. Please try again."});	
				})
				.always(function () {
					$update_button.button("reset");
				})
		}

		function saveMapping() {

			var 
				selected_managers = $manager_table.find('tbody input:checked'),
				selected_branches = $branch_table.find('tbody input:checked'),
				selected_managers_length = selected_managers.length,
				selected_branches_length = selected_branches.length,
				managers = [], branches = [],
				ajax_options = {};

			// Getting the EMP_CODE of the manager from the selected rows.
			for (var i = 0; i < selected_managers_length; i++) {
	            managers.push( $(selected_managers[i]).val());
	        };

	        // Getting the BRANCH_CODE of the branch from the selected row.s
	        for (var i = 0; i < selected_branches_length; i++) {
	            branches.push( $(selected_branches[i]).val());
	        };

	        if (_validate(managers, branches)) {
	        	$update_button.button('loading');
        		_save(branches, managers);
        	}
		}

		return {
			save: saveMapping
		};

	}) (jQuery, window, document);


	/************************************************************************************************************************************************/

	BranchMappingMasterView = (function ($, w, d) {

		var
			$manager_table = $("#managerHRTable"),
			$manager_table_body = $manager_table.find("tbody"),
			$company = $("#filter_company"),
			$filter = $("#filter"),
			page_elements = {
				branch: $("#branchData"),
				manager: $("#managerHRData")
			},
			button_events = {
				"branch-mapping/save": BranchMapping.save,
				"branch-mapping/manager": function () {
					$manager_table_body.find("[type=\"checkbox\"]").prop("checked", false);
					page_elements.branch.addClass("hide");
					page_elements.manager.removeClass("hide");
				}
			};

		/* other */
		function _initOther() {

			$company.change(function () {
				$manager_table_body.empty();
				if ($(this).val() != "select") {
					SAXLoader.show();
					Manager.get($(this).val())
						.done(SAXLoader.close);
				}
			});

			$filter.change(function() {
				if ($company.val() != "select") {
					
					SAXLoader.showBlockingLoader();
					Manager.get($company.val())
					.done(SAXLoader.closeBlockingLoader);
				}
				else {
					SAXAlert.show({type: "error", message: "Please select a Company"});
				}
			});

			$manager_table_body.on("change", "[type=\"checkbox\"]", function () {
				if ($(this).is(":checked"))
					Branch.get($(this).val(), $company.val());
			});
		}

		/* buttons */
			function _buttonHandler(event) {
				var role = $(event.target).data('role');
				button_events[role].call(this, event);
			}

		function _initButtons() {
			$(document).on("click", "[data-control=\"button\"]", _buttonHandler);
		}

		function initialize() {
			_initButtons();
			_initOther();
		}

		return {
			init: initialize
		};

	}) (jQuery, window, document);

	// INITIAL PAGE LOAD
	SAXLoader.show();

	// initialize page components
	BranchMappingMasterView.init();

	Company.get()
		.done(SAXLoader.close);
});
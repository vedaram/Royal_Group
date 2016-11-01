$(function() {

    var Company = (function($, w, d) {

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
				"change_manager.aspx/GetCompanyData", {}
			)
			.done(_renderDropdown)
			.fail(function() {
			    SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." })
			});
        };

        return {
            get: getCompanyData
        };

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    var Branch = (function($, w, d) {

        var 
			$branch = $("#filter_branch");

        function _render(data) {
            var 
				data_length = data.length,
	            select_HTML = '<option value="select">Select Branch</option>',
	            counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter]["branch_code"] + '">' + data[counter]["branch_name"] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">No Branch data found</option>';
            }

            $branch.empty();
            $branch.append(select_HTML);
        }

        function getBranchData(company_code) {

            $branch.empty().append("<option value=\"select\">Loading ...</option>")

            return SAXHTTP.AJAX(
					"change_manager.aspx/GetBranchData",
					{ company_code: company_code }
				)
				.done(function(data) {
				    var results = JSON.parse(data.d.return_data);
				    _render(results);
				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Branch data. Please try again." });
				});
        }

        return {
            get: getBranchData
        }

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    var Manager = (function($, w, d) {

        var 
			$source_manager = $("#source_manager"),
			$new_manager = $('#new_manager'),
			forms = {
			    filter: $('#filterForm')
			};

        function _render(data, $element, default_text) {
            var 
				data_length = data.length,
	            select_HTML = '<option value="select">' + default_text + '</option>',
	            counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter]["employee_code"] + '">' + data[counter]["employee_name"] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">No Manager data found</option>';
            }

            $element.empty();
            $element.append(select_HTML);
        }

        function getManagerData() {

            var data = SAXForms.get(forms.filter);

            $source_manager.empty().append("<option value=\"select\">Loading ...</option>");
            $new_manager.empty().append("<option value=\"select\">Loading ...</option>")

            return SAXHTTP.AJAX(
					"change_manager.aspx/GetManagerData",
					{ filters: JSON.stringify(data) }
				)
				.done(function(data) {
				    var results = JSON.parse(data.d.return_data);
				    // _render(results, $source_manager, "Select Manager");
				    _render(results, $new_manager, "Select New Manager");
				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Manager data. Please try again." });
				});
        }

        return {
            get: getManagerData
        }




    })(jQuery, window, document);

    /************************************************************************************************************************************************/
    var SourceManager = (function($, w, d) {

        var 
			$source_manager = $("#source_manager"),
			$new_manager = $('#new_manager'),
			forms = {
			    filter: $('#filterForm')
			};

        function _render(data, $element, default_text) {
            var 
				data_length = data.length,
	            select_HTML = '<option value="select">' + default_text + '</option>',
	            counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter]["employee_code"] + '">' + data[counter]["employee_name"] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">No Manager data found</option>';
            }

            $element.empty();
            $element.append(select_HTML);
        }




        function getSourceManagerData() {

            var data = SAXForms.get(forms.filter);

            $source_manager.empty().append("<option value=\"select\">Loading ...</option>");


            return SAXHTTP.AJAX(
					"change_manager.aspx/getSourceManagerData",
					{ filters: JSON.stringify(data) }
				)
				.done(function(data) {
				    var results = JSON.parse(data.d.return_data);
				    _render(results, $source_manager, "Select Manager");

				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Manager data. Please try again." });
				});
        }

        return {
            get: getSourceManagerData
        }

    })(jQuery, window, document);



    Employee = (function($, w, d) {

        var 
			list_elements = {
			    table: $("#dataTable"),
			    message: $('#noData'),
			    listview: $("#listView")
			};

        function _getHTML(data) {

            var 
		        	data_length = data.length,
		            table_HTML = '',
		            counter = 0;

            for (counter = 0; counter < data_length; counter += 1) {

                table_HTML += '<tr id="' + data[counter]['employee_code'] + '" >' +
		                            '<td><input type="checkbox" value="' + data[counter]['employee_code'] + '" ></td>' +
		                            '<td>' + data[counter]['employee_code'] + '</td>' +
		                            '<td>' + data[counter]['employee_name'] + '</td>' +
		                        '</tr>';
            }

            return table_HTML;
        }

        function _render(data) {

            var 
					table_body,
					data_length = data.length;

            list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

            if (data_length > 0) {
                // if table is hidden, show the table
                list_elements.listview.is(":hidden") ? list_elements.listview.show() : 0;

                table_body = list_elements.table.find("tbody");
                // get the HTML and append to the table.
                table_HTML = _getHTML(data);
                table_body.append(table_HTML);
                // hiding the pagination button
            }
            else {
                list_elements.message.append("<h3>No Employee data found</h3>");
                // hide the table view.
                list_elements.listview.hide();
            }
        }

        function getEmployeeData(manager_id) {

            return SAXHTTP.AJAX(
					"change_manager.aspx/GetEmployeeData",
					{ manager_id: manager_id }
				)
				.done(function(data) {
				    var results = JSON.parse(data.d.return_data);
				    _render(results);
				})
				.fail(function() {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Employee data. Please try again." });
				});
        }

        return {
            get: getEmployeeData
        }

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    ChangeManager = (function($, w, d) {

        var 
			list_elements = {
			    table: $('#dataTable'),
			    table_body: $('#dataTable tbody')
			},
			buttons = {
			    update: $("#updateButton")
			},
			$source_manager = $("#source_manager"),
			$new_manager_id = $("#new_manager");

        function _removeUpdateEmployees(employees) {
            var 
				employees_length = employees.length,
				i = 0;
            for (i = 0; i < employees_length; i += 1) {
                $('#dataTable tr#' + employees[i]).remove();
            }
        }

        function updateManager() {
            var 
	            selected_employees = list_elements.table_body.find('input:checked'),
	            employees = [],
	            selected_employees_length = selected_employees.length;

            if (selected_employees_length == 0) {
                SAXAlert.show({ type: "error", message: "Please select one or more employees" });
                return false;
            }

            buttons.update.button("loading");

            for (var i = 0; i < selected_employees_length; i++) {
                employees.push($(selected_employees[i]).val());
            };

            SAXHTTP.AJAX(
	        		"change_manager.aspx/UpdateManager",
	        		{ employees: JSON.stringify(employees), new_manager_id: $new_manager_id.val() }
	        	)
	        	.done(function(data) {
	        	    SAXAlert.show({ type: "success", message: data.d.return_data + " rows successfully updated!" });
	        	    //_removeUpdateEmployees(employees);
	        	    $source_manager.val("select");
	        	    $new_manager_id.val("select");
	        	    list_elements.table_body.empty();
	        	})
	        	.fail(function() {
	        	    SAXAlert.show({ type: "error", message: "An error occurred while saving changes. Please try again." });
	        	})
	        	.always(function() {
	        	    buttons.update.button("reset");
	        	});
        }

        return {
            update: updateManager
        };

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    ChangeManagerView = (function($, w, d) {

        var 
			buttons = {
			    filter: $('#filterButton')
			},
			forms = {
			    filter: $('#filterForm')
			},
			$table_body = $('#dataTable tbody');

        function _validateFilters(data) {

            if (data.filter_company == "select") {
                SAXAlert.show({ type: "error", message: "Please select a Company" });
                return false;
            }

            if (data.filter_by != "0" && data.filter_keyword == "") {
                SAXAlert.show({ type: "error", message: "Please enter a Keyword" });
                return false;
            }

            if (data.filter_keyword != "" && data.filter_by == "0") {
                SAXAlert.show({ type: "error", message: "Please select a Filter By option" });
                return false;
            }

            return true;
        }

        function getData(event) {

            var data = SAXForms.get(forms.filter);

            if (_validateFilters(data)) {
                $table_body.empty();
                buttons.filter.button("loading");
                SAXLoader.show();
                SourceManager.get()
                Manager.get()
						.done(function() { buttons.filter.button("reset"); })
						.always(SAXLoader.close);
            }
        }

        return {
            get: getData
        };

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    ChangeManagerMasterView = (function($, w, d) {

        var 
			button_events = {
			    "change-manager/update": ChangeManager.update,
			    "filters/data": ChangeManagerView.get
			},
			$company = $('#filter_company'),
			$source_manager = $("#source_manager"),
			$table_body = $("#dataTable tbody");

        /* other */
        function _initOther() {
            $company.change(function() {
                Branch.get($(this).val());
            });

            $source_manager.change(function() {
                $table_body.empty();
                Employee.get($(this).val());
            });

            $("#checkall").change(function() {
                var is_checked = $(this).is(':checked'),
	                checkboxes = $table_body.find('input[type="checkbox"]');
                is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
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

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    // INITIAL PAGE LOAD
    SAXLoader.show();

    ChangeManagerMasterView.init();

    Company.get()
		.done(SAXLoader.close);
});
$(function () {

	var Employee = (function ($, w, d) {

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

			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['employee_code'] + '" >' +
								'<td><input type="checkbox" value="' + data[counter]['employee_code'] + '" ></td>' +
		                        '<td>' + data[counter]['employee_code'] + '</td>' +
		                        '<td>' + data[counter]['employee_name'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="permissions/delete" data-id="' + data[counter]["employee_code"] + '"></span>' +
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
				PermissionsMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No Employee data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function getEmployeeData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"permissions.aspx/GetEmployeeData",
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
			filter: filterData,
			reset: resetFilters,
			more: loadMoreData,
			render: render
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	var Permissions = (function ($, w, d) {

		var 
			buttons = {
				save: $("#saveButton"),
				next: $("#nextButton"),
				delete: $("#deleteButton")
			},
			tabs = {
				employee: $("#employeeTab"),
				permissions: $("#permissionsTab")
			},
			list_elements = {
				employee: $("#dataTable"),
				permissions_table: $("#permissions")
			},
			dialogs = {
				delete: $("#deleteDialog")
			},
			default_perms = {};

		function _getSelectedPermissions() {
			var 
				permissions = list_elements.permissions_table.find(".permission"),
				permissions_length = permissions.length,
				id;

			for ( i = 0; i < permissions_length; i += 1) {
				id = $(permissions[i]).data("id");
				default_perms[id]["value"] = $(permissions[i]).find("span").hasClass("fa-check-square-o") ? 1 : 0;
			}
		}

		function _getPermissionsString(perms) {

			var i, final = "";

			_getSelectedPermissions();

			for ( i in default_perms ) { 
				final += perms[i]["value"];
			}

			return final;
		}

		function deletePermissions(event) {

			buttons.delete.button("loading");

			SAXHTTP.AJAX(
					"permissions.aspx/DeletePermissions",
					{employee_code: $(event.target).data("employee-code")}
				)
				.done(function (data) {
					SAXAlert.show({type: data.d.status, message: data.d.return_data});
					dialogs.delete.modal("hide");
				})
				.fail(function () {
					SAXAlert.show({type: "error", message: "An error occurred while performing this action. Please try again."})
				})
				.always(function () {
					buttons.delete.button("reset");
				});
		}	

			function _render(data, type) {
				var
					i, j,
					sections = EmployeePermissions.sections(),
					permissions = {},
					permissions_HTML = "",
					icon = "",
					current_section, current_section_length;

				if (type == "single") {
					default_perms = EmployeePermissions.process(data["PERMISSIONS"]);
				}
				else {
					default_perms = data;
				}

				permissions = EmployeePermissions.group(default_perms);

				for ( i in permissions ) {

					current_section = permissions[i];
					current_section_length = current_section.length;

					if (_.where(permissions[i], {value: "1"}).length == current_section_length) 
						icon = "fa fa-check-square-o";
					else 
						icon = "fa fa-square-o";

					permissions_HTML += '<div class="permissions-section">' +
    									'<div class="permissions-section-header">' +
        									'<div class="container">' +
            									'<h4 class="pull-left">' + sections[i]["display"] + '</h4>' +
            									'<p><span class="' + icon + ' pull-right" data-section="' + i + '" data-control="button" data-role="permissions/select"></span></p>' +
        									'</div>' +	
    									'</div>' +
    									'<div class="permissions-section-body" id="' + i + '">' +
        									'<div class="container">';

					for ( j = 0; j < current_section_length; j += 1 ) {

						if (current_section[j]["value"] == 0) 
							icon = '<span class="fa fa-square-o" data-role="permissions/change"></span>';
						else 
							icon = '<span class="fa fa-check-square-o" data-role="permissions/change"></span>';

							permissions_HTML += '<div class="permission" id="' + current_section[j]["key"] + '" data-id="' + current_section[j]["key"] + '" data-section="' + current_section[j]["section"] + '" data-text="' + current_section[j]["display"] + '" data-control="button" data-role="permissions/change" >' +
                									icon + ' ' + current_section[j]["display"] +
            									'</div>';
					}

					permissions_HTML += "</div></div></div>";
				}

				list_elements.permissions_table.empty().append(permissions_HTML);
			}

		function changePermission(event) {
			var
				current = $(event.target);

			if (!current.hasClass("fa"))
				current = current.find('span');
			
			if (current.hasClass("fa-square-o")) 
				current.removeClass("fa-square-o").addClass("fa-check-square-o");
			else 
				current.removeClass("fa-check-square-o").addClass("fa-square-o")
		}

		function changeMutliplePermissions(event) {
			var
				current = $(event.target),
				section = current.data("section");

			if (current.hasClass("fa-square-o")) {
				current.removeClass("fa-square-o").addClass("fa-check-square-o");
				list_elements.permissions_table.find("#" + section + " .permission span").removeClass("fa-square-o").addClass("fa-check-square-o");
			}
			else {
				current.removeClass("fa-check-square-o").addClass("fa-square-o");
				list_elements.permissions_table.find("#" + section + " .permission span").removeClass("fa-check-square-o").addClass("fa-square-o");	
			}
		}

		function getPermissions(event) {

			var selected_employees = list_elements.employee.find("tbody input:checked"),
				employees = [],
	            selected_employees_length = selected_employees.length;

            if (selected_employees_length == 0) {
            	SAXAlert.show({type: "error", message: "Please select one or more employees"});
            	return false;
            }
            if (selected_employees_length > 1) {
            	_render(EmployeePermissions.defaultMap(), "multiple");
            	tabs.employee.addClass("hide");
				tabs.permissions.removeClass("hide");
				return false;
            }

            buttons.next.button("loading");

			SAXHTTP.AJAX(
					"permissions.aspx/GetPermissions",
					{employee_code: $(selected_employees[0]).val()}
				)
				.done(function (data) {
					var data = JSON.parse(data.d.return_data);

					if (data.length > 0) 
						_render(data[0], "single");
					else {
						_render(EmployeePermissions.defaultMap(), "multiple");
					}

					tabs.employee.addClass("hide");
					tabs.permissions.removeClass("hide");
				})
				.fail(function (data) {
					SAXAlert.show({type: "error", message: data.d.return_data});
				})
				.always(function () {
					buttons.next.button("reset");
				});
		}

		function savePermissions(event) {

			var selected_employees = list_elements.employee.find("input:checked"),
				employees = [],
	            selected_employees_length = selected_employees.length;

            if (selected_employees_length == 0) {
            	SAXAlert.show({type: "error", message: "Please select one or more employees"});
            	return false;
            }

            buttons.save.button("loading");

            for (var i = 0; i < selected_employees_length; i++) { 
	            employees.push( $(selected_employees[i]).val());
	        };

	        var permissions = _getPermissionsString(default_perms); 

	        SAXHTTP.AJAX(
	        		"permissions.aspx/SavePermissions",
	        		{employees: JSON.stringify(employees), permissions: permissions}
	        	)
	        	.done(function (data) {
	        		
	        		list_elements.employee.find("input:checked").prop("checked", false);

	        		SAXAlert.show({type: data.d.status, message: data.d.return_data});

	        		tabs.permissions.addClass("hide");
					tabs.employee.removeClass("hide");
	        	})
	        	.fail(function () {
	        		SAXAlert.show({type: "error", message: "An error occurred while saving changes. Please try again."});
	        	})
	        	.always(function () {
	        		buttons.save.button("reset");
	        	});
		}

		return {
			save: savePermissions,
			get: getPermissions,
			delete: deletePermissions,
			change: changePermission,
			changeMultiple: changeMutliplePermissions
		};

	}) (jQuery, window, document);

	/******************************************************************************************************************/

	var PermissionsMasterView = (function ($, w, d) {
		var 		
			dialogs = {
				"delete": $("#deleteDialog"),
				"filters": $('#filters')
			},
			tabs = {
				employee: $("#employeeTab"),
				permissions: $("#permissionsTab")
			},
			buttons = {
				delete: $('#deleteButton')
			},
			button_events = {
				"permissions/show": Permissions.get,
				"permissions/delete": Permissions.delete,
				"permissions/change": Permissions.change,
				"permissions/save": Permissions.save,
				"permissions/select": Permissions.changeMultiple,
				"employee/more": Employee.more,
				"filters/data": Employee.filter,
				"filters/reset": Employee.reset,
				"filters/toggle": function () {
					dialogs.filters.slideToggle();
				},
				"employee/show": function () {
					$("#dataTable").find("input:checked").prop("checked", false);
					tabs.permissions.addClass("hide");
					tabs.employee.removeClass("hide");
				}
			},
			$check_all = $("#check_all"),
			$table_body = $("#dataTable tbody"),
			model_class, collection_class, collection;

		/* dialogs */
			function _setModalButton(event) {

				var role = $(event.relatedTarget).data('role');

				switch (role) {
					case "permissions/delete": 
						buttons.delete.data("employee-code", $(event.relatedTarget).data("id"));
						break;
				}
			}

		function _initDialogs() {
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

		function _initOther() {
			$check_all.change(function(event){
				var is_checked = $(this).is(':checked'),
	                checkboxes = $table_body.find('input[type="checkbox"]');
	            is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
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

	// init page components
	PermissionsMasterView.init();

	Employee.get()
		.done(Employee.render)
		.fail(function () {
			SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."});
		})
		.always(function () {
			SAXLoader.close();
		});
});
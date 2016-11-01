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
				"holiday_list.aspx/GetCompanyData", {}
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

	var HolidayGroup = (function ($, w, d) {

		var 
			$holiday_group_code = $('#holiday_group_code');

		function _renderDropdown(result) {
			var select_HTML = "<option value=\"select\">Select Holiday Group</option>",
	        	data = JSON.parse(result.d.return_data)
	            data_length = data.length,
	            counter = 0;

	        for (counter = 0; counter < data_length; counter += 1) {
	            select_HTML += '<option value="' + data[counter]['holiday_group_code'] + '">' + data[counter]['holiday_group_name'] + '</option>';
	        }

	        $holiday_group_code.empty().append(select_HTML);
		}

		function getHolidayGroupData(company_code) {

			$holiday_group_code.empty().append("<option value=\"Loading ...\"></option>");

			return SAXHTTP.AJAX(
				"holiday_list.aspx/GetHolidayGroupData", {company_code: company_code}
			)
			.done(_renderDropdown)
			.fail(function () {
				SAXAlert.show({type: "error", message: "An error occurred while loading Holiday Group data. Please try again."})
			});
		}

		return {
			get: getHolidayGroupData
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var HolidayList = (function ($, w, d) {

		var 
			buttons = {
				save: $('#saveButton'),
				delete: $("#deleteButton")
			},
			dialogs = {
				save: $("#saveDialog")
			},
			form_elements = {
				holiday_list: $("#holiday_list"),	
				holiday_group_code: $("#holiday_group_code"),
			}
			$save_form = $('#saveForm'),
			list_elements = {
				table: $('#holidayListTable'),
				message: $('#noHolidayData')
			}
			$company_code = $("#company_code");

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['holcode'] + '" >' +
		                        '<td>' + data[counter]['holname'] + '</td>' +
		                        '<td>' + moment(data[counter]['holfrom']).format("DD-MMM-YYYY") + '</td>' +
		                        '<td>' + moment(data[counter]['holto']).format("DD-MMM-YYYY") + '</td>' +
		                    '</tr>' ;
				}

				return table_HTML;
			}

		function renderTable(data) { 
			
			var 
				table_body,
				data = JSON.parse(data.d.return_data).selected_holidays,
				data_length = data.length;

			list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

			if (data_length > 0) {
				// if table is hidden, show the table
				list_elements.table.is(":hidden") ? list_elements.table.show() : 0;

				table_body = list_elements.table.find("tbody");
				// get the HTML and append to the table.
				table_HTML = _getHTML(data, data_length);
				table_body.empty().append(table_HTML);
			}
			else { 
				list_elements.message.append("<h3>No Holidays data found</h3>");
				// hide the table view.
				list_elements.table.hide();
			}
		}

			function _groupByKey(data) {
				var 
					result = {},
					counter = 0,
					data_length = data.length;

				for (counter = 0; counter < data_length; counter+=1) {
					result[data[counter]["holcode"]] = data[counter]["holname"];
				}

				return result;
			}

		function renderDropdown(data) {

			var select_HTML = "",
	        	data = JSON.parse(data.d.return_data)
	            all_holidays = data.all_holidays,
	            all_holidays_length = all_holidays.length,
	            selected_holidays = data.selected_holidays,
	            counter = 0;

	        if (selected_holidays.length > 0) { 
	        	buttons.save.data("mode", "U");
	        }
	        else {
	        	buttons.save.data("mode", "I");
	        }

	        selected_holidays = _groupByKey(selected_holidays);

	        for (counter = 0; counter < all_holidays_length; counter += 1) { 
	        	selected = selected_holidays[all_holidays[counter]["holiday_code"]] != undefined ? "selected" : ""; 
	            select_HTML += '<option value="' + all_holidays[counter]['holiday_code'] + '" ' + selected + '>' + all_holidays[counter]['holiday_name'] + '</option>';
	        }

	        form_elements.holiday_list.empty().append(select_HTML);
		}

		function getHolidayList(year, holiday_group_code, get_selected) {
			return SAXHTTP.AJAX(
					"holiday_list.aspx/GetHolidayData",
					{year: year, holiday_group_code: holiday_group_code, get_selected: get_selected, company_code: $company_code.val()}
				)
				.fail(function() { SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."}); })
		}

		 	function _getSelectedHolidays() {

		        var selected_holidays = [];

		        $.each(form_elements.holiday_list.find("option:selected"), function() {
		            selected_holidays.push({'holiday_code': $(this).val(), 'holiday_name': $(this).text()});
		        });

		        return selected_holidays;
		    };

		        function _validate(data) {

			        if (data['company_code'] == 'select') {
			            SAXAlert.show({type: "error", message: "Please select a Company"});
			            return false;
			        }

			        if (data['holiday_group_code'] == 'select') {
			            SAXAlert.show({type: "error", message: "Please select a Holiday Group"});
			            return false;
			        }

			        if (data['year'] == 'select') {
			            SAXAlert.show({type: "error", message: "Please select a Year"});
			            return false;
			        }

			        return true;
			    };

	    function deleteHolidayList(event) {

			var 
				holidays = _getSelectedHolidays(),
				form_data = SAXForms.get($save_form);

			if (_validate(form_data)) {
				buttons.delete.button("loading");

				SAXHTTP.AJAX(
					"holiday_list.aspx/DeleteHolidayList",
					{current: JSON.stringify(form_data), holidays: JSON.stringify(holidays)}
				)
				.done(function (data) {
					var 
						status = data.d.status,
						message = data.d.return_data;

					if (status == "success") { 

						if (+message == 0) {
							HolidayListView.remove([form_data], 1);
						}

						$save_form[0].reset();
						// reset the selected holidays in the list.
						form_elements.holiday_list.val("");
						// hiding the modal after deleting
						dialogs.save.modal("hide");

						message = "Holiday List deleted successfully!";
					}

					SAXAlert.show({type: status, message: message});
				})
				.fail()
				.always(function () {
					buttons.delete.button("reset");
				});
			}
		}

		function saveHolidayList(event) {

			var 
				holidays = _getSelectedHolidays(),
				form_data = SAXForms.get($save_form);

			if (holidays.length == 0) {
				SAXAlert.show({type: "error", message: "Please select one or more Holidays"});
				return false;
			}

			if (_validate(form_data)) {
				buttons.save.button("loading");

				form_data["holiday_group_name"] = form_elements.holiday_group_code.find("option:selected").text();

				SAXHTTP.AJAX(
					"holiday_list.aspx/SaveHolidayList",
					{current: JSON.stringify(form_data), holidays: JSON.stringify(holidays), mode: $(event.target).data("mode")}
				)
				.done(function (data) {
					var status = data.d.status;
					if (status == "success") { 
						// first we remove the row from the table
						HolidayListView.remove([form_data], 1);
						// render the row again, but this time with the changed data.
						HolidayListView.render([form_data], 1); 
						dialogs.save.modal("hide");
					}

					SAXAlert.show({type: status, message: data.d.return_data});
				})
				.fail()
				.always(function () {
					buttons.save.button("reset");
				});
			}
		}

		return {
			save: saveHolidayList,
			remove: deleteHolidayList,
			get: getHolidayList,
			dropdown: renderDropdown,
			table: renderTable
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

	        getHolidayListData()
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

	            getHolidayListData()
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

			getHolidayListData()
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
			list_elements.table.find("tr#" + data[0]["holiday_group_code"]).remove();
			
			// since the row is being remove, we are also going to remove it from the collection.
			HolidayListMasterView.getCollection().unset(data[0]["holiday_group_code"]);
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['holiday_group_code'] + '" >' +
		                        '<td>' + data[counter]['holiday_group_name'] + '</td>' +
		                        '<td>' + 
		                            '<span class="fa fa-eye action-icon" data-toggle="modal" data-target="#holidayListDialog" data-role="holiday-list/view" data-id="' + data[counter]["holiday_group_code"] + '"></span>' +
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
				HolidayListMasterView.getCollection().set(data);
			}
			else { 
				list_elements.message.append("<h3>No Holiday List data found</h3>");
				// hide the table view.
				list_elements.listview.hide();
			}
		}

		function getHolidayListData() {
			var deferred = $.Deferred();

			SAXHTTP.AJAX(
				"holiday_list.aspx/GetHolidayListData",
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
			get: getHolidayListData,
			render: render,
			remove: removeRow,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var HolidayListMasterView = (function ($, w, d) {

		var 		
			dialogs = {
				"save": $("#saveDialog"),
				"view": $('#holidayListDialog'),
				"filters": $('#filters')
			},
			buttons = {
				save: $('#saveButton')
			},
			button_events = {
				"holiday-list/save": HolidayList.save,
				"holiday-list/remove": HolidayList.remove,
				"holiday-list/more": HolidayListView.more,
				"filters/data": HolidayListView.filter,
				"filters/reset": HolidayListView.reset,
				"filters/toggle": function () { dialogs.filters.slideToggle() }
			},
			forms = {
				save: $('#saveForm'),
			},
			$company_code = $('#company_code'),
			$holiday_group_code = $('#holiday_group_code'),
			$holiday_list = $('#holiday_list'),
			$year = $('#year'),
			$filter_year = $('#filter_year'),
			$view_table = $('#holidayListTable'),
			$holiday_no_data = $("#noHolidayData"),
			model_class, collection_class, collection;

			function _populateYear() {
				var
		            i = 2010,
		            select_HTML = '';

		        for (i = 2010; i < 2101; i+=1) {
		            select_HTML += '<option value="' + i + '">' + i + '</option>';
		        }

		        $year.append(select_HTML);
		        $filter_year.append(select_HTML);
			}

			function _processChangeYear(event) {
				var 
					year = $(event.target).val(),
					holiday_group_code = $holiday_group_code.val();

				if (holiday_group_code != "select") {
					$holiday_list.empty().append("<option value=\"select\">Loading ...</option>");
					HolidayList.get(year, holiday_group_code, true)
						.done(HolidayList.dropdown);
				}
			}

			function _processChangeHolidayGroup(event) {
				var
					holiday_group_code = $(event.target).val(),
					year = $year.val();

				if (year != "select") {
					$holiday_list.empty().append("<option value=\"select\">Loading ...</option>");
					HolidayList.get(year, holiday_group_code, true)
						.done(HolidayList.dropdown);
				}
			}

		function _initOther() {
			$company_code.change(function () {
				HolidayGroup.get($(this).val());
			});

			$year.change(_processChangeYear);

			$holiday_group_code.change(_processChangeHolidayGroup);

			$filter_year.change(function() {
				if ($(this).val() != "select") {
					HolidayList.get($(this).val(), $(this).data("holiday-group-code"), false)
					.done(HolidayList.table);
				}
			});

			_populateYear();
		}

		/* models */
			getCollection = function () {
				return collection;
			}
		function _initModels() {
			model_class = SAXModel.extend({ 'idAttribute': "holiday_group_code" });

			// define the collection class
			collection_class = SAXCollection.extend({ 'baseModel': model_class });

			// create an instance of the collection_class
			// passing an empty array as the default data
			collection = new collection_class([]); 
		}

			function _resetSaveModal(event) {
				$holiday_list.empty();
				forms.save[0].reset();
			}
			function _clearViewModal() { 
				$view_table.find("tbody").empty();
				$filter_year.val("select");
				$view_table.show();
				$holiday_no_data.empty();
			}
			function _setHolidayGroup(event) {
				$filter_year.data("holiday-group-code", $(event.relatedTarget).data("id"));
			}

		function _initDialogs() {
			// reset the form on modal close.
			dialogs.save.on("hidden.bs.modal", _resetSaveModal);

			dialogs.view.on("show.bs.modal", _setHolidayGroup);

			dialogs.view.on("hidden.bs.modal", _clearViewModal);
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
			_initDialogs();
			_initModels();
			_initOther();
		}

		return {
			init: initialize,
			getCollection: getCollection
		};

	}) (jQuery, window, document);

	// INITIAL PAGE LOAD
	SAXLoader.show();

	// initialize page components
	HolidayListMasterView.init();

	HolidayListView.get()
		.done(HolidayListView.render)
		.done(Company.get)
		.fail(function () {
			SAXAlert.show({type: "error", message: "An error occurred while loading page data. Please try again."});
		})
		.always(SAXLoader.close);
});
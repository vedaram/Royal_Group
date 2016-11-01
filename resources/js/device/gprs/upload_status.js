$(function () {

	var EnrollmentListView = (function ($, w, d) {

		var 
			page_number = 1,
			is_filter = false,
			buttons = {
				pagination: $('#paginationButton'),
			},
			list_elements = {
				table: $('#dataTable'),
				listview: $('#listView'),
				message: $("#noData")
			},
			forms = {
				filter: $("#filterForm")
			},
			dialogs = {
				filter: $("#filters")
			};

		function resetFilters() {
	        is_filter = false;
	        page_number = 1;

	        forms.filter[0].reset();
	        SAXLoader.show();

	        list_elements.table.find('tbody').empty();

	        getEnrollmentData()
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

	            getEnrollmentData()
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

			getEnrollmentData()
				.done(render)
				.fail(function () { 
					SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."}) 
				})
				.always(function () { 
					SAXLoader.close(); 
					buttons.pagination.button("reset"); 
				});
		}

		/* rendering functions */
			function _getHTML(data, data_length) {
				var  table_HTML = "", counter = 0;

				for (counter = 0; counter < data_length; counter += 1) {
					table_HTML += '<tr id="' + data[counter]['enroll_id'] + '" >' +
		                        '<td>' + data[counter]['device_id'] + '</td>' +
		                        '<td>' + data[counter]['enroll_id'] + '</td>' +
		                        '<td>' + data[counter]['employee_name'] + '</td>' +
		                        '<td>' + data[counter]['upload_flag'] + '</td>' +
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
			}
			else {
				list_elements.message.append("<h3>No Enrollment data found</h3>");
				// hdie the table view
				list_elements.listview.hide();
			}
		}

		/* get company data function */
		function getEnrollmentData() {

			var enrollmentDataDeferred = $.Deferred(),
				filters = SAXForms.get(forms.filter);

			SAXHTTP.AJAX(
				"upload_status.aspx/getEnrollmentData",
				{page_number: page_number, is_filter: is_filter, filters: JSON.stringify(filters)}
			)
			.done(function (data) {
				var data = JSON.parse(data.d.return_data);
				enrollmentDataDeferred.resolve(data, data.length);
			}).fail(function () {
				enrollmentDataDeferred.reject();
			});

			return enrollmentDataDeferred.promise();
		};

		return {
			get: getEnrollmentData,
			render: render,
			more: loadMoreData,
			filter: filterData,
			reset: resetFilters
		};

	}) (jQuery, window, document);

	/************************************************************************************************************************************************/

	var EnrollmentMasterView = (function ($, w, d) {

		var 		
			dialogs = {
				filter: $('#filters')
			},
			button_events = {
				"enrollment/more": EnrollmentListView.more,
				"filters/data": EnrollmentListView.filter,
				"filters/reset": EnrollmentListView.reset,
				"filters/toggle": function () {
					dialogs.filter.slideToggle();
				}
			};

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
		}

		return {
			init: initialize
		};

	}) (jQuery, window, document);

	// INITIAL PAGE LOAD
	SAXLoader.show();

	// initialize page components
	EnrollmentMasterView.init();

	// get company data
	EnrollmentListView.get()
		.done(EnrollmentListView.render)
		.fail(function() { 
			SAXAlert.show({type: "error", message: "An error occurred while loading data. Please try again."})
		})
		.always(SAXLoader.close);
});
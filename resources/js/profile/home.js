var HomePage = (function($, w, d) {

	var 
		main, _getCurrentUser, 
		_createAdminDashboard, _createEmployeeDashboard,
		_getEmployeeStrength, _processEmployeeStrengthData, _renderEmployeeStrengthGraph,
		_renderEmployeeDetails, _renderWorkedHoursGraph,
		_getCalendarData, _processCalendarData,
		_getLeavesData, _processLeavesData;

	var 
		$admin_dashboard         = $('#adminDashboard'),
		$employee_strength_graph = $('#employeeStrengthGraph'),
		$present_count           = $('#presentEmployeeCount'),
		$absent_count            = $('#absentEmployeeCount'),
		$calendar                = $('#calendar'),
		$worked_hours_graph      = $('#hoursWorkedGraph'),
		$leaves_balance          = $('#leavesAvailableTable'),
		$holiday_listing         = $('#holidayListingTable'),
		$employee_dashboard = $('#employeeDashboard'),
	    $lStarCount = $('#leaveStarCount');

	var
		page_name = 'home.aspx',
		$worked_hours_chart = null;

	/******************************************************************************************************************/

	_renderWorkedHoursGraph = function (data) {

		var 
			graph_data           = [],
			graph_options        = [],
			graph_labels         = [],
			graph_processed_data = [],
			ctx                  = undefined,
			data_length          = data.length,
			i                    = 0,
			$graph_canvas        = $worked_hours_graph.find('canvas'),
			$graph_parent        = $worked_hours_graph.parent(),
			$no_data             = $graph_parent.find('.no-data'),
			$loading             = $graph_parent.find('.loading');

		if ($worked_hours_chart) $worked_hours_chart.destroy();

		if (data_length === 0) {
			$no_data.removeClass('hide');
		} else {

			!$no_data.hasClass('hide') ? $no_data.addClass('hide') : '';

			for ( i = 0; i < data_length; i += 1 ) {

				graph_labels.push( moment(data[i]["pdate"]).date() );
				graph_processed_data.push( +data[i]["WorkHrs"].split(":")[0] );
			}

			graph_data = {
				labels: graph_labels,
		        datasets: [
		            {
						label      : "Number of Hours Worked",
						fillColor  : "rgba(39, 174, 96, 0.2)",
						strokeColor: "rgba(39, 174, 96, 0.8)",
						data       : graph_processed_data
		            }
		        ]
			};

			graph_options = {
					scaleShowGridLines      : true,
					scaleGridLineColor      : "rgba(0, 0, 0, 0.1)",
					scaleGridLineWidth      : 1,
					scaleShowHorizontalLines: true,
					scaleShowVerticalLines  : false,
					pointHitDetectionRadius : 5
			};

			ctx = $graph_canvas.get(0).getContext("2d");

			$worked_hours_chart = new Chart(ctx).Line(graph_data, graph_options); 
		}

		$loading.addClass('hide');

	};

	/******************************************************************************************************************/

	_processCalendarData = function (data) {

		var
			data_length = data.length,
			l_star_count = 0,
			events      = [],
			in_time     = '00:00',
			out_time    = '00:00';

		for ( i = 0; i < data_length; i++ ) {

			switch ($.trim(data[i]['status'])) {

			case 'P': 
				class_name = 'day-attendance present';
				break;
			case 'H':
				class_name = 'day-attendance holiday';
				break;
			default:
			    class_name = 'day-attendance absent';
			   // if (data[i]['status'] == 'L*') l_star_count++;
				break;
			}
		
			in_time  = data[i]['in_punch'] ? data[i]['in_punch'] : '00:00';
			out_time = data[i]['out_punch'] ? data[i]['out_punch'] : '00:00';

			events.push({ 
				title: data[i]['status'], 
				start: moment(data[i]['pdate']).format('YYYY-MM-DD'), 
				end: moment(data[i]['pdate']).format('YYYY-MM-DD'), 
				className: class_name,
				workedHours: data[i]['WorkHrs'],
				inTime: in_time,
				outTime: out_time
			});
		}
		// append the count of L* leaves
		//$lStarCount.text(l_star_count);
		return events;
	};

	_getCalendarData = function () {

		var 
			$calendar_parent = $calendar.parent(),
			$loading         = $calendar_parent.find('.loading');

		$calendar.fullCalendar({ //remove below key once demo is done.
			events: function(start, end, timezone, callback) {

				if ($loading.hasClass('hide')) {
					$loading.removeClass('hide');
				}
				
                //calling L* count
				callLeaveStarMethod((this.getDate().format('YYYY')));
				//end of L* count
				
				$.ajax({
					method: 'POST',
					url: page_name + '/GetCalendarData',
					contentType: 'application/json; charset=utf-8',
					data: JSON.stringify({today: this.getDate().format('DD/MM/YYYY') }),
					dataType: 'json',
					success: function (data) {

						var results = "",
						status      = data.d.status,
						events      = undefined;

						if (status === 'error') {
							SAXAlert.showAlert({'message': data.d.return_data, 'type': 'error'});
						} else {

							results = JSON.parse( data.d.return_data );
							events  = _processCalendarData( results ); 
							_renderWorkedHoursGraph( results );
							callback( events );
						}

						$calendar.parent().find('.loading').addClass('hide');
					}
				});

		    },
			'header': {'left': 'prev', 'center': 'title', 'right': 'today, next'},
			'contentHeight': 400,
			eventRender: function(event, element) {
				
				var content = '<table class=\'table\'>' +
									'<tbody>' +
										'<tr>' +
											'<td><strong>Date</strong></td>' +
											'<td>' + moment(event.start).format('DD-MMM-YYYY') + '</td>' +
										'</tr>' +
										'<tr>' +
											'<td><strong>Hours Worked</strong></td>' +
											'<td>' + event.workedHours + '</td>' +
										'</tr>' +
										'<tr>' +
											'<td><strong>In Time</strong></td>' +
											'<td>' + event.inTime + '</td>' +
										'</tr>' +
										'<tr>' +
											'<td><strong>Out Time</strong></td>' +
											'<td>' + event.outTime + '</td>' +
										'</tr>' +
									'</tbody>' +
								'</table>';

				element.context.innerHTML = '<div class="fc-content" tabindex="0" role="button" data-trigger="focus" data-toggle="popover" data-container="body" title="Attendance Details" data-content="' + content + '" data-html="true"><span class="fa fa-circle"></span> ' + event.title + '</div>';
		    },
		    eventClick: function() {
		    	// This is the parent element to .fc-content. This is set by FullCalendar when calling this method.
		    	var popover_element = $(this).find('.fc-content');
		    	// Manually forcing the popover using the Bootstrap API
		    	$(popover_element).popover('toggle');
		    }
		});
		
	};

	_processLeavesData = function (data, additional) {

		var 
			$leaves_table         = $leaves_balance.find('table');
			$leaves_table_parent  = $leaves_balance.parent(),
			$holiday_table        = $holiday_listing.find('table'),
			$holiday_table_parent = $holiday_listing.parent(),
			results               = '',
			leaves_data           = undefined,
			leaves_data_length    = 0,
			holiday_data          = undefined,
			holiday_data_length   = 0,
			holiday_table_HTML    = '',
			leaves_table_HTML     = '';

		if (data.status === "success") {

			results = JSON.parse( data.return_data );

			leaves_data        = results['leaves_balance'];
			leaves_data_length = leaves_data.length;

			holiday_data        = results['leaves_listing'];
			holiday_data_length = holiday_data.length;

			for ( i = 0; i < holiday_data_length; i += 1 ) {

				holiday_table_HTML += "<tr>" +
										"<td>" + holiday_data[i]['HolidayName'] + "</td>" +
										"<td>" + moment(holiday_data[i]['HolidayDate']).format('DD-MMM-YYYY') + "</td>" +
									"</tr>";
			}

			$holiday_table.detach();
			$holiday_table.append(holiday_table_HTML);
			$holiday_table_parent.append($holiday_table);
			$holiday_table_parent.find('.loading').addClass('hide');

			for ( i = 0; i < leaves_data_length; i += 1 ) {

				leaves_table_HTML += "<tr>" +
										"<td>" + leaves_data[i]['Leavetype'] + "</td>" +
										"<td>" + leaves_data[i]['Leaves_applied'] + "</td>" +
										"<td>" + leaves_data[i]['Leave_balance'] + "</td>" +
										"<td>" + leaves_data[i]['CarryForwardLeave'] + "</td>" +
									"</tr>";
			}

			$leaves_table.detach();
			$leaves_table.append(leaves_table_HTML);
			$leaves_table_parent.append($leaves_table);
			$leaves_table_parent.find('.loading').addClass('hide');

		} else {
			SAXAlert.showAlert({type: 'error', message: data.return_data});
		}
	};

	_getLeavesData = function () {

		var ajax_options = {};

		ajax_options = {
			url: page_name + '/GetLeaveDetails',
			data: {},
			callback: _processLeavesData,
			additional: {}
		}

		SAXHTTP.ajax( ajax_options );
	}; 

	_renderEmployeeDetails = function () {

		var user_details = sessvars.TAMS_ENV.user_details;
		$('#homePageUsername').text(user_details['employee_name']);
		$('#homePageDepartment').text(user_details['department_name']);
		$('#homePageDesignation').text(user_details['designation_name']);
		
		if (user_details['display_picture'] != '') $('#homePageDP').attr('src', user_details['display_picture']);
	};

	_createEmployeeDashboard = function () {

		var $calendar = $calendar;

		$employee_dashboard.removeClass('hide');

		_getCalendarData();
		_getLeavesData();
		_renderEmployeeDetails();
	};

	function callLeaveStarMethod(year) {

	    $.ajax({
	        type: "POST",
	        url: 'home.aspx' + '/GetLStarCount',
	        data: '{year: "' + year + '" }',
	        contentType: "application/json; charset=utf-8",
	        success: function(data) {

	            $lStarCount.text(data.d);
	        },
	        failure: function(response) {
	            //alert(response.d);
	        }
	    });
	}

	/******************************************************************************************************************/

	_renderEmployeeStrengthGraph = function (results) {

		var graph_data            = [],
		graph_options             = [],
		number_absent             = +results.absent,
		number_present            = +results.present,
		ctx                       = undefined,
		my_chart                  = undefined,
		$employee_strength_parent = $employee_strength_graph.parent(),
		$employee_strength_canvas = $employee_strength_graph.find('canvas'),
		$loading                  = $employee_strength_parent.find('.loading');

		graph_data = [
			{
				value    : number_absent,
				highlight: "#e74c3c",
				color    : "#c0392b",
				label    : "ABSENT"
			},
			{
				value    : number_present,
				highlight: "#2ecc71",
				color    : "#27ae60",
				label    : "PRESENT"
			}
		];

		graph_options = {
			percentageInnerCutout: 60
		};

		$present_count.html( "<strong>" + number_present + "</strong>" );
		$absent_count.html( "<strong>" + number_absent + "</strong>" );

		ctx      = $employee_strength_canvas.get(0).getContext('2d');
		my_chart = new Chart(ctx).Doughnut(graph_data, graph_options);

		$loading.addClass('hide');

	};

	_processEmployeeStrengthData = function (data, additional) {

		var error_messag_options = {
			message: 'An error occurred while performing this operation. Please try again. If the error persists, please contact Support.',
			type: 'error'
		}, 
		$employee_strength_parent = $employee_strength_graph.parent(),
		$loading                  = $employee_strength_parent.find('.loading'),
		$no_data				  = $employee_strength_parent.find('.no-data'),
		results					  = JSON.parse( data.return_data );

		if (data.status === 'success') {

			if ( +results.absent === 0 && +results.present === 0) {
				$employee_strength_graph.addClass('hide');
				$no_data.removeClass('hide');
				$loading.addClass('hide');

			} else {
				_renderEmployeeStrengthGraph(results);
			}
		} else {

			SAXAlert.showAlert(error_message_options);
		}

	};

	_getEmployeeStrength = function () { 

		var ajax_options = {};

		ajax_options = {
			url: page_name + '/GetEmployeeStrength',
			data: {},
			callback: _processEmployeeStrengthData,
			additional: {}
		};

		SAXHTTP.ajax( ajax_options );
	};

	_createAdminDashboard = function () {

		$admin_dashboard.removeClass('hide');
		_getEmployeeStrength();
	};

	/******************************************************************************************************************/

	_getCurrentUser = function () {
		return sessvars.TAMS_ENV.user_details.user_name;
	};

	main = function () {

		var user_name = _getCurrentUser();
		if (user_name === 'admin') {
			_createAdminDashboard();
		}
		else {
			_createEmployeeDashboard();
		}
	};

	return {
		'main': main
	};

})(jQuery, window, document);

$(function() {
	HomePage.main();
});
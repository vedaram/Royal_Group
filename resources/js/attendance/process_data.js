$(function () {

	var ProcessData = (function ($, w, d) {

		var 
			form_elements = {
				result: $("#result")
			},
			dialogs = {
				result: $("#resultDialog")
			},
			buttons = {
				process: $("#processButton")
			};

		function processData() {

            SAXLoader.show();
            buttons.process.button("loading");

            // clear the result box
            form_elements.result.val("");

            SAXHTTP.AJAX(
                    "process_data.aspx/ProcessData",
                    {}
                )
                .done(function (data) {
                	dialogs.result.modal("show");
                	form_elements.result.val(data.d.return_data);
                })
                .fail(function () {
                    SAXAlert.show({type: "error", message: "An error occurred while performing this operation. Please try again."});
                })
                .always(function () {
                    SAXLoader.close();
                    buttons.process.button("reset");
                });
		}

		return {
			process: processData
		}

	}) (jQuery, window, document);

    var ProcessDataListView = (function ($, w, d) {

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

        function loadMoreData() {

        	SAXLoader.show();

        	page_number += 1;

			ProcessDataListView.get()
					.done(ProcessDataListView.render)
					.fail(function () { 
						SAXAlert.show({type: "error", message: "An error occurred while loading this page. Please try again."});
					})
					.always(function () {
						SAXLoader.close();
					});
        }

        /* rendering functions */
            function _getHTML(data, data_length) {

                var  
                    table_HTML = "", 
                    counter = 0;

                for (counter = 0; counter < data_length; counter += 1) {

                    table_HTML += '<tr>' +
                            '<td>' + data[counter]['card_number'] + '</td>' +
                            '<td>' + data[counter]["employee_code"] + '</td>' +
                            '<td>' + moment(data[counter]['Punch_time']).format('DD-MM-YYYY HH:mm:ss') + '</td>' +
                            '<td>' + data[counter]["device_id"] + '</td>' +
                        '</tr>' ;
                }

                return table_HTML;
            }

        function renderData(data) { 
            
            var 
                data = JSON.parse(data.d.return_data),
                data_length = data.length,
                table_body;

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
                list_elements.message.append("<h3>No data found</h3>");
                // hdie the table view
                list_elements.listview.hide();
            }
        }

        function getUnprocessedData() {
            return SAXHTTP.AJAX(
                    "process_data.aspx/GetUnprocessedData",
                    {pageNumber: page_number}
                );
        }

        return {
            get: getUnprocessedData,
            render: renderData,
            more: loadMoreData
        };

    }) (jQuery, window, document);


	var ProcessDataMasterView = (function ($, w, d) {

        var 
            button_events = {
                "process": ProcessData.process,
                "process/more": ProcessDataListView.more
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

	ProcessDataMasterView.init();

	ProcessDataListView.get()
		.done(ProcessDataListView.render)
		.fail(function () { 
			SAXAlert.show({type: "error", message: "An error occurred while loading this page. Please try again."});
		})
		.always(function () {
			SAXLoader.close();
		});

});
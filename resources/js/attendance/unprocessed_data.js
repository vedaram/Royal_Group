$(function () {

    var UnprocessData = (function ($, w, d) {

        function reprocessData() {

            var 
                selected_employees;

            SAXLoader.show();

            SAXHTTP.AJAX(
                    "unprocessed_data.aspx/DoReprocess",
                    {}
                )
                .done(function (data) {
                    SAXAlert.show({type: data.d.status, message: data.d.return_data});
                })
                .fail(function () {
                    SAXAlert.show({type: "error", message: "An error occurred while performing this operation. Please try again."});
                })
                .always(function () {
                    SAXLoader.close();
                });
        }

        return {
            reprocess: reprocessData
        };

    }) (jQuery, window, document);

    var UnprocessedDataListView = (function ($, w, d) {

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

            UnprocessedDataListView.get()
                    .done(UnprocessedDataListView.render)
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

                    employee_code = data[counter]['employee_code'] == null ? '' : data[counter]['employee_code'];

                    table_HTML += '<tr id="' + data[counter]['row'] + '" >' +
                            '<td>' + data[counter]['card_number'] + '</td>' +
                            '<td>' + employee_code + '</td>' +
                            '<td>' + moment(data[counter]['punch_time']).format('DD-MM-YYYY HH:mm:ss') + '</td>' +
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
                    "unprocessed_data.aspx/GetUnprocessedData",
                    {page_number: page_number}
                );
        }

        return {
            get: getUnprocessedData,
            render: renderData,
            more: loadMoreData
        };

    }) (jQuery, window, document);
    
    var UnprocessedDataMasterView = (function ($, w, d) {

        var 
            button_events = {
                "reprocess": UnprocessData.reprocess,
                "reprocess/more": UnprocessedDataListView.more
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

    UnprocessedDataMasterView.init();

    UnprocessedDataListView.get()
        .done(UnprocessedDataListView.render)
        .fail(function () {
            SAXAlert.show({type: "error", message: "An error occurred while loading page data. Please try again."});
        })
        .always(function () {
            SAXLoader.close();
        });
});
$(function() {


    /************************************************************************************************************************************************/

    var RamadanHistoryListView = (function($, w, d) {

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

        list_elements = {
            table: $('#dataTable'),
            listview: $('#listView'),
            message: $("#noData")
        };

        function isFilter() {
            return is_filter;
        }

        function resetFilters() {
            is_filter = false;
            page_number = 1;

            forms.filter[0].reset();
            SAXLoader.show();

            list_elements.table.find('tbody').empty();

            getRamadanHistoryData()
            .done(render)
            .fail(function() {
                SAXLoader.show({ type: "error", message: "An error occurred while loading data. Please try again." })
            })
            .always(function() {
                SAXLoader.close();
                dialogs.filter.slideToggle();
            });
        }

        function _validateFilters() {

            var data = SAXForms.get(forms.filter);

            if (data.filter_CompanyCode == "select") {
                SAXAlert.show({ 'type': 'error', 'message': 'Please select a Company.' });
                return false;
            }

            if (data.filter_year == "") {
                SAXAlert.show({ 'type': 'error', 'message': 'Please enter a Year.' });
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

                getRamadanHistoryData()
                .done(render)
                .fail(function() {
                    SAXLoader.show({ type: "error", message: "An error occurred while loading data. Please try again." })
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

            getRamadanHistoryData()
                .done(render)
                .fail(function() {
                    SAXAlert.show({ type: "error", message: "An error occurred while loading data. Please try again." })
                })
                .always(function() {
                    SAXLoader.close();
                    buttons.pagination.button("reset");
                });
        }

        /* rendering functions */
        function _getHTML(data, data_length) {
            var table_HTML = "", counter = 0;

            for (counter = 0; counter < data_length; counter += 1) {
                table_HTML += '<tr>' +
                                '<td>' + data[counter]['company_code'] + '</td>' +
                                '<td>' + data[counter]['year'] + '</td>' +
                                '<td>' + moment(data[counter]['from_date']).format("DD-MMM-YYYY") + '</td>' +
                                '<td>' + moment(data[counter]['to_date']).format("DD-MMM-YYYY") + '</td>' +
                            '</tr>';
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
                table_body.children().length < page_number * 30 ? buttons.pagination.hide() : buttons.pagination.show();

                // add the data to the collection also
                //RamadanHistoryListView.getCollection().set(data);
            }
            else {
                list_elements.message.append("<h3>No data found</h3>");
                // hdie the table view
                list_elements.listview.hide();
            }
        }

        /* get RamadanHistory data function */
        function getRamadanHistoryData() {

            var ramadanHistoryDataDeferred = $.Deferred();

            SAXHTTP.AJAX(
                "ramadan_history.aspx/GetRamadanHistoryData", {}
                //{ page_number: page_number, is_filter: is_filter, filters: JSON.stringify(SAXForms.get(forms.filter)) }
            )
            .done(function(data) {
                var data = JSON.parse(data.d.return_data);
                ramadanHistoryDataDeferred.resolve(data, data.length);

            }).fail(function() {
                ramadanHistoryDataDeferred.reject();
            });

            return ramadanHistoryDataDeferred.promise();
        };

        return {
            get: getRamadanHistoryData,
            render: render,
            more: loadMoreData,
            filter: filterData,
            reset: resetFilters,
            isFilter: isFilter
        };

        return {
            init: initialize,
            getCollection: getCollection
        };

    })(jQuery, window, document);

    // INITIAL PAGE LOAD
    SAXLoader.show();

    // get company data
    RamadanHistoryListView.get()
        .done(RamadanHistoryListView.render)
        .fail(function() {
            SAXAlert.show({ type: "error", message: "An error occurred while loading data. Please try again." })
        })
        .always(SAXLoader.close);


});
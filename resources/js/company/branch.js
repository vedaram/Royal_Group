$(function () {


    var HolidayGroup = (function ($, w, d) {

        var
			$holiday_group_code = $("#holiday_group_code");

        function _groupByKey(data) {
            var
                result = {},
                counter = 0,
                data_length = data.length;

            for (counter = 0; counter < data_length; counter += 1) {
                result[data[counter]["holidaycode"]] = data[counter]["holidaycode"];
            }

            return result;
        }

        function renderDropdown(data) {

            var select_HTML = "",
	        	data = JSON.parse(data.d.return_data),

	        	all_holiday_groups = data.all_holiday_groups,
	        	all_holiday_groups_length = all_holiday_groups.length,            
	        	selected_holiday_groups = data.selected_holiday_groups,
	            counter = 0,
	            selected = "",
	            check_selected = false,
	            $element = $holiday_group_code,
	            $parent = $element.parent();

            if (selected_holiday_groups != undefined && selected_holiday_groups.length > 0) {
                check_selected = true;
                selected_holiday_groups = _groupByKey(selected_holiday_groups);
            }


            for (counter = 0; counter < all_holiday_groups_length; counter += 1) {

                if (check_selected)
                    selected = selected_holiday_groups[all_holiday_groups[counter]["holiday_group_code"]] != undefined ? "selected" : "";
                select_HTML += '<option value="' + all_holiday_groups[counter]['holiday_group_code'] + '" ' + selected + '>' + all_holiday_groups[counter]['holiday_group_name'] + '</option>';
            }

            $holiday_group_code.empty();
            $holiday_group_code.append(select_HTML);
        }
    
        function getHolidayGroupData(company_code, branch_code) {

            $holiday_group_code.append("<option value=\"select\">Loading ...</option>");

            return SAXHTTP.AJAX(
					"branch.aspx/GetHolidayGroupData", { company_code: company_code, branch_code: branch_code }
				)
				.fail(function () {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Holiday Groups data. Please try again." })
				});
        }

        return {
            get: getHolidayGroupData,
            render: renderDropdown
        };

    })(jQuery, window, document);
    /*------------------------------------*/

    var HrInfo = (function ($, w, d) {

        var
			$Hr_Incharge = $("#Hr_Incharge");
        function _groupByKey(data) {
            var
                result = {},
                counter = 0,
                data_length = data.length;

            for (counter = 0; counter < data_length; counter += 1) {
                result[data[counter]["HrIncharge"]] = data[counter]["HrIncharge"];
            }

            return result;
        }
        
        function renderDropdown(data) {

            var 
                select_HTML = "",
	        	data = JSON.parse(data.d.return_data),
	        	HrIncharge = data.HrIncharge,
	        	HrIncharge_length = HrIncharge.length,
                selectedhr = data.selectedhr,
                counter = 0,
	            selected = "",
	            check_selected = false,
	            $element = $Hr_Incharge,
	            $parent = $element.parent();
	        // resetting the value to be safe
	        //$('#Chk1').prop('checked', false);

            /*   if (HrIncharge != undefined && HrIncharge_length > 0) {
                   check_selected = true;
                   selected_holiday_groups = _groupByKey(HrIncharge);
               }*/
            if (selectedhr != undefined && selectedhr.length > 0) {
                check_selected = true;
                selectedhr = _groupByKey(selectedhr);
            }
              

            for (counter = 0; counter < HrIncharge_length; counter += 1) {    
                    if (check_selected)
                        selected = selectedhr[HrIncharge[counter]["Emp_Code"]] != undefined ? "selected" : "";
                    
                    if (selected == "selected") {
                        $('#Chk1').prop('checked', true);
                    }
                    select_HTML += '<option value="' + HrIncharge[counter]['Emp_Code'] + '" ' + selected + '>' + HrIncharge[counter]['Emp_name'] + '</option>';
            }

            $Hr_Incharge.empty();
            $Hr_Incharge.append(select_HTML);
        }



        function GetHrInfo(com_code, branch_code) {

            $Hr_Incharge.append("<option value=\"select\">Loading ...</option>");

            return SAXHTTP.AJAX(
                    "branch.aspx/GetHrInfo", { company_code: com_code, branch_code: branch_code }
                )
                .fail(function () {
                    SAXAlert.show({ type: "error", message: "An error occurred while loading Hr Info. Please try again." })
                });
        }

        return {
            get: GetHrInfo,
            render: renderDropdown
        };
    })(jQuery, window, document);


    var AlternativeHrInfo = (function ($, w, d) {

        var
        $Alternative_Hr_Incharge = $("#Alternative_Hr_Incharge");

        function _groupByKey(data) {
            var
                result = {},
                counter = 0,
                data_length = data.length;               

            for (counter = 0; counter < data_length; counter += 1) {
                result[data[counter]["AlternativeHrIncharge"]] = data[counter]["AlternativeHrIncharge"];
            }

            return result;
        }


        function renderDropdown(data) {

            var select_HTML = "",
                data = JSON.parse(data.d.return_data),
                AlternativeHrIncharge = data.AlternativeHrIncharge,
                AlternativeHrIncharge_length = AlternativeHrIncharge.length,
                selectedAlthr = data.selectedAlthr,
                counter = 0,
                selected = "",
                check_selected = false,
                $element = $Alternative_Hr_Incharge,
                $parent = $element.parent();
            // resetting the value to be on the safe side
            //$('#Chk2').prop('checked', false);

            if (selectedAlthr != undefined && selectedAlthr.length > 0) {
                check_selected = true;
                selectedAlthr = _groupByKey(selectedAlthr);
            }

            for (counter = 0; counter < AlternativeHrIncharge_length; counter += 1) {
                    if (check_selected)
                        selected = selectedAlthr[AlternativeHrIncharge[counter]["Emp_Code"]] != undefined ? "selected" : "";
                        
                    if (selected === "selected") {
                        $('#Chk2').prop('checked', true);
                    }
                    select_HTML += '<option value="' + AlternativeHrIncharge[counter]['Emp_Code'] + '" ' + selected + '>' + AlternativeHrIncharge[counter]['Emp_name'] + '</option>';
                
            }
            $Alternative_Hr_Incharge.empty();

            $Alternative_Hr_Incharge.append(select_HTML);
        }


        function GetAlternativeHrInfo(com_code, branch_code) {


            $Alternative_Hr_Incharge.append("<option value=\"select\">Loading ...</option>");

            return SAXHTTP.AJAX(
                    "branch.aspx/GetAlternativeHrInfo", { company_code: com_code, branch_code: branch_code }
                )
                .fail(function () {
                    SAXAlert.show({ type: "error", message: "An error occurred while loading Alternative Hr Info. Please try again." })
                });
        }

        return {
            get: GetAlternativeHrInfo,
            render: renderDropdown
        };

    })(jQuery, window, document);


    /************************************************************************************************************************************************/

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
                "branch.aspx/getCompanyData", {}
            )
            .done(_renderDropdown)
            .fail(function () {
                SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." })
            });
        };

        return {
            get: getCompanyData
        };

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    var BranchExport = (function ($, w, d) {

        var
            buttons = {
                export_button: $('#exportButton')
            },
            forms = {
                filter: $('#filterForm')
            }

        function _processExport(data) {
            var status = data.d.status;
            switch (status) {
                case 'success':
                    SAXAlert.showAlertBox({ 'type': status, 'url': SAXUtils.getApplicationURL() + data.d.return_data });
                    break;
                case 'info':
                    SAXAlert.show({ 'type': status, 'message': data.return_data });
                    break;
            };
        }

        function doExport() {

            var is_filter, filters = {};

            buttons.export_button.button("loading");

            is_filter = BranchListView.isFilter();

            if (is_filter)
                filters = SAXForms.get(forms.filter)

            SAXHTTP.AJAX(
                "branch.aspx/DoExport", { is_filter: is_filter, filters: JSON.stringify(filters) }
            )
            .done(_processExport)
            .fail(function () {
                SAXAlert.show({ type: "error", message: "An error occurred while exporting Branch data. Please try again." });
            })
            .always(function () { buttons.export_button.button("reset"); });
        }

        return {
            export: doExport
        };

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    var BranchView = (function ($, w, d) {

        var buttons = {
            save: $('#saveButton'),
            delete: $('#deleteButton')
        },
        dialogs = {
            save: $('#saveDialog'),
            delete: $('#deleteDialog')
        },
        forms = {
            save: $('#saveForm')
        },


         $branch_code = $('#branch_code');

        function GetBranchCode() {

          //  SAXLoader.show();
            var 
            $company_code = $('#company_code');
           

            SAXHTTP.AJAX(
                "branch.aspx/GenerateBranchCode",
                { company_code: $company_code.val() }
            )
            .done(function (data) {
                var branch_code = data.d.return_data;
                $branch_code.val(branch_code);

            })
            .fail(function () {
                SAXAlert.show({ type: "error", message: "An error occurred while generating the branch Group Code. Please try again." });
            })
            .always(SAXLoader.close);
        }


        $holiday_group_code = $("#holiday_group_code");
        $hrincharge = $("#Hr_Incharge");
        $Alternativehr = $("#Alternative_Hr_Incharge");


        function _request(url, data, success, error, additional) {

            var requestDeferred = $.Deferred();

            SAXHTTP.AJAX(url, data)
            .done(function (data) {
                if (data.d.status == "success") {
                    requestDeferred.resolve([additional.data], 1);
                }
                else {
                    requestDeferred.reject();
                }
                SAXAlert.show({ type: data.d.status, message: data.d.return_data });
            })
            .fail(function () {
                requestDeferred.reject();
                SAXAlert.show({ type: "error", message: error });
            })

            return requestDeferred.promise();
        }

        function _validate(data) {


            if (data.company_code === "select") {
                SAXAlert.show({ type: "error", message: "Please select a Company" });
                return false;
            }

            if (data.branch_code === "") {
                SAXAlert.show({ type: "error", message: "Please enter a Branch Code" });
                return false;
            }

            if (data.branch_code != "" && !SAXValidation.code(data.branch_code)) {
                SAXAlert.show({ type: "error", message: "Please enter a valid Branch Code" });
                return false;
            }

            if (data.branch_name === "") {
                SAXAlert.show({ type: "error", message: "Please enter a Branch Name" });
                return false;
            }

            if (data.branch_name != "" && !SAXValidation.name(data.branch_name)) {
                SAXAlert.show({ type: "error", message: "Please enter a valid Branch Name" });
                return false;
            }

            if (data["phone_number"] != "" && !SAXValidation.isNumber(data["phone_number"])) {
                SAXAlert.show({ type: "error", message: "Please enter a numeric value for Phone Number" });
                return false;
            }

            if (data["fax_number"] != "" && !SAXValidation.isNumber(data["fax_number"])) {
                SAXAlert.show({ type: "error", message: "Please enter a numeric value for Fax Number" });
                return false;
            }

            if (data["email_address"] !== '' && !SAXValidation.email(data["email_address"])) {
                SAXAlert.show({ 'type': 'error', 'message': 'Please enter a valid Email Address' });
                return false;
            }
            //Code added

            if (data.Alternative_Hr_Incharge  != "" &&  data.Hr_Incharge!="" ) {
                SAXAlert.show({ 'type': 'error', 'message': 'Please Select either HR or Alternative HR  for  a Branch' });
                return false;
            }
            //

            return true;
        }

        function deleteBranch(event) {
            var
                branch_code = $(event.target).data("branch-id"),
                data = BranchMasterView.getCollection().get(branch_code).toJSON(),
                success = "Branch deleted successfully!",
                error = "An error occurred while deleting the Branch. Please try again.";

            // disable the button to avoid multiple clicks
            buttons.delete.button("loading");

            $.when(_request("branch.aspx/DeleteBranch", { current: JSON.stringify(data) }, success, error, { data: data }))
                .then(BranchListView.delete)
                .done(function () { dialogs.delete.modal("hide"); })
                .always(function () { buttons.delete.button("reset"); });
        }




        function _getSelectedHolidayGroups() {
            var selected_holidays_groups = [];

            $.each($holiday_group_code.find("option:selected"), function () {
                selected_holidays_groups.push($(this).val());
            });

            return selected_holidays_groups;
        }
        function _getSelectedHr() {

            var selected_holidays_groups = [];

            $.each($hrincharge.find("option:selected"), function () {
                selected_holidays_groups.push($(this).val());
            });

            return selected_holidays_groups;
        }



        function editBranch(event) {
            var
                form_data = SAXForms.get(forms.save),
                branch_code = $(event.target).data("branch-id"),
                previous = BranchMasterView.getCollection().get(branch_code),
                success = "Branch details edited successfully!",
                error = "An error occurred while saving Branch details. Please try again.";

            if (_validate(form_data)) {
                // disable save button to avoid multiple clicks.
                holiday_groups = _getSelectedHolidayGroups();


                if (holiday_groups.length == 0) {
                    SAXAlert.show({ type: "error", message: "Please select atleast one Holiday Group for the Branch" });
                    return false;
                }
                buttons.save.button("loading");

                // adding company name to the form data
                form_data["company_name"] = $("#company_code option:selected").text();

                
              
                _request("branch.aspx/EditBranch", { current: JSON.stringify(form_data), previous: JSON.stringify(previous), holiday_groups: JSON.stringify(holiday_groups) }, success, error, { data: form_data })
                    .done(BranchListView.delete)
                    .done(BranchListView.render)
                    .done(function () { dialogs.save.modal("hide"); })
                    .always(function () { buttons.save.button("reset") });
            }
        }

        function addBranch() {
            
            var
                form_data = SAXForms.get(forms.save),
                success = "Branch added successfully!",
                error = "An error occurred while adding a new Branch . Please try again.";

            if (_validate(form_data)) {
                // disable the button to avoid multiple clicks

                holiday_groups = _getSelectedHolidayGroups();

                if (holiday_groups.length == 0) {
                    SAXAlert.show({ type: "error", message: "Please select the Holiday Group for the Branch" });
                    return false;
                }
                buttons.save.button("loading");

                // adding company name to the form data
                form_data["company_name"] = $("#company_code option:selected").text();

               

                $.when(_request("branch.aspx/AddBranch", { current: JSON.stringify(form_data), holiday_groups: JSON.stringify(holiday_groups) }, success, error, { data: form_data }))
                    .then(BranchListView.render)
                    .done(function () { dialogs.save.modal("hide"); })
                    .always(function () { buttons.save.button("reset") });
            }
        }

        return {
            add: addBranch,
            edit: editBranch,
            delete: deleteBranch,
            GetBranchCode: GetBranchCode
            //GetHrInfo: GetHrInfo,
            //GetAlternativeHrInfo: GetAlternativeHrInfo
        };

    })(jQuery, window, document);

    /************************************************************************************************************************************************/


    /****************************************************************************************/

    var BranchListView = (function ($, w, d) {

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
                listview: $("#listView")
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
            // var  companyCodeVal = $('#company_code').val();
            getBranchData()
            .done(render)
            .fail(function () {
                SAXLoader.show({ type: "error", message: "An error occurred while loading data. Please try again." })
            })
            .always(function () {
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

            if (data.filter_by == 0 || (data.filter_keyword != "" && data.filter_by == 0)) {
                SAXAlert.show({ 'type': 'error', 'message': 'Please select a Filter By option.' });
                return false;
            }

            if (data.filter_keyword == "" || (data.filter_by != 0 && data.keyword == "")) {
                SAXAlert.show({ 'type': 'error', 'message': 'Please enter a keyword.' });
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

                getBranchData()
                .done(render)
                .fail(function () {
                    SAXLoader.show({ type: "error", message: "An error occurred while loading data. Please try again." })
                })
                .always(function () {
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

            getBranchData()
                .done(render)
                .fail(function () {
                    SAXAlert.show({ type: "error", message: "An error occurred while loading data. Please try again." })
                })
                .always(function () {
                    SAXLoader.close();
                    buttons.pagination.button("reset");
                });
        }

        function removeRow(data, data_length) {

            // remove the row from the table
            list_elements.table.find("tr#" + data[0]["branch_code"]).remove();

            // since the row is being remove, we are also going to remove it from the collection.
            BranchMasterView.getCollection().unset(data[0]["branch_code"]);
        }

        /* rendering functions */
        function _getHTML(data, data_length) {
            var table_HTML = "", counter = 0;

            for (counter = 0; counter < data_length; counter += 1) {
                table_HTML += '<tr id="' + data[counter]['branch_code'] + '" >' +
                            '<td>' + data[counter]['branch_code'] + '</td>' +
                            '<td>' + data[counter]['branch_name'] + '</td>' +
                            '<td>' + data[counter]['company_name'] + '</td>' +
                            '<td>' +
                                '<span class="fa fa-pencil action-icon" data-toggle="modal" data-target="#saveDialog" data-role="branch/edit" data-id="' + data[counter]["branch_code"] + '"></span>' +
                                '<span class="fa fa-trash-o action-icon" data-toggle="modal" data-target="#deleteDialog" data-role="branch/delete" data-id="' + data[counter]["branch_code"] + '"></span>' +
                            '</td>' +
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
                BranchMasterView.getCollection().set(data);
            }
            else {
                list_elements.message.append("<h3>No Branch data found</h3>");
                // hdie the table view
                list_elements.listview.hide();
            }
        }

        function getBranchData() {
            var deferred = $.Deferred();

            SAXHTTP.AJAX(
                "branch.aspx/getBranchData",
                { page_number: page_number, is_filter: is_filter, filters: JSON.stringify(SAXForms.get(forms.filter)) }
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
            get: getBranchData,
            render: render,
            delete: removeRow,
            more: loadMoreData,
            filter: filterData,
            reset: resetFilters,
            isFilter: isFilter
        };

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    var BranchMasterView = (function ($, w, d) {
        debugger;
        var
            dialogs = {
                "save": $("#saveDialog"),
                "delete": $("#deleteDialog"),
                "filters": $('#filters')
            },
            buttons = {
                save: $('#saveButton'),
                delete: $('#deleteButton')
            },
            button_events = {
                "branch/add": BranchView.add,
                "branch/edit": BranchView.edit,
                "branch/delete": BranchView.delete,
                "branch/more": BranchListView.more,
                "branch/export": BranchExport.export,
                "filters/data": BranchListView.filter,
                "filters/reset": BranchListView.reset,
                "filters/toggle": function () { dialogs.filters.slideToggle() }
            },
            forms = {
                save: $('#saveForm'),
            },
            $company_code = $('#company_code'),
            $holiday_group_code = $("#holiday_group_code"),
            fields = ["company_code", "branch_code"],
            model_class, collection_class, collection;

        /* buttons */
        function _buttonHandler(event) {
            var role = $(event.target).data('role');
            button_events[role].call(this, event);
        }

        function _initButtons() {
            $(document).on("click", "[data-control=\"button\"]", _buttonHandler);
        }

        /* dialogs */
        function _setEditButton(event) {

            var branch_code = $(event.relatedTarget).data("id");

            // set data for edit button
            buttons.save.data("role", "branch/edit");
            buttons.save.data("branch-id", branch_code);

            // disable fields as required
            SAXForms.disable(fields);
        }
        function _setModalData(event) {
            var
                $target = $(event.relatedTarget),
                role = $target.data("role"),
                branch_code = $target.data("id"),
                data = {};

            if (role == "branch/edit") {
                debugger;
                SAXLoader.show();

                data = BranchMasterView.getCollection().get(branch_code).toJSON();

                

                HolidayGroup.get(data.company_code, branch_code)
                    .done(function () {
                        // fill save form data for the selected company                                              
                     //SAXForms.set(forms.save, data);
                      //  SAXLoader.close();
                    })
                    .done(HolidayGroup.render);
                //code added
                HrInfo.get(data.company_code, branch_code).done(function () {

                //  SAXForms.set(forms.save, data);
                 //  SAXLoader.close();
                }).done(HrInfo.render);

                AlternativeHrInfo.get(data.company_code, branch_code).done(function () {
                  // SAXForms.set(forms.save, data);
                  //  SAXLoader.close();
                }).done(AlternativeHrInfo.render);

               // BranchView.GetBranchCode();
                SAXForms.set(forms.save, data);
                 SAXLoader.close();
                //
            }
        }
        function _setModalButton(event) {

            var role = $(event.relatedTarget).data('role');

            switch (role) {
                case "branch/add":
                    buttons.save.data("role", "branch/add");
                    break;
                case "branch/edit":
                    _setEditButton(event);
                    break;
                case "branch/delete":
                    buttons.delete.data("branch-id", $(event.relatedTarget).data("id"));
                    break;
            }
        }
        function _resetSaveModal(event) {
           
            $holiday_group_code.empty();
            //
           
            //code added
            $hrincharge.empty();
            $Alternativehr.empty();
            //
            forms.save[0].reset();
            SAXForms.enable(fields);
        }

        function _initDialogs() {
            // before the modal is shown to the user,
            // change the function of the save button to add or edit.
            dialogs.save.on("show.bs.modal", _setModalButton);

            dialogs.save.on("shown.bs.modal", _setModalData);

            // reset the form on modal close.
            dialogs.save.on("hidden.bs.modal", _resetSaveModal);

            dialogs.delete.on("show.bs.modal", _setModalButton);
        }

        /* modals */
        getCollection = function () {
            return collection;
        }
        _initModels = function () {
            model_class = SAXModel.extend({ 'idAttribute': "branch_code" });

            // define the collection class
            collection_class = SAXCollection.extend({ 'baseModel': model_class });

            // create an instance of the collection_class
            // passing an empty array as the default data
            collection = new collection_class([]);
        };

        function _initOther() {

            $company_code.change(function () {       
                //SAXLoader.show(); 
                HolidayGroup.get($(this).val(), "")
                    .done(HolidayGroup.render)              
                .done(SAXLoader.close)  
             
              // c();
              //  HrInfo.get($(this).val(), "").done(HrInfo.render).done(SAXLoader.close());
                // AlternativeHrInfo.get($(this).val(), "").done(AlternativeHrInfo.render).done(SAXLoader.close());                
                 BranchView.GetBranchCode();                            
             //  $('#Hr_Incharge').hide();
             //  $('#Alternative_Hr_Incharge').hide();
                // a();
            });

            $('#Chk1').change(function () {
                var 
                    companyCode = $('#company_code').val()
                    isChecked = $('#Chk1').is(':checked');
                    
                if (isChecked) {
                    $('#Chk2').prop('checked', false);                    
                    $('#Alternative_Hr_Incharge').val(null);
                } else {
                    $('#Hr_Incharge').val(null);
                    $('#Chk2').prop('checked', true);
                }
                
                if ($('#Hr_Incharge').children().length == 0) {
                        SAXLoader.show();
                        HrInfo.get(companyCode, "")
                            .done(HrInfo.render)
                            .done(SAXLoader.close());
                    }
            });
            
            $('#Chk2').change(function () {
                var 
                    companyCode = $('#company_code').val()
                    isChecked = $('#Chk2').is(':checked');
                    
                if (isChecked) {
                    $('#Chk1').prop('checked', false);                    
                    $('#Hr_Incharge').val(null);
                } else {
                    $('#Alternative_Hr_Incharge').val(null);
                    $('#Chk1').prop('checked', true);
                }
                
                if ($('#Alternative_Hr_Incharge').children().length == 0) {
                        SAXLoader.show();
                        AlternativeHrInfo.get(companyCode, "")
                            .done(AlternativeHrInfo.render)
                            .done(SAXLoader.close());
                    }
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

    })(jQuery, window, document);

    /************************************************************************************************************************************************/

    // INITIAL PAGE LOAD
    SAXLoader.show();

    // initialize page components
    BranchMasterView.init();

    // get company data
    BranchListView.get()
         .done(BranchListView.render)
         .done(Company.get)
         .fail(function () {
             SAXAlert.show({ type: "error", message: "An error occurred while loading data. Please try again." })
         })
         .always(SAXLoader.close);

});
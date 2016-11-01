var _doExport, _processExport;
var page_name = 'employee_data_transaction.aspx'
var $export_button = $('#exportButton');
_processExport = function (data, additional) {

    var
        status = data.status,
        type_of_export = additional.type_of_export;

    switch (status) {

        case 'success':
            SAXAlert.showAlertBox({ 'type': status, 'url': SAXUtils.getApplicationURL() + data.return_data });
            break;
        case 'info':
            SAXAlert.show({ 'type': status, 'message': data.return_data });
            break;
        case 'error':
            SAXAlert.show({ 'type': status, 'message': data.return_data });
            break;
    }

    $export_button.button('reset');
    SAXLoader.closeBlockingLoader();
};

_doExport = function (event) {

    var
           ajax_options = {},
           type_of_export = $(event.target).data('role');

    ajax_options = {
        'url': page_name + '/DoExport',
        'data': {},
        'callback': _processExport,
        'additional': {
            'type_of_export': type_of_export
        }
    };

    $export_button.button('loading');
    SAXLoader.showBlockingLoader();
    SAXHTTP.ajax(ajax_options);

};
var dialogs = {
    "save": $("#empTransaction")
};

var txnData = "";

var empcode="";

var shiftOptions = "";

function formatdate(newvalue) {
   //var x = document.getElementById("fname");
     //alert(newvalue.value);
     newvalue.value='09-sep-2016';
 //   x.value = x.value.toUpperCase();
   }
$(document).ready(function () {
   /*  $(".datepicker").Zebra_DatePicker({
         format: "d-M-Y",
         direction: true + 0
     }); */
     
     

});     






/*$( function() {
    $( "#datepicker" ).datepicker();
  } ); */

$("#filter_data").click(function () {

    $("#filters").slideToggle('slow');
});
$("#filteremployeebutton").click(function () {
   // alert('filter button clicked');
    // getCompanyDataForEmployee();
    getOtherData(employee_data.value);
    getEmployeeTransactionData(employee_data.value);
    //$("#filters").slideToggle('slow');
    // $('#empTransaction').hide();
    //dialogs.save.modal("hide");
    //$("#filter_employee").slideToggle('slow');
});

OtherData = (function ($, w, d) {

        var
	        $shift_code = $("#shift_select_data");
            $manager_id = $("#line_manager_drop_down");

        function _render($element, data, key, value, default_data, no_data) {
            var
				data_length = data.length,
	            select_HTML = '<option value="select">' + default_data + '</option>',
	            counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">' + no_data + '</option>';
            }
            $element.empty();
            $element.append(select_HTML);
        }
/*
        function getOtherData(employee_code) {
           
           alert("to get other data for "+employee_code);
           

            return SAXHTTP.AJAX(
					"/ui/transaction/employee/employee_data_transaction.aspx/GetEmployeeData",
					{ employee_code: employee_code }
				)
				.done(function (data) {

				    var results = JSON.parse(data.d.return_data);
				  //  alert(results);
				    _render($shift_code, results.shift, 'shift_code', 'shift_desc', 'Select Shift', 'No Shifts found');

				})
				.fail(function () {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Shift data. Please try again." });
				});
        }

        return {
            get: getOtherData 
        }; */

    })(jQuery, window, document);

var Employee_data_transaction = (function ($, w, d) {

  //  var shiftOptions='';
    
    var
		main, _initOther, saveEmployeeData,deleteTransaction,addTransaction,getOtherData,
		_saveManualPunch, _changeInpunch, _processSaveManualPunch,
		_doUpload, _processUpload, _processEmployeeData,
		_doImport, _processImport, _onemployeeidchange;

    var $saveEmployeeTransactionForm = $('#saveForm');

    var dialogs = {
        "save": $("#empTransaction")
    };

    var
		page_name = 'employee_data_transaction.aspx',
		page = { file_name: "" };

    function getOtherData(employee_code) {
           
          // alert("to get other data for ----- "+employee_code);
           

            return SAXHTTP.AJAX(
					"employee_data_transaction.aspx/GetEmployeeData",
					{ employee_code: employee_code }
				)
				.done(function (data) {
                  //     alert(JSON.stringify(data, null, 4));
				    var results = JSON.parse(data.d.return_data);
				    shiftOptions=results.shift;
				   // alert(results);
				    _render(results.shift, 'shift_code', 'shift_desc', 'Select Shift', 'No Shifts found');

				})
				.fail(function () {
				    SAXAlert.show({ type: "error", message: "An error occurred while loading Shift data. Please try again." });
				});
        }
/*
        return {
            get: getOtherData 
        };*/

    function _render(data, key, value, default_data, no_data) {
            var
				data_length = data.length,
				shiftOptions ='',
	            select_HTML = '<option value="select">' + default_data + '</option>',
	            counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">' + no_data + '</option>';
            }
            //$element.empty();
            
          //  alert('shift options are' + shiftOptions);
            
        }

    function getEmployeeTransactionData(employee_code, company_code, branch_code) {
        empcode = employee_code;
        $("#employee_id").text(employee_code);
        //    alert(company_code);
        var $branch_code = $('#filter_branch');
        return SAXHTTP.AJAX(
                "employee_data_transaction.aspx/GetEmployeeTransactionData",
                { employee_code: employee_code, company_code:company_code, branch_code:branch_code }
            )
            .done(function (data) {

                var results = JSON.parse(data.d.return_data);

                _renderforEmployee(results);
             //   _updateEmployeeTransactionData(results);

            })
            .fail(function () {
                SAXAlert.show({ type: "error", message: "An error occurred while loading Company data. Please try again." });
            });
    }


    function _validate(data) {
        var shift_from_date = data['shift_From_date'],
       shift_to_date = data['shift_To_date'], ot_from_date = data['OT_from_date'], ot_to_date = data['OT_To_date'],
       ramadan_from_date = data['ramadan_from_date'], ramadan_to_date = data['ramadan_to_date'], punchexceeption_from_date = data['punch_from_date'],
       punchexceeption_to_date = data['punch_to_date'], WH_day_from_date = data['WH_day_from_date'],
       WH_day_to_date = data['WH_day_to_date'],
       WH_week_from_date = data['WH_week_from_date'],
       WH_week_to_date = data['WH_week_to_date'],
       WH_month_from_date = data['WH_month_from_date'],
       WH_month_to_date = data['WH_month_to_date']

        if (data.employee_data == "") {
            SAXAlert.show({ type: "error", message: "Please enter a Employee Code." });
            return false;
        }

        

        /*cheking  any one checkbox checked or not
        if (data.shift_date_chkbox == '0' && data.OT_date == '0' && data.ramadan_date == '0' && data.punch_date == '0' && data.maternity_date == '0' && data.WH_day_date == '0' && data.WH_week_date == '0' && data.WH_month_date == '0' && data.termination_date == '0' && data.line_manager == '0') {
            SAXAlert.show({ type: "error", message: "Please select any one checkbox" });
            return false;
        } */
        return true;
    }

    
    deleteTransaction = function (e) {
      //  alert("delete >> " + e.currentTarget.id);
        var remId = e.currentTarget.id;
        var tdDiv = remId.split("_")[2];
        var txnId = remId.split("_")[0];
       
        switch (txnId) {
            case 'shift':
                if ($('#shift_statusflag_'+tdDiv).val()=='I') {
                    $("#shift_"+tdDiv).hide();
                    $('#shift_statusflag_'+tdDiv).val('X');
                   // txnData.shiftData.splice(tdDiv,1);
                } else {
                    $("#shift_" + tdDiv).hide();
                    $('#shift_statusflag_'+tdDiv).val('D');
                }
                break;
            case 'ot':
                if ($('#ot_statusflag_'+tdDiv).val()=='I') {
                    $("#ot_"+tdDiv).hide();
                    $('#ot_statusflag_'+tdDiv).val('X');
                   // txnData.otData.splice(tdDiv,1);
                } else {
                    $("#ot_" + tdDiv).hide();
                    $('#ot_statusflag_'+tdDiv).val('D');
                }
                break;
            case 'ramadan':
                if ($('#ramadan_statusflag_'+tdDiv).val()=='I') {
                    $("#ramadan_"+tdDiv).hide();
                    $('#ramadan_statusflag_'+tdDiv).val('X');
                   // txnData.ramadanData.splice(tdDiv,1);
                } else {
                    $("#ramadan_" + tdDiv).hide();
                    $('#ramadan_statusflag_'+tdDiv).val('D');
                }
                break;    
            case 'punchexception':
                if ($('#punchexception_statusflag_'+tdDiv).val()=='I') {
                    $("#punchexception_"+tdDiv).hide();
                    $('#punchexception_statusflag_'+tdDiv).val('X');
                   // txnData.punchexceptionData.splice(tdDiv,1);
                } else {
                    $("#punchexception_" + tdDiv).hide();
                    $('#punchexception_statusflag_'+tdDiv).val('D');
                }
                break;
                
            case 'workhourperday':
                if ($('#workhourperday_statusflag_'+tdDiv).val()=='I') {
                    $("#workhourperday_"+tdDiv).hide();
                    $('#workhourperday_statusflag_'+tdDiv).val('X');
                   // txnData.workhourperday.splice(tdDiv,1);
                } else {
                    $("#workhourperday_" + tdDiv).hide();
                    $('#workhourperday_statusflag_'+tdDiv).val('D');
                }
                break;
                
             case 'workhourperweek':
                if ($('#workhourperweek_statusflag_'+tdDiv).val()=='I') {
                    $("#workhourperweek_"+tdDiv).hide();
                    $('#workhourperweek_statusflag_'+tdDiv).val('X');
                   // txnData.workhourperweek.splice(tdDiv,1);
                } else {
                    $("#workhourperweek_" + tdDiv).hide();
                    $('#workhourperweek_statusflag_'+tdDiv).val('D');
                }
                break;
                
             case 'workhourpermonth':
                if ($('#workhourpermonth_statusflag_'+tdDiv).val()=='I') {
                    $("#workhourpermonth_"+tdDiv).hide();
                    $('#workhourpermonth_statusflag_'+tdDiv).val('X');
                   // txnData.workhourpermonth.splice(tdDiv,1);
                } else {
                    $("#workhourpermonth_" + tdDiv).hide();
                    $('#workhourpermonth_statusflag_'+tdDiv).val('D');
                }
                break;
                
              case 'maternity':
                if ($('#maternity_statusflag_'+tdDiv).val()=='I') {
                    $("#maternity_"+tdDiv).hide();
                    $('#maternity_statusflag_'+tdDiv).val('X');
                   // txnData.maternityData.splice(tdDiv,1);
                } else {
                    $("#maternity_" + tdDiv).hide();
                    $('#maternity_statusflag_'+tdDiv).val('D');
                }
                break;
                
              case 'termination':
                if ($('#termination_statusflag_'+tdDiv).val()=='I') {
                    $("#termination_"+tdDiv).hide();
                    $('#termination_statusflag_'+tdDiv).val('X');
                   // txnData.terminationData.splice(tdDiv,1);
                } else {
                    $("#termination_" + tdDiv).hide();
                    $('#termination_statusflag_'+tdDiv).val('D');
                }
                break;
                
              case 'manager':
                if ($('#manager_statusflag_'+tdDiv).val()=='I') {
                    $("#manager_"+tdDiv).hide();
                    $('#manager_statusflag_'+tdDiv).val('X');
                   // txnData.manager.splice(tdDiv,1);
                } else {
                    $("#manager_" + tdDiv).hide();
                    $('#manager_statusflag_'+tdDiv).val('D');
                }
                break;
        }
      
    }
    
    
    addTransaction = function (e) {
      //  alert("emp code  "+select_HTML);
      //  getOtherData(empcode);
        
        var remId = e.currentTarget.id;
        var tdDiv = remId.split("_")[0];
        var rowNum=0;
        var newRow=''; 
        var newRec={};
        
                             
        
        switch (tdDiv) {
            case 'shift':  
                $('#shift tr:last').remove();
                rowNum=txnData.shiftData.length;
                newRow=''; 
                newRec={};
                var newshiftfromdate='shift_from_date_'+rowNum;
                var newshifttodate='shift_to_date_'+rowNum;
                var newshiftstatusflag='shift_statusflag_'+rowNum;
                var shiftname='shift_name_selected_'+rowNum;
               // alert('adding row for '+emp-val);
                debugger;
                //shiftOptions=getOtherData(10)
               
                newRow = "<tr id='shift_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newshiftfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate' onchange='alert('heelo');'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newshifttodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'>   <td>"+
                                "<select class='form-control' id='"+shiftname+"' name='shift_select_data'>"+
                                    select_HTML+
                                "</select></td>"+  
                                "<div class='col-3'>"+
                                "<td ><label id=shift_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newshiftstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
              //  $('#'+shiftname).append(shiftData);
                
                $('#shift tbody').append(newRow);
           
                $('#shift tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=shift_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.shiftData.push(newRec);
                break;
            
            case 'ot':
                $('#ot tr:last').remove();
                rowNum=txnData.ot.length;
                newRow=''; 
                newRec={};
                var newotfromdate='ot_from_date_'+rowNum;
                var newottodate='ot_to_date_'+rowNum;
                var newotstatusflag='ot_statusflag_'+rowNum;
                newRow = "<tr id='ot_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newotfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newottodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'><td></td>"+
                                "<td ></label><label id=ot_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newotstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#ot tbody').append(newRow);
                
                $('#ot tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                 "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=ot_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.ot.push(newRec);
                break;
                
             case 'ramadan':
                $('#ramadan tr:last').remove();
                rowNum=txnData.ramadan.length;
                newRow=''; 
                newRec={};
                var newramadanfromdate='ramadan_from_date_'+rowNum;
                var newramadantodate='ramadan_to_date_'+rowNum;
                var newramadanstatusflag='ramadan_statusflag_'+rowNum;
                newRow = "<tr id='ramadan_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newramadanfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newramadantodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'><td></td>"+
                                "<td ></label><label id=ramadan_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newramadanstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#ramadan tbody').append(newRow);
                
                $('#ramadan tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                 "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=ramadan_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.ramadan.push(newRec);
                break;
                
                
             case 'punchexception':
                $('#punchexception tr:last').remove();
                rowNum=txnData.punchexception.length;
                newRow=''; 
                newRec={};
                var newpunchexceptionfromdate='punchexception_from_date_'+rowNum;
                var newpunchexceptiontodate='punchexception_to_date_'+rowNum;
                var newpunchexceptionstatusflag='punchexception_statusflag_'+rowNum;
                newRow = "<tr id='punchexception_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newpunchexceptionfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newpunchexceptiontodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'><td></td>"+
                                "<td ></label><label id=punchexception_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newpunchexceptionstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#punchexception tbody').append(newRow);
                
                $('#punchexception tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                 "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=punchexception_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.punchexception.push(newRec);
                break;
                
             case 'workhourperday':  
                $('#workhourperday tr:last').remove();
                rowNum=txnData.workhourperday.length;
                newRow=''; 
                newRec={};
                var newworkhourperdayfromdate='workhourperday_from_date_'+rowNum;
                var newworkhourperdaytodate='workhourperday_to_date_'+rowNum;
                var newworkhourperdaystatusflag='workhourperday_statusflag_'+rowNum;
                var newworkhourperdayhours='workhourperday_hours_'+rowNum;
                newRow = "<tr id='workhourperday_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newworkhourperdayfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newworkhourperdaytodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'>   <td>"+
                              //  "<select class='form-control' id='workhourperweek_select_data' name='workhourperweek_select_data'>"+
                              //      "<option value='select'>Select</option>"+
                              //  "</select></td>"+ 
                                "<input type=text class='form-control' id="+newworkhourperdayhours+" ></td>"+
                                "<div class='col-3'>"+
                                "<td ><label id=workhourperday_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newworkhourperdaystatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#workhourperday tbody').append(newRow);
                
                $('#workhourperday tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=workhourperday_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.workhourperday.push(newRec);
                break;
                
             case 'workhourperweek':  
                $('#workhourperweek tr:last').remove();
                rowNum=txnData.workhourperweek.length;
                newRow=''; 
                newRec={};
                var newworkhourperweekfromdate='workhourperweek_from_date_'+rowNum;
                var newworkhourperweektodate='workhourperweek_to_date_'+rowNum;
                var newworkhourperweekstatusflag='workhourperweek_statusflag_'+rowNum;
                var newworkhourperweekhours='workhourperweek_hours_'+rowNum;
                newRow = "<tr id='workhourperweek_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newworkhourperweekfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newworkhourperweektodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'>   <td>"+
                                "<input type=text class='form-control' id="+newworkhourperweekhours+" ></td>"+
                                "<div class='col-3'>"+
                                "<td ><label id=workhourperweek_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newworkhourperweekstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#workhourperweek tbody').append(newRow);
                
                $('#workhourperweek tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=workhourperweek_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.workhourperweek.push(newRec);
                break;
                
             case 'workhourpermonth':  
                $('#workhourpermonth tr:last').remove();
                rowNum=txnData.workhourpermonth.length;
                newRow=''; 
                newRec={};
                var newworkhourpermonthfromdate='workhourpermonth_from_date_'+rowNum;
                var newworkhourpermonthtodate='workhourpermonth_to_date_'+rowNum;
                var newworkhourpermonthstatusflag='workhourpermonth_statusflag_'+rowNum;
                var newworkhourpermonthhours='workhourpermonth_hours_'+rowNum;
                newRow = "<tr id='workhourpermonth_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newworkhourpermonthfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newworkhourpermonthtodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'>   <td>"+
                                 "<input type=text class='form-control' id="+newworkhourpermonthhours+" ></td>"+ 
                                "<div class='col-3'>"+
                                "<td ><label id=workhourpermonth_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newworkhourpermonthstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#workhourpermonth tbody').append(newRow);
                
                $('#workhourpermonth tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=workhourpermonth_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.workhourpermonth.push(newRec);
                break;
                
             case 'maternity':
                $('#maternity tr:last').remove();
                rowNum=txnData.maternity.length;
                newRow=''; 
                newRec={};
                var newmaternityfromdate='maternity_from_date_'+rowNum;
                var newmaternitytodate='maternity_to_date_'+rowNum;
                var newmaternitystatusflag='maternity_statusflag_'+rowNum;
                newRow = "<tr id='maternity_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newmaternityfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newmaternitytodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'><td></td>"+
                                "<td ></label><label id=maternity_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newmaternitystatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#maternity tbody').append(newRow);
                
                $('#maternity tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=maternity_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.maternity.push(newRec);
                break;
                
            case 'termination':
                $('#termination tr:last').remove();
                rowNum=txnData.termination.length;
                newRow=''; 
                newRec={};
                var newterminationfromdate='termination_from_date_'+rowNum;
                var newterminationtodate='termination_to_date_'+rowNum;
                var newterminationstatusflag='termination_statusflag_'+rowNum;
                newRow = "<tr id='termination_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newterminationfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newterminationtodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'><td></td>"+
                                "<td ></label><label id=termination_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newterminationstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#termination tbody').append(newRow);
                
                $('#termination tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                 "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=termination_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.termination.push(newRec);
                break;
                
             case 'manager':  
                $('#manager tr:last').remove();
                rowNum=txnData.manager.length;
                newRow=''; 
                newRec={};
                var newmanagerfromdate='manager_from_date_'+rowNum;
                var newmanagertodate='manager_to_date_'+rowNum;
                var newmanagerstatusflag='manager_statusflag_'+rowNum;
                var managerid='manager_id_'+rowNum;
                newRow = "<tr id='manager_"+rowNum+"'><div class='col-2'><td></td></div>" +
                                "<div class='col-6'>   <td><input id="+newmanagerfromdate+" class='form-control datepicker' placeholder='fromdate' type='date' name='fromdate'> </td>  </div>" +
                                "<div class='col-3'>   <td><input id="+newmanagertodate+" class='form-control datepicker' placeholder='todate' type='date' name='todate'> </td>  </div>" +
                                "<div class='col-3'>   <td>"+
                                "<input type=text  id="+managerid+"></td>"+
                                "<div class='col-3'>"+
                                "<td ><label id=manager_del_" + rowNum + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                                "data-control='button'></label></td>"+
                                "<td> <input id="+newmanagerstatusflag+" type='hidden' value='I'></td></div>"+
                                "</tr>";        
                $('#manager tbody').append(newRow);
                
                $('#manager tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=manager_add_" + rowNum + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
              
                txnData.manager.push(newRec);
                break;
                
        }
    };
    
   
    saveEmployeeData = function () {
    
        data = SAXForms.get($saveEmployeeTransactionForm);
       // alert(JSON.stringify(data, null, 4));
        var status_flag='';
        
        //add new shift record to transaction data table to be saved - begin
        status_flag='';
        var newShiftRec={};
        
        for (var i=0;i<txnData.shiftData.length;i++) {
            status_flag=$('#shift_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.shiftData[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newShiftRec={};
                newShiftRec['id']=99;
                newShiftRec['transactiontype']=1;
                newShiftRec['fromdate']=$('#shift_from_date_'+i).val();
                newShiftRec['todate']=$('#shift_to_date_'+i).val();
                newShiftRec['transactiondata']=$('#shift_name_selected_'+i).val();
                newShiftRec['StatusFlag']=status_flag;
                txnData.shiftData[i]=newShiftRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing txnData');
        for (var i=0;i<txnData.shiftData.length;i++) {
           console.log('id-> '+txnData.shiftData[i].id);
           console.log('fromdate-> '+txnData.shiftData[i].fromdate);
           console.log('todate-> '+txnData.shiftData[i].todate);
           console.log('StatusFlag-> '+txnData.shiftData[i].StatusFlag);
          }
        //add new shift record to transaction data table to be saved - end
        
        
        //add new ot record to transaction data table to be saved - begin
        status_flag='';
        var newotRec={};
        
        for (var i=0;i<txnData.ot.length;i++) {
            status_flag=$('#ot_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.ot[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newotRec={};
                newotRec['id']=99;
                newotRec['transactiontype']=2;
                newotRec['fromdate']=$('#ot_from_date_'+i).val();
                newotRec['todate']=$('#ot_to_date_'+i).val();
                newotRec['transactiondata']=1;
                newotRec['StatusFlag']=status_flag;
                txnData.ot[i]=newotRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing txnData');
        for (var i=0;i<txnData.ot.length;i++) {
           console.log('id-> '+txnData.ot[i].id);
           console.log('fromdate-> '+txnData.ot[i].fromdate);
           console.log('todate-> '+txnData.ot[i].todate);
           console.log('StatusFlag-> '+txnData.ot[i].StatusFlag);
          }
        //add new ot record to transaction data table to be saved - end
        
         //add new ramadan record to transaction data table to be saved - begin
        status_flag='';
        var newramadanRec={};
        
        for (var i=0;i<txnData.ramadan.length;i++) {
            status_flag=$('#ramadan_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.ramadan[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newramadanRec={};
                newramadanRec['id']=99;
                newramadanRec['transactiontype']=3;
                newramadanRec['fromdate']=$('#ramadan_from_date_'+i).val();
                newramadanRec['todate']=$('#ramadan_to_date_'+i).val();
                newramadanRec['transactiondata']=1;
                newramadanRec['StatusFlag']=status_flag;
                txnData.ramadan[i]=newramadanRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing txnData');
        for (var i=0;i<txnData.ramadan.length;i++) {
           console.log('id-> '+txnData.ramadan[i].id);
           console.log('fromdate-> '+txnData.ramadan[i].fromdate);
           console.log('todate-> '+txnData.ramadan[i].todate);
           console.log('StatusFlag-> '+txnData.ramadan[i].StatusFlag);
          }
        //add new ramadan record to transaction data table to be saved - end
        
        //add new punchexception record to transaction data table to be saved - begin
        status_flag='';
        var newpunchexceptionRec={};
        
        for (var i=0;i<txnData.punchexception.length;i++) {
            status_flag=$('#punchexception_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.punchexception[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newpunchexceptionRec={};
                newpunchexceptionRec['id']=99;
                newpunchexceptionRec['transactiontype']=4;
                newpunchexceptionRec['fromdate']=$('#punchexception_from_date_'+i).val();
                newpunchexceptionRec['todate']=$('#punchexception_to_date_'+i).val();
                newpunchexceptionRec['transactiondata']=1;
                newpunchexceptionRec['StatusFlag']=status_flag;
                txnData.punchexception[i]=newpunchexceptionRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing txnData');
        for (var i=0;i<txnData.punchexception.length;i++) {
           console.log('id-> '+txnData.punchexception[i].id);
           console.log('fromdate-> '+txnData.punchexception[i].fromdate);
           console.log('todate-> '+txnData.punchexception[i].todate);
           console.log('StatusFlag-> '+txnData.punchexception[i].StatusFlag);
          }
        //add new punchexception record to transaction data table to be saved - end
        
        
        //add new workhourperday record to transaction data table to be saved - begin
        status_flag='';
        var newworkhourperdayRec={};
        
        for (var i=0;i<txnData.workhourperday.length;i++) {
            status_flag=$('#workhourperday_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.workhourperday[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newworkhourperdayRec={};
                newworkhourperdayRec['id']=99;
                newworkhourperdayRec['transactiontype']=6;
                newworkhourperdayRec['fromdate']=$('#workhourperday_from_date_'+i).val();
                newworkhourperdayRec['todate']=$('#workhourperday_to_date_'+i).val();
                newworkhourperdayRec['transactiondata']=$('#workhourperday_hours_'+i).val();
                newworkhourperdayRec['StatusFlag']=status_flag;
                txnData.workhourperday[i]=newworkhourperdayRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing wh day Data');
        for (var i=0;i<txnData.workhourperday.length;i++) {
           console.log('id-> '+txnData.workhourperday[i].id);
           console.log('fromdate-> '+txnData.workhourperday[i].fromdate);
           console.log('todate-> '+txnData.workhourperday[i].todate);
           console.log('StatusFlag-> '+txnData.workhourperday[i].StatusFlag);
          }
        //add new workhourperday record to transaction data table to be saved - end
        
        
        //add new workhourperweek record to transaction data table to be saved - begin
        status_flag='';
        var newworkhourperweekRec={};
        
        for (var i=0;i<txnData.workhourperweek.length;i++) {
            status_flag=$('#workhourperweek_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.workhourperweek[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newworkhourperweekRec={};
                newworkhourperweekRec['id']=99;
                newworkhourperweekRec['transactiontype']=7;
                newworkhourperweekRec['fromdate']=$('#workhourperweek_from_date_'+i).val();
                newworkhourperweekRec['todate']=$('#workhourperweek_to_date_'+i).val();
                newworkhourperweekRec['transactiondata']=$('#workhourperweek_hours_'+i).val();
                newworkhourperweekRec['StatusFlag']=status_flag;
                txnData.workhourperweek[i]=newworkhourperweekRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log(' printing wh week Data');
        for (var i=0;i<txnData.workhourperweek.length;i++) {
           console.log('id-> '+txnData.workhourperweek[i].id);
           console.log('fromdate-> '+txnData.workhourperweek[i].fromdate);
           console.log('todate-> '+txnData.workhourperweek[i].todate);
           console.log('StatusFlag-> '+txnData.workhourperweek[i].StatusFlag);
          }
        //add new workhourperweek record to transaction data table to be saved - end
        
        //add new workhourpermonth record to transaction data table to be saved - begin
        status_flag='';
        var newworkhourpermonthRec={};
        
        for (var i=0;i<txnData.workhourpermonth.length;i++) {
            status_flag=$('#workhourpermonth_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.workhourpermonth[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newworkhourpermonthRec={};
                newworkhourpermonthRec['id']=99;
                newworkhourpermonthRec['transactiontype']=8;
                newworkhourpermonthRec['fromdate']=$('#workhourpermonth_from_date_'+i).val();
                newworkhourpermonthRec['todate']=$('#workhourpermonth_to_date_'+i).val();
                newworkhourpermonthRec['transactiondata']=$('#workhourpermonth_hours_'+i).val();
                newworkhourpermonthRec['StatusFlag']=status_flag;
                txnData.workhourpermonth[i]=newworkhourpermonthRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing wh month Data');
        for (var i=0;i<txnData.workhourpermonth.length;i++) {
           console.log('id-> '+txnData.workhourpermonth[i].id);
           console.log('fromdate-> '+txnData.workhourpermonth[i].fromdate);
           console.log('todate-> '+txnData.workhourpermonth[i].todate);
           console.log('StatusFlag-> '+txnData.workhourpermonth[i].StatusFlag);
          }
        //add new workhourpermonth record to transaction data table to be saved - end
        
        
        //add new maternity record to transaction data table to be saved - begin
        status_flag='';
        var newmaternityRec={};
        
        for (var i=0;i<txnData.maternity.length;i++) {
            status_flag=$('#maternity_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.maternity[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newmaternityRec={};
                newmaternityRec['id']=99;
                newmaternityRec['transactiontype']=5;
                newmaternityRec['fromdate']=$('#maternity_from_date_'+i).val();
                newmaternityRec['todate']=$('#maternity_to_date_'+i).val();
                newmaternityRec['transactiondata']=1;
                newmaternityRec['StatusFlag']=status_flag;
                txnData.maternity[i]=newmaternityRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing txnData');
        for (var i=0;i<txnData.maternity.length;i++) {
           console.log('id-> '+txnData.maternity[i].id);
           console.log('fromdate-> '+txnData.maternity[i].fromdate);
           console.log('todate-> '+txnData.maternity[i].todate);
           console.log('StatusFlag-> '+txnData.maternity[i].StatusFlag);
          }
        //add new maternity record to transaction data table to be saved - end
        
        //add new termination record to transaction data table to be saved - begin
        status_flag='';
        var newterminationRec={};
        
        for (var i=0;i<txnData.termination.length;i++) {
            status_flag=$('#termination_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.termination[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newterminationRec={};
                newterminationRec['id']=99;
                newterminationRec['transactiontype']=9;
                newterminationRec['fromdate']=$('#termination_from_date_'+i).val();
                newterminationRec['todate']=$('#termination_to_date_'+i).val();
                newterminationRec['transactiondata']=1;
                newterminationRec['StatusFlag']=status_flag;
                txnData.termination[i]=newterminationRec;
                }
        }
        // alert(JSON.stringify(txnData, null, 4));
        console.log('printing txnData');
        for (var i=0;i<txnData.termination.length;i++) {
           console.log('id-> '+txnData.termination[i].id);
           console.log('fromdate-> '+txnData.termination[i].fromdate);
           console.log('todate-> '+txnData.termination[i].todate);
           console.log('StatusFlag-> '+txnData.termination[i].StatusFlag);
          }
        //add new termination record to transaction data table to be saved - end
        
         //add new manager record to transaction data table to be saved - begin
        status_flag='';
        var newmanagerRec={};
        
        for (var i=0;i<txnData.manager.length;i++) {
            status_flag=$('#manager_statusflag_'+i).val();
            if (status_flag == 'D') {
               txnData.manager[i].StatusFlag=status_flag;
            } else if (status_flag == 'I' || status_flag == 'X') {
                newmanagerRec={};
                newmanagerRec['id']=99;
                newmanagerRec['transactiontype']=10;
                newmanagerRec['fromdate']=$('#manager_from_date_'+i).val();
                newmanagerRec['todate']=$('#manager_to_date_'+i).val();               
                newmanagerRec['transactiondata']=$('#manager_id_'+i).val();
                newmanagerRec['StatusFlag']=status_flag;
                txnData.manager[i]=newmanagerRec;
                }
        }
         //alert(JSON.stringify(txnData, null, 4));
        console.log('printing txnData');
        for (var i=0;i<txnData.manager.length;i++) {
           console.log('id-> '+txnData.manager[i].id);
           console.log('fromdate-> '+txnData.manager[i].fromdate);
           console.log('todate-> '+txnData.manager[i].todate);
           console.log('StatusFlag-> '+txnData.manager[i].StatusFlag);
          }
        //add new manager record to transaction data table to be saved - end
        
        //alert('Employee Transaction data saved');
        
       
		var	ajax_options = {};
		//	data = SAXForms.get($saveEmployeeTransactionForm);
        
       

     //   if (_validate(txnData)) {


            SAXHTTP.AJAX(
						"employee_data_transaction.aspx/addEmployee",
						{ current: JSON.stringify(txnData) }
					)
                   .done(function (data) {

                     //  SAXAlert.show({ type: "success", message: "Employee Transaction data saved." });
                    
                       if (data.d.status == "success") {
                       SAXAlert.show({ type: "success", message: "Employee Transaction data saved." });
                           $('#saveForm')[0].reset();
                           $('#empTransaction').hide();
                           dialogs.save.modal("hide");
                       }
//                        if (data.d.status == "error")  {
//                       SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
//                           $('#saveForm')[0].reset();
//                           $('#empTransaction').hide();
//                           dialogs.save.modal("hide");
//                       }


                   })
				.fail(function () {
				    SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
				    $('#empTransaction').show();
				    dialogs.save.modal("show");
				})
            // .always(function () { buttons.save.button("reset") }


      //  } */

    };
    
    
     var $import_button = $("#importButton"),
         $file_upload = $("#file_upload"),
         $display_result = $("#importResult");

    _processImport = function (data, additional) {

        var
        status = data.status,
        message = data.return_data;

        if (status === 'success') {

            $display_result.val(message);

            $file_upload.val('');
        }
        else {
            SAXAlert.show({ 'type': status, 'message': message });
        }

        $import_button.button('reset');
        SAXLoader.closeBlockingLoader();
    };
    
    _doImport = function () {

        var
        ajax_options = {
            'url': page_name + '/DoImport',
            'data': { file_name: page.file_name },
            'callback': _processImport,
            'additional': {}
        };


        if ($file_upload.val() == '') {
            SAXAlert.show({ type: 'error', message: 'Please select a file for import' });
            return false;
        }

        $import_button.button('loading');

        SAXLoader.showBlockingLoader();
        SAXHTTP.ajax(ajax_options);
    };
    
    
    _processUpload = function (data, additional) {

        var
        result = JSON.parse(data),
        status = result.status;

        if (status === 'success') {
            $import_button.removeAttr('disabled');
            page.file_name = result.return_data;
        }
        else {
            SAXAlert.show({ 'type': status, 'message': result.return_data });
        }

        SAXLoader.closeBlockingLoader();
    };
    
    _doUpload = function (that) {

        var uploaded_files = $(that).get(0).files,
        form_data = new FormData(),
        ajax_options = {},
        i = 0,
        employee_code = sessvars.TAMS_ENV.user_details.user_name,
        now = new Date(),
        now = Date.parse(now),
        file_name = employee_code + "-" + now + "-" + uploaded_files[0].name,
        file_extension = file_name.slice((file_name.lastIndexOf(".") - 1 >>> 0) + 2).toLowerCase();

        if (file_extension === 'xls' || file_extension === 'xlsx') {

            form_data.append('file_name', uploaded_files[0]);
            form_data.append('filename', file_name);

            ajax_options = {
                'url': 'fileupload.ashx',
                'type': 'POST',
                'data': form_data,
                'contentType': false,
                'processData': false,
                'success': _processUpload,
                'error': function () {
                    SAXLoader.closeBlockingLoader();
                    SAXAlert.show({ 'type': 'error', 'message': 'An error occurred while uploading the file. Please try again. If the error persists, please contact Support.' });
                }
            };

            SAXLoader.showBlockingLoader();
            $.ajax(ajax_options);
        }
        else {
            SAXAlert.show({ 'type': 'error', 'message': 'Extension of the file uploaded is incorrect. Please try again.' });
            return false;
        }
    };

    function _renderforEmployee(data) {
        $('#empTransaction').modal('show');
        getOtherData
       //  alert(JSON.stringify(data, null, 4));
       
        
    //    $(".datepicker").datepicker("destroy");
        $('#Punch tbody').empty();
        for (i = 0; i < data.employeedata.length; i++) {
           var employeecode = data.employeedata[i].emp_code;
            }
      
        txnData=data;
        // begin rendering of shift data
        $('#shift tbody').empty();
        var shiftrec = '';
       // debugger;
        shiftrec = '<tr>' +
                                '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Shift</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
                                '            <label class="text-blue"><strong>From Date</strong></label></td>'+
                                '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Shift Name</strong></label></td>'+
                                '   </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
                                '   </div>'+
                                '</tr>';
        $('#shift tbody').append(shiftrec);
        
        for (i = 0; i < data.shiftData.length; i++) {
            id = data.shiftData[i].id;
            shiftFromDate = new Date(data.shiftData[i].fromdate);
            
            shiftToDate = new Date(data.shiftData[i].todate);
            shiftname = data.shiftData[i].shift_desc;
            shiftStatus= data.shiftData[i].StatusFlag;
            
            shiftrec = '';
            
            shiftrec += '<tr id="shift_' + i + '"><td></td>';
            
           
                       
            if (shiftFromDate != null) {
               
                shiftrec += '<td ><label id=shift_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(shiftFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                shiftrec += '<td>'
            }
            shiftrec += '</td>';

            if (shiftToDate != null) {
                // $("#shift_To_date").val(moment(shiftToDate).format("DD-MMM-YYYY"));
                shiftrec += '<td><label id=shift_to_date_'+i+' class="text-blue"><small>' + moment(shiftToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#shift_To_date").val("");
                shiftrec += '<td>'
            }
            shiftrec += '</td>';

            if (shiftname != null) {
                // $("#shift_To_date").val(moment(shiftToDate).format("DD-MMM-YYYY"));
                shiftrec += '<td> <label id=shift_name_'+i+' class="text-blue"><small>' + shiftname + "</small></label>";
            } else {
                // $("#shift_To_date").val("");
                shiftrec += '<td>'
            }
            
            shiftrec += "<td ><label id=shift_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
             /*   <td><label id=shift_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>"; */
                
            if (shiftStatus != null) {
                // $("#shift_To_date").val(moment(shiftToDate).format("DD-MMM-YYYY"));
                shiftrec += '<td> <input id=shift_statusflag_'+i+' type="hidden" value="'+shiftStatus+'">'; 
            } else {
                // $("#shift_To_date").val("");
                shiftrec += '<td>'
            }
            shiftrec += '</td>';
            
            shiftrec += '</tr>';
            $('#shift tbody').append(shiftrec);
          
        }

        $('#shift tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=shift_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering shift data
        
        
        // begin rendering of OT data
         $('#ot tbody').empty();
        var otrec = '';
       // debugger;
        otrec = '<tr>' +
                                    '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>OT</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>From Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2"><td></td>'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
        '   </div>'+
        '</tr>';
        $('#ot tbody').append(otrec);
        
        for (i = 0; i < data.ot.length; i++) {
            id = data.ot[i].id;
            otFromDate = new Date(data.ot[i].fromdate);
            
            otToDate = new Date(data.ot[i].todate);
            
            otStatus= data.ot[i].StatusFlag;
            otrec = '';
            
            otrec += '<tr id="ot_' + i + '"><td></td>';
            if (otFromDate != null) {
               
                otrec += '<td ><label id=ot_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(otFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                otrec += '<td>'
            }
            otrec += '</td>';

            if (otToDate != null) {
                // $("#ot_To_date").val(moment(otToDate).format("DD-MMM-YYYY"));
                otrec += '<td><label id=ot_to_date_'+i+' class="text-blue"><small>' + moment(otToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#ot_To_date").val("");
                otrec += '<td>'
            }
            otrec += '</td><td></td>';

            
            otrec += "<td ><label id=ot_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=ot_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>"; */
                
            if (otStatus != null) {
                // $("#ot_To_date").val(moment(otToDate).format("DD-MMM-YYYY"));
                otrec += '<td> <input id=ot_statusflag_'+i+' type="hidden" value="'+otStatus+'">'; 
            } else {
                // $("#ot_To_date").val("");
                otrec += '<td>'
            }
            otrec += '</td>';
            otrec += '</tr>';
            $('#ot tbody').append(otrec);
        }

        $('#ot tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                 "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=ot_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering OT data
        
         // begin rendering of ramadan data
          $('#ramadan tbody').empty();
        var ramadanrec = '';
       // debugger;
        ramadanrec = '<tr>' +
                                '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Ramadan</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>From Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2"><td></td>'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
        '   </div>'+
        '</tr>';
        $('#ramadan tbody').append(ramadanrec);
        
        for (i = 0; i < data.ramadan.length; i++) {
            id = data.ramadan[i].id;
            ramadanFromDate = new Date(data.ramadan[i].fromdate);
            
            ramadanToDate = new Date(data.ramadan[i].todate);
            
            ramadanStatus= data.ramadan[i].StatusFlag;
            ramadanrec = '';
            
            ramadanrec += '<tr id="ramadan_' + i + '"><td></td>';
            if (ramadanFromDate != null) {
               
                ramadanrec += '<td ><label id=ramadan_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(ramadanFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                ramadanrec += '<td>'
            }
            ramadanrec += '</td>';

            if (ramadanToDate != null) {
                // $("#ramadan_To_date").val(moment(ramadanToDate).format("DD-MMM-YYYY"));
                ramadanrec += '<td><label id=ramadan_to_date_'+i+' class="text-blue"><small>' + moment(ramadanToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#ramadan_To_date").val("");
                ramadanrec += '<td>'
            }
            ramadanrec += '</td><td></td>';

            
            ramadanrec += "<td ><label id=ramadan_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=ramadan_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>"; */
                
            if (ramadanStatus != null) {
                // $("#ramadan_To_date").val(moment(ramadanToDate).format("DD-MMM-YYYY"));
                ramadanrec += '<td> <input id=ramadan_statusflag_'+i+' type="hidden" value="'+ramadanStatus+'">'; 
            } else {
                // $("#ramadan_To_date").val("");
                ramadanrec += '<td>'
            }
            ramadanrec += '</td>';
            ramadanrec += '</tr>';
            $('#ramadan tbody').append(ramadanrec);
        }

        $('#ramadan tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                  "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=ramadan_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering ramadan data
        
        // begin rendering of punchexception data
         $('#punchexception tbody').empty();
        var punchexceptionrec = '';
       // debugger;
        punchexceptionrec = '<tr>' +
                                 '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Punch exception</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>From Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2"><td></td>'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
        '   </div>'+
        '</tr>';
        $('#punchexception tbody').append(punchexceptionrec);
        
        for (i = 0; i < data.punchexception.length; i++) {
            id = data.punchexception[i].id;
            punchexceptionFromDate = new Date(data.punchexception[i].fromdate);
            
            punchexceptionToDate = new Date(data.punchexception[i].todate);
            
            punchexceptionStatus= data.punchexception[i].StatusFlag;
            punchexceptionrec = '';
            
            punchexceptionrec += '<tr id="punchexception_' + i + '"><td></td>';
            if (punchexceptionFromDate != null) {
               
                punchexceptionrec += '<td ><label id=punchexception_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(punchexceptionFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                punchexceptionrec += '<td>'
            }
            punchexceptionrec += '</td>';

            if (punchexceptionToDate != null) {
                // $("#punchexception_To_date").val(moment(punchexceptionToDate).format("DD-MMM-YYYY"));
                punchexceptionrec += '<td><label id=punchexception_to_date_'+i+' class="text-blue"><small>' + moment(punchexceptionToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#punchexception_To_date").val("");
                punchexceptionrec += '<td>'
            }
            punchexceptionrec += '</td><td></td>';

            
            punchexceptionrec += "<td ><label id=punchexception_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=punchexception_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>";*/
                
            if (punchexceptionStatus != null) {
                // $("#punchexception_To_date").val(moment(punchexceptionToDate).format("DD-MMM-YYYY"));
                punchexceptionrec += '<td> <input id=punchexception_statusflag_'+i+' type="hidden" value="'+punchexceptionStatus+'">'; 
            } else {
                // $("#punchexception_To_date").val("");
                punchexceptionrec += '<td>'
            }
            punchexceptionrec += '</td>';
            punchexceptionrec += '</tr>';
            $('#punchexception tbody').append(punchexceptionrec);
        }

        $('#punchexception tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                  "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=punchexception_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering punchexception data
        
        // begin rendering of workhourperday data
        $('#workhourperday tbody').empty();
        var workhourperdayrec = '';
       // debugger;
        workhourperdayrec = '<tr>' +
                                '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Work Hours per Day</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
                                '            <label class="text-blue"><strong>From Date</strong></label></td>'+
                                '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>No of hours</strong></label></td>'+
                                '   </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
                                '   </div>'+
                                '</tr>';
        $('#workhourperday tbody').append(workhourperdayrec);
        
        for (i = 0; i < data.workhourperday.length; i++) {
            id = data.workhourperday[i].id;
            workhourperdayFromDate = new Date(data.workhourperday[i].fromdate);
            
            workhourperdayToDate = new Date(data.workhourperday[i].todate);
            workhourperdayname = data.workhourperday[i].transactiondata;
            workhourperdayStatus= data.workhourperday[i].StatusFlag;
            workhourperdayrec = '';
            
            workhourperdayrec += '<tr id="workhourperday_' + i + '"><td></td>';
            if (workhourperdayFromDate != null) {
               
                workhourperdayrec += '<td ><label id=workhourperday_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(workhourperdayFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                workhourperdayrec += '<td>'
            }
            workhourperdayrec += '</td>';

            if (workhourperdayToDate != null) {
                // $("#workhourperday_To_date").val(moment(workhourperdayToDate).format("DD-MMM-YYYY"));
                workhourperdayrec += '<td><label id=workhourperday_to_date_'+i+' class="text-blue"><small>' + moment(workhourperdayToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#workhourperday_To_date").val("");
                workhourperdayrec += '<td>'
            }
            workhourperdayrec += '</td>';

            if (workhourperdayname != null) {
                // $("#workhourperday_To_date").val(moment(workhourperdayToDate).format("DD-MMM-YYYY"));
                workhourperdayrec += '<td> <label id=workhourperday_name_'+i+' class="text-blue"><small>' + workhourperdayname + "</small></label>";
            } else {
                // $("#workhourperday_To_date").val("");
                workhourperdayrec += '<td>'
            }
            
            workhourperdayrec += "<td ><label id=workhourperday_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=workhourperday_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>"; */
                
            if (workhourperdayStatus != null) {
                // $("#workhourperday_To_date").val(moment(workhourperdayToDate).format("DD-MMM-YYYY"));
                workhourperdayrec += '<td> <input id=workhourperday_statusflag_'+i+' type="hidden" value="'+workhourperdayStatus+'">'; 
            } else {
                // $("#workhourperday_To_date").val("");
                workhourperdayrec += '<td>'
            }
            workhourperdayrec += '</td>';
            
            workhourperdayrec += '</tr>';
            $('#workhourperday tbody').append(workhourperdayrec);
        }

        $('#workhourperday tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=workhourperday_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering workhourperday data
        
        // begin rendering of workhourperweek data
        $('#workhourperweek tbody').empty();
        var workhourperweekrec = '';
       // debugger;
        workhourperweekrec = '<tr>' +
                                 '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Work Hours per Week</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
                                '            <label class="text-blue"><strong>From Date</strong></label></td>'+
                                '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>No of hours</strong></label></td>'+
                                '   </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
                                '   </div>'+
                                '</tr>';
        $('#workhourperweek tbody').append(workhourperweekrec);
        
        for (i = 0; i < data.workhourperweek.length; i++) {
            id = data.workhourperweek[i].id;
            workhourperweekFromDate = new Date(data.workhourperweek[i].fromdate);
            
            workhourperweekToDate = new Date(data.workhourperweek[i].todate);
            workhourperweekname = data.workhourperweek[i].transactiondata;
            workhourperweekStatus= data.workhourperweek[i].StatusFlag;
            workhourperweekrec = '';
            
            workhourperweekrec += '<tr id="workhourperweek_' + i + '"><td></td>';
            if (workhourperweekFromDate != null) {
               
                workhourperweekrec += '<td ><label id=workhourperweek_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(workhourperweekFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                workhourperweekrec += '<td>'
            }
            workhourperweekrec += '</td>';

            if (workhourperweekToDate != null) {
                // $("#workhourperweek_To_date").val(moment(workhourperweekToDate).format("DD-MMM-YYYY"));
                workhourperweekrec += '<td><label id=workhourperweek_to_date_'+i+' class="text-blue"><small>' + moment(workhourperweekToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#workhourperweek_To_date").val("");
                workhourperweekrec += '<td>'
            }
            workhourperweekrec += '</td>';

            if (workhourperweekname != null) {
                // $("#workhourperweek_To_date").val(moment(workhourperweekToDate).format("DD-MMM-YYYY"));
                workhourperweekrec += '<td> <label id=workhourperweek_name_'+i+' class="text-blue"><small>' + workhourperweekname + "</small></label>";
            } else {
                // $("#workhourperweek_To_date").val("");
                workhourperweekrec += '<td>'
            }
            
            workhourperweekrec += "<td ><label id=workhourperweek_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=workhourperweek_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>";*/
                
            if (workhourperweekStatus != null) {
                // $("#workhourperweek_To_date").val(moment(workhourperweekToDate).format("DD-MMM-YYYY"));
                workhourperweekrec += '<td> <input id=workhourperweek_statusflag_'+i+' type="hidden" value="'+workhourperweekStatus+'">'; 
            } else {
                // $("#workhourperweek_To_date").val("");
                workhourperweekrec += '<td>'
            }
            workhourperweekrec += '</td>';
            
            workhourperweekrec += '</tr>';
            $('#workhourperweek tbody').append(workhourperweekrec);
        }

        $('#workhourperweek tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=workhourperweek_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering workhourperweek data
        
        // begin rendering of workhourpermonth data
        $('#workhourpermonth tbody').empty();
        var workhourpermonthrec = '';
       // debugger;
        workhourpermonthrec = '<tr>' +
                                 '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Work Hours per Month</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
                                '            <label class="text-blue"><strong>From Date</strong></label></td>'+
                                '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>No of hours</strong></label></td>'+
                                '   </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
                                '   </div>'+
                                '</tr>';
        $('#workhourpermonth tbody').append(workhourpermonthrec);
        
        for (i = 0; i < data.workhourpermonth.length; i++) {
            id = data.workhourpermonth[i].id;
            workhourpermonthFromDate = new Date(data.workhourpermonth[i].fromdate);
            
            workhourpermonthToDate = new Date(data.workhourpermonth[i].todate);
            workhourpermonthname = data.workhourpermonth[i].transactiondata;
            workhourpermonthStatus= data.workhourpermonth[i].StatusFlag;
            workhourpermonthrec = '';
            
            workhourpermonthrec += '<tr id="workhourpermonth_' + i + '"><td></td>';
            if (workhourpermonthFromDate != null) {
               
                workhourpermonthrec += '<td ><label id=workhourpermonth_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(workhourpermonthFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                workhourpermonthrec += '<td>'
            }
            workhourpermonthrec += '</td>';

            if (workhourpermonthToDate != null) {
                // $("#workhourpermonth_To_date").val(moment(workhourpermonthToDate).format("DD-MMM-YYYY"));
                workhourpermonthrec += '<td><label id=workhourpermonth_to_date_'+i+' class="text-blue"><small>' + moment(workhourpermonthToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#workhourpermonth_To_date").val("");
                workhourpermonthrec += '<td>'
            }
            workhourpermonthrec += '</td>';

            if (workhourpermonthname != null) {
                // $("#workhourpermonth_To_date").val(moment(workhourpermonthToDate).format("DD-MMM-YYYY"));
                workhourpermonthrec += '<td> <label id=workhourpermonth_name_'+i+' class="text-blue"><small>' + workhourpermonthname + "</small></label>";
            } else {
                // $("#workhourpermonth_To_date").val("");
                workhourpermonthrec += '<td>'
            }
            
            workhourpermonthrec += "<td ><label id=workhourpermonth_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=workhourpermonth_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>";*/
                
            if (workhourpermonthStatus != null) {
                // $("#workhourpermonth_To_date").val(moment(workhourpermonthToDate).format("DD-MMM-YYYY"));
                workhourpermonthrec += '<td> <input id=workhourpermonth_statusflag_'+i+' type="hidden" value="'+workhourpermonthStatus+'">'; 
            } else {
                // $("#workhourpermonth_To_date").val("");
                workhourpermonthrec += '<td>'
            }
            workhourpermonthrec += '</td>';
            
            workhourpermonthrec += '</tr>';
            $('#workhourpermonth tbody').append(workhourpermonthrec);
        }

        $('#workhourpermonth tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=workhourpermonth_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering workhourpermonth data
        
        // begin rendering of maternity data
         $('#maternity tbody').empty();
        var maternityrec = '';
       // debugger;
        maternityrec = '<tr>' +
                                 '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Maternity</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>From Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2"><td></td>'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
        '   </div>'+
        '</tr>';
        $('#maternity tbody').append(maternityrec);
        
        for (i = 0; i < data.maternity.length; i++) {
            id = data.maternity[i].id;
            maternityFromDate = new Date(data.maternity[i].fromdate);
            
            maternityToDate = new Date(data.maternity[i].todate);
            
            maternityStatus= data.maternity[i].StatusFlag;
            maternityrec = '';
            
            maternityrec += '<tr id="maternity_' + i + '"><td></td>';
            if (maternityFromDate != null) {
               
                maternityrec += '<td ><label id=maternity_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(maternityFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                maternityrec += '<td>'
            }
            maternityrec += '</td>';

            if (maternityToDate != null) {
                // $("#maternity_To_date").val(moment(maternityToDate).format("DD-MMM-YYYY"));
                maternityrec += '<td><label id=maternity_to_date_'+i+' class="text-blue"><small>' + moment(maternityToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#maternity_To_date").val("");
                maternityrec += '<td>'
            }
            maternityrec += '</td><td></td>';

            
            maternityrec += "<td ><label id=maternity_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=maternity_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>";*/
                
            if (maternityStatus != null) {
                // $("#maternity_To_date").val(moment(maternityToDate).format("DD-MMM-YYYY"));
                maternityrec += '<td> <input id=maternity_statusflag_'+i+' type="hidden" value="'+maternityStatus+'">'; 
            } else {
                // $("#maternity_To_date").val("");
                maternityrec += '<td>'
            }
            maternityrec += '</td>';
            maternityrec += '</tr>';
            $('#maternity tbody').append(maternityrec);
        }

        $('#maternity tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                 "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=maternity_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering maternity data
        
        // begin rendering of termination data
         $('#termination tbody').empty();
        var terminationrec = '';
       // debugger;
        terminationrec = '<tr>' +
                               '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Termination</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>From Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2">'+
        '        <td></td><td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
        '   </div>'+
        '</tr>';
        $('#termination tbody').append(terminationrec);
        
        for (i = 0; i < data.termination.length; i++) {
            id = data.termination[i].id;
            terminationFromDate = new Date(data.termination[i].fromdate);
            
            terminationToDate = new Date(data.termination[i].todate);
            
            terminationStatus= data.termination[i].StatusFlag;
            terminationrec = '';
            
            terminationrec += '<tr id="termination_' + i + '"><td></td>';
            if (terminationFromDate != null) {
               
                terminationrec += '<td ><label id=termination_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(terminationFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                terminationrec += '<td>'
            }
            terminationrec += '</td>';

            if (terminationToDate != null) {
                // $("#termination_To_date").val(moment(terminationToDate).format("DD-MMM-YYYY"));
                terminationrec += '<td><label id=termination_to_date_'+i+' class="text-blue"><small>' + moment(terminationToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#termination_To_date").val("");
                terminationrec += '<td>'
            }
            terminationrec += '</td><td></td>';

            
            terminationrec += "<td ><label id=termination_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=termination_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>";*/
                
            if (terminationStatus != null) {
                // $("#termination_To_date").val(moment(terminationToDate).format("DD-MMM-YYYY"));
                terminationrec += '<td> <input id=termination_statusflag_'+i+' type="hidden" value="'+terminationStatus+'">'; 
            } else {
                // $("#termination_To_date").val("");
                terminationrec += '<td>'
            }
            terminationrec += '</td>';
            terminationrec += '</tr>';
            $('#termination tbody').append(terminationrec);
        }

        $('#termination tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                  "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-1'><td><label id=termination_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering termination data
        
        // begin rendering of manager data
        $('#manager tbody').empty();
        var managerrec = '';
       // debugger;
        managerrec = '<tr>' +
                                 '   <td width="100"></td>'+
                                '    <td width="150"></td>'+
        '    <td width="150"></td>'+
        '    <td width="350"></td>'+
        '    <td width="50"></td>'+
        '</tr>'+
        '<tr>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Manager</strong></label></td>'+
        '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
                                '            <label class="text-blue"><strong>From Date</strong></label></td>'+
                                '    </div>'+
        '    <div class="col-3">'+
        '        <td>'+
        '            <label class="text-blue"><strong>End Date</strong></label></td>'+
        '    </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Employee Id</strong></label></td>'+
                                '   </div>'+
        '    <div class="col-2">'+
        '        <td>'+
        '            <label class="text-blue"><strong>Action</strong></label></td>'+
                                '   </div>'+
                                '</tr>';
        $('#manager tbody').append(managerrec);
        
        for (i = 0; i < data.manager.length; i++) {
            id = data.manager[i].id;
            managerFromDate = new Date(data.manager[i].fromdate);
            
            managerToDate = new Date(data.manager[i].todate);
            managername = data.manager[i].transactiondata;
            managerStatus= data.manager[i].StatusFlag;
            managerrec = '';
            
            managerrec += '<tr id="manager_' + i + '"><td></td>';
            if (managerFromDate != null) {
               
                managerrec += '<td ><label id=manager_from_date_'+i+' class="text-blue" readonly="readonly" ><small>' + moment(managerFromDate).format("DD-MMM-YYYY") + "</small></label>";
            
            } else {
                
                managerrec += '<td>'
            }
            managerrec += '</td>';

            if (managerToDate != null) {
                // $("#manager_To_date").val(moment(managerToDate).format("DD-MMM-YYYY"));
                managerrec += '<td><label id=manager_to_date_'+i+' class="text-blue"><small>' + moment(managerToDate).format("DD-MMM-YYYY") + "</small></label>";
               
            } else {
                // $("#manager_To_date").val("");
                managerrec += '<td>'
            }
            managerrec += '</td>';

            if (managername != null) {
                // $("#manager_To_date").val(moment(managerToDate).format("DD-MMM-YYYY"));
                managerrec += '<td> <label id=manager_name_'+i+' class="text-blue"><small>' + managername + "</small></label>";
            } else {
                // $("#manager_To_date").val("");
                managerrec += '<td>'
            }
            
            managerrec += "<td ><label id=manager_del_" + i + " class='fa fa-trash-o action-icon' data-role='txn-del' " +
                "data-control='button'></label></td>";
                /*<td><label id=manager_edit_" + i + " class='fa fa-pencil action-icon' data-role='txn-edit' " +
                "data-control='button'></label></td>";*/
                
            if (managerStatus != null) {
                // $("#manager_To_date").val(moment(managerToDate).format("DD-MMM-YYYY"));
                managerrec += '<td> <input id=manager_statusflag_'+i+' type="hidden" value="'+managerStatus+'">'; 
            } else {
                // $("#manager_To_date").val("");
                managerrec += '<td>'
            }
            managerrec += '</td>';
            
            managerrec += '</tr>';
            $('#manager tbody').append(managerrec);
        }

        $('#manager tbody').append("<tr><div class='col-2'><td></td></div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div class='col-3'>   <td> </td>  </div>" +
                                "<div clawss='col-3'>   <td> </td>  </div>" + 
                                "<div class='col-1'><td><label id=manager_add_" + i + " class='fa fa-plus' data-role='txn-add' " + "data-control='button'></label></td></div></tr>");
        //end of rendering manager data
      //   alert("loc0 >>>>>>>>>>>>>>>>>> "+JSON.stringify(shiftOptions, null, 4));
         
         select_HTML = '<option value="select">Select Shift</option>';
	     counter = 0;
              // alert(shiftOptions.length);
            if (shiftOptions.length > 0) {
                for (counter = 0; counter < shiftOptions.length; counter += 1) {
                  //  alert(shiftOptions[counter]['shift_code']);
                    select_HTML += '<option value="' + shiftOptions[counter]['shift_code'] + '">' + shiftOptions[counter]['shift_desc'] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">No Shifts found</option>';
            }
         // alert(select_HTML);
     //   $(".datepicker").datepicker();
        
    }
   
    _initOther = function () {


        var shift_date_chkbox = $('#shift_date_chkbox'),
            manager_data_chkbox = $('#line_manager'),
            $file_upload = $("#file_upload"),
            $import_result = $("#importResult"),
            $filter_company_code = $("#filter_company"),
            employee_code = $('#employee_data');
            companycode = $('#filter_company'),
            cancelButton = $('#cancelBtn'),
            filterButton_employeeBtn = $('#filterButton_employee'),
            closeBtn = $('#closeBtn');
            
        var $child_date_of_birth = $('#child_date_of_birth');
        
        /*  $child_date_of_birth.Zebra_DatePicker({
            format: 'd-M-Y'
        }); */

//        datepickr('.datepickr', { dateFormat: 'd-m-Y'});
//        datepickr('.calendar-icon', { altInput: document.getElementById('calendar-input') });
        
        shift_date_chkbox.click(function () {
            OtherData.get(employee_code.val());
        });
        manager_data_chkbox.click(function () {
            OtherDataManager.get(employee_code.val());
            //(employee_code.val());
        });
        $file_upload.change(function (event) {
            event.preventDefault();
            _doUpload(this);
        });

        $filter_company_code.change(function (event) {
            event.preventDefault();
            getManagerData(companycode.val());

        });
        //on cancel resetting form
        cancelButton.click(function () {
            $('#saveForm')[0].reset();
        });
/*
         $('.datepicker:last').Zebra_DatePicker({
            format: 'd-M-Y'
        }); */


        closeBtn.click(function () {
            $('#saveForm')[0].reset();
        });
       
        

    };

    _initButtons = function () {
        var emp_code = $('#emp_code');
        var company_code = $("#filter_company");
        var branch_code = $("#filter_branch");
        var emp_data = $('#employee_data');
        var $import_box = $("#importLeavesBox")
        var role = '',
            button_actions =
            {
                "import/toggle": function () {
                    $import_box.slideToggle();
                },
                "import/transaction": function (event) {
                    _doImport(event);
                    //importEmployeeTransactionRoster(event);
                },
                'employee-save': function (event) {
                     saveEmployeeData();
                },
                'txn-del': function (event) {
                    deleteTransaction(event);
                },
                'txn-add': function (event) {
                    addTransaction(event);
                },
                 'txn-save': function (event) {
                    saveTransaction(event);
                },
                "employee/export": function (event) {
                    _doExport(event);
                },
                "toggle-filters": function (event) {
                   // getCompanyData();
                },
                "employee-filter": function (event) {

                  //  getCompanyDataForEmployee();
                    getEmployeeTransactionData(emp_data.val(), company_code.val(), branch_code.val());
                },
                "filters/data": function (event) {

                    getOtherData(emp_code.val());
                    getEmployeeTransactionData(emp_code.val(), company_code.val(), branch_code.val());
                }

                //
            };

        $('body').on('click', '[data-control="button"]', function (event) {
            
            role = $(event.target).data('role');
         //   alert(role);
            button_actions[role].call(this, event);
        });

    };



    main = function () {

        // set the Employee ID to same as the logged in user.
        // _setEmployeeID();

        _initButtons();
        _initOther();
         
         $('body').on('focus',".df", function(){
           // alert('came here');
            var $j = jQuery.noConflict();
            $j(".df").datepicker();
         });
         /*
         $( function() {
             $( "#datepicker" ).datepicker();
         } );*/
         
    }
    return {
        'main': main
    };

})(jQuery, window, document);



    
$(function () {
    Employee_data_transaction.main();

});

$(".delImgAnchr").on("click", function () {
   // alert("In delete");
});


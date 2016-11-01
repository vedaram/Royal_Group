/********************chking employee applied leave on passed date************************/
 
var weekendArray = [];
function chkStatus(employee_id, shift_id, shift_value, day) {

    day = Number(day);
    if (day < 10) {

        day = '0' + day.toString();

    }
    var calenderVal = $("#calendar").val();
    var appendDate = day + '-' + calenderVal.toString();


    // alert(appendDate + " " + employee_id + " " + shift_id + " " + shift_value); 

    valiadateShiftAssign(employee_id, appendDate, shift_id);

}


function getTotalMonthHour(employee_id)
{
    var calenderVal = $("#calendar").val().split('-');
    var month = calenderVal[0];

    return SAXHTTP.AJAX(
           "roster.aspx/valiadateMonthHours",
           { emp_id: employee_id, month: month }
            
       )
       .done(function (data) {

           var status = data.d.status;

           if (status == "success") {
               var idValue='total_'+employee_id;
               var totalMonthHoursId = $('#' + idValue + '');
                
               totalMonthHoursId.val(data.d.return_data);
               
             //  SAXAlert.show({ type: data.d.status, message: data.d.return_data });

           }
           else {
               
               SAXAlert.show({ type: data.d.status, message: data.d.return_data });
               // SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
           }
       });
}

  function getSelectedShiftValueByPaasingId(shift_id) {
    var weekendDropdownId = $('#' + shift_id + '');
    return weekendDropdownId.val();
    }
//chkStatus on Weekend
function chkStatusForWeekend(employee_id, shift_id, shift_value, day) {


    day = Number(day);
    if (day < 10) {

        day = '0' + day.toString();

    }
    var calenderValTxt = $("#calendar").val();
    var appendDateTxt = day + '-' + calenderValTxt.toString();
    valiadateShiftAssign(employee_id, appendDateTxt, shift_id);


    var weekendDropdownId = $('#' + shift_id + '');

    day = Number(day);
    var beginIndex = 0, endIndex = 0;
    var shiftIdValues = "";
    if (day <= 5) {
        beginIndex = 1;
        endIndex = day;
        var count = 0;
        for (var i = beginIndex; i <= endIndex; i++) {
            var appendId = employee_id + '_' + i;
            if (count == 0) {
                shiftIdValues = getSelectedShiftValueByPaasingId(appendId);
            }
            else {
                shiftIdValues = shiftIdValues + ',' + getSelectedShiftValueByPaasingId(appendId);
            }
            count++;
        }
    } else {
        beginIndex = day - 6;
        endIndex = day;
        var count2 = 0;
        for (var i = beginIndex; i <= endIndex; i++) {
            var appendId = employee_id + '_' + i;
            if (count2 == 0) {
                shiftIdValues = getSelectedShiftValueByPaasingId(appendId);
            }
            else {
                shiftIdValues = shiftIdValues + ',' + getSelectedShiftValueByPaasingId(appendId);
            }
            count2++;
        }

    }
    beginIndex = Number(beginIndex);
    if (beginIndex < 10) {

        beginIndex = '0' + beginIndex.toString();

    }
    endIndex = Number(endIndex);
    if (endIndex < 10) {

        endIndex = '0' + endIndex.toString();

    }
    //alert(day);
    //alert(day);
    var calenderVal = $("#calendar").val();
    var appendDate = day + '-' + calenderVal.toString();
    var weekFromDate = beginIndex + '-' + calenderVal.toString();
    var weekToDate = endIndex + '-' + calenderVal.toString();
    // alert(beginIndex);
    valiadateShiftHours(shiftIdValues, weekFromDate, weekToDate, employee_id, shift_id);

    // alert(appendDate + " " + employee_id + " " + shift_id + " " + shift_value);




}

/******************************************************************************/



/******************************************************************************/
function valiadateShiftHours(shiftData, weekFromDate, weekToDate, employee_id, shift_id) {

    return SAXHTTP.AJAX(
            "roster.aspx/valiadateShiftHours",
            { shiftData: shiftData, weekFromDate: weekFromDate, weekToDate: weekToDate, emp_id: employee_id }
            //{employees: JSON.stringify(final_data), date: date.format("MM-YYYY")}
        )
        .done(function (data) {

            var status = data.d.status;

            if (status == "success")
            {

                //data = JSON.parse(data.d.return_data);



                var appendTotalId = shift_id + '_total_hrs';
                var appendTotalWeekId = shift_id + '_total_week_hrs';
                var appendRedSpanId = shift_id + '_red_span';
                var appendGreenSpanId = shift_id + '_green_span';

                var totalHrstxtId = $('#' + appendTotalId + '');
                var greenSpanId = $('#' + appendGreenSpanId + '');
                var redSpanId = $('#' + appendRedSpanId + '');
                var shiftTotalHrsSelected = data.d.return_data;
                var shiftTotalHrsSelectedArray = data.d.return_data.split(',');

                // alert(shiftTotalHrsSelectedArray[0]);
                // alert(shiftTotalHrsSelectedArray[1]);

                shiftTotalHrsSelectedArray[0] = Number(shiftTotalHrsSelectedArray[0]);
                shiftTotalHrsSelectedArray[1] = Number(shiftTotalHrsSelectedArray[1]);
                if (shiftTotalHrsSelectedArray[0] > shiftTotalHrsSelectedArray[1])
                { 
                    //greenSpanId.hide();
                    greenSpanId.removeClass('text-green');
                    greenSpanId.addClass('text-red');
                   // redSpanId.show();
                } else {
                    greenSpanId.addClass('text-green');
                    greenSpanId.removeClass('text-red');
                }
                var detils = "TSH: " + shiftTotalHrsSelectedArray[0] + " TSH/W: " + shiftTotalHrsSelectedArray[1];
                totalHrstxtId.val(detils);
                var shiftDropDown = $('#' + shift_id + '');
                shiftDropDown.css("border-color", "#dddddd");
              //  SAXAlert.show({ type: data.d.status, message: data.d.return_data });

            }
            else {

                if (data.d.return_data == '1') {
                    var shiftDropDown = $('#' + shift_id + '');
                    shiftDropDown.css("border-color", "red");
                    //shiftDropDown.val('select');
                    //data.d.return_data="Employee is having Holiday for this date, So you can't assign Shift";
                    data.d.return_data = "please select at least one day-off";
                }
                else if (data.d.return_data == '3') {

                    data.d.return_data = "An error occurred while loading validating  shift data. Please try again. If the error persists, please contact Support";
                }
                SAXAlert.show({ type: data.d.status, message: data.d.return_data });
                // SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
            }
        });

}

/******************************************************************************/




function valiadateShiftAssign(employee_id, date, shift_id) {

    return SAXHTTP.AJAX(
           "roster.aspx/valiadateShiftAssign",
           { employee_id: employee_id, date: date }
           //{employees: JSON.stringify(final_data), date: date.format("MM-YYYY")}
       )
       .done(function (data) {

           var status = data.d.status;

           if (status == "success") {

               data = JSON.parse(data.d.return_data);
               var shiftDropDown = $('#' + shift_id + '');
               var shiftDropDownValue = shiftDropDown.val();
               if (shiftDropDownValue == 'leave')
                   {
                       //data.d.return_data = "You are selecting Day off on leave";
                       SAXAlert.show({ type: "error", message: "You can't assign Leave because , Employee does't have approved leave on this day" });
                       shiftDropDown.val('select');
                   }
                    else if (shiftDropDownValue == 'holiday')
                   {
                       //data.d.return_data = "You are selecting Day off on leave";
                       SAXAlert.show({ type: "error", message: "You can't assign Holiday because , Employee does't have Holiday on this day" });
                       shiftDropDown.val('select');
                   }


           }
           else {
               var shiftDropDown = $('#' + shift_id + '');

               if (data.d.return_data == '1') {

                   data.d.return_data = "Employee is having Holiday for this date, So you can't assign Shift";
                   shiftDropDown.val('holiday');

               }
               else if (data.d.return_data == '2') 
               {
                   var shiftDropDownValue = shiftDropDown.val();
                   if (shiftDropDownValue == 'woff')
                   {
                       data.d.return_data = "You are selecting Day off on leave";
                       shiftDropDown.val('woff');
                   }
                   else
                   {
                       data.d.return_data = "Employee is on Leave, So you can't assign Shift";
                       shiftDropDown.val('leave');
                   }
                  
               }
               else if (data.d.return_data == '3') {

                   data.d.return_data = "An error occurred while loading validating  shift data. Please try again. If the error persists, please contact Support";
                   shiftDropDown.val('select');
               }
               SAXAlert.show({ type: data.d.status, message: data.d.return_data });
               // SAXAlert.show({ type: "error", message: "An error occurred while saving Employee details. Please try again." });
           }
       });

}

/**********************end of chking employee applied leave on passed date**************************************/
/********************************************************************* checking for bulk and individual employee ***********************************/
function app_handle_listing_horisontal_scroll(listing_obj) {
    //get table object   
    table_obj = $('.table', listing_obj);

    //get count fixed collumns params
    var count_fixed_collumns = table_obj.attr('data-count-fixed-columns')

    if (count_fixed_collumns > 0) {
        //get wrapper object
        wrapper_obj = $('.table-scrollable', listing_obj);

        wrapper_left_margin = 0;

        table_collumns_width = new Array();
        table_collumns_margin = new Array();

        //calculate wrapper margin and fixed column width
        $('th', table_obj).each(function (index) {
            if (index < count_fixed_collumns) {
                wrapper_left_margin += $(this).outerWidth();
                table_collumns_width[index] = $(this).outerWidth();
            }
        })

        //calcualte margin for each column  
        $.each(table_collumns_width, function (key, value) {
            if (key == 0) {
                table_collumns_margin[key] = wrapper_left_margin;
            }
            else {
                next_margin = 0;
                $.each(table_collumns_width, function (key_next, value_next) {
                    if (key_next < key) {
                        next_margin += value_next;
                    }
                });

                table_collumns_margin[key] = wrapper_left_margin - next_margin;
            }
        });

        //set wrapper margin               
        if (wrapper_left_margin > 0) {
            wrapper_obj.css('cssText', 'margin-left:' + wrapper_left_margin + 'px !important; width: auto')
        }

        //set position for fixed columns
        $('tr', table_obj).each(function () {

            //get current row height
            current_row_height = $(this).outerHeight();

            $('th,td', $(this)).each(function (index) {

                //set row height for all cells
                $(this).css('height', current_row_height)

                //set position 
                if (index < count_fixed_collumns) {
                    $(this).css('position', 'absolute')
                           .css('margin-left', '-' + table_collumns_margin[index] + 'px')
                           .css('width', table_collumns_width[index])

                    $(this).addClass('table-fixed-cell')
                }
            })
        })
    }
}

function uncheck(getId,date,fridayarray)
{
  
    var getAllId = $('#' + getId + '');
    
    
    var selectfromdropdown = 'main_select_';
    if(getAllId.is(':checked')==false)   // making select all employee false
    {
        $('#checkallemployee').attr('checked', false);
    }
    else
    {
        var shift_Roster_Table = $('#shiftRosterTable'),
            get_employeeId = $('#' + getId + '').val();  // from check box getting each employee id value

      //   alert(get_employeeId);
        //   alert(date);

        var array = fridayarray.split(',');
       
        

        for( var i=1;i<=date;i++)
        {
         
            var dropdownvalue = $('#' + selectfromdropdown + i + '').val();//main drop down value
            $('#' + get_employeeId + '_' + i + '').val(dropdownvalue);//

            var dropdownidForWeekDays = get_employeeId + '_' + i;
            chkStatus(get_employeeId, dropdownidForWeekDays, dropdownvalue, i);

            for (var j = 0; j < array.length; j++)
            {
                if(array[j]==Number(i))
                {
                    var dropdownid = get_employeeId + '_' + i;
                    chkStatusForWeekend(get_employeeId, dropdownid, dropdownvalue, i);
                }

            }
            //   if (weekendArray.indexOf(j) >= 0)
             

            //if (fridayarray.indexOf(Number(i))>= 0)//indexOf
            //{
            //    alert(i);
            //    var dropdownid = $('#' + selectfromdropdown + i + '');
            //    var k;
            //    chkStatusForWeekend(get_employeeId, dropdownid, dropdownvalue, i);

            //}

        }
     //   alert(fridayarray.toString());
        
        //chkStatusForWeekend(10,this.id,this.value,9);
    }

}


function appendIdToHeaderDropdown(beforeString, newStringWithID)
{

     var replace = '<select class="form-control">';
     var res = beforeString.replace(replace, newStringWithID);
     return res;
}
//function test(headerdropdownid,columnid)
//{
//    var commonselect, commonselect1, commonselect2;

//    var appendTotalId = headerdropdownid;

//    var individualdropdown = $('#' + appendTotalId + '');

//    $('#checkallemployee').attr('checked', false);
//   // $("#shiftRosterTable").find('tbody input[type="checkbox"]').attr('checked', false);
//        commonselect2 = individualdropdown.val();
//        columnid = Number(columnid) - 1;
        
//        $('#checkallemployee').change(function () {
//            if ($('#checkallemployee').is(':checked') == true )
//            {
//                $('#shiftRosterTable tr').each(function () {              
//                    var selectalltd = $(this).find('td:eq(' + columnid + ')').find('select');
//                    selectalltd.val(commonselect2);

//                });
//            }
//        });

//}
//
 function test(headerdropdownid, columnid, date)
 {
    var commonselect, commonselect1, commonselect2;

    var appendTotalId = headerdropdownid;

    var individualdropdown = $('#' + appendTotalId + '');

    $('#checkallemployee').attr('checked', false);
    // ($("#shiftRosterTable tbody input[type='checkbox']").is(':checked') == true  ("#shiftRosterTable").find('tbody input[type="checkbox"]').attr('checked', false);
    commonselect2 = individualdropdown.val();
    columnid = Number(columnid) - 1;

    $('#checkallemployee').change(function ()
    {

       if (($('#checkallemployee').is(':checked') == true) )

       {

        $('#shiftRosterTable tr').each(function () {

                var selectalltd = $(this).find('td:eq(' + columnid + ')').find('select'); 
                selectalltd.val(commonselect2);
            //

                $("#shiftRosterTable tr ").find("input:checked").each(function () {
                    var current_empid = $(this).val();
                    if (current_empid != 'CommonShift') {

                        var number = Number(columnid) + 1;
                        var dropdownid = current_empid + '_' + number;
                        var dropdownidvalue = getSelectedShiftValueByPaasingId(dropdownid);
                        chkStatus(current_empid, dropdownid, dropdownidvalue, number);
                    }
                });
        });

        //$("#shiftRosterTable tr ").find("input:checked").each(function () {
        //    var current_empid = $(this).val();
        //    if (current_empid != 'CommonShift')
        //    {    
          
        //    var number = Number(columnid) + 1;
        //    var dropdownid = current_empid + '_' + number;
        //    var dropdownidvalue=getSelectedShiftValueByPaasingId(dropdownid);
        //    chkStatus(current_empid, dropdownid, dropdownidvalue, number);
        //        //chkStatus(get_employeeId, dropdownidForWeekDays, dropdownvalue, i);
        //}
        //});
        
    }

    });
   
   $("#shiftRosterTable tr").find("input:checked").each(function () {
       var current_empid = $(this).val();
       var selectfromdropdown = 'main_select_';
       var headerdropdownidarray = headerdropdownid.split('_');
       date = Number(headerdropdownidarray[2]);
   
      
               var dropdownvalue = $('#' + selectfromdropdown + date + '').val(); //main drop down value
               $('#' + current_empid + '_' + date + '').val(dropdownvalue);//
 

       var number = Number(columnid) + 1;
       var dropdownid = current_empid + '_' + number;
       var dropdownidvalue = getSelectedShiftValueByPaasingId(dropdownid);
       chkStatus(current_empid, dropdownid, dropdownidvalue, number);
   
   });


}


function ChkStatusOnWeeKendOfMainSelect(headerdropdownid, columnid, date) {
    var commonselect, commonselect1, commonselect2;

    var appendTotalId = headerdropdownid;

    var individualdropdown = $('#' + appendTotalId + '');

    $('#checkallemployee').attr('checked', false);
    // ($("#shiftRosterTable tbody input[type='checkbox']").is(':checked') == true  ("#shiftRosterTable").find('tbody input[type="checkbox"]').attr('checked', false);
    commonselect2 = individualdropdown.val();
    columnid = Number(columnid) - 1;

   $('#checkallemployee').change(function () {

        if (($('#checkallemployee').is(':checked') == true)) {

            $('#shiftRosterTable tr').each(function () {

                var selectalltd = $(this).find('td:eq(' + columnid + ')').find('select');
                selectalltd.val(commonselect2);
                //

                $("#shiftRosterTable tr ").find("input:checked").each(function () {
                    var current_empid = $(this).val();
                       if (current_empid != 'CommonShift') {

                            var number = Number(columnid) + 1;
                            var dropdownid = current_empid + '_' + number;
                            var dropdownidvalue = getSelectedShiftValueByPaasingId(dropdownid);
                            chkStatusForWeekend(current_empid, dropdownid, dropdownidvalue, number);
                        }
                    });

            });

            //$("#shiftRosterTable tr ").find("input:checked").each(function () {
            //    var current_empid = $(this).val();
            //    if (current_empid != 'CommonShift') {

            //        var number = Number(columnid) + 1;
            //        var dropdownid = current_empid + '_' + number;
            //        var dropdownidvalue = getSelectedShiftValueByPaasingId(dropdownid);
            //        chkStatusForWeekend(current_empid, dropdownid, dropdownidvalue, number);
            //    }
            //});

        }

    });

    $("#shiftRosterTable tr").find("input:checked").each(function () {
        var current_empid = $(this).val();
        var selectfromdropdown = 'main_select_';

        // for (var i = 1; i <= date ; i++) {

        var headerdropdownidarray = headerdropdownid.split('_');
        date = Number(headerdropdownidarray[2]);
        var dropdownvalue = $('#' + selectfromdropdown + date + '').val(); //main drop down value
        $('#' + current_empid + '_' + date + '').val(dropdownvalue);//

        var number = Number(columnid) + 1;
        var dropdownid = current_empid + '_' + number;
        var dropdownidvalue = getSelectedShiftValueByPaasingId(dropdownid);
        chkStatusForWeekend(current_empid, dropdownid, dropdownidvalue, number);

    });
    //
    //if (($('#checkallemployee').is(':checked') == true))
    //{
    //    //$("#shiftRosterTable tr").find("input:checked").each(function () {
    //    //    var current_empid = $(this).val();
    //    //    alert(current_empid);
    //    //    var number = Number(columnid) + 1;
    //    //    var dropdownid = current_empid + '_' + number;

    //    //  //  chkStatusForWeekend(current_empid, dropdownid, '', number);

    //    //});

    //}

    //


}

//function onSelectAllChkBox() {

//    if (($('#checkallemployee').is(':checked') == true)) {

//        $('#shiftRosterTable tr').each(function () {

//            var selectalltd = $(this).find('td:eq(' + columnid + ')').find('select');
//            selectalltd.val(commonselect2);
//            //

//            $("#shiftRosterTable tr ").find("input:checked").each(function () {
//                var current_empid = $(this).val();
//                if (current_empid != 'CommonShift') {

//                    var number = Number(columnid) + 1;
//                    var dropdownid = current_empid + '_' + number;
//                    var dropdownidvalue = getSelectedShiftValueByPaasingId(dropdownid);
//                    chkStatus(current_empid, dropdownid, dropdownidvalue, number);
//                }
//            });
//        });


//    }
//}

//
//
function scrolling()
{
    app_handle_listing_horisontal_scroll($('#table-listing'));
}

function checkboxclick() {

    var $table1 = $("#shiftRosterTable");
    $("#checkallemployee").change(function () {
        var is_checked = $(this).is(':checked'),
          checkboxes = $table1.find('tbody input[type="checkbox"]');
        is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);

    });

}
/**********************End of all function  related to shift roaster select option **************************************/

$(function () {

    var Company = (function ($, w, d) {

        var
			$company = $("#filter_company"),
			$department = $("#filter_department"),
			$designation = $("#filter_designation"),
			$employee_category = $("#filter_employee_category");

        function _renderDropdown($element, data, key, value, default_text, no_data_text) {

            var
				data_length = data.length,
	            select_HTML = '<option value="select">' + default_text + '</option>',
	            counter = 0;

            if (data_length > 0) {
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter][key] + '">' + data[counter][value] + '</option>';
                }
            }
            else {
                select_HTML = '<option value="select">' + no_data_text + '</option>';
            }

            $element.empty().append(select_HTML);
        }

        function getCompanyData() {
            return SAXHTTP.AJAX(
					"roster.aspx/GetCompanyData",
					{ company_code: $company.val() }
				)
				.done(function (data) {
				    var status = data.d.status;

				    if (status == "success") {

				        data = JSON.parse(data.d.return_data);

				        _renderDropdown($department, data["department"], "department_code", "department_name", "Select a Department", "No Departments found");
				        _renderDropdown($designation, data["designation"], "designation_code", "designation_name", "Select a Designation", "No Designations found");
				        _renderDropdown($employee_category, data["employee_category"], "employee_category_code", "employee_category_name", "Select a Employee Cateogry", "No Employee Categories found");
				    }
				});
        }

        function getAllCompanies() {

            var
				default_text = "Select a Company",
				no_data_text = "No Companies found";

            return SAXHTTP.AJAX(
				"roster.aspx/GetAllCompanies",
				{}
			)
			.done(function (data) {
			    if (data.d.status == "success") {
			        data = JSON.parse(data.d.return_data);
			        _renderDropdown($company, data, "company_code", "company_name", default_text, no_data_text);
			    }
			})
			.fail(function () {
			    SAXAlert.show({ type: "error", message: "An error occurred while loading data. Please try again." });
			});
        }

        return {
            get: getAllCompanies,
            other: getCompanyData
        };

    })(jQuery, window, document);

    /******************************************************************************************************************/

    var ShiftRosterView = (function ($, w, d) {

        var
            list_elements = {
                table: $("#employeeTable"),
                roster: $("#shiftRosterTable")
            },
            tabs = {
                employee: $("#employeeTab"),
                shift: $("#rosterTab")
            },
	        $company_code = {
	            ref: $("#filter_company"),
	            get value() {
	                return this.ref.val();
	            },
	            set value(company_code) {
	                this.ref.val(company_code);
	            }
	        };
        /*******************************************End of save shift roaster****************************************/
        function saveShiftRoster() {
                    
            

            //  code added
            var selected_shiftselectedpageemployees = $('#shiftRosterTable').find("tbody input:checked"),
              selected_shiftselectedpageemployees_length = selected_shiftselectedpageemployees.length,
              employee1_length = 0,
              employees = [];
            j = 0,
            date = moment($("#calendar").val(), "MM-YYYY"),
            final1_data = [];

            var end_of_Last_Month=date.endOf('month').date();

               var newWeekendArray=[],newWeekendCounter=0;
                    for (i = 1; i <= end_of_Last_Month ; i += 1) 
                    {
                    var header_date1 = i + '-' + (date.month() + 1) + '-' + date.year();
                    header_date1 = moment(header_date1, "DD-MM-YYYY");
                    if (header_date1.format("dddd") == 'Friday') {
                    newWeekendArray[newWeekendCounter] = i;
                    newWeekendCounter++;
                    }
                     }
                

            //dialog part 
            //getting count of redlabels
            var redLabelCount = $('table tbody tr th span.text-red').length;

            // alert(redLabelCount);
            if (redLabelCount > 0) {
                var message = 'Error please check the data, Do you want to continue YES/NO?';
                var $redLabel_confirmation_dialog = $('#checkRedLabelDialog'), $ooo_confirmation_button = $('#confirmshift');
                $redLabel_confirmation_dialog.find('.modal-body').html('<p>' + message + '</p>');
                $redLabel_confirmation_dialog.modal('show');
            }
             else {


                //end of dialog part
                SAXLoader.show();

                for (m = 0; m < selected_shiftselectedpageemployees_length ; m += 1) {
                    employees.push($(selected_shiftselectedpageemployees[m]).val());
                }
                employee1_length = employees.length;

                for (i = 0; i < employee1_length; i += 1) {
                    // dropdowns = list_elements.roster.find("tr#" + employees[i] + " select");
                    dropdowns = $('#shiftRosterTable').find("tr#" + employees[i] + " select");
                    dropdown_length = dropdowns.length;

                    final1_data.push({ employee_code: employees[i], shifts: [] })
                    for (j = 0; j < dropdown_length; j += 1) {
                        final1_data[i]["shifts"].push($(dropdowns[j]).val());
                    }
                }
                // upto
              
                SAXHTTP.AJAX(
                        "roster.aspx/SaveShiftRoster",
                        { employees: JSON.stringify(final1_data), date: date.format("MM-YYYY"), totalShifts: end_of_Last_Month ,ArrayofWeekends:newWeekendArray}
                    )
                    .done(function (data) {
                        SAXAlert.show({ type: data.d.status, message: data.d.return_data });
                    })
                    .fail(function () {
                        SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again." });
                    })
                    .always(SAXLoader.close);
            }//end of else block	

        }

        /****End of save shift roaster**/
        /*new function need to add*/
        /******************************************************************************* save shift roster on dialog *************************************/

        function saveShiftRosterOnDialog() {
        var end_of_Last_Month=date.endOf('month').date();
            var $redLabel_confirmation_dialog = $('#checkRedLabelDialog');
            $redLabel_confirmation_dialog.modal('hide');

            var selected_shiftselectedpageemployees = $('#shiftRosterTable').find("tbody input:checked"),
                      selected_shiftselectedpageemployees_length = selected_shiftselectedpageemployees.length,
                      employee1_length = 0,
                      employees = [];
            j = 0,
            date = moment($("#calendar").val(), "MM-YYYY"),
            final1_data = [];

         var newWeekendArray=[],newWeekendCounter=0;
                    for (i = 1; i <= end_of_Last_Month ; i += 1) 
                    {
                    var header_date1 = i + '-' + (date.month() + 1) + '-' + date.year();
                    header_date1 = moment(header_date1, "DD-MM-YYYY");
                    if (header_date1.format("dddd") == 'Friday') {
                    newWeekendArray[newWeekendCounter] = i;
                    newWeekendCounter++;
                    }
                     }

            SAXLoader.close();

            for (m = 0; m < selected_shiftselectedpageemployees_length ; m += 1) {
                employees.push($(selected_shiftselectedpageemployees[m]).val());
            }
            employee1_length = employees.length;

            for (i = 0; i < employee1_length; i += 1) {
                // dropdowns = list_elements.roster.find("tr#" + employees[i] + " select");
                dropdowns = $('#shiftRosterTable').find("tr#" + employees[i] + " select");
                dropdown_length = dropdowns.length;

                final1_data.push({ employee_code: employees[i], shifts: [] })
                for (j = 0; j < dropdown_length; j += 1) {
                    final1_data[i]["shifts"].push($(dropdowns[j]).val());
                }
            }


            SAXHTTP.AJAX(
	    			"roster.aspx/SaveShiftRoster",
	    			{ employees: JSON.stringify(final1_data), date: date.format("MM-YYYY"), totalShifts: end_of_Last_Month,ArrayofWeekends:newWeekendArray }
	    		)
	    		.done(function (data) {
	    		    //cloding dialog
	    		    //  redLabel_confirmation_dialog.close();
	    		    SAXAlert.show({ type: data.d.status, message: data.d.return_data });
	    		    // redLabel_confirmation_dialog.hide();
	    		})
	    		.fail(function () {
	    		    SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again." });
	    		})
	    		.always(SAXLoader.close);
        }
        /*******************************************************************************  End of save shift roster on dialog *************************************/


        function _showShiftTab() {

            tabs.employee.addClass("hide");
            tabs.shift.removeClass("hide");
        }

        /*
			This function processes all the shifts and creates the HTML.
    	*/
        function _renderShiftsDropdown(data) {

            var
				data_length = data.length,
	            select_HTML = '',
	            counter = 0;

            if (data_length > 0) {
                select_HTML = '<select class="form-control">' +
					'<option value="select">Select a Shift</option>' +
					'<option value="woff">Day Off</option>' + '<option value="leave">Leave</option>' + '<option value="holiday">Holiday</option>';
                for (counter = 0; counter < data_length; counter += 1) {
                    select_HTML += '<option value="' + data[counter]["shift_code"] + '">' + data[counter]["shift_name"] + '</option>';
                }
                select_HTML += '</select>';
            }
            else {
                select_HTML = '<select class="form-control">' +
	            				'<option value="select">No Shifts found</option>' +
        					'</select>';
            }

            return select_HTML;
        }

        /*
			This function sets up the header for the table.
	    */
        //weekend array
    
        function _setupShiftRosterHeader(data,date,employees) {

            var
	    		i = 0, base_HTML = "", header_date,
	    		end_of_month = date.endOf("month").date();

            base_HTML = '<tr> <th style="font-size:10px" > Select All</th> <th style="font-size:10px;"> Employee Name</th> <th style="white-space:nowrap;width:100px;font-size:10px;">MMWH </th> ';  // start of first row of header
            var weekendCounter = 0;
            for (i = 1; i <= end_of_month; i += 1) {
                header_date = i + '-' + (date.month() + 1) + '-' + date.year();
                header_date = moment(header_date, "DD-MM-YYYY");
                base_HTML += '<th style="white-space:nowrap">' + i + ' ' + header_date.format("dddd") + '</th>';

                if (header_date.format("dddd") == 'Friday') {
                    weekendArray[weekendCounter] = i;
                    weekendCounter++;
                    base_HTML += '<th></th>';

                }
            }
   

            base_HTML += '</tr>';  //  closing here first row of header 
            select_HTML = _renderShiftsDropdown(data["all_shifts"]);
           // select_HTML = '<select></select>';
            // second row starts

           // base_HTML += '<tr> <th style="text-align: center; vertical-align: middle;"><input type="checkbox" id="checkallemployee"></th> <th> CommonShift  </th>  ';
            base_HTML += '<tr> <th ><input type="checkbox" id="checkallemployee" value="CommonShift"></th> <th style="font-size:10px;"> CommonShift  </th> <th> </th>';
         //   base_HTML += '<tr> <th ><input type="checkbox" id="checkallemployee"></th> <th> CommonShift  </th>  ';
            for (j = 1; j <= end_of_month; j += 1)
            {
                
                
                var idValue = 'main_select_' + j;
                var addIdToAllHeaderDropdown = '<select style="width:138px; class="form-control" id="' + idValue + '" onChange=test("' + idValue + '","' + j + '","' + end_of_month + '");>';
                var newSelect_Html = appendIdToHeaderDropdown(select_HTML, addIdToAllHeaderDropdown);

                if (weekendArray.indexOf(j) >= 0)
                {

                     addIdToAllHeaderDropdown = '<select style="width:138px; class="form-control" id="' + idValue + '" onChange=ChkStatusOnWeeKendOfMainSelect("' + idValue + '","' + j + '","' + end_of_month + '");>';
                     newSelect_Html = appendIdToHeaderDropdown(select_HTML, addIdToAllHeaderDropdown);

                }

                base_HTML += '<th>' + newSelect_Html + '</th>';
                if (weekendArray.indexOf(j) >= 0) {

                    base_HTML += '<th></th>';
                }
            }

            base_HTML += '</tr>';
             // end of second row
            list_elements.roster.find("thead").append(base_HTML);
        }

        function appendIdToDropdown(beforeString, newStringWithID) {

            var replace = '<select class="form-control">';
            var res = beforeString.replace(replace, newStringWithID);//<select class="form-control">

            return res;

        }


        function _renderShiftRows(data, employee, date) {

            var
	    		i = 0, j = 0, base_HTML = "",
	    		select_HTML = "",
	    		end_of_month = date.endOf("month").date(),
	    		employee_name = "",
	    		employee_length = employee.length;

            select_HTML = _renderShiftsDropdown(data["all_shifts"]);
            //
            var newWeekendArray=[],newWeekendCounter=0;
            for (i = 1; i <= end_of_month; i += 1) {
                var header_date1 = i + '-' + (date.month() + 1) + '-' + date.year();
                header_date1 = moment(header_date1, "DD-MM-YYYY");
                if (header_date1.format("dddd") == 'Friday') {
                    newWeekendArray[newWeekendCounter] = i;
                    newWeekendCounter++;
                }

            }
            //

            for (i = 0; i < employee_length; i += 1) {

                employee_name = ShiftRosterPage.getCollection().get(employee[i]).toJSON()["employee_name"];
                // code added
                var allcheckboxid = 'main_select1_' + i;

                //base_HTML += "<tr id='" + employee[i] + "' title='" + employee_name + " (" + employee[i] + ")' >" +
                //              "<th><input type='checkbox' value='" + employee[i] + "'  id='" + allcheckboxid + "' onchange=uncheck('"+ allcheckboxid +"','" + end_of_month + "','"+newWeekendArray+"') ></th><th>" + employee_name + "<small>(" + employee[i] + ")</small></th>";
                getTotalMonthHour(employee[i]);
                var totalMonthText = 'total_' + employee[i];
                base_HTML += "<tr id='" + employee[i] + "' title='" + employee_name + " (" + employee[i] + ")' >" +
                             "<th><input type='checkbox' value='" + employee[i] + "'  id='" + allcheckboxid + "' checked='true'  onchange=uncheck('" + allcheckboxid + "','" + end_of_month + "','" + newWeekendArray + "') ></th><th style='font-size:10px;'>" + employee_name + "<small>(" + employee[i] + ")</small></th> <th> <input type='text' id='" + totalMonthText + "' readonly='true' style='width: 60px;border-style: none;color:green ;font-weight:bold;text-align:center'  /> </th>";
                // upto
                //base_HTML += '<tr id="' + employee[i] + '" title="' + employee_name + ' (' + employee[i] + ')" >' +     
            	//						'<th>' + employee_name + '<small>(' + employee[i] + ')</small></th>';

                     
                       for (j = 1; j <= end_of_month; j += 1) 
                        
                       {

                    var header_date = j + '-' + (date.month() + 1) + '-' + date.year();
                    header_date = moment(header_date, "DD-MM-YYYY");
                                     
                   // alert(header_date.format("dddd") == 'Friday');
                   
                    var idValue = employee[i] + "_" + j;
                    var idValueForTotalTxt = employee[i] + "_" + j + "_total_hrs";
                    var idValueForTotalWeekHrsTxt = employee[i] + "_" + j + "_total_week_hrs";
                    var idValueForRedSpan = employee[i] + "_" + j + "_red_span";
                    var idValueForGreenSpan = employee[i] + "_" + j + "_green_span";
                    var employeeId = employee[i];
                    var day = j;
                 
                    // '<th><input type= "checkbox" value="' + employee[i] + '"  /> </th>' + (employee_id,shift_id,shift_value,day)
                    if (header_date.format("dddd") == 'Friday') {

                         
                        var afterAddIdForWeekend = '<select class="form-control" id="' + idValue + '" onchange="chkStatusForWeekend(' + employeeId + ',this.id,this.value,' + day + ');">';
                        var newstringForWeekend = appendIdToDropdown(select_HTML, afterAddIdForWeekend);
                        base_HTML += '<td>' + newstringForWeekend + '</td>';
                        base_HTML += '<th>' + '<span  class="fa fa-circle text-green" id="' + idValueForGreenSpan + '"></span><input type ="text" id="' + idValueForTotalTxt + '" value="TSH: 0 TSH/W: 0" style="width:auto; border-style:none;text-align:center; font-size:10px;" readonly>' + '</th>';
                    }
                    else {
                        var afterAddId = '<select  class="form-control" id="' + idValue + '" onchange="chkStatus(' + employeeId + ',this.id,this.value,' + day + ');">';
                        var newstring = appendIdToDropdown(select_HTML, afterAddId);
                        base_HTML += '<td>' + newstring + '</td>';
                    }



                }

                base_HTML += "</tr>";
            }

            list_elements.roster.find("tbody").append(base_HTML);
        }

        function _setSelectedShifts(data, employees) {

            var
	    		i = 0, j = 0,
	    		shift_length = 0, dropdowns = [],
	    		dropdown_length = 0,
	    		employee_length = employees.length;

            for (i = 0; i < employee_length; i += 1) {
                dropdowns = list_elements.roster.find("tr#" + employees[i] + " select");
                dropdown_length = dropdowns.length;
                for (j = 0; j < dropdown_length; j += 1) {
                    var day = j + 1;
                    if (data[employees[i]].length > 0)
                        if (data[employees[i]][0]["day" + day] != undefined && data[employees[i]][0]["day" + day] != null && data[employees[i]][0]["day" + day] != "")
                            $(dropdowns[j]).val(data[employees[i]][0]["day" + day]);
                }
            }
        }

        function _renderShifts(data, employees, date) {

            var data_length = data.length,
	            employees_length = employees.length,
	            shifts_dropdown = _renderShiftsDropdown(data);
            _renderShiftRows(data, employees, date);
            _setSelectedShifts(data, employees);
            _setupShiftRosterHeader(data,date, employees);
                        checkboxclick();
                        scrolling();

        }

        function getShiftsForEmployees(date) {

            var
                selected_employees = list_elements.table.find("tbody input:checked"),
                selected_employees_length = selected_employees.length,
                employees = [], i = 0;

            if (date != "" && date != undefined && date != null) {
                date = moment(date, "MM-YYYY");
                list_elements.roster.find('thead').empty();
                list_elements.roster.find('tbody').empty();
            }
            else {
                date = moment();
            }


            if (selected_employees_length == 0) {
                SAXAlert.show({ type: "error", message: "Please select at least one employee" });
                return false;
            }

            SAXLoader.show();

            for (i = 0; i < selected_employees_length; i += 1) {
                employees.push($(selected_employees[i]).val());
            }

            SAXHTTP.AJAX(
                    "roster.aspx/GetShiftsForEmployees",
                    { company_code: $company_code.value, date: date.format("MM-YYYY"), employees: JSON.stringify(employees) }
                )
                .done(function (data) {
                    if (data.d.status == "success") {
                        data = JSON.parse(data.d.return_data);
                        _renderShifts(data, employees, date);
                        _showShiftTab();
                    }
                })
                .fail(function () {
                    SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again." });
                })
                .always(function () {
                    SAXLoader.close();
                });
        }

        return {
            get: getShiftsForEmployees,
            save: saveShiftRoster,
            saveOnDialog: saveShiftRosterOnDialog
        };

    })(jQuery, window, document);

    /******************************************************************************************************************/

    var EmployeeList = (function ($, w, d) {

        var
            page_number = {
                current: 1,
                get value() {
                    return this.current;
                },
                set value(new_page_number) {
                    this.current = new_page_number;
                }
            },
            $company_code = {
                ref: $("#filter_company"),
                get value() {
                    return this.ref.val();
                },
                set value(company_code) {
                    this.ref.val(company_code);
                }
            },
            buttons = {
                pagination: $('#paginationButton'),
            },
			forms = {
			    filter: $("#filterForm")
			},
			tabs = {
			    employee: $("#employeeTab"),
			    roster: $("#rosterTab")
			}
        list_elements = {
            table: $('#employeeTable'),
            listview: $('#employeeListView'),
            message: $("#noData"),
            roster: $("#shiftRosterTable")
        };

        /*
            This function will generate the HTML for the table.
        */
        function _getHTML(data) {

            var data_length = data.length,
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

        function _renderTable(data) {

            var table_body;

            list_elements.message.children().length > 0 ? list_elements.message.empty() : 0;

            if (data.length > 0) {
                // if table view is hidden, show the table view
                list_elements.listview.is(":hidden") ? list_elements.listview.show() : 0;

                table_body = list_elements.table.find("tbody");
                // get the HTML and append to the table.
                table_HTML = _getHTML(data);
                table_body.append(table_HTML);
                // hiding the pagination button
                table_body.children().length < page_number.value * 30 ? buttons.pagination.hide() : buttons.pagination.show();
                console.log(ShiftRosterPage.getCollection());
                ShiftRosterPage.getCollection().set(data);
            }
            else {
                list_elements.message.append("<h3>No Employees found</h3>");
                // hdie the table view
                list_elements.listview.hide();
            }
        }

        /*
            Abstracting the AJAX call to get employees. 
            This function will be called multiple times in this module.
        */
        function _getEmployees() {

            var
                filters = SAXForms.get(forms.filter);

            return SAXHTTP.AJAX(
					"roster.aspx/FilterEmployeeData",
					{ page_number: page_number.value, company_code: $company_code.value, filters: JSON.stringify(filters) }
				);
        }

        function resetSearchOptions() {

            SAXLoader.show();

            // store the company code
            // this value will be set once the form is reset
            var company_code = $company_code.value;

            // reset the page number
            page_number.value = 1;

            // reset the filter form
            forms.filter[0].reset();

            // set the company code
            $company_code.value = company_code;

            // empty the table
            list_elements.table.find("tbody").empty();

            _getEmployees()
                .done(function (data) {
                    if (data.d.status == "success") {
                        data = JSON.parse(data.d.return_data);
                        _renderTable(data);
                    }
                    else {
                        SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again" });
                    }
                })
                .fail(function () {
                    SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again" });
                })
                .always(function () {
                    SAXLoader.close();
                });
        }

        function loadMoreEmployees() {

            SAXLoader.show();

            // increment the page number
            page_number.value += 1;

            _getEmployees()
                .done(function (data) {
                    if (data.d.status == "success") {
                        data = JSON.parse(data.d.return_data);
                        _renderTable(data);
                    }
                })
                .fail(function () {
                    SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again" });
                })
                .always(function () {
                    SAXLoader.close();
                });

        }

        function searchEmployees() {
            var company_code = $company_code.value;
            // Show the loader
            SAXLoader.show();

            // empty the table
            list_elements.table.find("tbody").empty();

            // Start the AJAX call.
            _getEmployees()
                .done(function (data) {

                    if (data.d.status == "success") {
                        data = JSON.parse(data.d.return_data);
                        _renderTable(data);
                    }
                    else {
                        SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again." });
                    }
                })
                .fail(function () {
                    SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again" });
                })
                .always(function () {
                    SAXLoader.close();
                });
        }

        function showEmployeeTab() {

            tabs.roster.addClass("hide");
            // clear previous selections
            list_elements.table.find("input:checked").prop("checked", false);
            // unhide the employee tab
            tabs.employee.removeClass("hide");
            // empty the roster header and body
            list_elements.roster.find('thead').empty();
            list_elements.roster.find('tbody').empty();
        }

        return {
            search: searchEmployees,
            more: loadMoreEmployees,
            reset: resetSearchOptions,
            show: showEmployeeTab
        };

    })(jQuery, window, document);

    /******************************************************************************************************************/

    var ShiftRosterPage = (function ($, w, d) {

        var
			$company = $("#filter_company"),
			$table = $("#employeeTable"),
            button_events = {
                "filters/data": EmployeeList.search,
                "employee/more": EmployeeList.more,
                "filters/reset": EmployeeList.reset,
                "next-page": function () {
                    $("#calendar").val(moment().format("MM-YYYY"));
                    ShiftRosterView.get("")
                },
                "shift/save": ShiftRosterView.save,
                "shift/saveDialog": ShiftRosterView.saveOnDialog,
                "previous-page": EmployeeList.show
            },
            model_class, collection_class, collection;

        function _initOther() {

            $company.change(function (event) {
                SAXLoader.show();
                Company.other()
					.fail(function () {
					    SAXAlert.show({ type: "error", message: "An error occurred while performing this operation. Please try again." });
					})
					.always(function () {
					    SAXLoader.close();
					})
            });

            $("#calendar").Zebra_DatePicker({
                format: 'm-Y',
                view: 'months',
                start_date: moment().format('MM-YYYY'),
                onSelect: function (view, elements) {
                    ShiftRosterView.get($(this).val());
                }
            });

            $("#checkall").change(function () {
                var is_checked = $(this).is(':checked'),
	                checkboxes = $table.find('tbody input[type="checkbox"]');
                is_checked ? $(checkboxes).prop('checked', true) : $(checkboxes).prop('checked', false);
            });
        }

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

        function init() {
            _initButtons();
            _initOther();
            _initModels();

            Company.get()
				.always(function () {
				    SAXLoader.close();
				});
        }

        return {
            init: init,
            getCollection: getCollection
        };

    })(jQuery, window, document)

    // INITIAL PAGE LOAD
    SAXLoader.show();

    ShiftRosterPage.init();
});


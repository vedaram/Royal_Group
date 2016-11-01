<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="employee_data_transaction.aspx.cs" Inherits="employee_data_transaction" Title="Employee Transaction | SecurTime" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">EMPLOYEE DATA TRANSACTION</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <button class="btn btn-blue" data-control="button" data-role="employee/export" id="exportButton" data-loading-text="Exporting ...">
                    <span class="fa fa-file-excel-o"></span>Export
                </button>
                <a href="../../../exports/templates/employee_transaction.xlsx" class="btn btn-blue " id="downloadTemplateButton">
                    <span class="fa fa-download"></span>Download Template
                </a>
                <a href="javascript:void(0);" class="btn btn-blue " data-control="button" data-role="import/toggle" id="importToggleButton">
                    <span class="fa fa-upload"></span>Import
                </a>
                <a class="btn btn-blue" style="display:none" data-toggle="modal" data-target="#empTransaction" data-role="employee-category/add">
                    <span class="fa fa-plus"></span>Add Employee Transaction
                </a>
                <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters" id="filter_data">
                    <span class="fa fa-filter"></span>Filters
                </a>
                  
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="importLeavesBox">
        <div class="drawer-body">
            <form method="post" id="importLeavesForm">
                <div class="container">
                    <div class="form-group">
                        <div class="col-4">
                            <label class="text-blue"><strong><small>Choose file to import</small></strong></label>
                            <input type="file" class="form-control" id="file_upload" name="file_upload" />
                        </div>
                        <!-- /.col-4 -->
                        <div class="col-4">
                            <button type="button" class="btn btn-blue" id="importButton" data-control="button" data-role="import/transaction" data-loading-text="Importing ..." disabled>
                                <span class="fa fa-upload"></span>Import Transaction Data
                            </button>
                        </div>
                        <!-- /.col-4 -->
                    </div>
                    <div class="form-group">
                        <div></div>
                    </div>
                    <div class="container">
                        <div class="col-12">
                            <label class="control-label">Import Result</label><br />
                            <textarea class="form-control" id="importResult" style="height: 300px; border-bottom: 1px solid #ddd;" disabled></textarea>
                        </div>
                        <!-- /.col-12 -->
                    </div>
                    <!-- /.row -->
                </div>
                <!-- /.container -->
            </form>
            <!-- /form -->
        </div>
        <!-- /.drawer-body -->
    </div>
    <div class="container drawer" id="filter_employee">
        <div class="drawer-body">
            <form method="" id="EmployeeForm">
                <div class="form-group">
                    <div class="col-3">
                        <label class="text-blue"><strong><small>Company</small></strong></label>
                        <select class="form-control" id="filter_company_employee" name="filter_company_employee">
                        </select>
                    </div>

                    <div class="col-3">
                        <label class="text-blue"><strong><small>Branch</small></strong></label>
                        <select class="form-control" id="filter_branch_employee" name="filter_branch_employee">
                            <option value="select">Select Branch</option>
                        </select>
                    </div>
                    <div class="col-3">
                        <label class="text-blue"><strong><small>Department</small></strong></label>
                        <select class="form-control" id="filter_department_employee" name="filter_department_employee">
                            <option value="select">Select Department</option>
                        </select>
                    </div>

                    
                </div>
                <!-- /.form-group -->
                <div class="form-group">
                    <div class="col-3">
                        <label class="text-blue"><strong><small>Employee Code</small></strong></label>
                        <input type="text" class="form-control" id="" name="" />
                    </div>

                    <div class="col-3">
                    </div>

                    <div class="col-3">
                    </div>
                </div>
                <!-- /.form-group -->
            </form>
            <!-- /.form -->
        </div>
        <!-- /.drawer-body -->
        <div class="drawer-footer">
            <div class="container">
                <div class="pull-right">
                    <a class="btn" data-control="button" data-role="filters/reset">
                        <span class="fa fa-refresh"></span>Reset Filters
                    </a>
                    <button class="btn btn-blue" id="filterButton" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
                        <span class="fa fa-search"></span>Search
                    </button>
                </div>
                <!-- /.pull-right -->
            </div>
            <!-- /.container -->
        </div>
        <!-- /.drawer-footer -->
    </div>

        <!-- /for filter employee Data -->
     <div class="container drawer" id="filters">
        <div class="drawer-body">
            <form method="" id="filterForm">
                <div class="form-group">
                    <div class="col-3" style="display:none">
                        <label class="text-blue"><strong><small>Company</small></strong></label>
                        <select class="form-control" id="filter_company" name="filter_company">
                        </select>
                    </div>

                    <div class="col-3" style="display:none">
                        <label class="text-blue"><strong><small>Branch</small></strong></label>
                        <select class="form-control" id="filter_branch" name="filter_branch">
                            <option value="select">Select Branch</option>
                        </select>
                    </div>
                    <!--
                    <div class="col-3">
                        <label class="text-blue"><strong><small>From Date</small></strong></label>
                        <input type="text" class="form-control datepicker" id="" name="" />
                    </div>
                    <div class="col-3">
                        <label class="text-blue"><strong><small>To Date</small></strong></label>
                        <input type="text" class="form-control datepicker" id="" name="" />
                    </div>
                </div>
                <!-- /.form-group 
                <div class="form-group"> -->
                    <div class="col-3">
                        <label class="text-blue"><strong><small>Employee Code</small></strong></label>
                        <input type="text" class="form-control" id="emp_code" name="emp_code" />
                    </div>

                </div>
                <!-- /.form-group -->
            </form>
            <!-- /.form -->
        </div>
        <!-- /.drawer-body -->
        <div class="drawer-footer">
            <div class="container">
                <div class="pull-right">
                    <a class="btn" data-control="button" data-role="filters/reset">
                        <span class="fa fa-refresh"></span>Reset Filters
                    </a>
                    <button class="btn btn-blue" id="filterButton_employee" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
                        <span class="fa fa-search"></span>Search
                    </button>
                </div>
                <!-- /.pull-right -->
            </div>
            <!-- /.container -->
        </div>
        <!-- /.drawer-footer -->
    </div>

    

    <!-- /.filter empoyee data -->

    <!-- /.drawer -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container" style="display:none">
        <div class="card no-padding">
            <div class="card-header">
                <ul class="tablist clearfix">
                    <li class="tab active">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#shiftTab" id="shift_id">Shift
                        </a>
                    </li>
                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#otTab" id="ot_id">OT Eligibility
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#ramadanTab" id="ramadan_id">Ramadan Eligibility
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#punch" id="punch_id">Punch Exception
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#meternityTab" id="meternity_id">Meternity
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#WHdayTab" id="WH_day">WH/day
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#WHweekTab" id="WH_week">WH/Week
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#WHmonthTab" id="WH_month">WH/Month
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#terminationTab" id="termination_id">Termination Date
                        </a>
                    </li>

                    <li class="tab">
                        <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#linemamagerTab" id="linemanager_id">Line Manager
                        </a>
                    </li>



                </ul>
            </div>
            <!-- /.card-header -->
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-panel active" id="shiftTab">
                        <table class="table width-12" cellpadding="0" cellspacing="0" id="shift_data">
                            <thead>
                                <tr>
                                    <th>Employee Code</th>
                                    <th>Employee Name</th>
                                    <th>Company</th>
                                    <th>Branch</th>
                                    <th>From Date</th>
                                    <th>To Date</th>
                                    <th>Shift Name </th>
                                    <th>Actions</th>
                                </tr>
                            </thead>
                            <tbody>
                            </tbody>
                        </table>

                        <div class="form-group text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="" title="Click here to load more data." data-control="button" data-role="load-more-normal-data">Click here to load more data
                            </a>
                        </div>



                    </div>
                    <!-- END OF SHIFT TAB -->

                    <div class="tab-panel" id="otTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="ot_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>Eligibility</th>
                                        <th>Actions</th>

                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="lwpPagination" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>


                    </div>
                    <!-- END OF OT TAB -->


                    <div class="tab-panel" id="ramadanTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="ramadan_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>Eligibility</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="#" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>

                    </div>

                    <!-----ramadan tab ends here---------->

                    <!------punch exception starts here---------->

                    <div class="tab-panel" id="punch">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="punch_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="#" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>

                    </div>

                    <!---------punch exception ends here------------->

                    <!------meternity tab starts here--------->

                    <div class="tab-panel" id="meternityTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="meternity_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>Checkbox</th>
                                        <th>Actions</th>

                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="A1" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>
                    </div>

                    <!------meternity tab ends here--------->

                    <!------working hours day tab starts here--------->

                    <div class="tab-panel" id="WHdayTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="WH_day_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>TextBox</th>
                                        <th>Actions</th>

                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="A2" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>
                    </div>
                    <!------working hours day tab ends here--------->

                    <!------working hours week tab starts here--------->

                    <div class="tab-panel" id="WHweekTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="WH_week_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>TextBox</th>
                                        <th>Actions</th>

                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="A3" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>


                    </div>

                    <!------working hours week tab ends here--------->

                    <!------working hours month tab starts here--------->


                    <div class="tab-panel" id="WHmonthTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="WH_month_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>TextBox</th>
                                        <th>Actions</th>

                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="A4" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>


                    </div>

                    <!------working hours month tab ends here--------->

                    <!------termination tab starts here--------->

                    <div class="tab-panel" id="terminationTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="termination_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>Status</th>
                                        <th>Actions</th>
                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="A5" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>


                    </div>

                    <!---------termination tab ends here--------------->

                    <!---------line manager tab starts here--------------->

                    <div class="tab-panel" id="linemamagerTab">
                        <div class="form-group">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="linemanager_data">
                                <thead>
                                    <tr>
                                        <th>Employee Code</th>
                                        <th>Employee Name</th>
                                        <th>Company</th>
                                        <th>Branch</th>
                                        <th>From Date</th>
                                        <th>To Date</th>
                                        <th>Line Manager</th>
                                        <th>Actions</th>

                                    </tr>
                                </thead>

                            </table>
                        </div>
                        <!-- /.row -->

                        <div class="text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="A6" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">Click here to load more data
                            </a>
                        </div>


                    </div>

                    <!---------line manager tab ends here-------------->


                </div>
                <!-- /.tab-content -->
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
    </div>
    <!-- /.container -->
    <div class="modal fade" id="approvalCommentDialog" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <a href="javascript:void(0);" class="icon pull-right" data-control="close" data-dismiss="modal" data-target="#approvalCommentDialog">X</a>
                    <h4>Comments</h4>
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <textarea id="approvalComment" class="form-control" rows="4"></textarea>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#approvalCommentDialog">Cancel
                    </a>
                    <button class="btn btn-blue" id="approveLeaveButton" data-control="button" data-role="confirm-approval" data-loading-text="Processing ...">
                        Proceed
                    </button>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    <!-----add employee model pop up starts here--------->

    <div class="modal fade" id="empTransaction" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Employee Transaction Data</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#" class="text-red">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">

                    <!-- Employee Transaction Form -->
                   
 
                    <form method="post" id="saveForm">
                        <div class="form-group eligible_class">
                            <div class="col-4">
                                <label class="form-control1">Employee Code</label>
                                <label class="form-control" id="employee_id" name="employee_data">Emp Id</label>
                             </div>
                            <div class="col-4">
                               <%--<button class="btn btn-blue" id="filteremployeebutton"   data-role="employee-filter" data-loading-text="Saving ..."  >Filter Employee</button>--%>
                             <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="employee-filter" style="display:none" id="filteremployeebutton">
                    <span class="fa fa-filter"></span>Filters
                </a>
                  
                                 </div>


                            <div class="col-4" style="display: none">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="to_date" name="to_date" />
                            </div>
                        </div>
                        <table id="shift" width="800" cellspacing="0" cellpadding="0">
                            <tbody>
                                <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                                </tr>
                                <tr>
                                    
                                        <td>
                                            <label class="text-blue"><strong>Shift</strong></label></td>
                                   
                                        <td>
                                            <label class="text-blue"><strong>From Date</strong></label></td>
                                   
                                        <td>
                                            <label class="text-blue"><strong>End Date</strong></label></td>
                                   
                                        <td>
                                            <label class="text-blue"><strong>Shift Name</strong></label></td>
                                        <td></td>
                                   
                                </tr>
                                
                                 
                            </tbody>
                    </table>
                    
                        <hr/>
                        <table width="800" cellspacing="0" cellpadding="0" id="ot">
                            <tbody>
                             <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                                </tr>
                      
                            <tr>
                           <td> <label class="text-blue"><strong>OT</strong></label></td>  
                             <td> <label class="text-blue"><strong>From Date</strong></label></td> 
                             <td> <label class="text-blue"><strong>End Date</strong></label></td>
                             <td> <label class="text-blue"><strong>Action</strong></label></td>  
                             <td></td>
                                </tr>
                                
                           </tbody>
                    </table>
                        <hr/>
                         <table width="800" cellspacing="0" cellpadding="0" id="ramadan">
                         <tbody>
                            <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                                 <td> <label class="text-blue"><strong>Ramdan</strong></label></td>  
                                 <td> <label class="text-blue"><strong>From Date</strong></label></td> 
                                  <td> <label class="text-blue"><strong>End Date</strong></label></td>
                                  <td></td>  
                                  <td></td>
                             </tr>
                                
                            </tbody>
                    </table>
                    
                        <hr/>
                         <table width="800" cellspacing="0" cellpadding="0" id="punchexception">
                         <tbody>
                             <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                          <td> <label class="text-blue"><strong>Punch</strong></label></td> 
                           <td> <label class="text-blue"><strong>From Date</strong></label></td>  
                            <td> <label class="text-blue"><strong>End Date</strong></label></td> 
                            <td></td>
                            <td></td>
                                </tr>
                                 
                                </tbody>
                            
                    </table>
                        <hr/>
                        
                        
                        
                        <table width="800" cellspacing="0" cellpadding="0" id="workhourperday">
                        <tbody>
                             <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                           <td> <label class="text-blue"><strong>WH Per Day</strong></label></td>
                           <td> <label class="text-blue"><strong>From Date</strong></label></td> 
                           <td> <label class="text-blue"><strong>End Date</strong></label></td> 
                                <td> <label class="text-blue"><strong>Hours</strong></label></td> 
                                <td></td>
                                </tr>
                                 
                                </tbody>
                            
                    </table>
                        <hr/>

                        <table width="800" cellspacing="0" cellpadding="0" id="workhourperweek">
                         <tbody>
                             <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                           <td> <label class="text-blue"><strong>WH Per Week</strong></label></td>
                           <td> <label class="text-blue"><strong>From Date</strong></label></td> 
                           <td> <label class="text-blue"><strong>End Date</strong></label></td> 
                                <td> <label class="text-blue"><strong>Hours</strong></label></td> 
                                <td></td>
                                </tr>
                                 
                                </tbody>
                        
                    </table>
                        <hr/>
                        <table width="800" cellspacing="0" cellpadding="0" id="workhourpermonth">
                         <tbody>
                             <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                           <td> <label class="text-blue"><strong>WH per Month</strong></label></td>
                           <td> <label class="text-blue"><strong>From Date</strong></label></td> 
                           <td> <label class="text-blue"><strong>End Date</strong></label></td> 
                                <td> <label class="text-blue"><strong>Hours</strong></label></td> 
                                <td></td>
                                </tr>
                                 
                                </tbody>
                        
                    </table>
                        <hr/>
                        <table width="800" cellspacing="0" cellpadding="0" id="maternity">
                        <tbody>
                              <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                           <td> <label class="text-blue"><strong>Maternity</strong></label></td>  
                           <td> <label class="text-blue"><strong>Child date of birth</strong></label></td> 
                           <td></td>
                           <td></td>
                           <td></td>
                                </tr>
                                
                            </tbody>
                            
                    </table>
                       <hr/>
                        <table width="800" cellspacing="0" cellpadding="0" id="termination">
                        <tbody>
                             <tr>
                                    <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                           <td> <label class="text-blue"><strong>Termination</strong></label></td>  
                           <td> <label class="text-blue"><strong>Date of Termination</strong></label></td> 
                           <td></td>
                           <td></td>
                           <td></td>
                                </tr>
                            </tbody>
                    </table>
                        <hr/>
                         <table width="800" cellspacing="0" cellpadding="0" id="manager">
                         <tbody>
                             <tr>
                                 <td width="100"></td>
                                    <td width="150"></td>
                                    <td width="150"></td>
                                    <td width="350"></td>
                                    <td width="50"></td>

                            </tr>
                            <tr>
                            <td> <label class="text-blue"><strong>Manager</strong></label></td>
                           <td> <label class="text-blue"><strong>From Date</strong></label></td> 
                           <td> <label class="text-blue"><strong>End Date</strong></label></td> 
                                <td> <label class="text-blue"><strong>Emp Id</strong></label></td> 
                                <td></td>
                                </tr>
                            </tbody>
                        
                    </table>
                    
                   
              <!--
                            <td></td>
                               <td> <label class="text-blue"><small>From Date</small></label></td>
                                <td> <label class="text-blue"><small>To Date</small></label></td>
                                <td> <label class="text-blue"><small>Shift Code</small></label></td>
                            </tr>
                            <tr>
                            <td></td>
                                <td>
                            <div class="col-3">
                                <input type="checkbox" id="shift_date_chkbox" name="shift_date_chkbox" />
                                <span class="form-control1">Shift</span>
                            </div>
                                </td>
                            <td>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="shift_From_date" name="shift_From_date" disabled />
                            </div>
                                </td>
                            <td>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker " id="shift_To_date" name="shift_To_date" disabled />
                            </div>
                                </td>
                            <td>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>Shift</small></strong></label>
                                <select class="form-control" id="shift_select_data" name="shift_select_data">
                                    <option value="select">Select</option>
                                </select>
                            </div>
                            </td>
                        </div>
                            </tr>
                            
                        <!-- /.col-4 -->
                        <!-- /.form-group -->

                <!--
                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="OT_date" name="OT_date" />
                                <span class="form-control1">OT</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="OT_from_date" name="OT_from_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="OT_To_date" name="OT_To_date" disabled />
                            </div>
                            <div class="col-3" style="display: none">
                                <label class="text-blue"><strong><small>Eligibility</small></strong></label>
                                <select class="form-control" id="OT_drop_down" name="OT_drop_down">
                                    <option value="select">Select</option>
                                </select>
                            </div>
                        </div>

                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="ramadan_date" name="ramadan_date" />
                                <span class="form-control1">Ramdan</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="ramadan_from_date" name="ramadan_from_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="ramadan_to_date" name="ramadan_to_date" disabled />
                            </div>
                            <div class="col-3" style="display: none">
                                <label class="text-blue"><strong><small>Eligibility</small></strong></label>
                                <select class="form-control" id="ramadan_drop_down" name="ramadan_drop_down">
                                    <option value="select">Select</option>
                                </select>
                            </div>
                        </div>

                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="punch_date" name="punch_date" />
                                <span class="form-control1">Punch</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="punch_from_date" name="punch_from_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="punch_to_date" name="punch_to_date" disabled />
                            </div>
                            <div class="col-3" style="display: none">
                                <label class="text-blue"><strong><small>Eligibility</small></strong></label>
                                <select class="form-control" id="punch_drop_down" name="punch_drop_down">
                                    <option value="select">Select</option>
                                </select>
                            </div>
                        </div>
                        
                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="maternity_date" name="maternity_date" />
                                <span class="form-control1">Maternity</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>Child Date of Birth</small></strong></label>
                                <input type="text" class="form-control datepicker" id="child_date_of_birth" name="child_date_of_birth" disabled />
                            </div>

                        </div> 

                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="WH_day_date" name="WH_day_date" />
                                <span class="form-control1">WH/day</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="WH_day_from_date" name="WH_day_from_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="WH_day_to_date" name="WH_day_to_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>Hours</small></strong></label>
                                <input type="text"  class="form-control" id="WH_day_drop_drown" name="WH_day_drop_drown" />


                            </div>
                        </div>
                    
                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="WH_week_date" name="WH_week_date" />
                                <span class="form-control1">WH/week</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="WH_week_from_date" name="WH_week_from_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="WH_week_to_date" name="WH_week_to_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>Hours</small></strong></label>
                                <input type="text" class="form-control" id="WH_week_drop_down" name="WH_week_drop_down" />
                            </div>
                        </div>

                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="WH_month_date" name="WH_month_date" />
                                <span class="form-control1">WH/month</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="WH_month_from_date" name="WH_month_from_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="WH_month_to_date" name="WH_month_to_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>Hours</small></strong></label>
                                <input type="text" class="form-control" id="WH_month_drop_down" name="WH_month_drop_down" />
                            </div>
                        </div>
                    
                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="termination_date" name="termination_date" />
                                <span class="form-control1">Termination</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="termination_from_date" name="termination_from_date" disabled />
                            </div>

                        </div>
                    
                        <div class="form-group eligible_class">
                            <div class="col-3">
                                <input type="checkbox" id="line_manager" name="line_manager" />
                                <span class="form-control1">Line Manager</span>
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>From Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="line_from_date" name="line_from_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>To Date</small></strong></label>
                                <input type="text" class="form-control datepicker" id="line_to_date" name="line_to_date" disabled />
                            </div>
                            <div class="col-3">
                                <label class="text-blue"><strong><small>Eligibility</small></strong></label>
                                <select class="form-control" id="line_manager_drop_down" name="line_manager_drop_down">
                                    <option value="select">Select</option>
                                </select>
                            </div>
                        </div>
                    -->
                    <!--
                    <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button class="btn btn-blue" id="Button1" data-control="button" data-role="employee-save" data-loading-text="Saving ...">
                        Save Employee Transaction</button>
                </div>
                 <!--   <label id=saveButton class='btn btn-blue' data-role='txn-save' data-control='button'>Save Employee Transaction</label> -->
                    </form>


                    <!-- End Employee Transaction Form -->
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button class="btn btn-blue" id="saveButton" data-control="button" data-role="employee-save" data-loading-text="Saving ...">
                        Save Employee Transaction</button>
                </div>
                <!-- /.modal-footer 
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css" />
    <link rel="stylesheet" href="../../../resources/css/datepickr.min.css" />
   <!-- <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script> -->
   <script type="text/javascript" src="../../../resources/lib/datepickr.min.js"></script>
	<script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/transaction/employee/employee_data_transaction.js"></script>
</asp:Content>







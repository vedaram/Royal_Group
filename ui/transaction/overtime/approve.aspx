<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="approve.aspx.cs" Inherits="overtime_approve" Title="Overtime Approval | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">OVERTIME APPROVAL</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar" id="filterBtn">
                <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters" data-target="#overTimeApprovalFilters">
                    <span class="fa fa-filter"></span>
                    Filters
                </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="overTimeApprovalFilters">
        <div class="drawer-body">
            <form method="" id="overTimeApprovalFiltersForm">
                <div class="container">
                    <div class="form-group">


                        <div class="col-3">
                            <label class="text-blue"><strong><small>Company</small></strong></label>
                            <select class="form-control" id="filter_CompanyCode" name="filter_CompanyCode">
                            </select>
                        </div>
                         <div class="col-3">
                           

                            <label class="text-blue"><strong><small>Employee ID</small></strong></label>
                            <input type="text" class="form-control" name="employee_id" id="employee_id">
                        </div>

                        <div class="col-3">
                            <label class="text-blue"><strong><small>View</small></strong></label>
                            <select class="form-control" id="filter_View" name="filter_View">
                                <option value="1">Overtime Days</option>
                                <option value="2">All Days</option>

                            </select>
                        </div>


                        <div class="col-3 hide">
                            <label class="text-blue"><strong><small>Department</small></strong></label>
                            <select class="form-control" id="filter_DepartmentCode" name="filter_DepartmentCode">
                            </select>
                        </div>

                        <div class="col-3 hide">
                            <label class="text-blue"><strong><small>Designation</small></strong></label>
                            <select class="form-control" id="filter_DesignationCode" name="filter_DesignationCode">
                            </select>
                        </div>


                    </div>
                    <div class="form-group">
                       
                        <div class="col-3 hide">
                            <label class="text-blue"><strong><small>Employee Name</small></strong></label>
                            <input type="text" class="form-control" name="employee_name" id="employee_name">
                        </div>

                        
                    </div>
                    <div class="form-group">
                        <div class="col-3">
                            <label class="text-blue"><strong><small>From</small></strong></label>
                            <input type="text" class="date-picker form-control" id="filter_from" name="filter_from" disabled/>
                        </div>

                        <div class="col-3">
                            <label class="text-blue"><strong><small>To</small></strong></label>
                            <input type="text" class="date-picker form-control" id="filter_to" name="filter_to" disabled/>
                        </div>
                        <div class="col-3">
                            <label class="text-blue"><strong><small>Requirements</small></strong></label>
                            <select class="form-control" id="filter_Requirement" name="filter_Requirement">
                                <option value="1">All</option>
                                <option value="2">Within Legal Requirements</option>
                                <option value="3">Above Legal Requirements</option>
                            </select>
                        </div>
                    </div>
                    
             
                    
                    <%--<div class="form-group">   
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>OT Date</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_date" name="filter_date" />
	                    </div>

	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>OT Time</small></strong></label>
	                        <input type="text" class="time-picker form-control" id="filter_hours" name="filter_hours" />
	                    </div>
	                </div>
	                <!-- /.form-group -->
	                <div class="form-group">
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Filter By</small></strong></label>
	                        <select class="form-control" id="filter_by" name="filter_by">
	                            <option value="0">-- Select --</option>
	                            <option value="1">Employee ID</option>
	                            <option value="2">Employee Name</option>
	                        </select>
	                    </div>

	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Keyword</small></strong></label>
	                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
	                    </div>

	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Status</small></strong></label>
	                        <select class="form-control" id="filter_LeaveStatus" name="filter_LeaveStatus">
	                            <option value="0">-- Select --</option>
	                            <option value="1">Submitted</option>
	                            <option value="2">Approved</option>
	                            <option value="3">Declined</option>
	                            <option value="4">Cancelled</option>
	                        </select>
	                    </div>
	                </div>--%>
                    <!-- /.form-group -->
                </div>
                <!-- /.container -->
            </form>
            <!-- /form -->
        </div>
        <!-- /.drawer-body -->
        <div class="drawer-footer">
            <div class="container">
                <div class="pull-right">
                    <a href="javascript:void(0);" class="btn" data-control="button" data-role="reset-filters">
                        <span class="fa fa-refresh"></span>
                        Reset Filters
                    </a>
                    <button type="button" id="filterButton" class="btn btn-blue" data-control="button" data-role="filter-data" data-loading-text="Filtering ...">
                        <span class="fa fa-search"></span>
                        Search
                    </button>
                </div>
                <!-- /.pull-right -->
            </div>
            <!-- /.container -->
        </div>
        <!-- /.drawer-footer -->
    </div>
    <!-- /.drawer -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container">
        <div class="card no-padding">
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-panel active">
                        <div class="action-bar" style="display: none">
                            <a href="javascript:void(0);" class="btn btn-green" data-control="button" data-role="action-button-click" data-operation="2">
                                <span class="fa fa-thumbs-o-up"></span>
                                Approve Selected
                            </a>

                            <a href="javascript:void(0);" class="btn btn-red" data-control="button" data-role="action-button-click" data-operation="3">
                                <span class="fa fa-thumbs-o-down"></span>
                                Decline Selected
                            </a>

                            <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="action-button-click" data-operation="4">
                                <span class="fa fa-close"></span>
                                Cancel Selected
                            </a>
                        </div>
                       
                     <div class="form-group">
                        <div class="col-3">
                           

                            <label  id='employee_lable' style="font-weight:bold"><b> </b>  </label>
                            </div>
                            
                            
                        </div>

                     </div>
                        <!-- /.action-bar -->

    
<div id="Ot_Approval_Table_First" >
                    
    <table id="mainTable">

     <tr>
         <td>
                        <table class="table"  id="overTimeApprovalTableTr"  >
                            <thead>
                                <tr>
                                     <%-- <input type="checkbox" id="CheckAll" /></th>--%>
                                    <th style="width:54px"><input type="checkbox" id="OtCheckAll" checked class="hide" ></th>
                                    <th style="width:95px">Weekday</th>
                                  
                                    <th style="width:103px;" >Date</th>
                                    <th style="width:81px;" >Timing From</th>
                                    <th style="width:81px;" >Timing To</th>
                                    <th style="width:81px;" >Total Rounded Worked Hours</th>
                                    <th style="width:70px;" >Within Legal Requirements</th>
                                    <th style="width:90px;"> Confirmed</th>
                                    <th style="width:154px;" >Above Legal Requirements</th>
                                    <th>Confirmed</th>
                                    <th>Shortage Working Hours</th>
                                </tr>
                            </thead>
                          
                        </table>
          </td>
</tr>
 <tr>

         <td>
             <div  style="height:300px; overflow:auto;width:auto;">
                  <table class="table"  id="overTimeApprovalTable"  >
                                <tbody></tbody>
                  </table>
               </div>
         </td>
  </tr>

    </table>
    <%--<table cellspacing="0" cellpadding="0" border="0" width="325">
 <tr>
        <td>
               <table cellspacing="0" cellpadding="1" border="1" width="300" >
                 <tr style="color:white;background-color:grey">
                    <th>Header 1</th>
                    <th>Header 2</th>
                 </tr>
               </table>
       </td>
 </tr>

<tr>
<td>
   <div style="width:320px; height:60px; overflow:auto;">
     <table cellspacing="0" cellpadding="1" border="1" width="300" >
       <tr>
         <td>new item</td>
         <td>new item</td>
       </tr>
       <tr>
         <td>new item</td>
         <td>new item</td>
       </tr>
          <tr>
         <td>new item</td>
         <td>new item</td>
       </tr>
          <tr>
         <td>new item</td>
         <td>new item</td>
       </tr>   
     </table>  
   </div>
  </td>
 </tr>
</table>--%>
  <div class="card-footer">
            	<div class="text-center">
		            <button class="btn btn-green" id="grantOTEligibility" data-toggle="modal" data-target="#grantDialog" data-loading-text="Processing ..." onclick="saveApprovedData();">
		                <span class="fa fa-check"></span> Continue...
		            </button>
		            <button class="btn btn-red hide" id="rejectOTEligibility" data-toggle="modal" data-target="#rejectDialog" data-loading-text="Processing ...">
		                <span class="fa fa-ban"></span> Reject
		            </button>
		        </div>
            </div>

</div>
                        <!-- for OT Second screen -->

<div id="Ot_Approval_Table_Second" class="hide"  style="overflow: scroll;">
    
                              <table   cellpadding="0" cellspacing="0" id="overTimeApprovalTable2" class="table width-12">
                            <thead>
                                <%--<tr style="color: #3498db;font-size: 10px;white-space: normal;border-width: 1px;">--%>
                                    <tr>
                                       <%-- <input type="checkbox" id="CheckAll" /></th>--%>
                                        
                                   <%-- <th>EmpId</th>&nbsp;
                                    <th >Name</th>--%>
                                    <th>Week Dates</th>
                                    <th>Weekly Mandatory Hours</th>
                                    <th>Actual Weekly Rounded Hours</th>
                                    <th>Rejected OT</th>
                                    <th> Within Legal Limits</th>
                                    <th>Above Legal Limits</th>
                                    <th>Shortage of Completed WH</th>
                                    <th>Total Regular OT</th>
                                    <th>Total Night OT</th>
                                    <th>Within Legal </th>
                                    <th>Above Legal </th>
                                    <th>Public Holiday OT</th>
                                    <th>WO compensate with CO</th>
                                     <th>confirmed</th>
                                    <th>Holidays compensate with CO</th>
                                     <th>confirmed</th>
                                     <th><input type="checkbox" id="Checkbox2" checked class='hide'></th>
                                     

                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
  <div class="card-footer">
            	<div class="text-center">
		            <button class="btn btn-green" id="approve_final" data-control="button" data-loading-text="Processing ..."  data-role="data/approve_final">
		                <span class="fa fa-check"></span> Approve
		            </button>
		            <button class="btn btn-green" id="back" data-control="button" data-role="data/back" data-loading-text="Processing ...">
		                <span class="fa fa-ban"></span> Back
		            </button>
		        </div>
            </div>
    </div>
   <!-- End of  OT Second screen -->


                        <div class="form-group text-center hide">
                            <a href="javascript:void(0);" class="btn pagination" id="otPagination" title="Click here to load more data." data-control="button" data-role="load-more-ot-data">Click here to load more data
                            </a>
                        </div>

                        <div class="form-group no-data text-center hide" id="otNoData">
                        </div>
                    </div>
                    <!-- /.tab-panel -->
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
                    <button class="btn btn-blue" id="approveOverTimeButton" data-control="button" data-role="confirm-approval" data-loading-text="Processing ...">
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
    
     <!--Dialog box for check red lable in table-->
	
    <div class="modal fade" tabindex="-1" role="dialog" id="checkFridayDailog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Overtime filter Confirmation</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#checkFridayDailog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#checkRedLabelDialog">No</a>
                    <button class="btn btn-blue" id="confirmshift" data-control="button" data-role="Ot/filter" data-loading-text="Processing ...">Yes</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    
    
    
    
    
    
    
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/timepicki.css" />
    <link rel="stylesheet" href="../../../resources/css/datepicker.css" />
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/lib/timepicki.js"></script>
       
   
    <script type="text/javascript" src="../../../resources/js/transaction/overtime/approve.js"></script>
    //
</asp:Content>

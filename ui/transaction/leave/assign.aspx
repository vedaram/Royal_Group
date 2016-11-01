<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="assign.aspx.cs" Inherits="leave_assign" Title="Leave Assign | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">LEAVE ASSIGN</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
	            <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters">
	                <span class="fa fa-filter"></span>
	                 Filters
	            </a>
	            <a href="../../../exports/templates/employee_leave.xlsx" target="_blank" class="btn btn-blue">
	                <span class="fa fa-file-excel-o"></span>
	                 Download Leave Template
	            </a>
	            <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-import" data-loading-text="Processing......">
	                <span class="fa fa-upload"></span>
	                 Import Leaves
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
	                        <button class="btn btn-blue" id="importLeavesButton" data-control="button" data-role="import-leaves">
	                            Import Leaves
	                        </button>
	                    </div>
	                    <!-- /.col-4 -->
	                </div>
	                <!-- /.row -->
	            </div>
	            <!-- /.container -->
	        </form>
	        <!-- /form -->
        </div>
        <!-- /.drawer-body -->
    </div>
    <!-- /.drawer -->
    <div class="drawer" id="employeeFilters">
    	<div class="drawer-body">
	        <form method="" id="employeeFiltersForm">
	            <div class="container">
	                <div class="form-group">
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Company</small></strong></label>
	                        <select class="form-control" id="filter_company" name="filter_company" disabled>
	                            <option value="select">Select a Company</option>
	                        </select>
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Branch</small></strong></label>
	                        <select class="form-control" id="filter_branch" name="filter_branch" disabled>
	                            <option value="select">Select a Branch</option>
	                        </select>
	                    </div>
                    	<!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Department</small></strong></label>
	                        <select class="form-control" id="filter_department" name="filter_department" disabled>
	                            <option value="select">Select a Department</option>
	                        </select>
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Shift</small></strong></label>
	                        <select class="form-control" id="filter_shift" name="filter_shift" disabled>
	                            <option value="select">Select a Shift</option>
	                        </select>
	                    </div>
	                    <!-- /.col-3 -->
	                </div>
	                <!-- /.form-group -->
	                <div class="form-group">
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Designation</small></strong></label>
	                        <select class="form-control" id="filter_designation" name="filter_designation" disabled>
	                            <option value="select"></option>
	                        </select>
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Employee Category</small></strong></label>
	                        <select class="form-control" id="filter_employee_category" name="filter_employee_category" disabled>
	                            <option value="select"></option>
	                        </select>
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Employee Name</small></strong></label>
	                        <input type="text" class="form-control" id="filter_employee_name" name="filter_employee_name" />
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Employee ID</small></strong></label>
	                        <input type="text" class="form-control" id="filter_employee_id" name="filter_employee_id" />
	                    </div>
	                    <!-- /.col-3 -->
	                </div>
	                <!-- /.form-group -->
	            </div>
	            <!-- /.container -->
	        </form>
	        <!-- /.form -->
        </div>
        <!-- /.drawer-body -->
        <div class="drawer-footer">
        	<div class="container">
                <div class="pull-right">
                	<a href="javascript:void(0);" class="btn" data-control="button" data-role="reset-filters">
                        <span class="fa fa-refresh"></span>
                         Reset Filters
                    </a>
                    <button type="button" class="btn btn-blue" id="employeeFilterButton" data-control="button" data-role="filter-data" data-loading-text="Filtering ...">
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
			<div class="card-header">
				<ul class="tablist clearfix">
                	<li class="tab active" id="employeeTabOption">
	                    <a href="javascript:void(0);" data-id="employee">
	                        1. Select Employee
	                    </a>
	                </li>
	                <li class="tab" id="leaveTabOption">
	                    <a href="javascript:void(0);" data-id="leave">
	                        2. Leave Information
	                    </a>
	                </li>
	            </ul>
            <!-- /.tablist -->
			</div>
			<!-- /.card-header -->
			<div class="card-body">
				<div class="tab-content">
	                <div class="tab-panel active" id="employeeTab">
	                    <div class="container">
	                        <table class="table width-12" cellpadding="0" cellspacing="0" id="employeeTable">
	                            <thead>
	                                <tr>
	                                    <th>Employee ID</th>
	                                    <th>Employee Name</th>
	                                </tr>
	                            </thead>
	                            <tbody></tbody>
	                        </table>
	                    </div>
	                    <!-- /.container -->
	                    <div class="form-group text-center hide">
	                        <a href="javascript:void(0);" class="btn pagination" id="pagination" title="Click here to load more data." data-control="button" data-role="load-more-data">
	                            Click here to load more data
	                        </a>
	                    </div>

	                    <div class="no-data text-center hide" id="noData">
	                    </div>
	                </div>
	                <!-- /#employeeTab -->
	                <div class="tab-panel" id="leaveTab">
	                    <div class="form-group">
	                        <p>Currently viewing Assigned Leave details for: <br /></p>
                        	<div class="container">
		                        <div class="width-4">
		                            <label class="control-label">Employee ID</label>
		                            <input type="text" class="form-control" id="selected_employee_id" name="selected_employee_id" disabled />
		                        </div>

		                        <div class="width-4">
		                            <label class="control-label">Employee Name</label>
		                            <input type="text" class="form-control" id="selected_employee_name" name="selected_employee_name" disabled />
		                        </div>

		                        <div class="width-4">
		                            <label class="control-label">Employee Category</label>
		                            <input type="text" class="form-control" id="selected_employee_category" name="selected_employee_category" disabled />
		                        </div>
	                        </div>
	                        <!-- /.container -->
	                    </div>
	                    <!-- /.form-group -->
	                    <div class="container">
	                        <table class="table width-12" cellpadding="0" cellspacing="0" id="leaveTable">
	                            <thead>
	                                <tr>
	                                    <th>Leave Code</th>
	                                    <th>Leave Name</th>
	                                    <th>Maximum Leave</th>
	                                    <th>Leave Applied</th>
	                                    <th>Leave Balance</th>
	                                </tr>
	                            </thead>
	                            <tbody></tbody>
	                        </table>
	                    </div>
	                    <!-- /.container -->
	                    <div class="form-group">
	                        <div class="width-10 text-center">
	                            <a href="javascript:void(0);" class="text-red" data-control="button" data-role="select-employee">
	                                <span class="fa fa-long-arrow-left"></span>
	                                 Back to Employee Selection
	                            </a>
	                            <button class="btn btn-green" id="updateLeavesButton" data-control="button" data-role="update-leave" data-loading-text="Processing ...">
	                                <span class="fa fa-floppy-o"></span>
	                                 Update Leave
	                            </button>
	                        </div>
	                    </div>
	                    <!-- /.form-group -->
	                </div>
	                <!-- /#leaveTab -->
	            </div>
	            <!-- /.tab-content -->
			</div>
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
    <div class="modal fade" id="importResultDialog" role="dialog" tabindex="-1">
        <div class="modal-dialog"> 
            <div class="modal-content">
                <div class="modal-header">
                   <div class="container">
                      <div class="pull-left">
                        <h4>Leave Assigne Import Result</h4>
                      </div>
                      <div class="pull-right">
                          <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#importResultDialog">X</a>
                      </div>
                   </div>                    
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <textarea class="form-control" id="importResult" style="height: 250px;" readonly></textarea>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer text-right">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#importResultDialog">
                        Close
                    </a>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<script type="text/javascript" src="../../../resources/js/transaction/leave/assign.js" ></script>
</asp:Content>
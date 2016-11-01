<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="details.aspx.cs" Inherits="leave_details" Title="Leave Details | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">LEAVE DETAILS</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
	            <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters" data-target="#leavesApprovalFilters">
	                <span class="fa fa-filter"></span>
	                 Filters
	            </a>
	            <a href="../../../exports/templates/HRMS_leave.xlsx" class="btn btn-blue hide" id="downloadTemplateButton">
            		<span class="fa fa-download"></span> Download Leave Template
            	</a>
	            <a href="javascript:void(0);" class="btn btn-blue hide" data-control="button" data-role="import/toggle" id="importToggleButton">
            		<span class="fa fa-upload"></span> Import
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
	                        <button type="button" class="btn btn-blue" id="importButton" data-control="button" data-role="import/leave" data-loading-text="Importing ..." disabled>
			                    <span class="fa fa-upload"></span> Import Leaves
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
    <div class="drawer" id="leavesApprovalFilters">
    	<div class="drawer-body">
	        <form method="" id="leavesApprovalFiltersForm">
	            <div class="container">
	                <div class="form-group">   
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>From</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_from" name="filter_from" />
	                    </div>

	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>To</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_to" name="filter_to" />
	                    </div>

	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Company</small></strong></label>
	                        <select class="form-control" id="filter_CompanyCode" name="filter_CompanyCode">
	                        </select>
	                    </div>

	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Leave Type</small></strong></label>
	                        <select class="form-control" id="filter_LeaveType" name="filter_LeaveType">
	                        </select>
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
	                            <option value="select">-- Select --</option>
	                            <option value="1">Submitted</option>
	                            <option value="2">Approved</option>
	                            <option value="3">Declined</option>
	                            <option value="4">Cancelled</option>
	                        </select>
	                    </div>
	                </div>
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
        			<button class="btn btn-blue" data-control="button" data-role="filter-data" data-loading-text="Filtering ...">
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
	                <li class="tab active">
	                    <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#leaveApprovalTab" data-id="leave">
	                        Leave Approval
	                    </a>
	                </li>
	                <li class="tab">
	                    <a href="javascript:void(0);" data-control="toggle" data-toggle="tab" data-target="#lwpApprovalTab" data-id="lwp">
	                        LWP Approval
	                    </a>
	                </li>
	            </ul>
			</div>
			<!-- /.card-header -->
			<div class="card-body">
		        <div class="tab-content">
		            <div class="tab-panel active" id="leaveApprovalTab">                
	                    <table class="table width-12" cellpadding="0" cellspacing="0" id="NormalLeaveTable">
	                        <thead>
	                            <tr>
	                                 <th>Employee ID</th> 
	                                <th>Employee Name</th>
	                                <th>Leave Type</th>
	                                <th>From</th>
	                                <th>To</th>
	                                <th>Half Day</th>
	                                <th>Remarks</th>
	                                <th>Approval</th>
	                                <th>ApprovalBy</th>                    
	                            </tr>
	                        </thead>
	                        <tbody></tbody>
	                    </table>

		                <div class="form-group text-center hide">
		                    <a href="javascript:void(0);" class="btn pagination" id="normalPagination" title="Click here to load more data." data-control="button" data-role="load-more-normal-data">    
		                        Click here to load more data
		                    </a>
		                </div>

		                <div class="form-group no-data text-center hide" id="normalNoData">
		                </div>

		            </div>
		            <!-- END OF NORMAL LEAVE APPROVAL TAB -->

		            <div class="tab-panel" id="lwpApprovalTab">
		                <div class="form-group">
		                    <table class="table width-12" cellpadding="0" cellspacing="0" id="lwpLeaveTable">
		                        <thead>
		                            <tr>                                
		                                <th>Employee ID</th> 
		                                <th>Employee Name</th>
		                                <th>Leave Type</th>
		                                <th>From</th>
		                                <th>To</th>
		                                <th>Half Day</th>
		                                <th>Remarks</th>
		                                <th>Approval</th>
		                                <th>ApprovalBy</th>                          
		                            </tr>
		                        </thead>
		                        <tbody></tbody>
		                    </table>
		                </div>
		                <!-- /.row -->

		                <div class="text-center hide">
		                    <a href="javascript:void(0);" class="btn pagination" id="lwpPagination" title="Click here to load more data." data-control="button" data-role="load-more-lwp-data">    
		                        Click here to load more data
		                    </a>
		                </div>

		                <div class="no-data text-center hide" id="lwpNoData">
		                </div>

		            </div>
		            <!-- END OF LWP APPROVAL TAB -->
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
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#approvalCommentDialog">
                        Cancel
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
    <div class="modal fade" id="resultDialog" role="dialog" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <a href="javascript:void(0);" class="close pull-right" data-control="close" data-dismiss="modal" data-target="#resultDialog">X</a>
                    <h4>Leave Import Result</h4>
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#resultDialog">
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
    <link rel="stylesheet" href="../../../resources/css/datepicker.css" />
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/transaction/leave/details.js"></script>
</asp:Content>
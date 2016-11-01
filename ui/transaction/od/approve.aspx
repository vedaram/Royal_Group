<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="approve.aspx.cs" Inherits="od_approve" Title="OD Approval | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">OD APPROVAL</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
				<a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters" data-target="#leavesApprovalFilters">
	                <span class="fa fa-filter"></span>
	                 Filters
	            </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="odLeavesApprovalFilters">
    	<div class="drawer-body">
	        <form method="" id="odLeavesApprovalFiltersForm">
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
        			<button type="button" class="btn btn-blue" id="filterButton" data-control="button" data-role="filter-data" data-loading-text="Filtering ...">
                        <span class="fa fa-search"></span>
                         Search
                    </a>
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
		            <div class="tab-panel active" id="leaveApprovalTab">
		                <div class="action-bar">
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
		                <!-- /.action-bar -->
		                <table class="table width-12" cellpadding="0" cellspacing="0" id="odLeaveApprovalTable">
		                    <thead>
		                        <tr>
		                            <th><input type="checkbox" id="CheckAll"/></th>
		                            <th>Employee ID</th> 
		                            <th>Employee Name</th>
		                            <th>Leave Type</th>
		                            <th>From</th>
		                            <th>To</th>
		                            <th>Half Day</th>
		                            <th>Remarks</th>
		                            <th>Status</th>                        
		                        </tr>
		                    </thead>
		                    <tbody></tbody>
		                </table>

			            <div class="form-group text-center hide">
			                <a href="javascript:void(0);" class="btn pagination" id="odPagination" title="Click here to load more data." data-control="button" data-role="load-more-od-data">    
			                    Click here to load more data
			                </a>
			            </div>

			            <div class="form-group no-data text-center hide" id="odNoData">
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
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css" />
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/transaction/od/approve.js"></script>
</asp:Content>
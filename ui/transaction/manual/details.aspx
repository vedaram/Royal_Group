<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="details.aspx.cs" Inherits="manual_details" Title="Manual Punch Details | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">MANUAL PUNCH DETAILS</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
	            <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters" data-target="#ManualpunchApprovalFilters">
	                <span class="fa fa-filter"></span>
	                 Filters
	            </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="ManualpunchApprovalFilters">
    	<div class="drawer-body">
	        <form method="" id="ManualpunchFiltersForm">
	            <div class="container">
	                <div class="form-group">   
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>From</small></strong></label>
	                        <input type="text" class="datepicker form-control" id="filter_indate" name="filter_indate" />
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>To</small></strong></label>
	                        <input type="text" class="datepicker form-control" id="filter_outdate" name="filter_outdate" />
	                    </div>
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Status</small></strong></label>
	                        <select class="form-control" id="filter_ManualStatus" name="filter_ManualStatus">
	                            <option value="select">-- Select --</option>
	                            <option value="1">Submitted</option>
	                            <option value="2">Approved</option>
	                            <option value="3">Declined</option>
	                            <option value="4">Cancelled</option>
	                        </select>
	                    </div>
	                    <!-- /.col-3 -->
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
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Keyword</small></strong></label>
	                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
	                    </div>
	                    <!-- /.col-3 -->
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
                    </button>
                </div>
            </div>
        </div>
        <!-- /.drawer-footer -->
    </div>
    <!-- /.drawer -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<div class="card no-padding">
			<div class="card-body">
                <table class="table width-12" cellpadding="0" cellspacing="0" id="ManualPunchTable">
                    <thead>
                        <tr>
                            <th>Employee ID</th> 
                            <th>Employee Name</th>                                
                            <th>Work Date</th>
                            <th>Out Date</th>
                            <th>In Punch</th>
                            <th>Out Punch</th>
                            <th>Remarks</th> 
                            <th>Manager Remarks</th> 
                            <th>Hr Remarks</th> 
                            <th>Status</th>
                            <th>Approved By</th>                       
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>

                <div class="form-group text-center hide">
                    <button type="button" class="btn pagination" id="ManualPunchPagination" title="Click here to load more data." data-control="button" data-role="load-more-manual-punch-data">    
                        Click here to load more data
                    </button>
                </div>

                <div class="form-group no-data text-center hide" id="ManuaPunchNoData">
                </div>
			</div>
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
                    <button class="btn btn-blue" id="ApproveManualPunchButton" data-control="button" data-role="confirm-approval" data-loading-text="Processing ...">
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
    <script type="text/javascript" src="../../../resources/js/transaction/manual/details.js"></script>
</asp:Content>
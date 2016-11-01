<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="details.aspx.cs" Inherits="od_details" Title="OD Details | SecurTime" EnableEventValidation="false" %>

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
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="odLeavesDetailsFilters">
    	<div class="drawer-body">
	        <form method="" id="odLeavesDetailsFiltersForm">
	            <div class="container">
	                <div class="form-group">   
	                    <div class="col-4">
	                        <label class="text-blue"><strong><small>From</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_from" name="filter_from" />
	                    </div>

	                    <div class="col-4">
	                        <label class="text-blue"><strong><small>To</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_to" name="filter_to" />
	                    </div>

	                    <div class="col-4">
	                        <label class="text-blue"><strong><small>Company</small></strong></label>
	                        <select class="form-control" id="filter_CompanyCode" name="filter_CompanyCode">
	                        </select>
	                    </div>
	                </div>
	                <!-- /.form-group -->
	                <div class="form-group">
	                    <div class="col-4">
	                        <label class="text-blue"><strong><small>Filter By</small></strong></label>
	                        <select class="form-control" id="filter_by" name="filter_by">
	                            <option value="0">-- Select --</option>
	                            <option value="1">Employee ID</option>
	                            <option value="2">Employee Name</option>
	                        </select>
	                    </div>

	                    <div class="col-4">
	                        <label class="text-blue"><strong><small>Keyword</small></strong></label>
	                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
	                    </div>

	                    <div class="col-4">
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
                    <button type="button" id="filterButton" class="btn btn-blue" data-control="button" data-role="filter-data" data-loading-text="Filtering ...">
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
				<table class="table width-12" cellpadding="0" cellspacing="0" id="odLeaveDetailsTable">
                    <thead>
                        <tr>                            
                            <th>Employee ID</th> 
                            <th>Employee Name</th>
                            <th>Leave Type</th>
                            <th>From</th>
                            <th>To</th>
                            <th>Remarks</th>
                            <th>Status</th>   
                            <th>ApproveBy</th>                        
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
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css" />
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/transaction/od/details.js"></script>
</asp:Content>
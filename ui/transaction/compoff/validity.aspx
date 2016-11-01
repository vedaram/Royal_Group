<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="validity.aspx.cs" Inherits="compoff_validity" Title="Comp Off Validity | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">COMP OFF VALIDITY</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
				<a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters" data-target="#CompoffValidityFilters">
	                <span class="fa fa-filter"></span>
	                 Filters
	            </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="CompoffValidityFilters">
    	<div class="drawer-body">
	        <form method="" id="CompoffFiltersForm">
	            <div class="container">
	                <div class="form-group">   
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>From</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_indate" name="filter_indate" />
	                    </div>

	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>To</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_outdate" name="filter_outdate" />
	                    </div>
	                    
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Status</small></strong></label>
	                        <select class="form-control" id="filter_CompoffStatus" name="filter_CompoffStatus">
	                            <option value="select">-- Select --</option>
	                            <option value="0">Available</option>
	                            <option value="1">Applied</option>
	                            <option value="2">Approved</option>
	                            <option value="3">Expired</option>
	                            
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
				<table class="table width-12" cellpadding="0" cellspacing="0" id="CompoffTable">
	                <thead>
	                    <tr>
	                        <th>Employee ID</th> 
	                        <th>Employee Name</th>                                
	                        <th>CompOff Date</th>
	                        <th>Valid Till</th>
	                        <th>Status</th>
	                    </tr>
	                </thead>
	                <tbody></tbody>
	            </table>

	            <div class="form-group text-center hide">
		            <a href="javascript:void(0);" class="btn pagination" id="CompoffPagination" title="Click here to load more data." data-control="button" data-role="load-more-Comp-off-data">    
		                Click here to load more data
		            </a>
		        </div>

		        <div class="form-group no-data text-center hide" id="CompoffNoData">
		        </div>
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
    <script type="text/javascript" src="../../../resources/js/transaction/compoff/validity.js"></script>
</asp:Content>
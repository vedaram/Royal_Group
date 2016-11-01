<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="ot_eligibility.aspx.cs" Inherits="masters_ot_eligibility" Title="OT Eligibility Master | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <style type="text/css">
        #filterForm label {
            margin-left: 10px;
        }
    </style>
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">OT ELIGIBILITY MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter"></span> Filters
                </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container" id="filters">
    	<div class="card">
	        <div class="card-body">
	            <form method="" id="filterForm">
	                <div class="form-group">
	                    <div class="col-2">
	                        <label><strong><small>Company</small></strong></label>
	                        <select class="form-control" id="filter_company" name="filter_company">
	                            <option value="select">Select Company</option>
	                        </select>
	                    </div>

	                    <div class="col-2">
		                    <label><strong><small>Branch</small></strong></label>
		                    <select class="form-control" id="filter_branch" name="filter_branch">
		                        <option value="select">Select Branch</option>
		                    </select>
		                </div>

		                <div class="col-2">
		                    <label><strong><small>Department</small></strong></label>
		                    <select class="form-control" id="filter_department" name="filter_department">
		                        <option value="select">Select Department</option>
		                    </select>
		                </div>

		                <div class="col-2">
		                    <label><strong><small>Designation</small></strong></label>
		                    <select class="form-control" id="filter_designation" name="filter_designation">
		                        <option value="select">Select Designation</option>
		                    </select>
		                </div>
	                    
	                    <div class="col-2">
	                        <label><strong><small>Filter By</small></strong></label>
	                        <select class="form-control" id="filter_by" name="filter_by">
	                            <option value="0">-- Select --</option>
	                            <option value="1">Employee Code</option>
	                            <option value="2">Employee Name</option>
                                <option value="3">Enrollment ID</option>
	                        </select>
	                    </div>
	                    
	                    <div class="col-2">
	                        <label><strong><small>Keyword</small></strong></label>
	                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
	                    </div>
	                </div>
	                <!-- /.form-group -->
	            </form>
	            <!-- /.form -->
	        </div>
	        <!-- /.drawer-body -->
	        <div class="card-footer">
	            <div class="container">
	                <div class="pull-right">
	                    <a class="btn" data-control="button" data-role="filters/reset">
	                        <span class="fa fa-refresh"></span> Reset Filters
	                    </a>
	                    <button class="btn btn-blue" id="filterButton" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
	                        <span class="fa fa-search"></span> Search
	                    </button>
	                </div>
	                <!-- /.pull-right -->
	            </div>
	            <!-- /.container -->
	        </div>
	        <!-- /.drawer-footer -->
       	</div>
       	<!-- /.card -->
    </div>
	<!-- /.container -->
    <div class="container text-center" id="noData">
    </div>
    <!-- /.container -->
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table" cellpadding="0" cellspacing="0" id="dataTable">
                    <thead>
                        <tr>
                            <th><input type="checkbox" name="checkall" id="checkall" /></th>
	                        <th>OT Eligibility Status</th>
	                        <th>Employee ID</th>                
	                        <th>Employee Name</th>               
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <!--/. card-body -->
            <div class="card-footer">
            	<div class="text-center">
		            <button class="btn btn-green" id="grantOTEligibility" data-toggle="modal" data-target="#grantDialog" data-loading-text="Processing ...">
		                <span class="fa fa-check"></span> Grant
		            </button>
		            <button class="btn btn-red" id="rejectOTEligibility" data-toggle="modal" data-target="#rejectDialog" data-loading-text="Processing ...">
		                <span class="fa fa-ban"></span> Reject
		            </button>
		        </div>
            </div>
            <!-- /.card-footer -->
        </div>
        <!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="ot-eligibility/more">
                load more data
            </button>
        </div>
    </div>
    <!-- /.container -->
    <div id="grantDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                	<div class="container">
                		<div class="pull-left">
		                	<h4>Grant OT Eligibility</h4>
	                	</div>
	                	<!-- /.pull-left -->
	                	<div class="pull-right">
		                    <a href="javascript:void(0);" class="pull-right close" data-control="close" data-dismiss="modal" data-target="#grantDialog">X</a>
	                   	</div>
	                   	<!-- /.pull-right -->
                   	</div>
                   	<!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p>Are you sure you would like to grant OT Eligibility to the selected employees?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#grantDialog">
                        Cancel
                    </a>
                    <button class="btn btn-blue" id="grantOTEligibilityButton" data-control="button" data-role="ot-eligibility/grant" data-loading-text="Processing ...">
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
    <div id="rejectDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                	<div class="container">
                		<div class="pull-left">
                			<h4>Reject OT Eligibility</h4>
                		</div>
                		<!-- /.pull-left -->
                		<div class="pull-right">
                    		<a href="javascript:void(0);" class="pull-right close" data-control="close" data-dismiss="modal" data-target="#rejectDialog">X</a>
                    	</div>
                    	<!-- /.pull-right -->
                	</div>
                	<!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p>Are you sure you would like to reject OT Eligibility for the selected employees?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#rejectDialog">
                        Cancel
                    </a>
                    <button class="btn btn-blue" id="rejectOTEligibilityButton" data-control="button" data-role="ot-eligibility/reject" data-loading-text="Processing ...">
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
    <script type="text/javascript" src="../../../resources/js/company/ot_eligibility.js"></script>
</asp:Content>
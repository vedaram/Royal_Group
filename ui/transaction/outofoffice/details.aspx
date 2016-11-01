<%@ Page Title="OOO Details | SecurTime" Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="details.aspx.cs" Inherits="outofoffice_details" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">OUT OFF OFFICE DETAILS</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
				<a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="toggle-filters" data-target="#OutOffOfficeApprovalFilters">
	                <span class="fa fa-filter"></span>
	                 Filters
	            </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="OutOffOfficeApprovalFilters">
		<div class="drawer-body">
	        <form method="" id="OutOffOfficeFiltersForm">
	            <div class="container">
	                <div class="form-group">   
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>From</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_indate" name="filter_indate" />
	                    </div>
	                    <div class="col-3 hide">
	                        <label class="text-blue"><strong><small>To</small></strong></label>
	                        <input type="text" class="date-picker form-control" id="filter_outdate" name="filter_outdate" />
	                    </div>
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Status</small></strong></label>
	                        <select class="form-control" id="filter_OutOffOffice" name="filter_OutOffOffice">
	                            <option value="select">-- Select --</option>
	                            <option value="1">Submitted</option>
	                            <option value="2">Approved</option>
	                            <option value="3">Rejected</option>	                            
	                        </select>
	                    </div>
	                     <div class="col-3">
	                        <label class="text-blue"><strong><small>OutOffOffice Type</small></strong></label>
	                        <select class="form-control" id="filter_OutOffOfficetype" name="filter_OutOffOfficetype">
	                            <option value="select">-- Select --</option>
	                            <option value="1">Personal</option>
	                            <option value="2">Local Business</option>
	                            <option value="3">Business Trip/Training</option>	                            
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
	                    <!-- /.col-3 -->
	                    <div class="col-3">
	                        <label class="text-blue"><strong><small>Keyword</small></strong></label>
	                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
	                    </div>
	                   
	                    <!-- /.col-3 -->
	                </div>
	            </div>
	            <!-- /.container -->
	        </form>
	        <!-- /form -->
        </div>
        <!-- /.card-body -->
        <div class="card-footer">
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
        <!-- /.card-footer -->
    </div>
    <!-- /.filters -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<div class="card no-padding">
			<div class="card-body">
				<div class="tab-content">
					<div class="tab-panel active">
						
				        <!-- /.action-bar -->
				        <table class="table width-12" cellpadding="0" cellspacing="0" id="OutOffOfficeTable">
			                <thead>
			                    <tr>			                        
			                        <th>Emp ID</th> 
			                        <th>Emp Name</th>   
			                        <th>OutOfOffice Type</th>                             
			                        <th>From Date</th>
			                        <th>From Time</th>
			                        <th>To Date</th>
			                        <th>To Time</th>
			                        <th>Hours</th>			                        
			                        <th>Total Hours Availed</th>
			                        <th id="HRC1">Manager</th>
			                        <th id="HRC2">Manager Remark</th>
			                        <th>Reason</th> 
			                        <th>Approval</th> 			                        
			                    </tr>
			                </thead>
			                <tbody></tbody>
			            </table>
			            <div class="form-group text-center hide">
				            <a href="javascript:void(0);" class="btn pagination" id="OutOffOfficePagination" title="Click here to load more data." data-control="button" data-role="load-more-out-off-office-data">    
				                Click here to load more data
				            </a>
				        </div>

				        <div class="form-group no-data text-center hide" id="OutOffOfficeNoData">
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
    
    <!-- /.modal -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css" />
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/transaction/outofoffice/details.js"></script>
</asp:Content>


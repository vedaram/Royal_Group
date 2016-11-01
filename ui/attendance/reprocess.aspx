<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="reprocess.aspx.cs" Inherits="attendance_reprocess" Title="Reprocess | SecurTime" EnableEventValidation="false"%>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">ATTENDANCE</small> / <span class="current-page">REPROCESSED DATA</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        	<div class="action-bar">
        	</div>
        	<!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<div class="card" id="filters">
			<div class="card-body">
				<form id="reprocessDataFiltersForm" class="form" method="post">
		            <div class="form-group">
		                <div class="col-2">
		                    <label class="text-blue"><strong><small>Company</small></strong></label>
		                    <select class="form-control" id="filter_company" name="filter_company" disabled>
		                        <option value="select">Select a Company</option>
		                    </select>
		                </div>
		                <!--/.col-2 -->
		                <div class="col-2">
		                    <label class="text-blue"><strong><small>Branch</small></strong></label>
		                    <select class="form-control" id="filter_branch" name="filter_branch" disabled>
		                        <option value="select">Select a Branch</option>
		                    </select>
		                </div>
		                <!--/.col-2 -->
		                <div class="col-2">
		                    <label class="text-blue"><strong><small>Department</small></strong></label>
		                    <select class="form-control" id="filter_department" name="filter_department" disabled>
		                        <option value="select">Select a Department</option>
		                    </select>
		                </div>
		                <!--/.col-2 -->
		                <div class="col-2">
		                    <label class="text-blue"><strong><small>Designation</small></strong></label>
		                    <select class="form-control" id="filter_designation" name="filter_designation" disabled>
		                        <option value="select">Select a Designation</option>
		                    </select>
		                </div>
		                <!--/.col-2 -->
		                <div class="col-2">
		                    <label class="text-blue"><strong><small>Filter By</small></strong></label>
		                    <select class="form-control" id="filter_by" name="filter_by">
		                        <option value="0">--Select--</option>
		                        <option value="1">Employee ID</option>
		                        <option value="2">Employee Name</option>
		                        <option value="3">Enrollment ID</option>
		                    </select>
		                </div>
		                <!--/.col-2 -->
		                <div class="col-2">
		                    <label class="text-blue"><strong><small>Keyword</small></strong></label>
		                    <input type="text" class="form-control" name="filter_keyword" id="filter_keyword" />
		                </div>
		                <!--/.col-2 -->
					</div>
				</form>
			</div>
			<!-- /.card-body -->
			<div class="card-footer">
				<div class="container">
					<button class="btn btn-blue" id="filterDataButton" data-control="button" data-role="filter-data" data-loading-text="Processing ...">
                        <span class="fa fa-search"></span>
                         Search
                    </button>

                   <a tabindex="0" class="btn btn-lg btn-danger pull-right" role="button" data-toggle="popover" data-trigger="focus" data-placement="left" title="Help" data-content="1. Use the filters to generate a list of employees. <br /> 2. Select one or more employees from the list generated. <br /> 3. Scroll down to the bottom and select your From &amp; To dates. <br /> 4. Click on Reprocess to begin reprocessing.">
                        Help
                    </a>
				</div>
				<!-- /.container -->
			</div>
			<!-- /.card-footer -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
	<div class="container text-center hide" id="noData">
	</div>
	<!-- /.container -->
	<div class="container">
		<div class="card no-padding">
			<div class="card-body">
				<table id="reprocessDataTable" cellspacing="0" cellpadding="0" class="table">
	                <thead>
	                    <th><input type="checkbox" id="checkall" /></th>
	                    <th>Employee ID</th>
	                    <th>Employee Name</th>
	                </thead>
	                <tbody></tbody>
	            </table>
	            <!-- /.table -->
			</div>
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
		<div class="form-group text-center hide">
            <a href="javascript:void(0);" class="btn pagination" id="pagination" title="Click here to load more data." data-control="button" data-role="load-more-data">    
                Click here to load more data
            </a>
        </div>
	</div>
	<!-- /.container -->
	<div class="container hide" id="reprocessContainer">
		<div class="card">
			<div class="card-body">
	            <form class="form" id="reprocessDataForm" method="post">
	                <div class="container">
	                    <div class="col-offset-2 col-4">
	                        <label class="text-blue"><strong><small>From Date</small></strong></label>
	                        <input type="text" class="form-control date-picker" id="from_date" name="from_date" />
	                    </div>
	                    <div class="col-4">
	                        <label class="text-blue"><strong><small>To Date</small></strong></label>
	                        <input type="text" class="form-control date-picker" id="to_date" name="to_date" />
	                    </div>
	                </div>
	                <!-- /.row -->
	            </form>
	            <!-- /.form -->
			</div>
			<!-- /.card-body -->
			<div class="card-footer text-center">
				<button type="button" class="btn btn-blue" id="reprocessDataButton" data-control="button" data-role="reprocess-data" data-loading-text="Reprocessing ...">
                    Reprocess
                </button>
			</div>
			<!-- /.card-footer -->
		</div>
		<!-- /.card -->
	</div>
	<!--/. container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link href="../../resources/css/datepicker.css" rel="stylesheet" type="text/css" />
    <script src="../../resources/lib/datepicker.js" type="text/javascript"></script>
    <script type="text/javascript" src="../../resources/js/attendance/reprocess.js"></script>
    <script type="text/javascript" src="../../resources/lib/moment.min.js"></script>
</asp:Content>
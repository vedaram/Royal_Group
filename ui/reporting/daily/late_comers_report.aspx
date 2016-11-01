<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="late_comers_report.aspx.cs" Inherits="daily_late_comers_report" Title="Daily LateComers Report | SecurTime" EnableEventValidation="false"  %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" Runat="Server">
<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">REPORTING</small> / <small class="current-section">DAILY</small> / <span class="current-page">LATE COMMERS REPORT</span>
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
<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" Runat="Server">
<div class="container">
		<div class="card">
			<div class="card-header">
				<h4>SELECT FILTERS</h4>
			</div>
			<!-- /.card-header -->
			<div class="card-body">
				<div class="container">
					<div class="col-offset-3 col-6">
						<form class="form" id="filterForm" method="post">
							<div class="form-group">
								<div class="col-6">
			                        <label class="control-label">From Date</label>
			                        <input type="text" class="form-control datepicker" name="from_date" id="from_date">
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                        <label class="control-label">To Date</label>
			                        <input type="text" class="form-control datepicker" name="to_date" id="to_date">
			                    </div>
			                    <!-- /.col-6 -->
							</div>
							<!-- /.form-group -->
							<div class="form-group">
								<div class="col-6">
			                        <label class="control-label">Company</label>
			                        <select class="form-control" name="company" id="company" disabled>
			                            <option value="select">Select a Company</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                        <label class="control-label container">
		                        		Branch
			                        	<a href="javascript:void(0);" class="pull-right" data-control="button" data-role="filter/branch/reset">(clear)</a>
		                        	</label>
			                        <select class="form-control" name="branch" id="branch" multiple disabled style="height: 100px;">
			                            <option value="select">Select a Branch</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
							</div>
							<!-- /.form-group -->
							
							<div class="form-group">
			                    <div class="col-6">
			                        <label class="control-label">Department</label>
			                        <select class="form-control" name="department" id="department" disabled>
			                            <option value="select">Select a Department</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                        <label class="control-label">Shift</label>
			                        <select class="form-control" name="shift" id="shift" disabled>
			                            <option value="select">Select a Shift</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                </div>
			                <!-- /.form-group -->
			                
			                <div class="form-group">
			                    <div class="col-6">
			                       <label class="control-label">Category</label>
			                        <select class="form-control" name="category" id="category" disabled >
			                            <option value="select">Select Category</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                        <label class="control-label">Empployee Status</label>
			                        <select class="form-control" name="status" id="status" >
			                           <option value="Active">Active</option>
			                           <option value="Terminated">Terminated</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                </div>
			               
		                </form>
		                <!-- /.form -->
	                </div>
	                <!-- /.col-offset-3 col-6 -->
                </div>
                <!-- /.container -->
			</div>
			<!-- /.card-body -->
			<div class="card-footer">
				<div class="text-center">
					<button class="btn btn-blue" id="exportExcelButton" data-control="button" data-role="export/excel" data-loading-text="Exporting ...">
                        <span class="fa fa-file-image-o" data-role="export/excel"></span>
                         Generating Report
                    </button>
				</div>
				<!-- /.text-center -->
			</div>
			<!-- /.card-footer -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" Runat="Server">
 <link rel="stylesheet" href="../../../resources/css/datepicker.css">
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/reporting/daily/late_comers_report.js"></script>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="leave_report.aspx.cs" Inherits="monthly_leave_report" Title="Leave Report | SecurTime" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">REPORTING</small> / <small class="current-section">MONTHLY</small> / <span class="current-page">LEAVE REPORT</span>
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

<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" runat="server">
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
			                        <label class="control-label">Branch</label>
			                        <select class="form-control" name="branch" id="branch" disabled>
			                            <option value="select">Select a Branch</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
							</div>
							<!-- /.form-group -->
							<div class="form-group">
			                    <div class="col-6">
			                        <label class="control-label">Designation</label>
			                        <select class="form-control" name="designation" id="designation" disabled>
			                            <option value="select">Select a Designation</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                       <label class="control-label">Category</label>
			                        <select class="form-control" name="category" id="category" disabled >
			                            <option value="select">Select Category</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                </div>
			                <!-- /.form-group -->
			                <div class="form-group">
			                    <div class="col-6">
			                        <label class="control-label">Employee ID</label>
			                        <input type="text" class="form-control" name="employee_id" id="employee_id">
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                        <label class="control-label">Employee Name</label>
			                        <input type="text" class="form-control" name="employee_name" id="employee_name">
			                    </div>
			                    <!-- /.col-6 -->
			                </div>
			                <!-- /.form-group -->
			                
			                 <div class="form-group">
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
                        <span class="fa fa-file-excel-o"></span>
                         Export to Excel
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

<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css">
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/reporting/monthly/leave_report.js" ></script>   
</asp:Content>

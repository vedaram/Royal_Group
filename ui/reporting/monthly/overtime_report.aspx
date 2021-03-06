﻿<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="overtime_report.aspx.cs" Inherits="monthly_overtime_report" Title="Monthly Overtime Report | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">REPORTING</small> / <small class="current-section">MONTHLY</small> / <span class="current-page">OVERTIME REPORT</span>
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
			                        <label class="control-label">Month</label>
			                        <select class="form-control" name="month" id="month">
			                            <option value="select">Select a Month</option>
			                            <option value="Jan">January</option>
			                            <option value="Feb">February</option>
			                            <option value="March">March</option>
			                            <option value="April">April</option>
			                            <option value="May">May</option>
			                            <option value="June">June</option>
			                            <option value="July">July</option>
			                            <option value="Aug">August</option>
			                            <option value="Sep">September</option>
			                            <option value="Oct">October</option>
			                            <option value="Nov">November</option>
			                            <option value="Dec">December</option>
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                        <label class="control-label">Year</label>
			                        <select class="form-control" name="year" id="year">
			                            <option value="select">Select a Year</option>
			                        </select>
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

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css">
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/reporting/monthly/overtime_report.js"></script>
</asp:Content>
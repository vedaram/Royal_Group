<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="import_masters.aspx.cs" Inherits="masters_import_masters" Title="Import Masters | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">IMPORT MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">                
                <a href="employee.aspx" class="btn btn-blue">
	        		<span class="fa fa-long-arrow-left"></span> Back
	    		</a>
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
                <div class="container">
                    <div class="pull-left" style="line-height: 30px;">
                        <h4>IMPORT EMPLOYEE MASTER</h4>
                    </div>
                    <!-- /.pull-left -->
                    <div class="pull-right">
                        <a href="../../../exports/templates/employee_masters.xlsx" class="btn btn-green" target="_blank">
                            <span class="fa fa-file-excel-o"></span> Employee Master Template
                        </a>
                    </div>
                    <!-- /.pull-right -->
                </div>
                <!-- /.container -->
            </div>
            <!-- /.card-header -->
			<div class="card-body">
                <div class="form-group">
                    <div class="col-4">
                        <label class="control-label">Choose file to import</label>
                        <input type="file" class="form-control" id="employee_file_upload" name="employee_file_upload" />
                    </div>
                    <!-- /.col-4 -->
                    <div class="col-4">
                        <button class="btn btn-blue" id="importEmployeeButton" data-control="button" data-role="import/employee" data-control-loading="Importing ...">
                            <span class="fa fa-upload"></span>
                             Import Employee Master
                        </button>
                    </div>
                    <!-- /.col-4 -->
                </div>
                <!-- /.form-group -->
                <div class="form-group">
                    <label class="control-label">Employee Master Import Result</label><br />
                    <textarea class="form-control" id="importEmployeeResult" style="height: 200px; border-bottom: 1px solid #ddd;" disabled></textarea>
                </div>
                <!-- /.form-group -->
            </div>
            <!-- /.card-body -->
		</div>
		<!-- /.card -->
        <div class="card">
            <div class="card-header">
                <div class="container">
                    <div class="pull-left" style="line-height: 30px;">
                        <h4>IMPORT COMPANY MASTER</h4>
                    </div>
                    <!-- /.pull-left -->
                    <div class="pull-right">
                        <a href="../../../exports/templates/company_masters.xlsx" class="btn btn-green" target="_blank">
                            <span class="fa fa-file-excel-o"></span> Company Master Template
                        </a>
                    </div>
                    <!-- /.pull-right -->
                </div>
                <!-- /.contanier -->
            </div>
            <!-- /.card-header -->
            <div class="card-body">
                <div class="form-group">
                    <div class="col-4">
                        <label class="control-label">Choose file to import</label>
                        <input type="file" class="form-control" id="company_file_upload" name="company_file_upload" />
                    </div>
                    <!-- /.col-4 -->
                    <div class="col-4">
                        <button class="btn btn-blue" id="importCompanyButton" data-control="button" data-role="import/company" data-control-loading="Importing ...">
                            <span class="fa fa-upload"></span>
                             Import Company Master
                        </button>
                    </div>
                    <!-- /.col-4 -->
                </div>
                <!-- /.form-group -->
                <div class="form-group">
                    <label class="control-label">Company Master Import Result</label><br />
                    <textarea class="form-control" id="importCompanyResult" style="height: 200px; border-bottom: none;" disabled></textarea>
                </div>
                <!-- /.form-group -->
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
		<div class="card">
            <div class="card-header">
                <div class="container">
                    <div class="pull-left" style="line-height: 30px;">
                        <h4>IMPORT EMPLOYEE TRANSACTION</h4>
                    </div>
                    <!-- /.pull-left -->
                    <div class="pull-right">
                        <a href="../../../exports/templates/employee_transaction.xlsx" class="btn btn-green" target="_blank">
                            <span class="fa fa-file-excel-o"></span> Employee Transaction Template
                        </a>
                    </div>
                    <!-- /.pull-right -->
                </div>
                <!-- /.container -->
            </div>
            <!-- /.card-header -->
			<div class="card-body">
                <div class="form-group">
                    <div class="col-4">
                        <label class="control-label">Choose file to import</label>
                        <input type="file" class="form-control" id="transaction_file_upload" name="transaction_file_upload" />
                    </div>
                    <!-- /.col-4 -->
                    <div class="col-4">
                        <button class="btn btn-blue" id="importTransactionButton" data-control="button" data-role="import/transaction" data-control-loading="Importing ...">
                            <span class="fa fa-upload"></span>
                             Import Employee Transaction
                        </button>
                    </div>
                    <!-- /.col-4 -->
                </div>
                <!-- /.form-group -->
                <div class="form-group">
                    <label class="control-label">Employee Transaction Import Result</label><br />
                    <textarea class="form-control" id="importEmployeeTransactionResult" style="height: 200px; border-bottom: 1px solid #ddd;" disabled></textarea>
                </div>
                <!-- /.form-group -->
            </div>
            <!-- /.card-body -->
		</div>
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../../resources/js/company/import_masters.js"></script>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="branch_mapping.aspx.cs" Inherits="masters_branch_mapping" Title="Branch Mapping to HR | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <style type="text/css">
        #branchTable td,
        #managerHRTable td {
            text-align: left!important;
        }
        #branchData .card-body,
        #managerHRData .card-body {
            max-height: 320px;
            overflow-y: hidden;
        }

        #branchData:hover .card-body,
        #managerHRData:hover .card-body {
            overflow-y: scroll;
        }
    </style>
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">BRANCH MAPPING TO HR</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container" style="height: 60%; max-height: 80%;">
		<div class="col-offset-3 col-6" id="managerHRData">
            <div class="container text-center">
                <div class="form-group">
                    <div class="col-6">
                        <label class="pull-left"><strong><small>Company</small></strong></label>
                        <select class="form-control" id="filter_company" name="filter_company">
                            <option value="select">Select Company</option>
                        </select>
                    </div>
                    <!-- /.col-4 -->
                    <div class="col-4">
                        <label class="pull-left"><strong><small>Filter By</small></strong></label>
                        <select class="form-control" id="filter" name="filter">
                            <option value="0">All</option>
                            <option value="1">HR</option>
                            <option value="2">Manager</option>
                        </select>
                    </div>
                    <!-- /.col-4 -->
                </div>
                <!-- /.form-group -->
            </div>
            <!-- /. container -->
			<div class="card no-padding">
				<div class="card-header">
				   <h4>Manager/ HR List</h4>
				</div>
				<!-- /.card-header -->
				<div class="card-body">
                    <table class="table" cellpadding="0" cellspacing="0" id="managerHRTable" style="text-align: left;">
                        <tbody></tbody>
                    </table>

	                <div class="no-data text-center hide" id="noData"></div>
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
		</div>
		<!-- /.col-6 -->
		<div class="col-offset-3 col-6 hide" id="branchData">
			<div class="card no-padding">
				<div class="card-header">
					<h4>Branch List</h4>
				</div>
				<!-- /.card-header -->
				<div class="card-body">
                    <table class="table" cellpadding="0" cellspacing="0" id="branchTable" style="text-align: left;">
                        <tbody></tbody>
                    </table>

	                <div class="no-data text-center hide" id="noData"></div>
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
            <div class="container text-center">
                <a href="javascript:void(0);" class="btn" data-control="button" data-role="branch-mapping/manager"><span class="fa fa-long-arrow-left"></span> Back</a>
                <button class="btn btn-blue" id="updateButton" data-control="button" data-role="branch-mapping/save" data-loading-text="Updating ...">
                    Update Branch Mapping to HR/ Manager
                </button>
            </div>
            <!-- /.container -->
		</div>
		<!-- /.col-6 -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../../resources/js/company/branch_mapping.js"></script>
</asp:Content>
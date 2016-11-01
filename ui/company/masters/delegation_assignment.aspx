<%@ Page Language="C#"MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="delegation_assignment.aspx.cs" Inherits="masters_delegation_assignment" Title="Delegation Assignment | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">DELEGATION ASSIGNMENT MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="delegation-assingment/add">
                    <span class="fa fa-plus"></span> Add Delegation Assignment
                </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container text-center" id="noData">
    </div>
    <!-- /.container -->
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table" cellpadding="0" cellspacing="0" id="dataTable">
                    <thead>
                        <tr>
                            <th>From Date</th> 
                            <th>To Date</th>                      
                            <th>Manager ID</th>
                            <th>Delegation Manager ID</th>
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <!--/. card-body -->
        </div>
        <!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="holiday/more">
                load more data
            </button>
        </div>
    </div>
    <!-- /.container -->
	<div id="saveDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                
                <div class="modal-header">
                	<div class="container">
            			<div class="pull-left">
		                	<h4>Assign Delegation</h4>
		                </div>
		                <!-- /.pull-left -->
		                <div class="pull-right">
		                	<a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#saveDialog" class="text-red pull-right">X</a>
		                </div>
		                <!-- /.pull-right -->
	               	</div>
	               	<!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                	<form class="form" method="post" id="saveForm">
						<div class="form-group">	
                            <label class="control-label">Company</label>
							<select class="form-control" id="company_code" name="company_code">
								<option value="select">Select a Company</option>
							</select>
						</div>
						<div class="form-group">
                            <label class="control-label">Branch</label>
							<select class="form-control" id="branch_code" name="branch_code">
								<option value="select">Select a Branch</option>
							</select>
						</div>
						<!-- /.form-group -->
						<div class="form-group">
                            <label class="control-label">From Date</label>
							<input type="text" class="form-control datepicker" name="from_date" id="from_date" />
						</div>
                        <!-- /.form-group -->
						<div class="form-group">
                            <label class="control-label">To Date</label>
							<input type="text" class="form-control datepicker" name="to_date" id="to_date" />
						</div>
						<!-- /.form-group -->
						<div class="form-group">
							<label class="control-label">Manager</label>
							<select class="form-control" id="manager_id" name="manager_id">
								<option value="select">Select a Manager</option>
							</select>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <label class="control-label">Delegation Manager</label>
							<select class="form-control" id="delegation_manager_id" name="delegation_manager_id">
								<option value="select">Select a Delegation Manager</option>
							</select>
						</div>
						<!-- /.form-group -->
					</form>
					<!--- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                	<a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">
                        Cancel
                    </a>
                	<button class="btn btn-blue" id="saveButton" data-control="button" data-role="delegation-assignment/save" data-loading-text="Processing ...">
						Assign Delegation
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
    <link rel="stylesheet" href="../../../resources/css/datepicker.css">
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/company/delegation_assignment.js"></script>
</asp:Content>
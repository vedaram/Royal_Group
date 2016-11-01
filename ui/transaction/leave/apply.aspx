<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="apply.aspx.cs" Inherits="leave_apply" Title="Leave Application | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">

    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">LEAVE APPLICATION</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
            	<button class="btn btn-blue hide" id="exportButton" data-control="button" data-role="leave/export" data-loading-text="Exporting ...">
            		<span class="fa fa-file-excel-o" data-role="leave/export"></span> Export to Excel
            	</button>
            	<a href="javascript:void(0);" class="btn btn-blue hide" data-control="button" data-role="toggle-import" data-loading-text="Processing ...">
	                <span class="fa fa-upload" data-role="toggle-import"></span>
	                 Import Template
	            </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->     
    
    <div class="drawer" id="importTemplate" >
    	<div class="drawer-body">
	        <form class="form" id="importTemplateForm">
	            <div class="form-group">
	                <div class="col-4">
	                    <label class="text-blue"><strong><small>Choose file to upload</small></strong></label>
	                    <input type="file" class="form-control" id="import_leaves" name="import_leaves">
	                </div>
	                <!-- /.width-4 -->
	                <div class="col-4">
	                    <button type="button" class="btn btn-blue" id="importTemplateButton" data-control="button" data-role="import-excel" data-loading-text="Processing ..." disabled>Import Leaves</button>
	                </div>
	                <!-- /.width-4 -->
	            </div>
	            <!-- /.row -->
	        </form>
        </div>
        <!-- /.drawer-body -->
    </div>
    <!-- /.drawer-body -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<div class="col-offset-2 col-4">
			<div class="card">
				<div class="card-header">
					<h4>APPLY LEAVE</h4>
				</div>
				<!-- /.card-header -->
				<div class="card-body">
	                <form class="form" method="post" id="saveLeaveForm">
	                    <div class="form-group">
	                        <input type="text" class="form-control" name="employee_id" id="employee_id" data-loading-text="Validating ..."/>
	                        <label class="control-label">Employee ID</label>
	                    </div>
                    	<!-- /.form-group -->
	                    <div class="form-group">
	                        <select class="form-control" name="leave_type" id="leave_type">
	                        </select>
	                        <label class="control-label">Leave Type</label>
	                    </div>
	                    <!-- /.form-group -->
	                    <div class="form-group">
	                        <label>
	                            <input type="checkbox" id="half_day" name="half_day" /> <strong><small class="text-blue">Half Day</small></strong>
	                        </label>
	                    </div>
	                    <!-- /.form-group -->
	                    <div class="form-group">
	                        <div class="col-6">
	                        	<label class="text-blue"><strong><small>From</small></strong></label>
	                            <input type="text" class="form-control datepicker" name="from_date" id="from_date">
	                        </div>

	                        <div class="col-6">
	                        	<label class="text-blue"><strong><small>To</small></strong></label>
	                            <input type="text" class="form-control datepicker" name="to_date" id="to_date">
	                        </div>
	                    </div>
	                    <!-- /.form-group -->
	                    <div class="form-group">
	                        <textarea rows="3" class="form-control" name="reason" id="reason"></textarea>
	                        <label class="control-label">Reason</label>
	                    </div>
	                    <!-- /.form-group -->
                	</form>
				</div>
				<!-- /.card-body -->
				<div class="card-footer">
					<div class="text-center">
						<button class="btn btn-blue" id="saveLeaveButton" data-control="button" data-role="leave/validate" data-loading-text="Saving ...">
	                        <span class="fa fa-floppy-o" data-role="leave/validate"></span> Save
	                    </button>
                    </div>
                    <!-- /.text-center -->
				</div>
				<!-- /.card-footer -->
			</div>
			<!-- /.card -->
		</div>
		<!-- /.col-8 -->
		<div class="col-4">
			<div class="card no-padding">
				<div class="card-header">
					<h4>AVAILABLE LEAVE BALANCE</h4>
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<table class="table width-12" cellpadding="0" cellspacing="0" id="leavesAvailableTable">
	                    <thead>
	                        <th>Leave Type</th>
	                        <th>Number of Leaves</th>
	                    </thead>
	                    <tbody></tbody>
	                </table>
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
		</div>
		<!-- /.col-4 -->
	</div>
	<!-- /.container -->
	<div class="modal fade" id="importResultDialog" role="dialog" tabindex="-1">
		<div class="modal-dialog">
			<div class="modal-content">
				<div class="modal-header">
					<div class="container">
						<div class="pull-left">
							Leave Import Result
						</div>
						<!-- /.pull-left -->
						<div class="pull-right">
							<a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#importResultDialog">X</a>		
						</div>
						<!-- /.pull-right -->
					</div>
					<!-- /.container -->
				</div>
				<!-- /.modal-header -->
				<div class="modal-body">
					<textarea class="form-control" id="importResult" style="height: 250px;" readonly></textarea>
				</div>
				<!-- /.modal-body -->
				<div class="modal-footer text-right">
					<a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#importResultDialog">Close</a>
				</div>
			</div>
			<!-- /.modal-content -->
		</div>
		<!-- /.modal-dialog -->
	</div>
	<!-- /.modal -->
	
	<div class="modal fade" tabindex="-1" role="dialog" id="checkSandwichDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Leave Application</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#checkSandwichDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#checkSandwichDialog">No</a>
                    <button class="btn btn-blue" id="confirmSandwichButton" data-control="button" data-role="leave/submit" data-loading-text="Processing ...">Yes</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../../resources/js/transaction/leave/apply.js"></script>
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <link rel="stylesheet" href="../../../resources/css/datepicker.css">
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="apply.aspx.cs" Inherits="manual_apply" Title="Manual Punch Application | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">MANUAL PUNCH APPLICATION</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a href="../../../exports/templates/employee_manual_punch.xlsx" class="btn btn-blue" target="_blank">
                    <span class="fa fa-download"></span> Download Template
                </a>
            	<a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="import/toggle">
            		<span class="fa fa-upload"></span> Import
            	</a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="importLeavesBox">
    	<div class="drawer-body">
	        <form method="post" id="importLeavesForm">
	            <div class="container">
	                <div class="form-group">
	                    <div class="col-4">
	                        <label class="text-blue"><strong><small>Choose file to import</small></strong></label>
	                        <input type="file" class="form-control" id="file_upload" name="file_upload" />
	                    </div>
	                    <!-- /.col-4 -->
	                    <div class="col-4">
	                        <button type="button" class="btn btn-blue" id="manualPunchImportButton" data-control="button" data-role="import-manual-punch" data-loading-text="Importing ...">
			                    <span class="fa fa-upload"></span> Import Manual Punch
			                </button>
	                    </div>
	                    <!-- /.col-4 -->
	                </div>
	                <!-- /.row -->
	            </div>
	            <!-- /.container -->
	        </form>
	        <!-- /form -->
        </div>
        <!-- /.drawer-body -->
    </div>
    <!-- /.drawer -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">'
	<div class="container">
		<div class="card">
			<div class="card-header">
				<h4>APPLY MANUAL PUNCH</h4>
			</div>
			<div class="card-body">
				<div class="container">
					<div class="col-12">
						<form id="saveManualPunchForm" method="post">
							<div class="form-group">
								<div class="col-offset-2 col-8">
									<label class="control-label">Employee ID</label>
	                            	<input type="text" class="form-control" id="employee_id" name="employee_id" />
	                            </div>
	                            <div class="col-offset-2 col-8" style="display:none">
									<label class="control-label">status</label>
	                            	<input type="text" class="form-control" id="status" name="status" />
	                            </div>
							</div>
							<!-- /.form-group -->
							<div class="form-group">
								<div class="container">
									<div class="col-6">
										<div class="container">
											<div class="col-6">
			                                    <input type="text" class="form-control" id="punch_in_date" name="punch_in_date" />
			                                    <label class="control-label">Punch In Date</label>
			                                </div>
			                                <div class="col-6">
			                                    <input type="text" class="form-control timepicker" id="punch_in_time" name="punch_in_time" />
			                                    <label class="control-label">Punch In Time</label>
			                                </div>
										</div>
										<!-- /.container -->
									</div>
									<!-- /.col-6 -->
									<div class="col-6">
										<div class="container">
											<div class="col-6">
			                                    <input type="text" class="form-control datepicker" id="punch_out_date" name="punch_out_date" />
			                                    <label class="control-label">Punch Out Date</label>
			                                </div>
			                                <div class="col-6">
			                                    <input type="text" class="form-control timepicker" id="punch_out_time" name="punch_out_time" />
			                                    <label class="control-label">Punch Out Time</label>
			                                </div>
										</div>
										<!-- /.container -->
									</div>
									<!-- /.col-6 -->
								</div>
								<!-- /.container -->
							</div>
							<!-- /.form-group -->
							<div class="form-group">
								<div class="container">
									<div class="col-6">
										<div class="container">
											<div class="col-6">
			                                    <input type="text" class="form-control datepicker" id="break_out_date" name="break_out_date" disabled/>
		                                    	<label class="control-label">Break Out Date</label>
			                                </div>
			                                <div class="col-6">
			                                    <input type="text" class="form-control timepicker" id="break_out_time" name="break_out_time" disabled/>
			                                    <label class="control-label">Break Out Time</label>
			                                </div>
										</div>
										<!-- /.container -->
									</div>
									<!-- /.col-6 -->
									<div class="col-6">
										<div class="container">
											<div class="col-6">
												<input type="text" class="form-control datepicker" id="break_in_date" name="break_in_date" disabled/>
												<label class="control-label">Break In Date</label>
											</div>
											<!-- /.col-6 -->
											<div class="col-6">
												<input type="text" class="form-control timepicker" id="break_in_time" name="break_in_time" disabled/>
												<label class="control-label">Break In Time</label>
											</div>
											<!-- /.col-6 -->
										</div>
										<!-- /.container -->
									</div>
									<!-- /.col-6 -->
								</div>
								<!-- /.container -->
							</div>
							<!-- /.form-group -->
							<div class="form-group">
								<div class="col-offset-2 col-8">
									<label class="control-label">Remarks</label>
	                        		<textarea class="form-control" id="remarks" name="remarks"></textarea>
                        		</div>
                        		<!-- /.col-offset-2 col-8 -->
							</div>
							<!-- /.form-group -->
						</form>
						<!-- /.form -->
					</div>
					<!-- /.col-8 -->
				</div>
				<!-- /.container -->
			</div>
			<!-- /.card-body -->
			<div class="card-footer">
				<div class="text-center">
				<button type="button"    class="btn btn-blue" id="move_inpunch" data-control="button" data-role="move-inpunch" style="display:none">
                        <span class="fa fa-floppy-o"></span>
                         Move inpunch to outpunch
                    </button>
					<button type="button" class="btn btn-blue" id="saveManualPunchButton" data-control="button" data-role="save-leave" data-loading-text="Saving ...">
                        <span class="fa fa-floppy-o"></span>
                         Save
                    </button>
				</div>
				<!-- /.text-center -->
			</div>
			<!-- /.card-footer -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
	<div class="modal fade" id="resultDialog" role="dialog" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <a href="javascript:void(0);" class="close pull-right" data-control="close" data-dismiss="modal" data-target="#resultDialog">X</a>
                    <h4>Manual Punch Import Result</h4>
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#resultDialog">
                        Close
                    </a>
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
    <link rel="stylesheet" href="../../../resources/css/timepicki.css">
    
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/timepicki.js"></script>
    <script type="text/javascript" src="../../../resources/js/transaction/manual/apply.js"></script>
</asp:Content>
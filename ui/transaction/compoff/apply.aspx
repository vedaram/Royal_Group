<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="apply.aspx.cs" Inherits="compoff_apply" Title="Comp Off Application | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">COMP OFF APPLICATION</span>
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
				<h4>APPLY COMP OFF</h4>
			</div>
			<!-- /.card-header -->
			<div class="card-body">
				<div class="container">
					<div class="col-offset-3 col-6">
						<form class="form" method="post" id="saveCompoffForm">
		                    <div class="form-group">
		                        <label class="control-label">Employee ID</label>
		                        <input type="text" class="form-control" name="employee_id" id="employee_id" Readonly="true" />
		                    </div>
		                    <!-- /.form-group -->
		                    <div class="form-group">
		                    	<div class="container">
			                        <div class="col-6">
			                            <label class="control-label">From</label>
			                            <input type="text" class="form-control date-picker" name="from_date" id="from_date">
			                        </div>

			                        <div class="col-6">
			                            <label class="control-label">To</label>
			                            <input type="text" class="form-control date-picker" name="to_date" id="to_date">
			                        </div>
		                        </div>
		                        <!-- /.container -->
		                    </div>
		                    <!-- /.form-group -->
		                    <div class="form-group">
		                        <label class="control-label">Reason</label>
		                        <textarea rows="3" class="form-control" name="reason" id="reason"></textarea>
		                    </div>
		                    <!-- /.form-group -->
		                </form>
					</div>
					<!-- /.col-4 -->
				</div>
				<!-- /.container -->
			</div>
			<!-- /.card-body -->
			<div class="card-footer">
				<div class="text-center">
					<button type="button" class="btn btn-green" id="saveCompoffButton" data-control="button" data-role="save-compoff" data-loading-text="Saving ...">
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
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <link rel="stylesheet" href="../../../resources/css/datepicker.css">
    <script type="text/javascript" src="../../../resources/lib/datepicker.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
    <script type="text/javascript" src="../../../resources/js/transaction/compoff/apply.js"></script>
</asp:Content>
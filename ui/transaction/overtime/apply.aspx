<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="apply.aspx.cs" Inherits="overtime_apply" Title="Overtime Application | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">OVERTIME APPLICATION</span>
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
				<h4>APPLY OVERTIME</h4>
			</div>
			<!-- /.card-header -->
			<div class="card-body">
				<div class="container">
					<div class="col-offset-4 col-4">
						<form id="otApplicationForm" action="" method="" class="center-block">
		                    <div class="form-group">
		                        <label class="control-label">Employee ID</label>
		                        <input type="text" class="form-control" readonly="readonly" name="employee_id" id="employee_id"/>
		                    </div>

		                    <div class="form-group">
		                        <label class="control-label">Select Date</label>
		                        <input type="text" class="form-control date-picker"  name="date" id="date" />
		                    </div>

		                    <div class="form-group">
		                        <label class="control-label">Available Overtime</label>
		                        <input type="text" class="form-control" name="available_ot" id="available_ot" disabled="disabled"/>
		                    </div>
		                </form>	
	                </div>
	                <!-- /.col-4 -->
            	</div>
            	<!-- /.container -->
			</div>
			<!-- /.card-body -->
			<div class="card-footer">
				<div class="text-center">
					<button type="button" class="btn btn-blue" id="save_OTbutton" data-control="button" data-action="saveOvertime" data-role="save_OT" data-loading-text="Saving ...">
	                    <span class="fa fa-save"></span>
	                     Save Overtime Application
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
    <script type="text/javascript" src="../../../resources/js/transaction/overtime/apply.js"></script>
</asp:Content>
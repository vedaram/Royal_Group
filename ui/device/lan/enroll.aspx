<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="enroll.aspx.cs" Inherits="lan_enroll" Title="Enroll Card | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">DEVICE</small> / <small class="current-section">LAN</small> / <span class="current-page">ENROLL CARD</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        	<div class="action-bar">
        		<a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="enrollment/add">
	                <span class="fa fa-plus"></span>
	                 New Enrollment
	            </a>
        	</div>
        	<!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container form-group text-center hide" id="noData">
    </div>
    <!-- /.container -->
	<div class="container" id="listView">
        <div class="card no-padding">
        	<div class="card-body">
	            <table class="table width-12" cellpadding="0" cellspacing="0" id="enrollmentTable">
	                <thead>
	                    <tr>
	                        <th>Enroll ID</th>
	                        <th>Card ID</th>
	                        <th>Pin Number</th>
	                        <th>Employee ID</th>
	                        <th>Employee Name</th>
	                        <th>Actions</th>
	                    </tr>
	                </thead>
	                <tbody></tbody>
	            </table>
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
        <div class="form-group text-center hide">
            <button type="button" class="btn btn-blue" id="pagination" data-control="button" data-role="enrollment/more" data-loading-text="Loading ...">
                load more data
            </button>
        </div>
    </div>
    <!-- /.container -->
    
    <div class="modal fade" id="saveDialog" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                	<div class="container">
                		<div class="pull-left">
                			<h4>Enroll Card</h4>
            			</div>
            			<!-- /.pull-left -->
            			<div class="pull-right">
	                    	<a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#saveDialog" class="text-red">X</a>
	                    </div>
	                    <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <form method="post" action="" id="saveForm">
                        <div class="form-group">
                            <label class="control-label">Enroll ID</label>
                            <input type="text" class="form-control required" id="Enrollid" name="Enrollid" />
                        </div>
                        <div class="form-group">
                            <label class="control-label">Card ID</label>
                            <input type="text" class="form-control required" id="cardid" name="cardid" />
                        </div>
                        <div class="form-group">
                            <label class="control-label">Pin Number</label>
                            <input type="text" class="form-control required" id="pin" name="pin" />
                        </div>
                        <div class="form-group">
                            <label class="control-label">Employee ID</label>
                            <input type="text" class="form-control required" id="Empid" name="Empid" disabled />
                        </div>
                        <div class="form-group">
                            <label class="control-label">Employee Name</label>
                            <input type="text" class="form-control required" id="Name" name="Name" disabled />
                        </div>
                        <div class="form-group">
                            <label class="control-label">
                                <input type="checkbox" id="input_mifare" name="input_mifare" value="1"/>&nbsp;
                                MIFare card number input from USB reader
                            </label>
                        </div>
                    </form>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#saveDialog" class="text-red">
                        Cancel
                    </a>
                    <button type="button" id="saveEnrollmentButton" class="btn btn-blue" data-control="button" data-role="enrollment/save" data-loading-text="Saving ...">
                        <span class="fa fa-save"></span>
                         Save
                    </button>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!-- /.modal-content -->
       </div>
       <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    <div class="modal fade" id="deleteDialog" role="dialog" tabindex="-1" >
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                	<div class="container">
                		<div class="pull-left">
		                	<h4>Delete Enrollment</h4>
                		</div>
                		<!-- /.pull-left -->
                		<div class="pull-right">
		                    <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#deleteDialog" class="close pull-right">X</a>
	                    </div>
	                    <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p>Are you sure you would like to delete this Enrollment?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#deleteDialog" class="text-red">
                        Cancel
                    </a>
                    <button class="btn btn-blue" id="deleteEnrollmentButton" data-control="button" data-role="enrollment/confirm-delete" data-loading-text="Processing ...">
                        Proceed
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
	<script type="text/javascript" src="../../../resources/js/device/lan/enroll.js"></script>
</asp:Content>
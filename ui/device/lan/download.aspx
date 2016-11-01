<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="download.aspx.cs" Inherits="lan_download" Title="LAN Template Download | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">DEVICE</small> / <small class="current-section">LAN</small> / <span class="current-page">TEMPLATE DOWNLOAD</span>
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
        <div class="card no-padding">
        	<div class="card-header">
	            <ul class="tablist clearfix">
	                <li class="tab active" id="deviceTabOption">
	                    <a href="javascript:void(0);" data-id="device">
	                        1. Select Device
	                    </a>
	                </li>
	                <li class="tab" id="enrollmentTabOption">
	                    <a href="javascript:void(0);" data-id="enrollment">
	                        2. Enrollment Information
	                    </a>
	                </li>
	            </ul>
            </div>
            <!-- /.card-header -->
            <div class="card-body">
            	<div class="tab-content">
                	<div class="tab-panel active" id="deviceTab">
                        <div class="form-group">
                            <p><span class="fa fa-info-circle"></span> Click on a row to display Enrollment Details for the device.</p>
                        </div>
                        <!-- /.form-group -->
                        <div class="container">
		                    <table class="table width-12" cellpadding="0" cellspacing="0" id="deviceTable">
		                        <thead>
		                            <tr>
		                                <th>Device ID</th>
		                                <th>Device Name</th>
		                                <th>Communication Type</th>
		                                <th>Device IP</th>
		                                <th>Device Model</th>
		                                <th>Status</th>
		                            </tr>
		                        </thead>
		                        <tbody></tbody>
		                    </table>

		                    <div class="row text-center hide">
		                        <a href="javascript:void(0);" class="btn pagination" id="pagination" title="Click here to load more data." data-control="button" data-role="load-more-device-data">
		                            Click here to load more data
		                        </a>
		                    </div>
	                    </div>
	                    <!-- /.container -->
	                    <div class="no-data text-center hide" id="noDeviceData">
	                    </div>
               	 	</div>
               	 	<!-- /.tab-panel -->
	                <div class="tab-panel" id="enrollmentTab">
	                	<div class="form-group container text-center hide" id="noEnrollmentData">
	                    </div>
	                    <!-- /.container -->
	                    <div class="container">
	                        <table class="table width-12" cellpadding="0" cellspacing="0" id="enrollmentTable">
	                            <thead>
	                                <tr>
	                                    <th><input type="checkbox" id="chk_all" name="chk_all" /></th>
	                                    <th>Enroll ID</th>
	                                    <th>Employee_Name</th>
	                                </tr>
	                            </thead>
	                            <tbody></tbody>
	                        </table>
	                    </div>
	                    <!-- /.container -->
	                    <div class="text-center form-group">
	                        <a href="javascript:void(0);" class="text-red" data-control="button" data-role="choose-device">
	                            <span class="fa fa-arow-left"></span>
	                             Choose a different device
	                        </a>
	                        <a href="javascript:void(0);" class="btn btn-red" data-control="button" data-role="delete-template">
	                            <span class="fa fa-trash"></span>
	                             Delete Template
	                        </a>
	                        <button class="btn btn-blue" id="downloadTemplateButton" data-control="button" data-role="download-template" data-loading-text="Downloading ...">
	                            <span class="fa fa-download"></span>
	                             Download Template
	                        </button>
	                    </div>
	                    <!-- /.form-group -->
	                </div>
	                <!-- /.tab-panel -->
	            </div>
	            <!-- /.tab-content -->
	        </div>
	        <!-- /.card-body -->
        </div>
        <!-- /.card -->
    </div><!-- /.container -->

    <div class="modal fade" id="deleteDialog" role="dialog" tabindex="-1">
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
                    <p>Are you sure you would like to delete the enrollments?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#deleteDialog" class="text-red">
                        Cancel
                    </a>
                    <button id="deleteEnrollmentButton" class="btn btn-blue" data-control="button" data-role="confirm-delete-enrollments" data-loading-text="Processing ...">
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
    <script type="text/javascript" src="../../../resources/js/device/lan/download.js"></script>
</asp:Content>
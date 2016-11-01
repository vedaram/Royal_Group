<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="upload.aspx.cs" Inherits="lan_upload" Title="LAN Template Upload | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">DEVICE</small> / <small class="current-section">LAN</small> / <span class="current-page">TEMPLATE UPLOAD</span>
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
                    <li class="tab active" id="enrollmentTabOption">
                        <a href="javascript:void(0);" data-id="enrollment">
                            1. Select Enrollment
                        </a>
                    </li>
                    <li class="tab" id="deviceTabOption">
                        <a href="javascript:void(0);" data-id="device">
                            2. Select Device
                        </a>
                    </li>
                </ul>
            </div>
            <!-- /.card-header -->
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-panel active" id="enrollmentTab">
                        <div class="form-group">
                            <p><span class="fa fa-info-circle"></span> Select one or more Enrollment Templates &amp; click on Next to select device(s).</p>
                        </div>
                        <!-- /.form-group -->
                        <div class="container form-group text-center hide" id="noEnrollmentData">
                        </div>
                        <!-- /.container -->
                        <div class="container">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="enrollmentTable">
                                <thead>
                                    <tr>
                                        <th><input type="checkbox" id="enrollment_check_all" name="enrollment_check_all" /></th>
                                        <th>Enroll ID</th>
                                        <th>Finger 1</th>
                                        <th>Finger 2</th>
                                        <th>Card No.</th>
                                        <th>PIN</th>
                                        <th>Employee Id</th>
                                        <th>Employee_Name</th>
                                    </tr>
                                </thead>
                                <tbody></tbody>
                            </table>

                            <div class="form-group text-center">
                                <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="choose-device">
                                    Next <span class="fa fa-long-arrow-right"></span>
                                </a>
                            </div>
                            <!-- /.form-group -->
                        </div>
                        <!-- /.container -->
                    </div>
                    <!-- /.tab-panel -->
                    <div class="tab-panel" id="deviceTab">
                        <div class="form-group">
                            <p><span class="fa fa-info-circle"></span> Select one or more Devices &amp; click on Upload Templates to upload Templates.</p>
                        </div>
                        <!-- /.form-group -->
                        <div class="no-data text-center hide" id="noDeviceData">
                        </div>
                        <div class="container">
                            <table class="table width-12" cellpadding="0" cellspacing="0" id="deviceTable">
                                <thead>
                                    <tr>
                                        <th><input type="checkbox" id="device_check_all" name="device_check_all" /></th>
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
                        </div>
                        <!-- /.container -->
                        <div class="form-group text-center">
                            <a href="javascript:void(0);" class="text-red" data-control="button" data-role="choose-enrollment">
                                <span class="fa fa-long-arrow-left"></span>
                                 Choose different enrollments
                            </a>
                            <button class="btn btn-blue" id="uploadTemplateButton" data-control="button" data-role="upload-template" data-loading-text="Uploading ...">
                                <span class="fa fa-upload"></span>
                                 Upload Template
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
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../../resources/js/device/lan/upload.js"></script>
</asp:Content>
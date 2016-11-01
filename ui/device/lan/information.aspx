<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="information.aspx.cs" Inherits="lan_information" Title="LAN Device Information | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">DEVICE</small> / <small class="current-section">LAN</small> / <span class="current-page">DEVICE INFORMATION</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="device/add">
                    <span class="fa fa-plus"></span> Add Device
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
                        <th>Device ID</th>
                        <th>Device Name</th>
                        <th>Communication Type</th>
                        <th>Host Name/IP</th>
                        <th>Device Model</th>
                        <th>Status</th>
                        <th>Actions</th>
                    </thead>
                    <tbody></tbody>
                </table>
                <!-- /.table -->
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="device/more">
                load more data
            </button>
        </div>
    </div>
    <!-- /.container -->
    <div class="modal fade" role="dialog" tabindex="-1" id="saveDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Manage Device</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!--/. modal-header -->
                <div class="modal-body">
                    <form method="post" class="form" id="saveForm">
                        <div class="form-group">
                            <input type="text" class="form-control required" id="device_id" name="device_id" />
                            <label class="control-label">Device ID</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control required" id="device_name" name="device_name"/>
                            <label class="control-label">Device Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <select class="form-control" id="communication_type" name="communication_type">
                            	<option value="select">--Select--</option>
	                              <option value="LAN">LAN</option>
	                              <option value="WAN">WAN</option>
	                              <option value="USB">USB</option>
	                              <option value="DNS">DNS</option>
                            </select>
                            <label class="control-label">Communication Type</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control required" id="device_ip" name="device_ip" />
                            <label class="control-label">Device IP</label>
                        </div>
                        <div class="form-group">
                            <select id="device_model" name="device_model" class="form-control">
                              <option value="select">--Select--</option>
                              <option value="I100">I100</option>
                              <option value="VF30">VF30</option>
                              <option value="VP30">VP30</option>
                              <option value="T60">T60</option>
                              <option value="T60">T60</option>
                              <option value="OA1000">OA1000</option>
                              <option value="BSFACE602">BSFACE602</option>
                              <option value="WS700">WS700</option>
                              <option value="Biolite Net">Biolite Net</option>
                              <option value="Biostation">Biostation</option>
                              <option value="HandPunch">HandPunch</option>
                              <option value="I50">I50</option>
                              <option value="T5">T5</option>
                              <option value="CB01">CB01</option>
                              <option value="CB02">CB02</option>
                            </select>
                            <label class="control-label">Device Model</label>
                        </div>
                        <div class="form-group">
                            <label class="control-label">Device Category</label>
                            <br>
                            <label>
                                <input type="checkbox" name="finger_print" id="finger_print">
                                 Finger Print
                            </label>
                            <br >
                            <label>
                                <input type="checkbox" name="card_number" id="card_number">
                                 Card Number
                            </label>
                            <br>
                            <label>
                                <input type="checkbox" name="pin_number" id="pin_number">
                                 PIN Number
                            </label>
                        </div>
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#saveDialog" class="text-red"> Cancel </a>
                    <button class="btn btn-blue" id="saveButton" data-control="button" data-loading-text="Saving ...">Save Device</button>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    <div class="modal fade" role="dialog" tabindex="-1" id="deleteDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Delete Device</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-taarget="#deleteDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <div class="container">
                        <p>Are you sure you would like to delete this device?</p>
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button class="btn btn-blue" id="deleteButton" data-control="button" data-role="device/delete" data-loading-text="Processing ...">Proceed</button>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    <div class="modal fade" id="deviceDialog" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Device Information</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#deviceDialog" class="close pull-right">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p id="statusPlaceholder"></p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" data-control="close" data-dismiss="modal" data-target="#deviceDialog" class="text-red">
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
	<script type="text/javascript" src="../../../resources/js/device/lan/information.js"></script>
</asp:Content>
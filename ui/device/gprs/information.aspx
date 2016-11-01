<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="information.aspx.cs" Inherits="device_information" Title="Device Information | SecurTime" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">DEVICE</small> / <small class="current-section">GPRS</small> / <span class="current-page">DEVICE INFORMATION</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter" data-role="filters/toggle"></span> Filters
                </a>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer container" id="filters">
        <div class="drawer-body">
            <form method="" id="filterForm">
                <div class="form-group">
                    <div class="col-3">
                        <label class="control-label">Filter By</label>
                        <select class="form-control" name="filter_by" id="filter_by">
                            <option value="0">-- Select --</option>
                            <option value="1">Device Location</option>
                            <option value="2">Device ID</option>
                        </select>
                    </div>
                    <!-- /.col-3 -->
                    <div class="col-3">
                        <label class="control-label">Keyword</label>
                        <input type="text" class="form-control" name="filter_keyword" id="filter_keyword">
                    </div>
                    <!-- /.col-3 -->
                </div>
                <!-- /.form-group -->
            </form>
            <!-- /.form -->
        </div>
        <!-- /.drawer-body -->
        <div class="drawer-footer">
            <a class="btn" data-control="button" data-role="filters/reset">
                <span class="fa fa-refresh" data-role="filters/reset"></span> Reset Filters
            </a>
            <button class="btn btn-blue" id="filterButton" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
                <span class="fa fa-search" data-role="filters/data"></span> Search
            </button>
        </div>
        <!-- /.drawer-footer -->
    </div>
    <!-- /.drawer -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container text-center" id="noData">
    </div>
    <!-- /.container -->
    <div class="container">
        <div class="card no-padding" id="listView">
            <div class="card-body">
                <table class="table" cellpadding="0" cellspacing="0" id="dataTable">
                    <thead>
                        <th>Device Serial No.</th>
                        <th>Device Location</th>
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
                            <input type="text" class="form-control required" id="device_location" name="device_location" />
                            <label class="control-label">Device Location</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control required" id="device_id" name="device_id" disabled/>
                            <label class="control-label">Device Serial</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="download_punch_time" name="download_punch_time" disabled="disabled"/>
                            <label class="control-label">Last Connection Date/ Time</label>
                        </div>
                        <!-- /.form-group -->
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
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../../resources/js/device/gprs/information.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="account.aspx.cs" Inherits="user_account" Title="Account | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">USERS</small> / <span class="current-page">ACCOUNT</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <button class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter" data-role="filters/toggle"></span>
                     Filters
                </button>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="container drawer" id="filters">
        <div class="drawer-body">
            <form class="form" method="post" id="filterForm">
                <div class="form-group">
                    <div class="container">
                        <div class="col-4">
                            <label class="text-blue"><strong><small>Access Level</small></strong></label>
                            <select class="form-control" id="filter_access_level" name="filter_access_level">
                                <option value="-99">--Select--</option>
                                <option value="0">Admin</option>
                                <option value="1">Manager</option>
                                <option value="2">Employee</option>
                                <option value="3">HR Manager</option>
                            </select>
                        </div>
                        <!-- /.col-4 -->
                        <div class="col-4">
                            <label class="text-blue"><strong><small>Filter By</small></strong></label>
                            <select class="form-control" id="filter_by" name="filter_by">
                                <option value="0">-- Select --</option>
                                <option value="1">Employee ID</option>
                                <option value="2">Employee Name</option>
                            </select>
                        </div>
                        <!-- /.col-4 -->
                        <div class="col-4">
                            <label class="text-blue"><strong><small>Keyword</small></strong></label>
                            <input type="text" class="form-control" id="filter_keyword" name="filter_keyword">
                        </div>
                        <!-- /.col-4 -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.form-group -->
            </form>
            <!-- /.form -->
        </div>
        <!-- /.drawer-body -->
        <div class="drawer-footer">
            <div class="container">
                <div class="pull-right">
                     <a class="btn" data-control="button" data-role="filters/reset">
                        <span class="fa fa-refresh" data-role="filters/reset"></span> Reset Filters
                    </a>
                    <button class="btn btn-blue" id="filterButton" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
                        <span class="fa fa-search" data-role="filters/data"></span> Search
                    </button>
                </div>
                <!-- /.pull-right -->
            </div>
            <!-- /.container -->
        </div>
        <!-- /.drawer-footer -->
    </div>
    <!-- /.drawer -->
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
                        <th>Employee ID</th>
                        <th>Username</th>
                        <th>Employee Name</th>
                        <th>Password</th>
                        <th>Actions</th>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="account/more">
                load more data
            </button>
        </div>
        <!-- /.text-center -->
    </div>
    <!-- /.container -->
    <div class="modal fade" tabindex="-1" role="dialog" id="deleteDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Delete Employee</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p>Are you sure you would like to delete this employee?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button class="btn btn-blue" id="deleteButton" data-control="button" data-role="account/delete" data-loading-text="Processing ...">Proceed</button>
                </div>
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
        <div class="modal fade" tabindex="-1" role="dialog" id="saveDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Manage Employee</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <form class="form" id="saveForm">
                        <div class="form-group">
                            <input type="text" class="form-control" id="employee_code" name="employee_code">
                            <label class="control-label">Employee Code</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="employee_name" name="employee_name" readonly>
                            <label class="control-label">Employee Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="username" name="username">
                            <label class="control-label">Username</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="password" name="password">
                            <label class="control-label">Password</label>
                        </div>
                        <!--/. form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="confirm_password" name="confirm_password">
                            <label class="control-label">Confirm Password</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <select class="form-control" id="access_level" name="access_level">
                                <option value="0">Admin</option>
                                <option value="1">Manager</option>
                                <option value="2">Employee</option>
                                <option value="3">HR Manage</option>
                            </select>
                            <label class="control-label">User Type</label>
                        </div>
                        <!-- /.form-group -->
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button class="btn btn-blue" id="saveButton" data-control="button" data-role="account/update" data-loading-text="Saving ...">Save Employee</button>
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
    <script type="text/javascript" src="../../resources/js/user/account.js"></script>
</asp:Content>

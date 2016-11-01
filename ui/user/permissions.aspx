<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="permissions.aspx.cs" Inherits="user_permissions" Title="Permissions | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">USERS</small> / <span class="current-page">PERMISSIONS</span>
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
    <div class="drawer" id="filters">
        <div class="drawer-body">
            <form method="post" class="form" id="filterForm">
                <div class="form-group">
                    <div class="container">
                        <div class="col-4">
                            <label class="text-blue"><strong><small>User Category</small></strong></label>
                            <select class="form-control" id="filter_user_category" name="filter_user_category">
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
                                <option value="0">--Select--</option>
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
    <div class="container" id="employeeTab">
        <div class="container text-center" id="noData">
        </div>
        <!-- /.container -->
        <div class="container" id="listView">
            <div class="card no-padding">
                <div class="card-header">
                    <h4>SELECT EMPLOYEE(s)</h4>
                </div>
                <div class="card-body">
                    <table class="table" cellpadding="0" cellspacing="0" id="dataTable">
                        <thead>
                            <th><input type="checkbox" id="check_all" /></th>
                            <th>Employee ID</th>
                            <th>Employee Name</th>
                            <th>Actions</th>
                        </thead>
                        <tbody></tbody>
                    </table>
                </div>
                <!-- /.card-body -->
                <div class="card-footer">
                    <div class="text-center">
                        <button type="button" class="btn btn-blue" id="nextButton" data-control="button" data-role="permissions/show" data-loading-text="Loading ...">
                            Next <span class="fa fa-long-arrow-right" data-role="permissions/show"></span>
                        </button>
                    </div>
                </div>
                <!-- /.card-footer -->
            </div>
            <!-- /.card -->
            <div class="form-group text-center">
                <button class="btn pagination" id="paginationButton" data-control="button" data-role="employee/more">
                    load more data
                </button>
            </div>
        </div>
        <!-- /.container -->
    </div>
    <!-- /#employeeTab -->
    <div class="container hide" id="permissionsTab">
        <div class="container">
            <div class="col-offset-2 col-8">
                <div class="card no-padding">
                    <div class="card-header">
                        <h4>PERMISSIONS</h4>
                        <p style="font-weight: normal; padding: 5px 0 0;"><small><span class="fa fa-info-circle"></span> Click on a permission to assign</small></p>
                    </div>
                    <!-- /.card-header -->
                    <div class="card-body" id="permissions">
                    </div>
                    <!-- /.card-body -->
                </div>
                <!-- /.card -->
            </div>
            <!-- /.col-8 -->
        </div>
        <!-- /.container -->
        <div class="container form-group text-center">
            <a href="javascript:void(0);" class="btn text-red"  data-control="button" data-role="employee/show">
                <span class="fa fa-long-arrow-left" data-role="employee/show"></span> Back
            </a>
            <button type="button" class="btn btn-blue" id="saveButton" data-control="button" data-role="permissions/save" data-loading-text="Saving ...">Save</button>
        </div>
    </div>
    <!-- /#permissionsTab -->
    <div id="deleteDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content"> 
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Delete Permissions</h4>
                        </div>
                        <!--/. pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p>Are you sure you would like to delete permissions for this user?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button id="deleteButton" class="btn btn-blue" data-control="button" data-role="permissions/delete" data-loading-text="Processing ...">Proceed</button>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!--/. modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../resources/js/user/permissions.js"></script>
    <script type="text/javascript" src="../../resources/js/common/permissions_map.js"></script>
    <style type="text/css">
        .permissions-section {
            padding: 20px;
        }
            .permissions-section-header {
                border-bottom: 1px solid #ddd;
                padding-bottom: 5px;
                margin-bottom: 10px;
            }
            .permission {
                cursor: pointer;
                display: inline-block;
                border: 1px solid #ddd;
                padding: 7px 10px;
                margin: 5px;
                font-size: 11px;
            }

        #permissions .fa {
            font-size: 16px;
        }
        #permissions .fa-check-square-o {
            color: #2980b9;
        }
    </style>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="designation.aspx.cs" Inherits="masters_designation" Title="Designation Master | SecurTime" EnableEventValidation="false"%>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">DESIGNATION MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="designation/add">
                    <span class="fa fa-plus"></span> Add Designation
                </a>
                <a class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter"></span> Filters
                </a>
                <button class="btn btn-blue" id="exportButton" data-control="button" data-role="designation/export">
                    <span class="fa fa-file-excel-o"></span> Export to Excel
                </button>
            </div>
            <!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="container drawer" id="filters">
        <div class="drawer-body">
            <form method="" id="filterForm">
                <div class="form-group">
                    <div class="col-4">
                        <label class="text-blue"><strong><small>Company</small></strong></label>
                        <select class="form-control" id="filter_company_code" name="filter_company_code">
                            <option value="select">Select Company</option>
                        </select>
                    </div>
                    
                    <div class="col-4">
                        <label class="text-blue"><strong><small>Filter By</small></strong></label>
                        <select class="form-control" id="filter_by" name="filter_by">
                            <option value="0">-- Select --</option>
                            <option value="1">Designation Code</option>
                            <option value="2">Designation Name</option>
                        </select>
                    </div>
                    
                    <div class="col-4">
                        <label class="text-blue"><strong><small>Keyword</small></strong></label>
                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
                    </div>
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
                        <span class="fa fa-refresh"></span> Reset Filters
                    </a>
                    <button class="btn btn-blue" id="filterButton" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
                        <span class="fa fa-search"></span> Search
                    </button>
                </div>
                <!-- /.pull-right -->
            </div>
            <!-- /.container -->
        </div>
        <!-- /.drawer-footer -->
    </div>
    <!--/. drawer -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container text-center" id="noData">
    </div>
    <!--/. container -->
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table" cellspacing="0" cellpadding="0" id="dataTable">
                    <thead>
                        <th>Designation Code</th>
                        <th>Designation Name</th>
                        <th>Company Name</th>
                        <th>Actions</th>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="designation/more">
                load more data
            </button>
        </div>
    </div>
    <!-- /.container -->
    <div class="modal fade" tabindex="-1" role="dialog" id="deleteDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Delete Designation</h4>
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
                    <p>Are you sure you would like to delete this designation?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button class="btn btn-blue" id="deleteButton" data-control="button" data-role="designation/delete" data-loading-text="Processing ...">Proceed</button>
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
                            <h4>Manage Designation</h4>
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
                            <select class="form-control" id="company_code" name="company_code">
                                <option value="select">Select a Company</option>
                            </select>
                            <label class="control-label">Company Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="designation_code" name="designation_code" placeholder="Designation Code">
                            <label class="control-label">Designation Code</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="designation_name" name="designation_name" placeholder="Designation Name">
                            <label class="control-label">Designation Name</label>
                        </div>
                        <!-- /.form-group -->
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button class="btn btn-blue" id="saveButton" data-control="button" data-loading-text="Saving ...">Save Designation</button>
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
    <script type="text/javascript" src="../../../resources/js/company/designation.js"></script>
</asp:Content>
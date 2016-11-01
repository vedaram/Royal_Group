<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="holiday_group_master.aspx.cs" Inherits="masters_holiday_group"
    Title="Holiday Group Master | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">HOLIDAY GROUP MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="holiday-group/add">
                    <span class="fa fa-plus"></span> Add Holiday Group
                </a>
                <a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter"></span> Filters
                </a>
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
                    <!-- /.col-4 -->
                    <div class="col-4">
                        <label class="text-blue"><strong><small>Filter By</small></strong></label>
                        <select class="form-control" id="filter_by" name="filter_by">
                            <option value="0">-- Select --</option>
                            <option value="1">Group Name</option>
                            <option value="2">Group Code</option>
                        </select>
                    </div>
                    <!-- /.col-4 -->
                    <div class="col-4">
                        <label class="text-blue"><strong><small>Keyword</small></strong></label>
                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
                    </div>
                    <!--/. col-4 -->
                </div>
                <!-- /.form-group -->
            </form>
        </div>
        <!-- /.drawer-body -->
        <div class="drawer-footer">
            <div class="container">
                <div class="pull-right">
                    <a href="javascript:void(0);" class="btn" data-control="button" data-role="filters/reset">
                        <span class="fa fa-refresh"></span> Reset
                    </a>
                    <button class="btn btn-blue" data-control="button" data-role="filters/data" id="filterButton">
                        <span class="fa fa-search"></span> Search
                    </button>
                </div>
                <!-- /.pull-right -->
            </div>
            <!-- /.container -->
        </div>
        <!-- /.drawer-footer -->
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
    <div class="container text-center" id="noData">
    </div>
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table width-12" cellpadding="0" cellspacing="0" id="dataTable">
                    <thead>
                        <tr>
                            <th>Holiday Group Code</th>                
                            <th>Holiday Group Name</th>
                            <th>Company Code</th> 
                            <th>Company Name</th>
                            <th>Actions</th>                          
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="holiday-group/more">
                load more data
            </button>
        </div>
    </div>
    <!-- /.container -->
    
    <div id="saveDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Manage Holiday Group</h4>
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
                    <form method="post" action="" id="saveForm">
                        <div class="form-group">
                            <select class="form-control" id="company_code" name="company_code">
                                <option value="select">Select Company</option>
                            </select>
                            <label class="control-label">Company Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="holiday_group_code" name="holiday_group_code" placeholder="Holiday Group Code"/>
                            <label class="control-label">Group Code</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="holiday_group_name" name="holiday_group_name" placeholder="Holiday Group Name"/>
                            <label class="control-label">Group Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="max_days" name="max_days" placeholder="Maximum Restricted Days"/>
                            <label class="control-label">Maximum Restricted Days</label>
                        </div>
                        <!-- /.form-group -->
                    </form>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button id="saveButton" class="btn btn-blue" data-control="button" data-loading-text="Saving ...">Save Holiday Group</button>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
     <!-- /.modal -->
    <div id="deleteDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Delete Holiday Group</h4>
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="close pull-right" data-control="close" data-dismiss="modal" data-target="#deleteDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p>Are you sure you would like to delete this Holiday Group?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button id="deleteButton" class="btn btn-blue" data-control="button" data-role="holiday-group/delete" data-loading-text="Processing ...">Proceed</button>
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
    <script type="text/javascript" src="../../../resources/js/company/holiday_group.js"></script>
</asp:Content>



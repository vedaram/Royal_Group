<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="holiday_list.aspx.cs" Inherits="masters_holiday_list" Title="Holiday List Master | SecurTime" EnableEventValidation="false" %>
<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">HOLIDAY LIST MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="holiday/add">
                    <span class="fa fa-plus"></span> Add Holiday List
                </a>
                <button class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter"></span> Filters
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
                            <option value="1">Holiday Group</option>
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
    <!-- /.container -->
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table" cellpadding="0" cellspacing="0" id="dataTable">
                    <thead>
                        <tr>
                            <th>Holiday Group</th>
                            <th>Actions</th>                          
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <!--/. card-body -->
        </div>
        <!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="holiday/more">
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
                            <h4>Manage Holiday List</h4>
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
                            <select class="form-control" id="company_code" name="company_code">
                                <option value="select">Select Company</option>
                            </select>
                            <label class="control-label">Company Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <select class="form-control" id="holiday_group_code" name="holiday_group_code">
                                <option value="select">Select Holiday Group</option>
                            </select>
                            <label class="control-label">Holiday Group</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <select class="form-control" id="year" name="year">
                                <option value="select">Select Year</option>
                            </select>
                            <label class="control-label">Year</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                        	<label class="control-label">Holidays</label>
                            <select class="form-control" id="holiday_list" name="holiday_list" style="height: 100px;" multiple>
                            </select>
                        </div>
                        <!-- /.form-group -->
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <div class="container">
                        <div class="pull-left">
                        </div>
                        <!-- /.pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                            <button id="deleteButton" class="btn btn-red" data-control="button" data-role="holiday-list/remove" data-loading-text="Deleting ...">Delete</button>
                            <button id="saveButton" class="btn btn-blue" data-control="button" data-role="holiday-list/save" data-loading-text="Saving ...">Save</button>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-footer -->
            </div>
            <!-- /.modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    <div id="holidayListDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog">
            <div class="modal-content"> 
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>View Holiday List</h4>
                        </div>
                        <!--/. pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#holidayListDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <div class="form-group">
                        <div class="col-offset-4 col-4">
                            <label><strong><small>Year</small></strong></label>
                            <select class="form-control" id="filter_year" name="filter_year">
                                <option value="select">Select Year</option>
                            </select>
                        </div>
                        <!-- /.col-4 -->
                    </div>
                    <!-- /.form-group -->
                    <div class="container">
                        <div class="container text-center" id="noHolidayData">
                        </div>
                        <!-- /.container -->
                        <table cellpadding="0" cellspacing="0" id="holidayListTable" class="table">
                            <thead>
                                <th>Holiday Name</th>
                                <th>From Date</th>
                                <th>To Date</th>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-body -->
            </div>
            <!--/. modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../../resources/js/company/holiday_list.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="leave.aspx.cs" Inherits="masters_leave" Title="Leave Master | SecurTime" EnableEventValidation="false" %>
    
<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">LEAVE MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="leave/add">
                    <span class="fa fa-plus"></span> Add Leave
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
                            <option value="1">Leave Name</option>
                            <option value="2">Leave Code</option>
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
                            <th>Leave Code</th>                
                            <th>Leave Name</th>
                            <th>Company Name</th>
                            <th>Employee Category</th>
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
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="leave/more">
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
                            <h4>Manage Leave</h4>
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
                            <select class="form-control" id="employee_category_code" name="employee_category_code">
                            </select>
                            <label class="control-label">Employee Category</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="leave_code" name="leave_code" placeholder="Leave Code" maxlength="5"/>
                            <label class="control-label">Leave Code</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="leave_name" name="leave_name" placeholder="Leave Name" />
                            <label class="control-label">Leave Name</label>
                        </div>
                        <!-- /.form-group -->
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="leave_status" name="leave_status" placeholder="Leave Status"  />
                            <label class="control-label">Leave Status</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group hide">
                            <input type="text" class="form-control" id="max_leave" name="max_leave" placeholder="Maximum Leave" />
                            <label class="control-label">Maximum Leave</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group hide">
                            <input type="text" class="form-control" id="max_leave_carry_forward" name="max_leave_carry_forward" placeholder="Maximum Leave Carry Forward" />
                            <label class="control-label">Maximum Leave Carry Forward</label>
                        </div>
                        <div class="form-group">
                            <select class="form-control" id="leave_type" name="leave_type">
                            </select>
                            <label class="control-label">Leave Type</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                        	<label>
                                Include week Off
                                 <input type="checkbox" name="week_off_flag" id="week_off_flag" >
                            </label>
                        </div>
                        <!-- /.form-group -->
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button id="saveButton" class="btn btn-blue" data-control="button" data-loading-text="Saving ...">Save Leave</button>
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
                            <h4>Delete Leave</h4>
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
                    <p>Are you sure you would like to delete this leave?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button id="deleteButton" class="btn btn-blue" data-control="button" data-role="leave/delete" data-loading-text="Processing ...">Proceed</button>
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
    <script type="text/javascript" src="../../../resources/js/company/leave.js"></script>
</asp:Content>
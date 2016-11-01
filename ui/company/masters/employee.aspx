<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="employee.aspx.cs" Inherits="masters_employee" Title="Employee Master | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">EMPLOYEE MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" href="manage_employee.aspx#/edit/">
                    <span class="fa fa-plus"></span> Add Employee
                </a>
                <button class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter"></span> Filters
                </button>
                <a class="btn btn-blue" href="import_masters.aspx">
                	<span class="fa fa-upload"></span> Import
                </a>
                <button class="btn btn-blue" data-control="button" data-role="employee/export" id="exportButton" data-loading-text="Exporting ...">
                    <span class="fa fa-file-excel-o"></span> Export Employee
                </button>
				<button class="btn btn-blue" data-control="button" data-role="employee/exporttransaction" id="exportButtonTransaction" data-loading-text="Exporting ...">
                    <span class="fa fa-file-excel-o"></span>Export Transaction
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
                    <div class="col-3">
                        <label class="text-blue"><strong><small>Company</small></strong></label>
                        <select class="form-control" id="filter_company" name="filter_company">
                            <option value="select">Select Company</option>
                        </select>
                    </div>
                    
                    <div class="col-3">
                        <label class="text-blue"><strong><small>Branch</small></strong></label>
                        <select class="form-control" id="filter_branch" name="filter_branch">
                            <option value="select">Select Branch</option>
                        </select>
                    </div>

                    <div class="col-3">
                        <label class="text-blue"><strong><small>Department</small></strong></label>
                        <select class="form-control" id="filter_department" name="filter_department">
                            <option value="select">Select Department</option>
                        </select>
                    </div>

                    <div class="col-3">
                        <label class="text-blue"><strong><small>Designation</small></strong></label>
                        <select class="form-control" id="filter_designation" name="filter_designation">
                            <option value="select">Select Designation</option>
                        </select>
                    </div>
                </div>
                <!-- /.form-group -->
                <div class="form-group">
                    <div class="col-3">
                        <label class="text-blue"><strong><small>Status</small></strong></label>
                        <select class="form-control" id="filter_status" name="filter_status">
                            <option value="1">Active</option>
                            <option value="2">Terminated</option>
                            <option value="3">Resigned</option>
                            <option value="4">Absconding</option>
                        </select>
                    </div>

                    <div class="col-3">
                        <label class="text-blue"><strong><small>Filter By</small></strong></label>
                        <select class="form-control" id="filter_by" name="filter_by">
                            <option value="0">-- Select --</option>
                            <option value="1">Employee Code</option>
                            <option value="2">Employee Name</option>
                            <option value="3">Emnrollment ID</option>
                        </select>
                    </div>
                    
                    <div class="col-3">
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
                            <th>Employee Code</th>                
                            <th>Employee Name</th>
                            <th>Company</th>
                            <th>Branch</th>
                            <th>Department</th>
                            <th>Designation</th>
                            <th>Enroll ID</th>
                             <th>OT eligibility</th>
                             <th>Ramadan eligibility</th>
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
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="employee/more">
                load more data
            </button>
        </div>
    </div>
    <!-- /.container -->
    <div id="deleteDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content"> 
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Delete Employee</h4>
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
                    <p>Are you sure you would like to terminate this employee?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button id="deleteButton" class="btn btn-blue" data-control="button" data-role="employee/delete" data-loading-text="Processing ...">Proceed</button>
                </div>
                <!-- /.modal-footer -->
            </div>
            <!--/. modal-content -->
        </div>
        <!-- /.modal-dialog -->
    </div>
    <!-- /.modal -->
    <div id="reinstateDialog" class="modal fade" role="dialog" tabindex="-1">
        <div class="modal-dialog modal-sm">
            <div class="modal-content"> 
                <div class="modal-header">
                    <div class="container">
                        <div class="pull-left">
                            <h4>Reinstate Employee</h4>
                        </div>
                        <!--/. pull-left -->
                        <div class="pull-right">
                            <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#reinstateDialog">X</a>
                        </div>
                        <!-- /.pull-right -->
                    </div>
                    <!-- /.container -->
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <p>Are you sure you would like to reinstate this employee?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <button id="reinstateButtonNo" class="btn btn-red" data-control="button" data-role="employee/reinstate" data-loading-text="Processing ..." data-reinstate="0">No</button>
                    <button id="reinstateButtonYes" class="btn btn-blue" data-control="button" data-role="employee/reinstate" data-loading-text="Processing ..." data-reinstate="1">Yes</button>
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
    <script type="text/javascript" src="../../../resources/js/company/employee.js"></script>
</asp:Content>
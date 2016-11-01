<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="employee_category.aspx.cs" Inherits="masters_employee_category" Title="Employee Category Master | SecurTime" EnableEventValidation="false" %>
    
<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">EMPLOYEE CATEGORY MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="employee-category/add">
                    <span class="fa fa-plus"></span> Add Employee Category
                </a>
                <button class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter"></span>
                     Filters
                </button>
                <button class="btn btn-blue" data-control="button" data-role="employee-category/export" data-loading-text="Exporting ...">
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
                            <option value="1">Employee Category Name</option>
                            <option value="2">Employee Category Code</option>
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
                            <th>Employee Catgeory Code</th>                
                            <th>Employee Catgeory Name</th>
                            <th>Company Name</th>
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
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="employee-category/more">
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
                            <h4>Manage Employee Catgeory</h4>
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
                            <input type="text" class="form-control" id="employee_category_code" name="employee_category_code" placeholder="Employee Catgeory Code"/>
                            <label class="control-label">Employee Catgeory Code</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="employee_category_name" name="employee_category_name" placeholder="Employee Catgeory Name"/>
                            <label class="control-label">Employee Catgeory Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <label>
                                <input type="checkbox" name="process" id="process">
                                Threshold Hours for OT
                            </label>
                        	<input type="text" class="form-control" id="total_hours" name="total_hours" value="00:00" disabled>
                        </div>
                        <!-- /.form-group -->
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button id="saveButton" class="btn btn-blue" data-control="button" data-loading-text="Saving ...">Save Employee Category</button>
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
                            <h4>Delete Employee Category</h4>
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
                    <p>Are you sure you would like to delete this employee category?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button id="deleteButton" class="btn btn-blue" data-control="button" data-role="employee-category/delete" data-loading-text="Processing ...">Proceed</button>
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
    <script type="text/javascript" src="../../../resources/js/company/employee_category.js"></script>
    <script type="text/javascript" src="../../../resources/lib/timepicki.js"></script>
    <link rel="stylesheet" href="../../../resources/css/timepicki.css" >
</asp:Content>



<%@ Page Language="C#" AutoEventWireup="true" CodeFile="company.aspx.cs" Inherits="masters_company" Title="Company Master | SecurTime" MasterPageFile="~/layout/base.master"%>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">MASTERS</small> / <span class="current-page">COMPANY MASTER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <a class="btn btn-blue" data-toggle="modal" data-target="#saveDialog" data-role="company/add">
                    <span class="fa fa-plus"></span> Add Company
                </a>
                <button class="btn btn-blue" data-control="button" data-role="company/export" data-loading-text="Exporting ..." id="exportButton">
                    <span class="fa fa-file-excel-o"></span> Export to Excel
                </button>
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
    <!--/. container -->
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table" cellspacing="0" cellpadding="0" id="dataTable">
                    <thead>
                        <th>Company Code</th>
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
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="company/more">
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
                            <h4>Delete Company</h4>
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
                    <p>Are you sure you would like to delete this company?</p>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#deleteDialog">Cancel</a>
                    <button class="btn btn-blue" id="deleteButton" data-control="button" data-role="company/delete" data-loading-text="Processing ...">Proceed</button>
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
                            <h4>Manage Company</h4>
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
                            <input type="text" class="form-control" id="company_code" name="company_code" placeholder="Company Code">
                            <label class="control-label">Company Code</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="company_name" name="company_name" placeholder="Company Name">
                            <label class="control-label">Company Name</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <textarea class="form-control" id="company_address" name="company_address" placeholder="Address"></textarea>
                            <label class="control-label">Address</label>
                        </div>
                        <!--/. form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="phone_number" name="phone_number" placeholder="Phone Number">
                            <label class="control-label">Phone Number</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="fax_number" name="fax_number" placeholder="Fax Number">
                            <label class="control-label">Fax Number</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="email_address" name="email_address" placeholder="Email Address">
                            <label class="control-label">Email Address</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="pin_code" name="pin_code" placeholder="PIN Code">
                            <label class="control-label">PIN Code</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <input type="text" class="form-control" id="website" name="website" placeholder="Website URL">
                            <label class="control-label">Website</label>
                        </div>
                        <!-- /.form-group -->
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-dismiss="modal" data-target="#saveDialog">Cancel</a>
                    <button class="btn btn-blue" id="saveButton" data-control="button" data-loading-text="Saving ...">Save Company</button>
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
    <script type="text/javascript" src="../../../resources/js/company/company.js"></script>
</asp:Content>
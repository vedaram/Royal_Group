<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" Title="Leaves Available | SecurTime" CodeFile="available.aspx.cs" Inherits="leave_available" EnableEventValidation="false"%>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">TRANSACTION</small> / <span class="current-page">LEAVES AVAILABLE</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
            <div class="action-bar">
                <button type="button" class="btn btn-blue" id="exportButton" data-control="button" data-role="leave-available/export" data-loading-text="Exporting ...">
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
    <div class="container text-center" id="leaveAvailableNoData">
    </div>
    <!-- /.container -->
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table width-12" cellpadding="0" cellspacing="0" id="leavesAvailableTable">
                    <thead>
                        <th>Employee ID</th>
                        <th>Employee Name</th>
                        <th>Leave Name</th>
                        <th>Max Leaves</th>
                        <th>Leaves Applied</th>
                        <th>Balance Leaves</th>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>
            <!-- /.card-body -->
        </div>
        <!-- /.card -->
        <div class="form-group text-center">
            <a href="javascript:void(0);" class="btn pagination" id="leaveAvailablePagination" title="Click here to load more data." data-control="button" data-role="load-more-leaves-available-data">    
                load more data
            </a>
        </div>
        <div class="form-group no-data text-center hide" id="leaveAvailableNoData">
        </div>
    </div><!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../../resources/js/transaction/leave/available.js"></script>
</asp:Content>

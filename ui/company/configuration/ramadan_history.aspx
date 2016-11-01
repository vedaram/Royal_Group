<%@ Page Title="Ramadan History | SecurTime" Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="ramadan_history.aspx.cs" Inherits="ramadan_history" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" Runat="Server">
     <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">CONFIGURATION</small> / <span class="current-page">Ramadan History</span>
            </div>
        </div>
        <!-- /.pull-left -->
        
       <%-- <div class="pull-right">
            <div class="action-bar">               
                <a class="btn btn-blue" data-control="button" data-role="filters/toggle">
                    <span class="fa fa-filter"></span> Filters
                </a>                
            </div>
            <!-- /.action-bar -->
        </div>--%>
        <!-- /.pull-right -->        
     </div>
     
      <!-- /.container -->
    <div class="container drawer" id="filters">
        <div class="drawer-body">
            <form method="" id="filterForm">
                <div class="form-group">
                    <div class="col-6">
                        <label class="text-blue"><strong><small>Company</small></strong></label>
                        <select class="form-control" id="filter_company_code" name="filter_company_code">
                            <option value="select">Select Company</option>
                        </select>
                    </div>
                    
                    <div class="col-6">
                        <label class="text-blue"><strong><small>Year</small></strong></label>
                        <input type="text" class="form-control" id="filter_year" name="filter_year" />
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
<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" Runat="Server">
     <div class="container text-center data-info" id="noData">
    </div>
    <!-- .container -->
    <div class="container" id="listView">
        <div class="card no-padding">
            <div class="card-body">
                <table class="table" cellpadding="0" cellspacing="0" id="dataTable">
                    <thead>
                        <tr>
                            <th>Company Name</th>                
                            <th>Year</th>
                            <th>From Date</th>
                            <th>To Date</th>                            
                        </tr>
                    </thead>
                    <tbody></tbody>
                </table>
            </div>            
        </div>
    </div>
     <!-- /.container -->
    <div class="text-center">
        <button class="btn pagination" id="paginationButton" data-control="button" data-role="ramadanhistory/more">
            load more data
        </button>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" Runat="Server">
    <script type="text/javascript" src="../../../resources/js/company/ramadan_history.js"></script>
    <script type="text/javascript" src="../../../resources/lib/moment.min.js"></script>
</asp:Content>


<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="upload_status.aspx.cs" Inherits="template_upload_status" Title="Template Upload Status | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">DEVICE</small> / <small class="current-section">GPRS</small> / <span class="current-page">TEMPLATE UPLOAD STATUS</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        	<div class="action-bar">
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
                        <label class="text-blue"><strong><small>Upload Status</small></strong></label>
                        <select class="form-control" id="filter_filter_upload_status" name="filter_upload_status">
                            <option value="-99">-- Select --</option>
                            <option value="2">Uploaded</option>
                            <option value="1">In Progress</option>
                        </select>
                    </div>
                    
                    <div class="col-4">
                        <label class="text-blue"><strong><small>Filter By</small></strong></label>
                        <select class="form-control" id="filter_by" name="filter_by">
                            <option value="0">-- Select --</option>
                            <option value="1">Enrollment ID</option>
                            <option value="2">Employee Name</option>
                            <option value="3">Device ID</option>
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
						<th>Device ID</th>
						<th>Enrollment ID</th>
						<th>Employee Name</th>
						<th>Status</th>
					</thead>
					<tbody></tbody>
				</table>
				<!-- /.table -->
			</div>
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
		<div class="text-center">
			<button type="button" class="btn" id="paginationButton" data-control="button" data-role="enrollment/more" data-loading-text="Loading ...">
				load more data
			</button>
		</div>
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<script type="text/javascript" src="../../../resources/js/device/gprs/upload_status.js"></script>
</asp:Content>
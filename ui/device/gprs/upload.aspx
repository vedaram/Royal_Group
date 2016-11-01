<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="upload.aspx.cs" Inherits="template_upload" Title="Template Upload | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">DEVICE</small> / <small class="current-section">GPRS</small> / <span class="current-page">TEMPLATE UPLOAD</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        	<div class="action-bar">
        	</div>
        	<!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
    <div class="drawer" id="enrollmentFilters">
    	<div class="drawer-header">
    	</div>
    	<!-- /.drawer-header -->
    	<div class="drawer-body">
    		<form class="form" id="filterForm">
	    		<div class="form-group">
					<div class="col-2">
						<label class="control-label">Company</label>
	                    <select class="form-control" id="filter_company" name="filter_company">
	                        <option value="select">Select Company</option>
	                    </select>
					</div>
					<!-- /.col-2 -->
					<div class="col-2">
						<label class="control-label">Branch</label>
	                    <select class="form-control" id="filter_branch" name="filter_branch">
	                        <option value="select">Select Branch</option>
	                    </select>
					</div>
					<!-- /.col-2 -->
					<div class="col-2">
						<label class="control-label">Department</label>
	                    <select class="form-control" id="filter_department" name="filter_department">
	                        <option value="select">Select Department</option>
	                    </select>
					</div>
					<!-- /.col-2 -->
					<div class="col-2">
						<label class="control-label">Filter By</label>
	                    <select class="form-control" id="filter_by" name="filter_by">
	                        <option value="0">-- Select --</option>
	                        <option value="1">Enrollment ID</option>
	                        <option value="2">Employee Code</option>
	                        <option value="3">Employee Name</option>
	                    </select>
					</div>
					<!-- /.col-2 -->
					<div class="col-2">
						<label class="control-label">Keyword</label>
	                	<input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />	
					</div>
					<!-- /.col-2 -->
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
	                    <span class="fa fa-refresh" data-role="filters/reset"></span> Reset Filters
	                </a>
	                <button class="btn btn-blue" id="filterButton" data-control="button" data-role="filters/data" data-loading-text="Filtering ...">
	                    <span class="fa fa-search" data-role="filters/data"></span> Search
	                </button>
    			</div>
    			<!-- /.pull-right -->
    		</div>
    		<!-- /.container -->
    	</div>
    	<!-- /.drawer-footer -->
    </div>
    <!-- /.drawer -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container" id="templateUpload">
		<div class="col-6" id="enrollmentData">
			<div class="card">
				<div class="card-header">
					<div class="container">
						<div class="pull-right">
							<a href="javascript:void(0);" class="btn btn-blue" data-control="button" data-role="enrollment/filters">
								<span class="fa fa-filter" data-role="enrollment/filters"></span> Filters
							</a>
						</div>
						<!-- /.pull-right -->
					</div>
					<!-- /.container -->
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<table class="table" id="dataTable" cellspacing="0" cellpadding="0">
						<thead>	
							<th><input type="checkbox" id="enrollment_check_all" name="check_all"></th>
							<th>Enrollment ID</th>
							<th>Employee Name</th>
						</thead>
						<tbody></tbody>
					</table>
					<!-- /.table -->
					<div class="form-group text-center" id="noData">
					</div>
					<!-- /.container -->
				</div> 
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
	        <div class="text-center">
	            <button class="btn pagination" id="paginationButton" data-control="button" data-role="enrollment/more">
	                load more data
	            </button>
	        </div>
		</div>
		<!-- /.col-6 -->
		<div class="col-6" id="deviceData">
			<div class="card">
				<div class="card-header">
					<div class="container">
						<div class="col-6">
							<input type="text" class="form-control" id="filter_device" name="filter_device" placeholder="Device ID" style="font-weight: normal;">
						</div>
						<!-- /.col-6 -->
						<div class="col-4">
							<button class="btn btn-blue" id="filterButton" data-control="button" data-role="device/filter" data-loading-text="Filtering ...">
	                    		<span class="fa fa-search"></span> Search
	                		</button>
						</div>
						<!-- /.col-4 -->
					</div>
					<!-- /.container -->
				</div>
				<!-- /.card-header -->
				<div class="card-body">
					<table class="table" id="deviceTable" cellspacing="0" cellpadding="0">
						<thead>
							<th><input type="checkbox" id="device_check_all"/></th>
							<th>Device ID</th>
							<th>Device Location</th>
						</thead>
						<tbody></tbody>
					</table>
					<!-- /.table -->
					<div class="form-group text-center" id="noDeviceData">
					</div>
					<!-- /.container -->
				</div>
				<!-- /.card-body -->
			</div>
			<!-- /.card -->
		</div>
		<!-- /.col-6 -->
	</div>
	<!-- /.container -->
	<div class="container">
		<div class="text-center action-container">
			<button type="button" class="btn btn-blue" id="saveButton" data-control="button" data-role="templates/save" data-loading-text="Saving ...">Save Enrollment</button>
			<button type="button" class="btn btn-red" id="deleteButton" data-control="button" data-role="templates/delete" data-loading-text="Deleting ...">Delete Enrollment</button>
			<button type="button" class="btn btn-green" id="rebootButton" data-control="button" data-role="device/reboot" data-loading-text="Processing ...">Reboot Device</button>
		</div>
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<script type="text/javascript" src="../../../resources/js/device/gprs/upload.js"></script>
</asp:Content>
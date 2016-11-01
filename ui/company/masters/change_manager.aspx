<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="change_manager.aspx.cs" Inherits="masters_change_manager" Title="Change Manager | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<style type="text/css">
        #filterForm label {
        	font-size: 11px;
            margin-left: 10px;
        }
    </style>
    <div class="container">
        <div class="pull-left">
        	<div class="current-page-outer">
            	<small class="current-section">MASTERS</small> / <span class="current-page">CHANGE MANAGER</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container" id="filters">
    	<div class="card">
	        <div class="card-body">
	            <form method="" id="filterForm">
	                <div class="form-group">
	                    <div class="col-3">
	                        <label><strong>Company</strong></label>
	                        <select class="form-control" id="filter_company" name="filter_company">
	                            <option value="select">Select Company</option>
	                        </select>
	                    </div>

	                    <div class="col-3">
		                    <label><strong>Branch</strong></label>
		                    <select class="form-control" id="filter_branch" name="filter_branch">
		                        <option value="select">Select Branch</option>
		                    </select>
		                </div>
	                    
	                    <div class="col-3">
	                        <label><strong>Filter By</strong></label>
	                        <select class="form-control" id="filter_by" name="filter_by">
	                            <option value="0">-- Select --</option>
	                            <option value="1">Employee Code</option>
	                            <option value="2">Employee Name</option>
	                            <option value="3">Enrollment ID</option>
	                        </select>
	                    </div>
	                    
	                    <div class="col-3">
	                        <label><strong>Keyword</strong></label>
	                        <input type="text" class="form-control" id="filter_keyword" name="filter_keyword" />
	                    </div>
	                </div>
	                <!-- /.form-group -->
	            </form>
	            <!-- /.form -->
	        </div>
	        <!-- /.drawer-body -->
	        <div class="card-footer">
	            <div class="container">
	                <div class="pull-right">
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
       	<!-- /.card -->
    </div>
	<!-- /.container -->
	<div class="container">
        <div class="card no-padding">
            <div class="card-body">
                <div class="container">
                	<div class="col-4">
                		<div class="form-group">
                			<label><strong>Source Manager</strong></label>
                			<select class="form-control" id="source_manager" name="source_manager">
                			</select>
                		</div>
                		<!-- /.form-group -->
                		<div class="form-group">
                			<label><strong>New Manager</strong></label>
                			<select class="form-control" id="new_manager" name="new_manager">
                			</select>
                		</div>
                		<!-- /.form-group -->
                		<div class="form-group">
                			<button class="btn btn-blue" id="updateButton" data-control="button" data-role="change-manager/update" data-loading-text="Updating ...">
                				Update
                			</button>
                		</div>
                	</div>
                	<!-- /.col-4 -->
                	<div class="col-8" id="employeeData">
                		<br />
                		<div class="container text-center" id="noData">
                		</div>
                		<!-- /.container -->
                		<div class="container" id="listView">
	                		<table class="table" cellpadding="0" cellspacing="0" id="dataTable">
			                    <thead>
			                        <tr>
			                            <th><input type="checkbox" name="checkall" id="checkall" /></th>
				                        <th>Employee ID</th>                
				                        <th>Employee Name</th>               
			                        </tr>
			                    </thead>
			                    <tbody></tbody>
			                </table>
		               	</div>
		               	<!-- /.container -->
                	</div>
                	<!-- /.col-8 -->
                </div>
                <!-- /.container -->
            </div>
            <!--/. card-body -->
        </div>
        <!-- /.card -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<script type="text/javascript" src="../../../resources/js/company/change_manager.js"></script>
</asp:Content>
<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="unprocessed_data.aspx.cs" Inherits="attendance_unprocessed_data" Title="Unprocessed Data | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">ATTENDANCE</small> / <span class="current-page">UNPROCESSED DATA</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        	<div class="action-bar">
        		<button type="button" id="reprocessDataButton" title="Un-Processed data" class="btn btn-blue" data-control="button" data-role="reprocess" data-loading-text="Processing ...">
	                <span class="fa fa-download"></span> Process Data 
	            </button>
        	</div>
        	<!-- /.action-bar -->
        </div>
        <!-- /.pull-right -->
    </div>
    <!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container form-group text-center" id="noData">
		
	</div>
	<!-- /.form-group -->
	<div class="container" id="listView">
		<div class="card no-padding">
			<div class="card-body">
				<table class="table" cellpadding="0" cellspacing="0" id="dataTable">
	                <thead>
	                    <tr>
	                        <th>Enrollment/ Card No.</th>
	                        <th>Employee Code</th>
	                        <th>Punch Time</th>
	                    </tr>
	                </thead>
	                <tbody></tbody>
	            </table>
			</div>
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
		<div class="form-group text-center">
	        <button type="button" class="btn pagination" id="paginationButton" title="Click here to load more data." data-control="button" data-role="reprocess/more" data-loading-text="Loading ...">    
	            Click here to load more data
	        </button>
	    </div>
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../resources/lib/moment.min.js"></script>
	<script type="text/javascript" src="../../resources/js/attendance/unprocessed_data.js"></script>
</asp:Content>
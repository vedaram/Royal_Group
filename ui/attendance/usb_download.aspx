<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="usb_download.aspx.cs" Inherits="attendance_usb_download" Title="USB Download & Process | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">ATTENDANCE</small> / <span class="current-page">USB DOWNLOAD</span>
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
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<div class="card">
			<div class="card-body">
				<div class="form-group">
					<div class="container">
			            <div class="col-3">
			                <label class="text-blue"><strong><small>Select Location</small></strong></label>
			                <select class="form-control" id="device" name="device" disabled>
			                    <option value="select">Select a Location</option>
			                </select>
			            </div>

			            <div class="col-3">
			                <label class="text-blue"><strong><small>Select File</small></strong></label>
			                <input type="file" class="form-control" id="file" name="file">
			            </div>

			            <div class="col-3">
			                <button type="button" class="btn btn-blue" id="importLogsButton" data-control="button" data-role="import-logs" data-loading-text="Processing ..." disabled>
			                    <span class="fa fa-upload"></span>
			                     Import Logs
			                </button>
			            </div>
				    </div>
				    <!-- /.container -->
			    </div>
			    <!-- /.form-group -->
			    <div class="form-group">
			    	<div class="container">
			    		<textarea class="form-control" id="usbDownloadAndProcessResult" style="height: 300px;" disabled></textarea>
			    	</div>
			    	<!-- /.container -->
			    </div>
			    <!-- /.form-group -->
			</div>
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
	<link rel="stylesheet" href="../../resources/css/select2.css">
	<script type="text/javascript" src="../../resources/js/attendance/usb_download.js"></script>
	<script type="text/javascript" src="../../resources/lib/select2.min.js"></script>
</asp:Content>
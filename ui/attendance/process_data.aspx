<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="process_data.aspx.cs" Inherits="attendance_process_data" Title="Process Data | SecurTime" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
	<style type="text/css">
		#listView {

		}
	</style>
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">ATTENDANCE</small> / <span class="current-page">PROCESSED DATA</span>
            </div>
        </div>
        <!-- /.pull-left -->
        <div class="pull-right">
        	<div class="action-bar">
                <button type="button" class="btn btn-blue" id="processButton" data-control="button" data-role="process" data-loading-text="Processing ...">Process Data</button>
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
	<!-- /.container -->
	<div class="container" id="listView">
		<div class="card no-padding">
			<div class="card-body">
				<table class="table" cellspacing="0" cellpadding="0" id="dataTable">
					<thead>
						<tr>
							<th>Enrollment/ Card No.</th>
							<th>Employee Code</th>
							<th>Punch Time</th>
							<th>Device ID</th>
						</tr>
					</thead>
					<tbody></tbody>
				</table>
				<!-- /.table -->
			</div>
			<!-- /.card-body -->
		</div>
		<!-- /.card -->
        <div class="text-center">
            <button class="btn pagination" id="paginationButton" data-control="button" data-role="process/more">
                load more data
            </button>
        </div>
	</div>
	<!-- /.container -->
    <div class="modal fade" role="dialog" tabindex="-1" id="resultDialog">
        <div class="modal-dialog modal-sm">
            <div class="modal-content">
                <div class="modal-header">
                    Process Data Result
                </div>
                <!-- /.modal-header -->
                <div class="modal-body">
                    <textarea id="result" rows="3" class="form-control" readonly></textarea>
                </div>
                <!-- /.modal-body -->
                <div class="modal-footer">
                    <a href="javascript:void(0);" class="btn text-red" data-control="close" data-target="#resultDialog" data-dismiss="modal">Close</a>
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
    <script type="text/javascript" src="../../resources/js/attendance/process_data.js"></script>
    <script type="text/javascript" src="../../resources/lib/moment.min.js"></script>
</asp:Content>

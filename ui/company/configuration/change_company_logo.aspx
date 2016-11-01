<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="change_company_logo.aspx.cs" Inherits="ui_company_configuration_change_company_logo" Title="Change Company Logo | SecurTime" EnableEventValidation="false" %>

<asp:Content ID="Content1" ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <small class="current-section">COMPANY</small> / <small class="current-section">CONFIGURATION</small> / <span class="current-page">CHANGE COMPANY LOGO</span>
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

<asp:Content ID="Content2" ContentPlaceHolderID="saxMasterPageContent" runat="server">
	<div class="container">
		<div class="card">
			<div class="card-header">
				<h4>IMPORT COMPANY LOGO</h4>
			</div>
			<!-- /.card-header -->
			<div class="card-body">
				<div class="container">
					<div class="col-offset-3 col-6">
						<form method="post" class="form" id="saveForm">
							<div class="form-group">
								
			                   
			                    <!-- /.col-6 -->
							</div>
							<!-- /.form-group -->
							<div class="form-group">
								<div class="col-6">
			                        <label class="control-label">Company</label>
			                        <select class="form-control" name="company" id="company" >
			                             <option value="select">Select Company</option>	
			                               	                        
			                        </select>
			                    </div>
			                    <!-- /.col-6 -->
			                    <div class="col-6">
			                        <label class="control-label">Choose file</label>
			                        <input type="file" alt="file upload" id="file"/>
			                    </div>
			                    <!-- /.col-6 -->
							</div>
							<!-- /.form-group -->
							 <div class="image-container" style=" display:none;width: 200px; height: 200px; margin: 0 auto;" id="imageContainer">
                            <img src='<%= Session["display_picture"].ToString() %>' id="display_pic"/>
                        </div>
		                </form>
		                <!-- /.form -->
	                </div>
	                <!-- /.col-offset-3 col-6 -->
                </div>
                <!-- /.container -->
			</div>
			<!-- /.card-body -->
			<div class="card-footer">
				<div class="text-center">
				<%--	<button class="btn btn-blue" data-control="button" id="logo_save" data-role="logo/save" data-loading-text="#" style="display:none">Save </button>
					<button class="btn btn-blue" data-control="button" id="logo_update" data-role="logo/update" data-loading-text="#">Update </button>
				--%></div>
				<!-- /.text-center -->
			</div>
			<!-- /.card-footer -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="saxMasterPageFooter" runat="server">
<script src="../../../resources/js/company/change_company_logo.js" type="text/javascript"></script>
</asp:Content>


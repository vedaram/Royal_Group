<%@ Page Language="C#" MasterPageFile="~/layout/base.master" AutoEventWireup="true" CodeFile="update.aspx.cs" Inherits="profile_update" Title="Profile | SecurTime" EnableEventValidation="false" %>

<asp:Content ContentPlaceHolderID="saxMasterPageTitle" runat="server">
    <style type="text/css">
        #file {
            opacity: 0;
            text-indent: -9999px;
            margin-top: -20px;
            z-index: -1;
        }
        #uploadButton {
            cursor: pointer;
            z-index: 5;
        }
        #display_pic {
            border: 3px solid #2980b9;
            border-radius: 100px;
            height: 200px;
            width: 200px;
            display: block;
            margin: 0 auto;
        }
    </style>
	<div class="container">
        <div class="pull-left">
            <div class="current-page-outer">
                <span class="current-page">PROFILE</span>
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
        <div class="card" id="profileDetails">
            <div class="card-header">
                PROFILE DETAILS
            </div>
            <!-- /.card-header -->
            <div class="card-body">
                <div class="container">
                    <div class="col-4">
                        <div class="image-container" style="width: 200px; height: 200px; margin: 0 auto;" id="imageContainer">
                            <img src='<%= Session["display_picture"].ToString() %>' id="display_pic">
                        </div>
                        <div class="form-group">
                            <button type="button" class="btn btn-blue" id="uploadImageButton" data-loading-text="Uploading ...">
                                Upload image
                                <input type="file" class="form-control" id="file" name="file">
                            </button>
                        </div>
                        <!-- /.form-group -->
                    </div>
                    <!-- /.col-4 -->
                    <div class="col-8">
                        <div class="form-group">
                            <div class="col-6">
                                <label class="text-blue"><strong><small>Employee Name</small></strong></label>
                                <input type="text" class="form-control" id="employee_name" name="employee_name" readonly>
                            </div>
                            <!-- /.col-6 -->
                            <div class="col-6">
                                <label class="text-blue"><strong><small>Employee Code</small></strong></label>
                                <input type="text" class="form-control" id="employee_code" name="employee_code" readonly>
                            </div>
                            <!-- /.col-6 -->
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <div class="col-6">
                                <label class="text-blue"><strong><small>Department</small></strong></label>
                                <input type="text" class="form-control" id="department_name" name="department_name" readonly>
                            </div>
                            <!-- /.col-6 -->
                            <div class="col-6">
                                <label class="text-blue"><strong><small>Designation</small></strong></label>
                                <input type="text" class="form-control" id="designation_name" name="designation_name" readonly>
                            </div>
                            <!-- /.col-6 -->
                        </div>
                        <!-- /.form-group -->
                    </div>
                    <!-- /.col-8 -->
                </div>
                <!-- /.container -->
            </div>
            <!-- /.card-body -->
        </div>
		<div class="card">
            <div class="card-header">
                CHANGE PASSWORD
            </div>
            <!-- /.card-header -->
			<div class="card-body">
                <div class="container">
                    <form method="post" class="form" id="saveForm">
                        <div class="form-group">
                            <div class="col-6">
                                <label class="text-blue"><strong><small>Current Password</small></strong></label>
                                <input type="text" class="form-control" id="current_password" name="current_password">
                            </div>
                            <!-- /.col-6 -->
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <div class="col-6">
                                <label class="text-blue"><strong><small>New Password</small></strong></label>
                                <input type="text" class="form-control" id="new_password" name="new_password">
                            </div>
                            <!-- /.col-6 -->
                            <div class="col-6">
                                <label class="text-blue"><strong><small>Confirm Password</small></strong></label>
                                <input type="text" class="form-control" id="confirm_password" name="confirm_password"> 
                            </div>
                            <!-- /.col-6 -->
                        </div>
                        <!-- /.form-group -->
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.container -->
			</div>
			<!-- /.card-body -->
            <div class="card-footer">
                <div class="text-center">
                    <button type="text" class="btn btn-blue" id="saveButton" data-control="button" data-role="profile/save" data-loading-text="Saving ...">Save</button>
                </div>
                <!-- /.text-center -->
            </div>
            <!-- /.card-footer -->
		</div>
		<!-- /.card -->
	</div>
	<!-- /.container -->
</asp:Content>

<asp:Content ContentPlaceHolderID="saxMasterPageFooter" runat="server">
    <script type="text/javascript" src="../../resources/js/profile/update.js"></script>
</asp:Content>
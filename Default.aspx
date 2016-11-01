<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="True" CodeFile="~/Default.aspx.cs" Inherits="_Default" Title="Log In | SecurTime" %>
<!DOCTYPE html>
<html>
    <head runat="server">
        <link rel="ICON" href="resources/images/favicon.png">
        <!-- page title -->
        <title>Sign In | SecurTime</title>
        <!-- normalize.css -->
        <link rel="stylesheet" href="resources/css/normalize.css" >
        <!-- critical CSS -->
        <style type="text/css">
            * {
                box-sizing: border-box;
            }
            html,
            body {
                height: 100%;
            }
            body {
                background-color: rgba(150, 193, 218, 0.89);
                background-image: radial-gradient(farthest-side ellipse at 10% 0, rgba(239, 239, 239, 0.89), rgba(150, 193, 218, 0.89) 80%, rgba(147, 187, 211, 0.89) 120%);
                background-image: -webkit-radial-gradient(10% 0, farthest-side ellipse, rgba(239, 239, 239, 0.89), rgba(150, 193, 218, 0.89) 80%, rgba(147, 187, 211, 0.89) 120%);
                background-image: -moz-radial-gradient(10% 0, farthest-side ellipse, rgba(239, 239, 239, 0.89), rgba(150, 193, 218, 0.89) 80%, rgba(147, 187, 211, 0.89) 120%);
                color: #4d4d4d;
                font-size: 13px;
                font-family: Helvetica, sans-serif;
                letter-spacing: 0.5px;
                padding: 0;
                margin: 0;
            }
            small {
                font-size: 11px !important;
            }
            a {
                color: #3498db;
                text-decoration: none;
            }
            a:hover {
                text-decoration: underline;
            }
            h1, h2, h3, h4, h5, h6 {
                padding: 0;
                margin: 0;
                font-weight: 300;
            }
            p {
                margin: 0;
                padding: 0;
            }

            .text-center {
                text-align: center;
            }
            .text-red {
                color: #e74c3c;
            }
            /* header */
            #saxPageHeader {
                padding: 10px 20px;
            }

            /* content */
            #saxPageContent {

            }
                .card {
                    position: absolute;
                    top: 25%;
                    left: 37%;
                    width: 320px;
                    padding: 30px;
                    background: #fff;
                    border: 1px solid #ccc;
                    box-shadow: 0 0 1px 0px rgba(0, 0, 0, 0.1);
                }
                    .card-header {
                        font-weight: bold;
                        font-size: 16px;
                        padding: 10px 0 20px;
                    }

            /* forms */
            .form-group {
                padding: 15px 0;
                position: relative;
            }
            .control-label {
                display: none;
                font-size: 11px;
                font-weight: bold;
                position: absolute;
                top: 2px;
                left: 5px;
                color: #3498db;
            }
                .form-control:focus + .control-label {
                    display: block;
                }
            .form-control {
                display: block;
                padding: 3px 6px;
                height: 30px;
                width: 100%;
                border: none;
                border-bottom: 1px solid #ccc;
                background: #fff;
                line-height: 22px;
            }
                .form-control:focus {
                    border-bottom: 1px solid #2980b9;
                    outline: none;
                }

            /* buttons */
            .btn {
                box-shadow: none;
                outline: none;
                border-radius: 2px;
                margin: 0 2px;
                padding: 4px 12px;
                line-height: 22px;
                background: transparent;
                color: #51586a;
                border: 1px solid transparent;
            }
                .btn-blue {
                    background: transparent;
                    color: #3498db;
                    border-color: #3498db;
                }
                    .btn-blue:hover {
                        background: #3498db;
                        color: #fff;
                    }

            /* login */
            .forgot-password {
                color: #999;
            }

            /* footer */
            #saxPageFooter {
                padding: 5px 20px;
                position: fixed;
                bottom: 0px;
            }
            .logo {
        			display: block;
        			width: 80px;
        			
					left:0px;
        		}
        </style>

    </head>
    <body>
        <header id="saxPageHeader">
            <div class="brand">
                <img src="resources/images/royal-group.jpg" alt="SecurAX" class="logo">
            </div>
        </header>

        <section id="saxPageContent">
            <div class="card">
                <div class="card-header">
                    <h3 class="text-center">Sign in to SecurTime </h3>
                </div>
                <!-- /.card-header -->
                <div class="card-body">
                    <form class="form" method="post" runat="server">
                        <div class="form-group">
                            <asp:TextBox ID="username" runat="server" CssClass="form-control" placeholder="Username" MaxLength='16'></asp:TextBox>
                            <label class="control-label">Username</label>
                        </div>
                        <!-- /. form-group -->
                        <div class="form-group">
                            <asp:TextBox ID="password" TextMode="Password" runat="server" CssClass="form-control" placeholder="Password" MaxLength='16'></asp:TextBox>
                            <label class="control-label">Password</label>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group text-center">
                            <asp:Button ID="loginButton" runat="server" CssClass="btn btn-blue login-button" OnClick="loginButtonClick" Text="Sign In"></asp:Button>
                        </div>
                        <!-- /.form-group -->
                        <div class="form-group">
                            <asp:Label ID="errorMessage" CssClass="text-red" runat="server"></asp:Label>
                        </div>
                        <div class="form-group text-center">
                            <small><asp:LinkButton ID="forgotPassword" runat="server" CssClass="forgot-password" Text="Forgot Password?"></asp:LinkButton></small>
                        </div>
                    </form>
                    <!-- /.form -->
                </div>
                <!-- /.card-body -->
                <div class="card-footer">
                </div>
                <!-- /.card-footer -->
            </div>
        </section>

        <footer id="saxPageFooter">
            <p><small>&copy; 2015 SecurAX Tech Solutions Pvt. Ltd. All rights reserved.</small></p>
        </footer>

        <script type="text/javascript" src="resources/lib/jquery.min.js"></script>

        <script type="text/javascript">
        
          $('form').submit(function(){

                var 
                    username = $.trim($("#username").val()),
                    password = $.trim($("#password").val()),
                    $login_button = $('.login-button');
                // disable login button to avoid multiple clicks
                $login_button.attr("disabled", "disabled");

                if(username == "" && password == "") {
                    $("#errorMessage").text("Please enter your Username & Password");
                    return false;
                }
                
                if(username == "") {
                    $("#errorMessage").text("Please enter your Username");
                    return false;
                }
                if (password == "") {
                    $("#errorMessage").text("Please enter your Password");
                    return false;
                }
                // remove disabled attribute on login button.
                // else user will be unable to login.
                $login_button.removeAttr("disabled");
                return true;
          });
            
        </script>
    </body>
</html>
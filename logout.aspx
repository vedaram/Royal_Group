<%@ Page Language="C#" AutoEventWireup="true" CodeFile="logout.aspx.cs" Inherits="logout" %>
<!DOCTYPE html>
<html>
	<head>
		<link rel="ICON" href="../resources/images/favicon.png">
        <!-- page title -->
        <title>Sign Out | SecurTime</title>
        <!-- normalize.css -->
        <link rel="stylesheet" href="../resources/css/normalize.css" >
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
                color: #2980b9;
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

            #saxPageHeader {
                padding: 10px 20px;
            }
            #saxPageFooter {
                padding: 5px 20px;
                position: fixed;
                bottom: 0px;
                left: 0px;
                right: 0px;
            }

            .text-center {
                text-align: center;
            }
        </style>
	</head>
	<body>
		<header id="saxPageHeader">
			<div class="brand">
                <img src="resources/images/logo100.png" alt="SecurAX" class="logo">
            </div>
		</header>
		<!-- /#saxPageHeader -->
		<section id="saxPageContent">
			<div class="text-center" style="position: absolute; top: 40%; left: 40%;">
				<h2>Thank you and have a great day!</h2>
                <br >
                <p class="text-center">
                    <small><a href="default.aspx">Sign in again?</a></small>
                </p>
			</div>
		</section>
		<!-- /#saxPageContent -->
        <footer id="saxPageFooter">
            <p><small>&copy; 2015 SecurAX Tech Solutions Pvt. Ltd. All rights reserved.</small></p>
        </footer>
		<!-- /#saxPageFooter -->
	</body>
</html>
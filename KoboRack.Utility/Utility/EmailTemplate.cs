
namespace KoboRack.Utility.Utility
{
    public static class EmailTemplate
    {
        private static string? _supportContact = "+2347025783611";
        private static string? _supportEmail = "info@travl.co";
        private static string? _InstagramUrl = "";
        private static string? _linkedinUrl = "";
        private static string? _facebookUrl = "";
        private static string? _twitterUrl = "";

        // Reusable Header
        private static string Header()
        {
            return $@"
                <!DOCTYPE html>
                <html lang='en'>
                    <head>
                        <meta charset='UTF-8'/>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'/>
                        <link rel='preconnect' href='https://fonts.googleapis.com' />
                        <link rel='preconnect' href='https://fonts.gstatic.com' crossorigin />
                        <link
                          href='https://fonts.googleapis.com/css2?family=Manrope:wght@200;300;400;500;600;700;800&display=swap'
                          rel=""stylesheet""
                        />
                        <style>
                            body {{
                                background-color: #E0EBFF;
                                font-family: 'Manrope', Arial, sans-serif;
                                line-height: 1.5;
                            }}
                            .email-container {{
                                max-width: 600px;
                                margin: 0 auto;
                                padding: 20px;
                            }}
                            .logo {{
                                text-align: center;
                                margin: 1.5rem auto;
                                margin-bottom: 20px;
                            }}
                            .email-content {{
                                background-color: #FFFFFF;
                                border: 1px solid #003CB2;
                                padding: 1.25rem;
                                text-align: justify;
                            }}
                            .subject {{
                                font-weight: bold;
                                font-size: 18px;
                                margin-bottom: 20px;
                                text-align: center;
                            }}
                            .separator {{
                                border-top: 1px solid #003CB2;
                                margin: 20px 0;
                            }}
                            .contact-info {{
                                font-size: 14px;
                                margin-bottom: 20px;
                                text-align: center;
                            }}
                            .social-icons {{
                                display: block;
                                justify-content: center;
                                gap: 15px;
                                margin-top: 20px;
                                margin: 1.25rem auto 0;
                                text-align: center;
                            }}
                            .social-icons a {{
                                color: #003CB2;
                                font-size: 24px;
                                text-decoration: none;
                            }}
                            .footer {{
                                text-align: center;
                                margin-top: 20px;
                                font-size: 12px;
                                color: #666666;
                            }}
                            a {{
                                color: #003CB2;
                                font-weight: bold;
                                text-decoration: none;
                            }}
                            .fcc-btn {{
                                background-color: #003CB2;
                                color: #FFFFFF;
                                padding: 10px 20px;
                                text-decoration: none;
                                border-radius: 5px;
                                display: inline-block;
                            }}
                        </style>
                    </head>
                    <body>
                        <div class='email-container'>
                            <!-- Logo -->
                            <div class='logo'>
                                <img src='cid:travl-logo' alt='Travl Logo' />
                            </div>

                            <!-- Email Content -->
                            <div class='email-content'>";
        }

        // Reusable Footer
        private static string Footer()
        {
            return $@"
                                <!-- Separator Line -->
                                <div class='separator'></div>

                                <!-- Contact Information -->
                                <div class='contact-info'>
                                    <p>If you have any questions, contact us at <a href='{_supportEmail}'>{_supportEmail}</a> or give us a call at <a href='tel:{_supportContact}'>{_supportContact}</a>.</p>
                                </div>

                                <!-- Social Media Icons -->
                                <div class='social-icons'>
                                    <a href='{_InstagramUrl}' target='_blank' aria-label='Instagram'>
                                        <img src='cid:instagram-icon' alt='Instagram' width='24' height='24' />
                                    </a>
                                    <a href='{_linkedinUrl}' target='_blank' aria-label='LinkedIn'>
                                        <img src='cid:linkedin-icon' alt='LinkedIn' width='24' height='24' />
                                    </a>
                                    <a href='{_facebookUrl}' target='_blank' aria-label='Facebook'>
                                        <img src='cid:facebook-icon' alt='Facebook' width='24' height='24' />
                                    </a>
                                    <a href='{_twitterUrl}' target='_blank' aria-label='Twitter'>
                                        <img src='cid:twitter-icon' alt='Twitter' width='24' height='24' />
                                    </a>
                                </div>
                            </div>

                            <!-- Footer -->
                            <div class='footer'>
                                <p>Copyright © Travl. {DateTime.UtcNow.Year} All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                </html>";
        }

        // Reusable Subject
        private static string Subject(string subject)
        {
            return $@"
                    <!-- Subject -->
                    <div class='subject'>
                        {subject}
                    </div>";
        }

        // Method to combine header, subject, body, and footer
        public static string GenerateEmailTemplate(string subject, string body)
        {
            return Header() + Subject(subject) + body + Footer();
        }

        // Specific notification templates:

        // 1. Email Verification Template
        public static string EmailVerificationTemplate(string cusFirstName, string otp, string expiryDuration)
        {
            string body = $@"
                    <p>Hello {cusFirstName},</p>
                    <p>
                        Thank you for signing up on Travl! We're excited to have you on board.
                        To complete your registration, please verify your email address by entering the One-Time Password (OTP) provided below on our website within {expiryDuration} minutes:
                    </p>
                    <p><strong>OTP: {otp}</strong></p>
                    <p>Best regards,<br />Travl Team</p>";
            return GenerateEmailTemplate("Complete your email verification for Travl", body);
        }

        // 2. Welcome Template
        public static string WelcomeTemplate(string cusFirstName)
        {
            string body = $@"
        <p>Hello {cusFirstName},</p>
        <p>
            Welcome to Travl! We’re excited to have you on board.
            Travl makes it easy for you to book rides, connect with trusted drivers, and enjoy a seamless travel experience.
        </p>
        <p>Start exploring and enjoy your rides!</p>
        <p>Best regards,<br />Travl Team</p>";

            return GenerateEmailTemplate("Welcome to Travl - Your Journey Starts Here!", body);
        }


        // 3. Password Reset Template
        public static string PasswordResetTemplate(string cusFirstName, string resetLink, string expiryDuration, TimeFormat timeformat)
        {
            string body = $@"
                    <p>Hello {cusFirstName},</p>
                    <p>We received a request to rest the password for your Travl account. To reset your password, please click on the link below:</p>
                    <p><a href='{resetLink}'>Reset Password</a></p>
                    <p>This reset link is valid for {expiryDuration} {timeformat.ToString().ToLower()}(s). If you did not request this, please contact our support team immediately.</p>
                    <p>Best regards,<br />Travl Team</p>";
            return GenerateEmailTemplate("Reset Your Password for Travl", body);
        }

        // 4. Password Reset Success Template
        public static string PasswordResetSuccessTemplate(string cusFirstName, string supportEmail)
        {
            string body = $@"
                    <p>Hello {cusFirstName},</p>
                    <p>Your password has been reset successfully!</p><br/>
                    <p>If you are worried that someone is trying to gain unauthorized access to your account, go ahead and change your password or contact us at <em>{supportEmail}</em> for assistance.</p>
                    <p>Best regards,<br/>Travl Team</p>";
            return GenerateEmailTemplate("Your Password Has Been Successfully Reset", body);
        }

        // 5. OTP Verification Template
        public static string OtpVerificationTemplate(string cusFirstName, string otp, string expiryDuration)
        {
            string body = $@"
                    <p>Hello {cusFirstName},</p>
                    <p>Please verify your account by entering the One-Time Password (OTP) provided below:</p>
                    <p><strong>OTP: {otp}</strong></p>
                    <p>This OTP is valid for {expiryDuration} minutes. If you did not request this, please contact our support team.</p>
                    <p>Best regards,<br />Travl Team</p>";
            return GenerateEmailTemplate("Verify your account with Travl", body);
        }

        // 6. Contact Us Enquiry Template (Internal)
        public static string ContactUsInternalTemplate(string userName, string userEmail, string userNumber, string userMessage)
        {
            var body = $@"
                    <p>Hello Team,</p>
                    <p>We have received a new inquiry through our 'Contact Us' form. Please review and attend to the request.</p>
                    <p><strong>Enquiry Details</strong></p>
                    <p>Name: {userName}</p>
                    <p>Email: {userEmail}</p>
                    <p>Phone Number: {userNumber}</p>
                    <p>Message: {userMessage}</p>
                    <p>Best regards,<br />Travl Team</p>";

            return GenerateEmailTemplate("Contact Us Enquiry", body);
        }

        // 7. Contact Us Acknowledgement Template (For User)
        public static string ContactUsUserTemplate(string userName)
        {
            var body = $@"
                    <p>Hello {userName},</p>
                    <p>Thank you for contacting us. This email is to acknowledge that we have received your inquiry and our team is currently reviewing it. We aim to provide you with a response as soon as possible.</p>
                    <p>Once again, thank you for reaching out to us. We value your interest in our product, and we look forward to assisting you further.</p>
                    <p>Best regards,<br/>Travl Team</p>";
            return GenerateEmailTemplate("Contact Us Inquiry", body);
        }

        // 8. New Staff Password Creation Template
        public static string NewStaffPasswordCreationTemplate(string staffName, string createPasswordLink)
        {
            var body = $@"
                    <p>Hello {staffName},</p>
                    <p>Click on the link below to create a new password:</p>
                    <p><a href='{createPasswordLink}'>Create Password</a></p>
                    <p>Best regards,<br/>Travl Team</p>";
            return GenerateEmailTemplate("Admin Creates a New Staff", body);
        }

    }

    public enum TimeFormat
    {
        Second = 1,
        Minute = 2,
        Hour = 3
    }
}

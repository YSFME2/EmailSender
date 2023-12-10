using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EmailSender
{
    public partial class MainPage : TabbedPage
    {
        private FileResult resumeFile;

        public MainPage()
        {
            InitializeComponent();
            btnSend.Clicked += BtnSend_Clicked;
            this.Appearing += MainPage_Appearing;
            this.Disappearing += MainPage_Disappearing;
        }

        private async void MainPage_Disappearing(object sender, EventArgs e)
        {
            try
            {
                await SecureStorage.SetAsync("Email", txtEmail.Text);
                await SecureStorage.SetAsync("Name", txtName.Text);
                await SecureStorage.SetAsync("Password", txtPassword.Text);
                await SecureStorage.SetAsync("SMTP", txtsmtp.Text);
                await SecureStorage.SetAsync("Port", txtPort.Text);
                await SecureStorage.SetAsync("Subject", txtSubject.Text);
                await SecureStorage.SetAsync("Message", txtMessage.Text);
                await SecureStorage.SetAsync("Emails", txtEmails.Text);
            }
            catch (Exception ex) { }
        }

        private async void MainPage_Appearing(object sender, EventArgs e)
        {
            try
            {
                txtEmail.Text = await SecureStorage.Default.GetAsync("Email") ?? "";
                txtName.Text = await SecureStorage.Default.GetAsync("Name") ?? "";
                txtPassword.Text = await SecureStorage.Default.GetAsync("Password") ?? "";
                txtsmtp.Text = await SecureStorage.Default.GetAsync("SMTP") ?? txtsmtp.Text;
                txtPort.Text = await SecureStorage.Default.GetAsync("Port") ?? txtPort.Text;
                txtSubject.Text = await SecureStorage.Default.GetAsync("Subject") ?? "";
                txtMessage.Text = await SecureStorage.Default.GetAsync("Message") ?? "";
                txtEmails.Text = await SecureStorage.Default.GetAsync("Emails") ?? "";
            }
            catch (Exception ex)
            {
            }
        }

        private async void BtnSend_Clicked(object sender, EventArgs e)
        {
            Disable_EnableControls(false);
            if (resumeFile == null)
            {
                await DisplayAlert("Missed information", "please add your resume!", "Ok");
                Disable_EnableControls(true);
                return;
            }

            var emails = txtEmails.Text.Trim().Split("\r").ToList().Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x));

            var toEmail = string.Join(",", emails);
            var resumeFileStream = await resumeFile.OpenReadAsync();
            try
            {
                await Task.Run(() => SendToMail(toEmail,new Attachment(resumeFileStream,resumeFile.FileName)));

                await DisplayAlert("Sent", "Your Email Sent successfully!", "Ok");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message + "\n\n\n" + ex.InnerException?.Message, "Ok");
            }

            Disable_EnableControls(true);
        }

        private void SendToMail(string toEmail, params Attachment[] attachments)
        {

            MailMessage mailMessage = new MailMessage(txtEmail.Text, toEmail);
            mailMessage.Subject = txtSubject.Text;
            mailMessage.Sender = new MailAddress(txtEmail.Text, txtName.Text);
            mailMessage.Body = txtMessage.Text;
            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }

            SmtpClient smtpClient = new SmtpClient(txtsmtp.Text, int.Parse(txtPort.Text));
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(txtEmail.Text.Trim(), txtPassword.Text.Trim());

            smtpClient.Send(mailMessage);
        }

        private void Disable_EnableControls(bool enable)
        {
            txtEmail.IsEnabled = txtEmails.IsEnabled = txtMessage.IsEnabled
                = txtPassword.IsEnabled = txtPort.IsEnabled = txtResume.IsEnabled = txtName.IsEnabled = txtsmtp.IsEnabled =
                txtSubject.IsEnabled = btnSend.IsEnabled = btnLoadResume.IsEnabled = enable;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new() { PickerTitle = "Please Select Resume", FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>> { { DevicePlatform.WinUI, new[] { "pdf", } } }) });
                if (result != null)
                {
                    if (result.FileName.EndsWith("pdf", StringComparison.CurrentCultureIgnoreCase))
                    {
                        txtResume.Text = result.FileName;
                        resumeFile = result;
                    }
                }
            }
            catch { }
        }
    }

}

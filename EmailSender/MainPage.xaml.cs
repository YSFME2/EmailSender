using System.Net;
using System.Net.Mail;

namespace EmailSender
{
    public partial class MainPage : TabbedPage
    {
        private FileResult resumeFile;

        public MainPage()
        {
            InitializeComponent();
            btnSend.Clicked += BtnSend_Clicked;
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
            int interval = 1;
            if (!int.TryParse(txtIntervals.Text, out interval))
            {
                await DisplayAlert("Invalid information", "please input valid delay interval in seconds!", "Ok");
                Disable_EnableControls(true);
                return;
            }

            string[] toEmails = txtEmails.Text.Trim().Split('\n');
            foreach (string toEmail in toEmails)
            {
                await SendToMail(toEmail);

                await Task.Delay(interval * 1000);
            }
            Disable_EnableControls(true);
        }

        private async Task SendToMail(string toEmail)
        {
            MailMessage mailMessage = new MailMessage(txtEmail.Text, toEmail);
            mailMessage.Subject = txtSubject.Text;
            mailMessage.Sender = new MailAddress(txtSubject.Text, txtName.Text);
            mailMessage.Body = txtMessage.Text;

            SmtpClient smtpClient = new SmtpClient(txtsmtp.Text, int.Parse(txtPort.Text));
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.EnableSsl = true;
            smtpClient.Credentials = new NetworkCredential(txtEmail.Text.Trim(), txtPassword.Text.Trim());
            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        private void Disable_EnableControls(bool enable)
        {
            txtEmail.IsEnabled = txtEmails.IsEnabled = txtIntervals.IsEnabled = txtMessage.IsEnabled
                = txtPassword.IsEnabled = txtPort.IsEnabled = txtResume.IsEnabled =
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

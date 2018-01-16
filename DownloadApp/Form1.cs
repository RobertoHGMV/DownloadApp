using System;
using System.ComponentModel;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace DownloadApp
{
    public partial class Form1 : Form
    {
        private WebClient client;

        public Form1()
        {
            InitializeComponent();
            client = new WebClient();
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileCompleted += Client_DownloadFileCompleted;
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            lbStatus.Text = "Download finalizado!";
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    progressBar.Minimum = 0;
                    var receive = double.Parse(e.BytesReceived.ToString());
                    var total = double.Parse(e.TotalBytesToReceive.ToString());
                    var percentage = receive / total * 100;
                    lbStatus.Text = $@"Progresso: {$"{percentage:0.##}"}%";
                    progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
                }));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Mensagem do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                ValidFields();

                var url = txtUrl.Text;
                var path = txtPath.Text;

                var thread = new Thread(() =>
                {
                    var uri = new Uri(url);
                    var fileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                    //client.DownloadFileAsync(uri, Application.StartupPath + "/" + fileName);
                    client.DownloadFileAsync(uri, path  + "/" + fileName);
                });
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Mensagem do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPath_Click(object sender, EventArgs e)
        {
            try
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                        txtPath.Text = folderDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Mensagem do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ValidFields()
        {
            if (string.IsNullOrEmpty(txtUrl.Text))
                throw new Exception("Informe a url!");

            if (string.IsNullOrEmpty(txtPath.Text))
                throw new Exception("Informe o caminho para salvar o arquivo!");
        }
    }
}
